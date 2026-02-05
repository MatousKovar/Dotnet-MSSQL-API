using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.Models;

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
    public async Task<ActionResult<List<Machine>>> getMachines()
    {
        List<Machine>? machines = await _context.Machines.Include(m => m.MachineType).ToListAsync();

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