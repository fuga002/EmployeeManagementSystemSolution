using EMS.APILibrary.Data;
using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace EMS.APILibrary.Repositories.Implementations;

public class GenericRepositoryInterface:IGenericRepositoryInterface<GeneralDepartment>
{
    private readonly AppDbContext _context;

    public GenericRepositoryInterface(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GeneralDepartment>> GetAll() => await _context.GeneralDepartments.ToListAsync();

    public async Task<GeneralDepartment> GetById(int id) => await _context.GeneralDepartments.FindAsync(id);

    public async Task<GeneralResponse> Insert(GeneralDepartment item)
    {
        if (!await CheckName(item.Name)) return new GeneralResponse(false, "Department already added");
        _context.GeneralDepartments.Add(item);
        return Success();
    }


    public async Task<GeneralResponse> Update(GeneralDepartment item)
    {
        var dep = await _context.GeneralDepartments.FindAsync(item.Id);
        if (dep is null) return NotFound();
        dep.Name = item.Name;
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> DeleteById(int id)
    {
        var dep = await _context.GeneralDepartments.FindAsync(id);
        if (dep is null) return NotFound();

        _context.GeneralDepartments.Remove(dep);
        await Commit();
        return Success();
    }

    private static GeneralResponse NotFound() => new(false, "Sorry department not found");
    private static GeneralResponse Success() => new(true, "Process completed");
    private async Task Commit() => await _context.SaveChangesAsync();
    
    private async Task<bool> CheckName(string? name)
    {
        var item = await _context.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name!.ToLower()));
        return item is null;
    }
}