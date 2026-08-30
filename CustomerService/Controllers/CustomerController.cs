using CustomerService.Application.Common.Authorization;
using CustomerService.Application.Common.Exceptions;
using CustomerService.Application.Features.Customers.DTOs.Requests;
using CustomerService.Application.Features.Customers.Interfaces;
using CustomerService.Shared.Pagination;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomerController(ICustomerService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.ManageCustomers)]
    public async Task<IActionResult> Create([FromForm] CreateCustomerRequest request)
    {
        var result = await _service.CreateAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    [Authorize(
        Policy = AuthorizationPolicies.ViewCustomers)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PagedRequest request)
    {
        var result =
            await _service.GetAllAsync(request);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(
        Policy = AuthorizationPolicies.ViewCustomers)]
    public async Task<IActionResult> GetById(int id)
    {
        var result =
            await _service.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.ManageCustomers)]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateCustomerRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.DeleteCustomers)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = AuthorizationPolicies.ChangeCustomerStatus)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeCustomerStatusRequest request)
    {
        var result = await _service.ChangeStatusAsync(id, request);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result =
            await _service.GetMyProfileAsync();

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("test-error")]
    public IActionResult TestError()
    {
        throw new Exception(
            "This is a test exception.");
    }

    [HttpGet("test/not-found")]
    public IActionResult TestNotFound()
    {
        throw new NotFoundException(
            "Customer was not found.");
    }
    [HttpGet("test/conflict")]
    public IActionResult TestConflict()
    {
        throw new ConflictException(
            "Customer already exists.");
    }

    
}