namespace Application.Orders.DTOs;

/// <summary>
/// DTO for creating a new order with multiple items
/// </summary>
public class CreateOrderDto
{
    public string CustomerId { get; set; } = null!;
    public int? EmployeeId { get; set; }
    public int? ShipperId { get; set; }
    public DateTime? RequiredDate { get; set; }
    public decimal? Freight { get; set; }
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }

    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public short Quantity { get; set; }
    public float Discount { get; set; } = 0;
}

/// <summary>
/// Result after creating order
/// </summary>
public class CreateOrderResultDto
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public string Message { get; set; } = null!;
}
