using EMS.APILibrary.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : Controller where T : class
    {
        private readonly IGenericRepositoryInterface<T> _genericRepositoryInterface;

        public GenericController(IGenericRepositoryInterface<T> genericRepositoryInterface)
        {
            _genericRepositoryInterface = genericRepositoryInterface;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await _genericRepositoryInterface.GetAll());

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid request sent");
            return Ok(await _genericRepositoryInterface.DeleteById(id));
        }

        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid request sent");
            return Ok(await _genericRepositoryInterface.GetById(id));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(await _genericRepositoryInterface.Insert(model));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(await _genericRepositoryInterface.Update(model));
        }
        
        
    }
}
