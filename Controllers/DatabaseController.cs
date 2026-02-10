using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.DTOs;
namespace SimpleAPI;


/***
* 
*/
[ApiController]
[Route("api/[controller]")] // controller takes the name of class and removes Contoller from it - thi is accessible via https://<hostaddress>/api/Database
public class DatabaseController : ControllerBase
{
    private readonly MachineDbContext _context;    


    public DatabaseController(MachineDbContext context)
    {
        _context = context;        
    }

    // All endpoints should return ActionResult<T> - contains HTTP status codes and data
    // Can return IActionResult, but that hides response type for swagger docs
    // Get all machines with their types
    // ActionResult class is a wrapper for HTTP responses - contains code, data and so on
    // almost always better to return async, worker does not have to wait for DB response
    // /api/Database/machines
    [HttpGet("machines")]
    public async Task<ActionResult<List<MachineDto>>> getMachines()
    {
        //Include looks for related data based on foreign keys - like JOIN in SQL
        //Can create cycles - that is what MachineDto is for, it is essentially a simplified version of the Machine model that only contains the data we want to return, and does not include navigation properties that could cause cycles
        List<MachineDto> machines = await _context.Machines
            .Include(m => m.MachineType)
            .Select(m => new MachineDto
        {
            Code = m.Code,
            Status = m.Status,
            Location = m.Location,
            
            MachineTypeName = m.MachineType.Name ?? "Unknown"
        })
        .ToListAsync();

        if (machines == null || machines.Count == 0)
        {
            return NotFound("No machines found");
        }

        return Ok(machines);
    }

    
    [HttpPost("workLog")]
    public async Task<ActionResult<int>> acceptSession(CreateWorkLogDto workLog)
    {
        bool machineExists = await _context.Machines.AnyAsync(m => m.Id == workLog.MachineId);


        if (!await IdExists<Machine>(workLog.MachineId))
        {
            return BadRequest($"Machine ID {workLog.MachineId} does not exist.");
        }

        if (!await IdExists<Operator>(workLog.OperatorId))
        {
            return BadRequest($"Operator ID {workLog.OperatorId} does not exist.");
        }

        if (!await IdExists<Project>(workLog.ProjectId))
        {
            return BadRequest($"Project ID {workLog.ProjectId} does not exist.");
        }

        var newLog = new WorkLog
        {
            MachineId = workLog.MachineId,
            OperatorId = workLog.OperatorId,
            ProjectId = workLog.ProjectId, // Assuming 0 is a valid default or handle as needed
            StartTime = workLog.StartTime,
            EndTime = workLog.EndTime,
            OutputQuantity = workLog.OutputQuantity,
            Notes = workLog.Notes
        };

        _context.WorkLogs.Add(newLog);
        await _context.SaveChangesAsync();

        return Ok($"New log ID: {newLog.Id}");
    }


    [HttpGet] // eg: /api/Database?id=5 
    public ActionResult<string> acceptIDQuery(int? id)
    {

        if (id == null)
        {
            return BadRequest("ID parameter required");
        }
        return Ok($"Requested ID: {id}");
    }


    // Generic function for checking if IDs exist in table T 
    private async Task<bool> IdExists<T>(int id) where T : class
    {
        // Cannot call e.Id - need to use EF.Property to access the property by name, because T is not known at compile time
        // EF.Property is a function which can tell compiler that there is property "Id" on object e 
        return await _context.Set<T>().AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }

}