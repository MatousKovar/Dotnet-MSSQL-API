using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.DTOs;
using SimpleAPI.Helpers;
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
                MachineTypeId = m.MachineTypeId
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
                    .Where(wl => wl.MachineId == m.Id && wl.WorkType.WorkName == "Maintenance")
                    .OrderByDescending(wl => wl.StartTime)
                    .Select(wl => new { wl.StartTime, wl.EndTime })
                    .FirstOrDefault()
            })
            .ToListAsync();

        var overdueMachines = new List<MachineMaintenanceOverdueDto>();

        foreach (var item in machinesData)
        {
            DateTime? lastMaintenance = item.LatestLog?.EndTime ?? item.LatestLog?.StartTime;
            
            bool isOverdue = false;
            DateTime? nextMaintenance = null;
            double overdueHours = 0;

            if (lastMaintenance.HasValue)
            {
                // It has been maintained before. Check if the next interval has passed.
                nextMaintenance = lastMaintenance.Value.AddHours(item.IntervalHours.Value);
                
                if (nextMaintenance < now)
                {
                    isOverdue = true;
                    overdueHours = Math.Round((now - nextMaintenance.Value).TotalHours, 1);
                }
            }
            else 
                isOverdue = true; 

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


        return Ok(overdueMachines);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MachineDto>> GetMachineById([FromRoute] int id)
    {
        
        var response = await context.Machines.FindAsync(id);
        if (response == null)
            return NotFound($"Machine with id {id} not found.");

        var machineDto = new MachineDto
        {
            Code = response.Code,
            Status = response.Status,
            Location = response.Location,
            MachineTypeId = response.MachineTypeId ?? null,
        };
        
        return Ok(response);
    }

    [HttpPost("create-machine")]
    public async Task<ActionResult<CreateMachineResponseDto>> CreateMachine(CreateMachineDto machine)
    {
        bool validateMachineTypeId = await IdExists<MachineType>(machine.MachineTypeId);
        
        if  (!validateMachineTypeId) return BadRequest("Invalid machine type id.");

        Machine newMachine = new Machine
        {
            Code = machine.Code,
            Status = machine.Status,
            Location = machine.Location,
            MachineTypeId = machine.MachineTypeId,
            PurchaseDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        
        context.Machines.Add(newMachine);
        await context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetMachineById), new { id = newMachine.Id }, new CreateMachineResponseDto {Id =  newMachine.Id});
    }
    
    [HttpPost("create-machine-type")]
    public async Task<ActionResult<CreateMachineTypeResponseDto>> CreateMachineType(CreateMachineTypeDto machineType)
    {
        MachineType newMachine = new MachineType
        {
            Name = machineType.Name,
            MaintenanceIntervalHours = machineType.MaintenanceIntervalHours,
            CreatedAt = DateTime.UtcNow
        };
        
        context.MachineTypes.Add(newMachine);
        await context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetMachineById), new { id = newMachine.Id }, new CreateMachineResponseDto {Id =  newMachine.Id});
    }


    [HttpGet("machine-types")]
    public async Task<ActionResult<List<MachineTypeDto>>> GetMachineTypes()
    {   

        var machineTypes =  await context.MachineTypes
            .Select(mt => new MachineTypeDto
            {
                Id = mt.Id,
                Name = mt.Name,
                MaintenanceIntervalHours = mt.MaintenanceIntervalHours,
                CreatedAt = mt.CreatedAt
            })
            .ToListAsync();

        return Ok(machineTypes);
    }
    
    [HttpGet("machine-type/{id}")]
    public async Task<ActionResult<MachineTypeDto>> GetMachineTypes([FromRoute] int id)
    {   
        
        var machineType = await context.MachineTypes.FindAsync(id);
        if (machineType == null) return NotFound($"Machine type with id {id} not found.");

        var machineTypeDto = new MachineTypeDto
        {
            Id = machineType.Id,
            Name = machineType.Name,
            MaintenanceIntervalHours = machineType.MaintenanceIntervalHours,
            CreatedAt = machineType.CreatedAt
        };
        
        return Ok(machineTypeDto);
    }
    
    private async Task<bool> IdExists<T>(int id) where T : class
    {
        // Cannot call e.Id - need to use EF.Property to access the property by name, because T is not known at compile time
        // EF.Property is a function which can tell compiler that there is property "Id" on object e 
        return await context.Set<T>().AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }
}

