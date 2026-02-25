namespace SimpleAPI.DTOs;

public class MachineTypeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? MaintenanceIntervalHours { get; set; }

    public DateTime? CreatedAt { get; set; }
}