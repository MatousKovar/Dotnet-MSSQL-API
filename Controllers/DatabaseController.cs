using Microsoft.AspNetCore.Mvc;

namespace SimpleAPI;


/***
* 
*/
[ApiController]
[Route("api/[controller]")] // controller takes the name of class and removes Contoller from it - thi is accessible via https://<hostaddress>/api/Database
public class DatabaseController : ControllerBase
{
    // Passing correct output parameter instead of IActionResult automatically generates swagger docs
    [HttpGet("hello")] // /api/Database/hello
    public ActionResult<string> getWelcomeMessage()
    {
        return Ok($"Hello world");
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