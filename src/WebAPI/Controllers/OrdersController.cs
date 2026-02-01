using Application.Orders.DTOs;
using Application.Orders.Queries.GetAllOrders;
using Application.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all orders with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetAllOrdersResult>> GetAll(
        [FromQuery] string? customerId = null,
        [FromQuery] int? employeeId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllOrdersQuery(
            customerId, employeeId, fromDate, toDate, pageNumber, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Get order by ID with full details
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDetailDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }
}
