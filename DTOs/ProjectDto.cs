using SimpleAPI.Models;
namespace SimpleAPI.DTOs;

public partial class ProjectDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ClientName { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Status { get; set; }
}
