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
    private struct maintainanceInfo
    {
        DateTime lastMaintainance;
        DateTime plannedMaintainance;
        int MaintenanceIntervalHours;
    }
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

    [HttpGet("machine-maintenance-overdue")]
    public async Task<ActionResult<List<MachineDto>>> GetMachinesWithOverdueMaintenance()
    {
        DateTime today = DateTime.UtcNow.Date;

        List<MachineDto> machines = await context.Machines
            .Where(m => m.NextMaintenanceDate < today)
            .OrderBy(m => m.NextMaintenanceDate)
            .Include(m => m.MachineType)
            .Select(m => new MachineDto
            {
                Code = m.Code,
                Status = m.Status,
                Location = m.Location,
                NextMaintenanceDate = m.NextMaintenanceDate,
                MachineTypeName = m.MachineType!.Name ?? "Unknown"
            })
            .ToListAsync();

        var machinesAfterDuedate = new List<MachineMaintainanceInfoDto>();


        foreach (var machine in machines)
        {
            var expectedMaintainanceDuedate = GetLastMaintenanceDateAsync(machine.Id);
            if (expectedMaintainanceDuedate == null || expectedMaintainanceDuedate < today)
            {
                var maintainanceDto = 
            }
                machinesAfterDuedate.Add(machine);    
        }


        if (machinesAfterDuedate.Count == 0)
        {
            return NotFound("No machines with overdue maintenance found");
        }

        return Ok(machines);
    }

    // Helper method to calculate the last maintenance date for machine
    // returns null if no maintenance logs found for the machine
    private async Task<DateTime?> GetLastMaintenanceDateAsync(int machineId)
    {
        var lastMaintenanceLog = await _context.WorkLogs
            .Where(wl => wl.MachineId == machineId && wl.WorkType.Name == "Maintenance") 
            .OrderByDescending(wl => wl.StartTime) 
            .Select(wl => new { wl.StartTime, wl.EndTime }) 
            .FirstOrDefaultAsync();
        if (lastMaintenanceLog == null) 
        {
            return null; 
        }

        return lastMaintenanceLog.EndTime ?? lastMaintenanceLog.StartTime;
    }

    //returns null if machine not found or if no maintenance logs found for the machine
    private async Task<DateTime?> GetNextMaintenanceDateAsync(int machineId)
    {
        DateTime? lastMaintenanceDate = await GetLastMaintenanceDateAsync(machineId);
        if (lastMaintenanceDate == null) 
        {
            return null; 
        }

        var machine = await _context.Machines.FindAsync(machineId);
        if (machine == null || machine.MachineType == null) 
        {
            return null; 
        }

        int maintenanceIntervalHours = machine.MachineType.MaintenanceIntervalHours;
        return new maintainanceInfo{ lastMaintenanceDate, lastMaintenanceDate.Value.AddHours(maintenanceIntervalHours), MaintenanceIntervalHours};
    }
}