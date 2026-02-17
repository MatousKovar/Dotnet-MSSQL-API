namespace SimpleAPI.DTOs;
using SimpleAPI.Models;
using SimpleAPI.Data;   

public class CreateWorkLogDto
{
    public int MachineId { get; set; }

    public int OperatorId { get; set; }

    public int ProjectId { get; set; }

    public int? OutputQuantity { get; set; }

    public string? Notes { get; set; }
}