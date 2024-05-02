using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BranchController : GenericController<Branch>
{
    public BranchController(IGenericRepositoryInterface<Branch> genericRepositoryInterface) : base(genericRepositoryInterface)
    {
    }
}