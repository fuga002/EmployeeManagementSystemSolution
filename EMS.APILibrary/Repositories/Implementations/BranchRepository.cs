using EMS.APILibrary.Data;
using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace EMS.APILibrary.Repositories.Implementations;

public class BranchRepository:IGenericRepositoryInterface<Branch>
{
    private readonly AppDbContext _context;

    public BranchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Branch>> GetAll() => await _context.Branches.AsNoTracking().Include(d=>d.Department).ToListAsync();

    public async Task<Branch> GetById(int id) => await _context.Branches.FindAsync(id);

    public async Task<GeneralResponse> Insert(Branch item)
    {
        if (!await CheckName(item.Name)) return new GeneralResponse(false, "Department already added");
        _context.Branches.Add(item);
        return Success();
    }

    public async Task<GeneralResponse> Update(Branch item)
    {
        var branch = await _context.Branches.FindAsync(item.Id);
        if (branch is null) return NotFound();
        branch.Name = item.Name;
        branch.DepartmentId = item.DepartmentId;
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> DeleteById(int id)
    {
        var dep = await _context.Branches.FindAsync(id);
        if (dep is null) return NotFound();

        _context.Branches.Remove(dep);
        await Commit();
        return Success();
    }
    
    private static GeneralResponse NotFound() => new(false, "Sorry department not found");
    private static GeneralResponse Success() => new(true, "Process completed");
    private async Task Commit() => await _context.SaveChangesAsync();
    
    private async Task<bool> CheckName(string? name)
    {
        var item = await _context.Branches.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name!.ToLower()));
        return item is null;
    }
}