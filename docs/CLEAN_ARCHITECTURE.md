# Clean Architecture Guide

A comprehensive guide to understanding and implementing Clean Architecture in .NET applications.

---

## Table of Contents

- [Introduction](#introduction)
- [Core Principles](#core-principles)
- [The Dependency Rule](#the-dependency-rule)
- [Architecture Layers](#architecture-layers)
  - [Domain Layer](#1-domain-layer-innermost)
  - [Application Layer](#2-application-layer)
  - [Infrastructure Layer](#3-infrastructure-layer)
  - [Presentation Layer](#4-presentation-layer-outermost)
- [Project Structure](#project-structure)
- [Data Flow](#data-flow)
- [Dependency Injection](#dependency-injection)
- [Benefits](#benefits)
- [Common Patterns](#common-patterns)
- [Best Practices](#best-practices)
- [References](#references)

---

## Introduction

**Clean Architecture** is a software design philosophy introduced by Robert C. Martin (Uncle Bob) in 2012. It emphasizes the separation of concerns and the independence of frameworks, UI, databases, and external agencies.

The main goal is to create systems that are:

- **Independent of frameworks**
- **Testable**
- **Independent of UI**
- **Independent of database**
- **Independent of any external agency**

```
        ┌──────────────────────────────────────────────────┐
        │                 Presentation                     │
        │    (Controllers, Views, API Endpoints)           │
        │  ┌──────────────────────────────────────────┐    │
        │  │            Infrastructure                │    │
        │  │    (DB, External APIs, File System)      │    │
        │  │  ┌──────────────────────────────────┐    │    │
        │  │  │          Application             │    │    │
        │  │  │    (Use Cases, Business Logic)   │    │    │
        │  │  │  ┌──────────────────────────┐    │    │    │
        │  │  │  │         Domain           │    │    │    │
        │  │  │  │  (Entities, Core Rules)  │    │    │    │
        │  │  │  └──────────────────────────┘    │    │    │
        │  │  └──────────────────────────────────┘    │    │
        │  └──────────────────────────────────────────┘    │
        └──────────────────────────────────────────────────┘
```

---

## Core Principles

### 1. Separation of Concerns

Each layer has a specific responsibility and should not leak into other layers.

### 2. Dependency Inversion

High-level modules should not depend on low-level modules. Both should depend on abstractions.

### 3. Single Responsibility

Each class and module should have one reason to change.

### 4. Open/Closed Principle

Software entities should be open for extension but closed for modification.

### 5. Interface Segregation

Clients should not be forced to depend on interfaces they do not use.

---

## The Dependency Rule

> **Source code dependencies must point only inward, toward higher-level policies.**

This is the overriding rule that makes Clean Architecture work:

- **Outer layers** can depend on **inner layers**
- **Inner layers** cannot know anything about **outer layers**
- Data formats declared in outer layers should not be used in inner layers

```
Presentation → Infrastructure → Application → Domain
     ↓              ↓               ↓            ↓
   Knows         Knows           Knows       Knows
   about         about           about       NOTHING
   all           Application     Domain      (Pure)
   layers        & Domain        only
```

---

## Architecture Layers

### 1. Domain Layer (Innermost)

The **Domain Layer** is the core of the application. It contains enterprise-wide business rules and entities.

#### Contents:

- **Entities** - Core business objects with identity
- **Value Objects** - Objects without identity, defined by their attributes
- **Domain Events** - Events that occur within the domain
- **Enumerations** - Domain-specific enums
- **Exceptions** - Custom domain exceptions
- **Interfaces** - Core abstractions (e.g., `IRepository<T>`)

#### Example Structure:

```
📂 Domain
├── 📂 Entities
│   ├── User.cs
│   ├── Order.cs
│   └── Product.cs
├── 📂 ValueObjects
│   ├── Email.cs
│   ├── Money.cs
│   └── Address.cs
├── 📂 Events
│   ├── OrderCreatedEvent.cs
│   └── UserRegisteredEvent.cs
├── 📂 Enums
│   ├── OrderStatus.cs
│   └── UserRole.cs
├── 📂 Exceptions
│   ├── DomainException.cs
│   └── InvalidEmailException.cs
└── 📂 Interfaces
    ├── IRepository.cs
    └── IUnitOfWork.cs
```

#### Example Code:

```csharp
// Entities/User.cs
namespace Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // For EF Core

    public static User Create(string email, string fullName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
    public void UpdateName(string newName) => FullName = newName;
}
```

---

### 2. Application Layer

The **Application Layer** contains application-specific business rules. It orchestrates the flow of data to and from entities and directs them to apply enterprise-wide business rules.

#### Contents:

- **Use Cases / Services** - Application business logic
- **DTOs (Data Transfer Objects)** - Data contracts for input/output
- **Interfaces** - Abstractions for infrastructure services
- **Validators** - Input validation logic
- **Mappers** - Object mapping configurations
- **Commands & Queries** - CQRS pattern (optional)

#### Example Structure:

```
📂 Application
├── 📂 Common
│   ├── 📂 Interfaces
│   │   ├── IApplicationDbContext.cs
│   │   ├── IEmailService.cs
│   │   └── ICurrentUserService.cs
│   ├── 📂 Mappings
│   │   └── MappingProfile.cs
│   └── 📂 Behaviors
│       ├── ValidationBehavior.cs
│       └── LoggingBehavior.cs
├── 📂 Users
│   ├── 📂 Commands
│   │   ├── CreateUser
│   │   │   ├── CreateUserCommand.cs
│   │   │   ├── CreateUserCommandHandler.cs
│   │   │   └── CreateUserCommandValidator.cs
│   │   └── UpdateUser
│   │       └── ...
│   ├── 📂 Queries
│   │   ├── GetUserById
│   │   │   ├── GetUserByIdQuery.cs
│   │   │   ├── GetUserByIdQueryHandler.cs
│   │   │   └── UserDto.cs
│   │   └── GetAllUsers
│   │       └── ...
│   └── 📂 DTOs
│       ├── UserDto.cs
│       └── CreateUserDto.cs
└── 📂 Orders
    └── ...
```

#### Example Code:

```csharp
// Users/Commands/CreateUser/CreateUserCommand.cs
namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string FullName) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
            throw new ApplicationException("User with this email already exists");

        // Create new user using domain factory method
        var user = User.Create(request.Email, request.FullName);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Send welcome email (infrastructure concern, but orchestrated here)
        await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

        return user.Id;
    }
}
```

---

### 3. Infrastructure Layer

The **Infrastructure Layer** contains all external concerns. This is where you implement the interfaces defined in the Application layer.

#### Contents:

- **Database Context** - EF Core DbContext
- **Repositories** - Data access implementations
- **External Services** - API clients, email services, file storage
- **Identity** - Authentication & Authorization
- **Configurations** - Entity configurations, settings

#### Example Structure:

```
📂 Infrastructure
├── 📂 Persistence
│   ├── ApplicationDbContext.cs
│   ├── 📂 Configurations
│   │   ├── UserConfiguration.cs
│   │   └── OrderConfiguration.cs
│   ├── 📂 Repositories
│   │   ├── UserRepository.cs
│   │   └── OrderRepository.cs
│   └── 📂 Migrations
│       └── ...
├── 📂 Services
│   ├── EmailService.cs
│   ├── FileStorageService.cs
│   └── DateTimeService.cs
├── 📂 Identity
│   ├── IdentityService.cs
│   └── CurrentUserService.cs
└── DependencyInjection.cs
```

#### Example Code:

```csharp
// Persistence/ApplicationDbContext.cs
namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}

// Services/EmailService.cs
namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string email, string fullName)
    {
        // Implementation using SendGrid, SMTP, etc.
        _logger.LogInformation("Sending welcome email to {Email}", email);
        // ...
    }
}
```

---

### 4. Presentation Layer (Outermost)

The **Presentation Layer** is the entry point of the application. It handles HTTP requests, authentication, and returns responses.

#### Contents:

- **Controllers** - API endpoints
- **Middlewares** - Request/Response pipeline
- **Filters** - Action filters, exception filters
- **ViewModels** - Request/Response models (optional)
- **Program.cs** - Application configuration

#### Example Structure:

```
📂 WebAPI
├── 📂 Controllers
│   ├── UsersController.cs
│   └── OrdersController.cs
├── 📂 Middlewares
│   └── ExceptionHandlingMiddleware.cs
├── 📂 Filters
│   └── ValidationFilter.cs
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

#### Example Code:

```csharp
// Controllers/UsersController.cs
namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = userId }, userId);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }
}
```

---

## Project Structure

### Recommended Solution Structure

```
📂 CleanArchitecture.sln
├── 📂 src
│   ├── 📂 Domain
│   │   └── Domain.csproj
│   ├── 📂 Application
│   │   └── Application.csproj          → References: Domain
│   ├── 📂 Infrastructure
│   │   └── Infrastructure.csproj       → References: Application
│   └── 📂 WebAPI
│       └── WebAPI.csproj               → References: Application, Infrastructure
│
├── 📂 tests
│   ├── 📂 Domain.UnitTests
│   ├── 📂 Application.UnitTests
│   ├── 📂 Infrastructure.IntegrationTests
│   └── 📂 WebAPI.FunctionalTests
│
└── 📂 docs
    └── CLEAN_ARCHITECTURE.md
```

### Project References

| Project        | References                            |
| -------------- | ------------------------------------- |
| Domain         | None (Core)                           |
| Application    | Domain                                |
| Infrastructure | Application (and transitively Domain) |
| WebAPI         | Application, Infrastructure           |

---

## Data Flow

### Request Flow (Create User Example)

```
1. HTTP Request → UsersController.Create()
                        ↓
2. Controller sends CreateUserCommand to Mediator
                        ↓
3. CreateUserCommandValidator validates the command
                        ↓
4. CreateUserCommandHandler executes business logic
                        ↓
5. Handler uses IApplicationDbContext (interface)
                        ↓
6. ApplicationDbContext (implementation) persists to DB
                        ↓
7. Handler calls IEmailService.SendWelcomeEmailAsync()
                        ↓
8. EmailService (implementation) sends email
                        ↓
9. Handler returns new User Id
                        ↓
10. Controller returns HTTP 201 Created
```

---

## Dependency Injection

### Registering Services

Each layer should have its own DI registration method:

```csharp
// Application/DependencyInjection.cs
namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}

// Infrastructure/DependencyInjection.cs
namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IDateTimeService, DateTimeService>();

        return services;
    }
}

// WebAPI/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ... rest of configuration
```

---

## Benefits

| Benefit                 | Description                                                       |
| ----------------------- | ----------------------------------------------------------------- |
| **Testability**         | Business logic can be tested without UI, DB, or external services |
| **Maintainability**     | Clear separation makes code easier to understand and modify       |
| **Flexibility**         | Easy to swap frameworks, databases, or external services          |
| **Scalability**         | Each layer can be scaled independently                            |
| **Team Collaboration**  | Teams can work on different layers simultaneously                 |
| **Long-term Viability** | The core business logic is protected from external changes        |

---

## Common Patterns

### 1. CQRS (Command Query Responsibility Segregation)

Separate read and write operations:

```csharp
// Commands (Write)
public record CreateUserCommand(string Email, string Name) : IRequest<Guid>;

// Queries (Read)
public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
```

### 2. Repository Pattern

Abstract data access:

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

### 3. Unit of Work Pattern

Manage transactions across multiple repositories:

```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 4. Specification Pattern

Encapsulate query logic:

```csharp
public class ActiveUsersSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.IsActive && user.EmailConfirmed;
    }
}
```

---

## Best Practices

### Do's ✅

1. **Keep Domain layer pure** - No dependencies on external libraries
2. **Use interfaces for external services** - Define in Application, implement in Infrastructure
3. **Validate input at the edge** - Use FluentValidation in Application layer
4. **Use meaningful names** - `CreateUserCommand` not `CreateCmd`
5. **Keep controllers thin** - Delegate to Application layer via MediatR
6. **Use async/await consistently** - Throughout all layers
7. **Document public APIs** - Use XML comments and OpenAPI/Swagger

### Don'ts ❌

1. **Don't reference Infrastructure from Domain/Application**
2. **Don't put business logic in Controllers**
3. **Don't expose entities directly** - Use DTOs
4. **Don't skip validation** - Validate at every layer boundary
5. **Don't create god classes** - Keep classes focused and small
6. **Don't hardcode configurations** - Use IOptions pattern
7. **Don't ignore exceptions** - Handle them appropriately at each layer

---

## References

- [The Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Robert C. Martin
- [Clean Architecture: A Craftsman's Guide to Software Structure and Design](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164) - Book
- [Jason Taylor's Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture) - GitHub
- [Ardalis Clean Architecture Template](https://github.com/ardalis/CleanArchitecture) - GitHub

---

## Getting Started

1. Create the solution structure following the project organization above
2. Set up project references (Domain → Application → Infrastructure ← WebAPI)
3. Implement your first entity in Domain
4. Create your first use case in Application
5. Implement the required interfaces in Infrastructure
6. Expose the functionality through WebAPI

Happy coding! 🚀

---

_Last updated: February 2026_
