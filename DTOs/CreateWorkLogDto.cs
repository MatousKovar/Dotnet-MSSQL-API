namespace SimpleAPI.DTOs;

public class CreateWorkLogDto
{
    public int MachineId { get; set; }

    public int OperatorId { get; set; }

    public int ProjectId { get; set; }

    public string? Notes { get; set; }
}