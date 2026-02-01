using Application.Categories.DTOs;
using Application.Customers.DTOs;
using Application.Employees.DTOs;
using Application.Orders.DTOs;
using Application.Products.DTOs;
using Application.Shippers.DTOs;
using Application.Suppliers.DTOs;
using AutoMapper;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Products
        CreateMap<Domain.Entities.Product, ProductDto>();
        CreateMap<Domain.Entities.Product, ProductDetailDto>();

        // Categories
        CreateMap<Domain.Entities.Category, CategoryDto>();
        CreateMap<Domain.Entities.Category, CategoryDetailDto>();

        // Customers
        CreateMap<Domain.Entities.Customer, CustomerDto>();
        CreateMap<Domain.Entities.Customer, CustomerDetailDto>();

        // Orders
        CreateMap<Domain.Entities.Order, OrderDto>();
        CreateMap<Domain.Entities.Order, OrderDetailDto>();
        CreateMap<Domain.Entities.OrderDetail, OrderItemDto>();

        // Suppliers
        CreateMap<Domain.Entities.Supplier, SupplierDto>();

        // Employees
        CreateMap<Domain.Entities.Employee, EmployeeDto>();

        // Shippers
        CreateMap<Domain.Entities.Shipper, ShipperDto>();
    }
}
