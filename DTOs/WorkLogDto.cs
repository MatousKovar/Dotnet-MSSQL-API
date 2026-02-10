namespace SimpleAPI.DTOs;
using SimpleAPI.Models;
using SimpleAPI.Data;   

public class WorkLogDto
{
    public int Id { get; set; }

    public int MachineId { get; set; }

    public int OperatorId { get; set; }

    public int ProjectId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? OutputQuantity { get; set; }

    public string? Notes { get; set; }
}