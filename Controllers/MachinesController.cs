using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.DTOs;
namespace SimpleAPI.Controllers;

[ApiController]
public class MachinesController(MachineDbContext context) : ControllerBase
{
    private readonly MachineDbContext _dbcontext = context;
    
    // All endpoints should return ActionResult<T> - contains HTTP status codes and data
    // Can return IActionResult, but that hides response type for swagger docs
    // Get all machines with their types
    // ActionResult class is a wrapper for HTTP responses - contains code, data and so on
    // almost always better to return async, worker does not have to wait for DB response
    // /api/Database/machines
    [HttpGet("machines")]
    public async Task<ActionResult<List<MachineDto>>> GetMachines()
    {
        //Include looks for related data based on foreign keys - like JOIN in SQL
        //Can create cycles - that is what MachineDto is for, it is essentially a simplified version of the Machine model that only contains the data we want to return, and does not include navigation properties that could cause cycles
        List<MachineDto> machines = await _dbcontext.Machines
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
}