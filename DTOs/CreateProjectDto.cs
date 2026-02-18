namespace SimpleAPI.DTOs;

public class CreateProjectDto
{
    public string Name { get; set; } = null!;

    public string? ClientName { get; set; }

    public DateOnly? Deadline { get; set; }

}