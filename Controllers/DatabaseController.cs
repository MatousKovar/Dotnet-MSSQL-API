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
    // /api/Database/machines
    // ActionResult class is a wrapper for HTTP responses - contains code, data and so on
    // Can return IActionResult, but that hides response type for swagger docs
    // almost always better to return async, worker does not have to wait for DB response
    [HttpGet("machines")]
    public async Task<ActionResult<List<MachineDto>>> getMachines()
    {
        var machines = await _context.Machines
        .Include(m => m.MachineType) // Still need to include to get the data
        .Select(m => new MachineDto
        {
            // Left side is DTO, Right side is Database Entity
            Code = m.Code,
            Status = m.Status,
            Location = m.Location,
            
            // FLATTENING: Grab the nested name and put it at the top level
            MachineTypeName = m.MachineType.Name 
        })
        .ToListAsync();

        // Now 'machines' is a List<MachineDto>, which has no cycles!
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