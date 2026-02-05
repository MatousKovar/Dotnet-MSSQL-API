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

    // Passing correct output parameter instead of IActionResult automatically generates swagger docs
    [HttpGet("hello")] // /api/Database/hello
    public ActionResult<string> getWelcomeMessage()
    {
        return Ok($"Hello world");
    }

    // Get all machines with their types
    // ActionResult class is a wrapper for HTTP responses - contains code, data and so on
    // Can return IActionResult, but that hides response type for swagger docs
    // almost always better to return async, worker does not have to wait for DB response
    // /api/Database/machines
    [HttpGet("machines")]
    public async Task<ActionResult<List<MachineDto>>> getMachines()
    {
        //Include looks for related data based on foreign keys - like JOIN in SQL
        //Can create cycles - that is what MachineDto is for
        List<MachineDto> machines = await _context.Machines
        .Include(m => m.MachineType) // Still need to include to get the data
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

    [HttpGet("{id:int}")] // eg: /api/Database/5 
    public ActionResult<string> acceptID(int id)
    {
        return Ok($"Requested ID: {id}");
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

}