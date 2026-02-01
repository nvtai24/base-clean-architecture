namespace Application.Orders.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public decimal? Freight { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipCountry { get; set; }
}

public class OrderDetailDto : OrderDto
{
    public string? EmployeeName { get; set; }
    public string? ShipperName { get; set; }
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    public float Discount { get; set; }
    public decimal ExtendedPrice => UnitPrice * Quantity * (1 - (decimal)Discount);
}
