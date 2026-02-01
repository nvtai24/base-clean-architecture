namespace Application.Categories.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}

public class CategoryDetailDto : CategoryDto
{
    public List<CategoryProductDto> Products { get; set; } = new();
}

public class CategoryProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal? UnitPrice { get; set; }
    public bool Discontinued { get; set; }
}

public class CreateCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}
