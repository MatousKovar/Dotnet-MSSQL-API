using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.DTOs;
using SimpleAPI.Helpers;
namespace SimpleAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProjectsController(MachineDbContext context) : ControllerBase
{
    // /api/Projects
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>>GetProjects()
    {
        var dto = await context.Projects
            .OrderBy(proj => proj.Name)
            .Select(proj => new ProjectDto{
                Id = proj.Id,
                Name = proj.Name,
                ClientName = proj.ClientName,
                Deadline = proj.Deadline,
                Status = proj.Status,
            })
            .ToListAsync();

        return Ok(dto);
    }
    
    
    [HttpGet("work-logs-for-project/{projectId}")]
    public async Task<ActionResult<List<WorkLogDto>>>GetWorkLogsForProject(int projectId, [FromQuery] int take = 50, [FromQuery] int skip = 0)
    {
        var skipTake = Helper.ValidateSkipAndTake(skip,take);
        if(skipTake != null)
            return BadRequest(skipTake);

        var workLogs = await context.WorkLogs
            .Where(log => log.ProjectId == projectId)
            .OrderByDescending(log => log.StartTime)
            .Take(take)
            .Skip(skip)
            .Select(log => new WorkLogDto
            {
                Id = log.Id,
                MachineId = log.MachineId,
                OperatorId = log.OperatorId,
            
                StartTime = log.StartTime,
                EndTime = log.EndTime,
                OutputQuantity = log.OutputQuantity,
                Notes = log.Notes,

                MachineCode = log.Machine.Code,
                OperatorFirstname = log.Operator.FirstName,
                OperatorLastname = log.Operator.LastName,
                ProjectName = log.Project.Name
            })
            .ToListAsync();

        if(workLogs.Count == 0)
        {
            return NotFound($"No work logs for project id {projectId}");
        }

        return Ok(workLogs);
    }
}