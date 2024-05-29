using EMS.APILibrary.Data;
using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace EMS.APILibrary.Repositories.Implementations;

public class TownRepository:IGenericRepositoryInterface<Town>
{
    private readonly AppDbContext _context;

    public TownRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Town>> GetAll() => await _context.Towns.AsNoTracking().Include(t => t.City).ToListAsync();

    public async Task<Town> GetById(int id) => await _context.Towns.FindAsync(id);

    public async Task<GeneralResponse> Insert(Town item)
    {
        if (!await CheckName(item.Name)) return new GeneralResponse(false, "Town already added");
        _context.Towns.Add(item);
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> Update(Town item)
    {
        var town = await _context.Towns.FindAsync(item.Id);
        if (town is null) return NotFound();
        town.Name = item.Name;
        town.CityId = item.CityId;
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> DeleteById(int id)
    {
        var town = await _context.Towns.FindAsync(id);
        if (town is null) return NotFound();

        _context.Towns.Remove(town);
        await Commit();
        return Success();
    }
    
    private static GeneralResponse NotFound() => new(false, "Sorry Town not found");
    private static GeneralResponse Success() => new(true, "Process completed");
    private async Task Commit() => await _context.SaveChangesAsync();
    
    private async Task<bool> CheckName(string? name)
    {
        var item = await _context.Towns.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name!.ToLower()));
        return item is null;
    }
}