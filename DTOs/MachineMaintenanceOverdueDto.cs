namespace SimpleAPI.DTOs;

public class MachineMaintenanceOverdueDto
{
    public int MachineId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string MachineTypeName { get; set; } = string.Empty;
    
    // Null if it has never been maintained
    public DateTime? LastMaintenanceDate { get; set; } 
    
    // When maintenance should have been
    public DateTime? NextExpectedMaintenanceDate { get; set; } 
    
    // How many hours past due is it? (Great for frontend sorting/coloring)
    public double OverdueByHours { get; set; } 
}