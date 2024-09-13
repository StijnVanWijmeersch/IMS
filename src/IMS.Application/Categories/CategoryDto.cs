namespace IMS.Application.Categories;

public sealed record CategoryDto
{
    public ulong? Id { get; set; }
    public string Name { get; set; } = null!;

}
