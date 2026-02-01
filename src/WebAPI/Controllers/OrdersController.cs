using Application.Orders.Commands.CreateOrder;
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

    /// <summary>
    /// Create a new order with multiple items (uses Unit of Work transaction)
    /// </summary>
    /// <remarks>
    /// This endpoint demonstrates Unit of Work pattern:
    /// - Validates customer and products exist
    /// - Checks stock availability
    /// - Creates Order + OrderDetails in a transaction
    /// - Updates product stock
    /// - Rolls back everything if any step fails
    /// 
    /// Sample request:
    /// ```json
    /// {
    ///   "customerId": "ALFKI",
    ///   "employeeId": 1,
    ///   "shipperId": 1,
    ///   "requiredDate": "2026-02-15",
    ///   "freight": 25.50,
    ///   "shipCity": "Berlin",
    ///   "shipCountry": "Germany",
    ///   "items": [
    ///     { "productId": 1, "quantity": 5, "discount": 0 },
    ///     { "productId": 2, "quantity": 3, "discount": 0.1 }
    ///   ]
    /// }
    /// ```
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateOrderResultDto>> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var result = await _mediator.Send(new CreateOrderCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = result.OrderId }, result);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
