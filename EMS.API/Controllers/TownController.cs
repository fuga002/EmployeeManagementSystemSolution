using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TownController : GenericController<Town>
{
    public TownController(IGenericRepositoryInterface<Town> genericRepositoryInterface) : base(genericRepositoryInterface)
    {
    }
}