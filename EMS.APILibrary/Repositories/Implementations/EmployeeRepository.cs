using EMS.APILibrary.Data;
using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace EMS.APILibrary.Repositories.Implementations;

public class EmployeeRepository:IGenericRepositoryInterface<Employee>
{
    private readonly AppDbContext _appDbContext;

    public EmployeeRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<Employee>> GetAll()
    {
        var employees = await _appDbContext.Employees.AsNoTracking().Include(t => t.Town)
            .ThenInclude(b => b.City)
            .ThenInclude(c => c.Country)
            .Include(b => b.Branch)
            .ThenInclude(d => d.Department)
            .ThenInclude(gd => gd.GeneralDepartment).ToListAsync();
        return employees;
    }

    public async Task<Employee> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<GeneralResponse> Insert(Employee item)
    {
        if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Employee already added");

        _appDbContext.Employees.Add(item);
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> Update(Employee item)
    {
        var findUser = await _appDbContext.Employees.FirstOrDefaultAsync(e => e.Id == item.Id);
        if (findUser is null) return new(false, "Employee does not exist");

        findUser.Name = item.Name;
        findUser.Other = item.Other;
        findUser.Address = item.Address;
        findUser.TelephoneNumber = item.TelephoneNumber;
        findUser.BranchId = item.BranchId;
        findUser.TownId = item.TownId;
        findUser.CivilId = item.CivilId;
        findUser.FileNumber = item.FileNumber;
        findUser.JobName = item.JobName;
        findUser.Photo = item.Photo;

        _appDbContext.Employees.Update(item);
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> DeleteById(int id)
    {
        var item = await _appDbContext.Employees.FindAsync(id);
        if (item is null) return NotFound();

        _appDbContext.Employees.Remove(item);
        await Commit();
        return Success();
    }

    private GeneralResponse Success() => new(true, "Process completed");

    private async Task Commit() => await _appDbContext.SaveChangesAsync();

    private GeneralResponse NotFound() => new (false, "Sorry branch not found");

    private async Task<bool> CheckName(string name)
    {
        var item = await _appDbContext.Employees.FirstOrDefaultAsync(e => e.Name!.ToLower().Equals(name.ToLower()));
        return item is null ? true : false;
    }
}