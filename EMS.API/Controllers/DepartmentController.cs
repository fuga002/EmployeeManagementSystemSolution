using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController : GenericController<Department>
{
    public DepartmentController(IGenericRepositoryInterface<Department> genericRepositoryInterface) : base(genericRepositoryInterface)
    {
    }
}