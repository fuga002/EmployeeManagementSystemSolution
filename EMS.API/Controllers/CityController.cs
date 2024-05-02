using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CityController : GenericController<City>
{
    public CityController(IGenericRepositoryInterface<City> genericRepositoryInterface) : base(genericRepositoryInterface)
    {
    }
}