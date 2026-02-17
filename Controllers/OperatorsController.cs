using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.DTOs;
namespace SimpleAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OperatorsController(MachineDbContext context) : ControllerBase
{
    // /api/Operators
    [HttpGet]
    public async Task<ActionResult<List<OperatorDto>>>GetOperators()
    {
        var dto = await context.Operators
            .OrderBy(op => op.LastName)
            .ThenBy(op => op.FirstName)
            .Select(op => new OperatorDto{
                Id = op.Id,
                FirstName = op.FirstName,
                LastName = op.LastName,
                BadgeNumber = op.BadgeNumber,
                Email = op.Email,
                IsActive = op.IsActive
            })
            .ToListAsync();

        return Ok(dto);
    }
}