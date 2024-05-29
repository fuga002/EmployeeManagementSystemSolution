using EMS.APILibrary.Data;
using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace EMS.APILibrary.Repositories.Implementations;

public class CityRepository:IGenericRepositoryInterface<City>
{
    private readonly AppDbContext _context;

    public CityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<City>> GetAll() => await _context.Cities.AsNoTracking().Include(c => c.Country).ToListAsync();

    public async Task<City> GetById(int id) => await _context.Cities.FindAsync(id);

    public async Task<GeneralResponse> Insert(City item)
    {
        if (!await CheckName(item.Name)) return new GeneralResponse(false, "Department already added");
        _context.Cities.Add(item);
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> Update(City item)
    {
        var city = await _context.Cities.FindAsync(item.Id);
        if (city is null) return NotFound();
        city.Name = item.Name;
        city.CountryId = item.CountryId;
        await Commit();
        return Success();
    }

    public async Task<GeneralResponse> DeleteById(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city is null) return NotFound();

        _context.Cities.Remove(city);
        await Commit();
        return Success();
    }
    
    private static GeneralResponse NotFound() => new(false, "Sorry department not found");
    private static GeneralResponse Success() => new(true, "Process completed");
    private async Task Commit() => await _context.SaveChangesAsync();
    
    private async Task<bool> CheckName(string? name)
    {
        var item = await _context.Cities.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name!.ToLower()));
        return item is null;
    }
}