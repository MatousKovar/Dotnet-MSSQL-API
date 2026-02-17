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
}