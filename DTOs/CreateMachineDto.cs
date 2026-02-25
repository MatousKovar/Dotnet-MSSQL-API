namespace SimpleAPI.DTOs;

public class CreateMachineDto
{
    public string Code { get; set; } = null!;

    public int MachineTypeId { get; set; }

    public string? Status { get; set; }

    public string? Location { get; set; }
}