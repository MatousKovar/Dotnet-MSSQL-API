namespace SimpleAPI.DTOs;

public class CreateMachineTypeDto
{
    public string Name { get; set; } = null!;

    public int? MaintenanceIntervalHours { get; set; }
}