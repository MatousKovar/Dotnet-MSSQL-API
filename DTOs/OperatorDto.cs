namespace SimpleAPI;
using SimpleAPI.Models;

public class OperatorDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string BadgeNumber { get; set; } = null!;

    public string? Email { get; set; }

    public bool? IsActive { get; set; }


}