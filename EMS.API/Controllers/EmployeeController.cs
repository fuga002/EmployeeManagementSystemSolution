using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : GenericController<Employee>
{
    public EmployeeController(IGenericRepositoryInterface<Employee> genericRepositoryInterface) : base(genericRepositoryInterface)
    {
    }
}