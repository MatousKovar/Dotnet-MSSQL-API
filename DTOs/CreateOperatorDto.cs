namespace SimpleAPI.DTOs;

public class CreateOperatorDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string BadgeNumber { get; set; } = null!;

    public string? Email { get; set; }
}