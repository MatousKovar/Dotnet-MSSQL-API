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

    /// <summary>
    /// DATETIME2 datatype and Utc timezone
    /// </summary>
    public DateTime StartTime { get; set; }
    /// <summary>
    /// DATETIME2 datatype and Utc timezone
    /// </summary>
    public DateTime? EndTime { get; set; }
    
    public string? Notes { get; set; }


}