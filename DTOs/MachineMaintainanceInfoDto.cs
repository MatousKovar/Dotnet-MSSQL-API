namespace SimpleAPI.DTOs;

// DTO is used for returning data to client without showing whole database structured. Prevents cycles
public class MachineMaintainanceInfoDto
{
    public string Code { get; set; } = null!;

    public string? Status { get; set; }

    public string? Location { get; set; }

    public string MachineTypeName { get; set; } = null!;

    public Datetime? NextExpectedMaintainanceTime { get; set; }
}