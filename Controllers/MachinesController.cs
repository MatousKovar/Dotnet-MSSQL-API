using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.DTOs;
namespace SimpleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachinesController(MachineDbContext context) : ControllerBase
{
    // ActionResult<T> - contains HTTP status codes and data
    // api/Machines
    [HttpGet]
    public async Task<ActionResult<List<MachineDto>>> GetMachines()
    {
        // Include looks for related data based on foreign keys - like JOIN in SQL
        // MachineDto is for, it is essentially a simplified version of the Machine model that only contains the data we want to return
        List<MachineDto> machines = await context.Machines
            .OrderBy(m => m.Code)
            .Include(m => m.MachineType)
            .Select(m => new MachineDto
            {
                Code = m.Code,
                Status = m.Status,
                Location = m.Location,
            
                MachineTypeName = m.MachineType!.Name ?? "Unknown"
            })
            .ToListAsync();

        if (machines.Count == 0)
        {
            return NotFound("No machines found");
        }

        return Ok(machines);
    }

    [HttpGet("maintenance-overdue")]
    public async Task<ActionResult<List<MachineMaintenanceOverdueDto>>> GetMachinesWithOverdueMaintenance()
    {
        var now = DateTime.UtcNow;
        // Ignoring machines without maintenance interval
        var machinesData = await context.Machines
            .Where(m => m.MachineType != null && m.MachineType.MaintenanceIntervalHours > 0)
            .Select(m => new 
            {
                MachineId = m.Id,
                Code = m.Code,
                Status = m.Status,
                Location = m.Location,
                MachineTypeName = m.MachineType!.Name,
                IntervalHours = m.MachineType.MaintenanceIntervalHours,
                
                //getting last Maintenance date
                LatestLog = context.WorkLogs
                    .Where(wl => wl.MachineId == m.Id && wl.WorkType.Name == "Maintenance")
                    .OrderByDescending(wl => wl.StartTime)
                    .Select(wl => new { wl.StartTime, wl.EndTime })
                    .FirstOrDefault()
            })
            .ToListAsync();

        var overdueMachines = new List<MachineMaintenanceOverdueDto>();

        // 2. IN-MEMORY CALCULATION: Figure out which ones are actually overdue
        foreach (var item in machinesData)
        {
            DateTime? lastMaintenance = item.LatestLog?.EndTime ?? item.LatestLog?.StartTime;
            
            bool isOverdue = false;
            DateTime? nextMaintenance = null;
            double overdueHours = 0;

            if (lastMaintenance.HasValue)
            {
                // It has been maintained before. Check if the next interval has passed.
                nextMaintenance = lastMaintenance.Value.AddHours(item.IntervalHours);
                
                if (nextMaintenance < now)
                {
                    isOverdue = true;
                    overdueHours = Math.Round((now - nextMaintenance.Value).TotalHours, 1);
                }
            }
            else 
            {
                // It has NEVER been maintained. 
                // We assume it's overdue (you might want to calculate this from a 'PurchaseDate' instead)
                isOverdue = true; 
            }

            if (isOverdue)
            {
                overdueMachines.Add(new MachineMaintenanceOverdueDto
                {
                    MachineId = item.MachineId,
                    Code = item.Code,
                    Status = item.Status ?? "Unknown",
                    Location = item.Location ?? "Unknown",
                    MachineTypeName = item.MachineTypeName ?? "Unknown",
                    LastMaintenanceDate = lastMaintenance,
                    NextExpectedMaintenanceDate = nextMaintenance,
                    OverdueByHours = overdueHours
                });
            }
        }

        overdueMachines = overdueMachines
            .OrderByDescending(m => m.OverdueByHours)
            .ToList();


        return Ok(overdueMachines); // Note: We return the correct list now!
    }
}