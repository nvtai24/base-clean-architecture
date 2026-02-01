using Application.Categories.DTOs;
using Application.Categories.Queries.GetAllCategories;
using Application.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get category by ID with products
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDetailDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }
}
