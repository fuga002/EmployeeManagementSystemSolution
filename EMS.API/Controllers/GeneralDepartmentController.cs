using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GeneralDepartmentController : GenericController<GeneralDepartment>
{
    public GeneralDepartmentController(IGenericRepositoryInterface<GeneralDepartment> genericRepositoryInterface) : base(genericRepositoryInterface)
    {
    }
}