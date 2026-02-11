namespace SimpleAPI;
using SimpleAPI.Models;


public class WorkLogDto
{
    public int Id { get; set; }
    public int MachineId { get; set; }
    public int OperatorId { get; set; }
    

    public string MachineCode { get; set; } = string.Empty;
    public string OperatorFirstname { get; set; } = string.Empty;
    public string OperatorLastname { get; set; } = string.Empty;
    public string ProjectName { get; set; } = "Unknown";

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? OutputQuantity { get; set; }
    public string? Notes { get; set; }
}