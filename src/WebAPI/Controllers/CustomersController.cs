using Application.Customers.DTOs;
using Application.Customers.Queries.GetAllCustomers;
using Application.Customers.Queries.GetCustomerById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all customers with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetAllCustomersResult>> GetAll(
        [FromQuery] string? country = null,
        [FromQuery] string? city = null,
        [FromQuery] string? search = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllCustomersQuery(
            country, city, search, pageNumber, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Get customer by ID with order count
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDetailDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }
}
