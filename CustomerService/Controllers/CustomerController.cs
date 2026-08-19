using CustomerService.Application.Features.Customers.DTOs.Requests;
using CustomerService.Application.Features.Customers.Interfaces;
using CustomerService.Shared.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(
            ICustomerService service)
        {
            _service = service;
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            var result = await _service.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult>GetAll([FromQuery] PagedRequest request)
        {
            var result = await _service.GetAllAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult>Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _service.UpdateAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> changeStatus(int id, ChangeCustomerStatusRequest request)
        {
            var result = await _service.ChangeStatusAsync(id, request);
            if(!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
