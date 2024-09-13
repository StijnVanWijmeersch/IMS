namespace IMS.Application.Statuses;

public sealed record StatusDto
{
    public ulong Id { get; set; }
    public string Name { get; set; } = null!;
}
