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

    /// <summary>
    /// Returns work logs for current project ordered by time descending
    /// </summary>
    /// <param name="take">how many work logs to return, can be max 1000</param>
    /// <param name="skip">how many work logs to skip</param>
    /// <returns></returns>
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

    //TODO when someone starts working in project that is only planned - it should be switched to in_progress
    [HttpPost("create-project")]
    public async Task<ActionResult<CreateProjectDto>> CreateProject(CreateProjectDto projectDto)
    {
        var newProject = new Project{
            Name = projectDto.Name,
            ClientName = projectDto.ClientName,
            Deadline = projectDto.Deadline,
            Status = "planned",
        };
        
        context.Projects.Add(newProject);
        await context.SaveChangesAsync();
        
        return CreatedAtAction(
            nameof(GetProjectById), // method to find inserted log
            new { id = newProject.Id }, // URL parameters for GetProject
            new CreateWorkLogResponseDto{ Id = newProject.Id });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
    {
        var projectDto = await context.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                ClientName = p.ClientName,
                Deadline = p.Deadline,
                Status = p.Status,
            })
            .FirstOrDefaultAsync();
        if (projectDto == null)
        {
            return NotFound("No project with id " + id);
        }
        return Ok(projectDto);
    }
}