# Clean Architecture Northwind API

A .NET 8 Web API project demonstrating **Clean Architecture** principles using the Northwind SQL Server database.

---

## 📚 About This Project

This project serves as a learning template for implementing Clean Architecture in .NET applications. It demonstrates:

- **Separation of Concerns** - Each layer has a specific responsibility
- **Dependency Inversion** - Inner layers don't depend on outer layers
- **CQRS Pattern** - Separate Commands and Queries using MediatR
- **Repository Pattern** - Abstracted data access through interfaces

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                         WebAPI                              │
│              (Controllers, Program.cs)                      │
├─────────────────────────────────────────────────────────────┤
│                      Application                            │
│         (Use Cases, DTOs, Commands, Queries)                │
├─────────────────────────────────────────────────────────────┤
│                        Domain                               │
│              (Entities, Interfaces)                         │
├─────────────────────────────────────────────────────────────┤
│                    Infrastructure                           │
│           (EF Core, DbContext, Repositories)                │
└─────────────────────────────────────────────────────────────┘
```

### Dependency Flow

```
WebAPI → Application, Infrastructure
Infrastructure → Application
Application → Domain
Domain → (nothing - it's the core)
```

---

## 📁 Project Structure

```
📂 CleanArchitectureNorthwind.sln
├── 📂 src
│   ├── 📂 Domain                    # Core entities (27 from Northwind)
│   │   └── 📂 Entities
│   │
│   ├── 📂 Application               # Business logic layer
│   │   ├── 📂 Common
│   │   │   ├── 📂 Interfaces        # INorthwindDbContext
│   │   │   └── 📂 Mappings          # AutoMapper profiles
│   │   ├── 📂 Products              # CRUD operations
│   │   │   ├── 📂 Commands          # Create, Update, Delete
│   │   │   ├── 📂 Queries           # GetAll, GetById
│   │   │   └── 📂 DTOs
│   │   ├── 📂 Categories
│   │   ├── 📂 Customers
│   │   ├── 📂 Orders
│   │   └── ...
│   │
│   ├── 📂 Infrastructure            # External concerns
│   │   ├── 📂 Persistence
│   │   │   └── NorthwindDbContext.cs
│   │   └── DependencyInjection.cs
│   │
│   └── 📂 WebAPI                    # Presentation layer
│       ├── 📂 Controllers
│       ├── Program.cs
│       └── appsettings.json
│
└── 📂 docs
    └── CLEAN_ARCHITECTURE.md        # Detailed architecture guide
```

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server with Northwind database
- Visual Studio 2022 / VS Code / Rider

### Configuration

Update the connection string in `src/WebAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Northwind;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

### Run the Application

```bash
cd src/WebAPI
dotnet run
```

Open your browser at: **http://localhost:5163/swagger**

---

## 📡 API Endpoints

### Products

| Method | Endpoint             | Description                  |
| ------ | -------------------- | ---------------------------- |
| GET    | `/api/products`      | Get all products (paginated) |
| GET    | `/api/products/{id}` | Get product by ID            |
| POST   | `/api/products`      | Create new product           |
| PUT    | `/api/products/{id}` | Update product               |
| DELETE | `/api/products/{id}` | Delete product               |

### Categories

| Method | Endpoint               | Description                |
| ------ | ---------------------- | -------------------------- |
| GET    | `/api/categories`      | Get all categories         |
| GET    | `/api/categories/{id}` | Get category with products |

### Customers

| Method | Endpoint              | Description                   |
| ------ | --------------------- | ----------------------------- |
| GET    | `/api/customers`      | Get all customers (paginated) |
| GET    | `/api/customers/{id}` | Get customer with order count |

### Orders

| Method | Endpoint           | Description                |
| ------ | ------------------ | -------------------------- |
| GET    | `/api/orders`      | Get all orders (paginated) |
| GET    | `/api/orders/{id}` | Get order with details     |

---

## 🔧 Technologies Used

| Technology            | Version | Purpose             |
| --------------------- | ------- | ------------------- |
| .NET                  | 8.0     | Framework           |
| Entity Framework Core | 8.0     | ORM                 |
| MediatR               | 12.x    | CQRS implementation |
| AutoMapper            | 13.x    | Object mapping      |
| FluentValidation      | 11.x    | Input validation    |
| Swashbuckle           | 6.x     | Swagger/OpenAPI     |

---

## 📖 Learning Resources

- Read the detailed [Clean Architecture Guide](docs/CLEAN_ARCHITECTURE.md)
- [The Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Uncle Bob
- [Jason Taylor's Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

---

## 🎯 Key Concepts Demonstrated

### 1. CQRS Pattern (Command Query Responsibility Segregation)

```csharp
// Query - Read operation
public record GetAllProductsQuery(...) : IRequest<GetAllProductsResult>;

// Command - Write operation
public record CreateProductCommand(CreateProductDto Dto) : IRequest<int>;
```

### 2. Dependency Inversion

```csharp
// Interface defined in Application layer
public interface INorthwindDbContext { ... }

// Implemented in Infrastructure layer
public class NorthwindDbContext : DbContext, INorthwindDbContext { ... }
```

### 3. MediatR Handler Pattern

```csharp
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsResult>
{
    public async Task<GetAllProductsResult> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // Business logic here
    }
}
```

---

## 📝 License

This project is for educational purposes.

---

## 🤝 Contributing

Feel free to extend this project by:

- Adding CRUD operations for Employees, Suppliers
- Implementing authentication/authorization
- Adding unit tests
- Implementing caching with Redis
