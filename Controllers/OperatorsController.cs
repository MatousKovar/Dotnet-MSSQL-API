using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.DTOs;
using SimpleAPI.Models;

namespace SimpleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperatorsController(MachineDbContext context) : ControllerBase
{
    // /api/Operators
    [HttpGet]
    public async Task<ActionResult<List<OperatorDto>>> GetOperators()
    {
        var dto = await context.Operators
            .OrderBy(op => op.LastName)
            .ThenBy(op => op.FirstName)
            .Select(op => new OperatorDto
            {
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

    /// <summary>
    /// Creates operator who is inactive by default
    /// </summary>
    /// <param name="operatorDto"></param>
    [HttpPost("register-operator")]
    public async Task<ActionResult<List<CreateOperatorResponseDto>>> RegisterOperator(CreateOperatorDto operatorDto)
    {
        var op = new Operator
        {
            FirstName = operatorDto.FirstName,
            LastName = operatorDto.LastName,
            BadgeNumber = operatorDto.BadgeNumber,
            Email = operatorDto.Email,
            IsActive = false
        };
        context.Operators.Add(op);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOperators), new { id = op.Id }, new CreateOperatorResponseDto { Id = op.Id });
    }
}