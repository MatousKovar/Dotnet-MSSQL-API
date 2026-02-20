using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.DTOs;
using SimpleAPI.Helpers;
namespace SimpleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkLogsController(MachineDbContext context) : ControllerBase
{
    [HttpPost("register-work-session")]
    public async Task<ActionResult<CreateWorkLogResponseDto>> RegisterWorkSession(CreateWorkLogDto workLog)
    {   
        // checking if ids mentioned in work-log exist
        string? validationError = await ValidateWorkLogAsync(workLog);
        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        
        var newLog = new WorkLog
        {
            MachineId = workLog.MachineId,
            OperatorId = workLog.OperatorId,
            ProjectId = workLog.ProjectId, // Assuming 0 is a valid default or handle as needed
            StartTime = DateTime.UtcNow, 
            EndTime = null,
            Notes = workLog.Notes
        };

        context.WorkLogs.Add(newLog);
        await context.SaveChangesAsync();

        // change status of project if possible
        var project = await context.Projects.FindAsync(workLog.ProjectId);
        if (project!.Status == null)
        {
            project.Status = "in_progress";
        }
        
        if (project!.Status == "planned")
        {
            project.Status = "in_progress";
        }
        
        
        //Best practice to return Created code 201 with location of new resource and its ID, so that client can easily access it
        return CreatedAtAction(
            nameof(GetLogById), // method to find inserted log
            new { id = newLog.Id }, // URL parameters for GetLogById
            new CreateWorkLogResponseDto{ Id = newLog.Id }); // return object
    }

    
    [HttpGet("work-logs/{id}")]
    public async Task<ActionResult<WorkLogDto>> GetLogById([FromRoute] int id)
    {
        var logDto = await context.WorkLogs
        .Where(log => log.Id == id)
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
        }).FirstOrDefaultAsync();
        
        if (logDto == null)
        {
            return NotFound($"Work log with ID {id} not found.");
        }

        return Ok(logDto);
    }


    // function sets end time of session when receives HttpPut request with valid work_log_id
    /// <summary>
    /// Sets end time to work log with given ID. Time is set as time when HttpPut was received by server
    /// </summary>
    // /api/WorkLogs/end-work-session
    [HttpPut("end-work-session")]
    public async Task<ActionResult<WorkLogDto>> EndWorkSession(int workLogId)
    {
        WorkLog? workLog = await context.WorkLogs.FindAsync(workLogId);

        if(workLog == null)
        {
            return NotFound($"Work session with id: {workLogId} not found.");
        }

        workLog.EndTime = DateTime.UtcNow; 
        await context.SaveChangesAsync();

        var resultDto = new WorkLogDto 
        {
            Id = workLog.Id,
            MachineId = workLog.MachineId,
            OperatorId = workLog.OperatorId,

            StartTime = workLog.StartTime,
            EndTime = workLog.EndTime,
            Notes = workLog.Notes,
            
            MachineCode = workLog.Machine.Code,
            OperatorFirstname = workLog.Operator.FirstName,
            OperatorLastname = workLog.Operator.LastName,
            ProjectName = workLog.Project.Name
        };

        return Ok(resultDto);
    }
    
    /// <summary>
    /// Retrieves a list of work logs, ordered by Start Time (newest first). Can return maximum 1000 logs per request.
    /// </summary>
    [HttpGet("get-recent-work-logs")]
    public async Task<ActionResult<List<WorkLogDto>>> GetRecentWorkLogs([FromQuery] int take = 50, [FromQuery] int skip = 0)
    {
        var skipTake = Helper.ValidateSkipAndTake(skip, take);
        if(skipTake != null)
        {
            return BadRequest(skipTake);
        }

        var logs = await context.WorkLogs
            .OrderByDescending(log => log.StartTime)
            .Skip(skip)
            .Take(take)
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
            }).ToListAsync();
        return Ok(logs);
    }
    
    
    // checks input, returns null if input is in order
    private async Task<string?> ValidateWorkLogAsync(CreateWorkLogDto workLog)
    {
        if (!await IdExists<Machine>(workLog.MachineId))
            return $"Machine ID {workLog.MachineId} does not exist.";

        if (!await IdExists<Operator>(workLog.OperatorId))
            return $"Operator ID {workLog.OperatorId} does not exist.";

        if (!await IdExists<Project>(workLog.ProjectId))
            return $"Project ID {workLog.ProjectId} does not exist.";

        return null;
    }

    // Generic function for checking if IDs exist in table T 
    private async Task<bool> IdExists<T>(int id) where T : class
    {
        // Cannot call e.Id - need to use EF.Property to access the property by name, because T is not known at compile time
        // EF.Property is a function which can tell compiler that there is property "Id" on object e 
        return await context.Set<T>().AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }
    
    [HttpGet("work-types")]
    public async Task<ActionResult<List<WorkType>>> GetWorkTypes()
    {
        var workTypes = await context.WorkTypes.ToListAsync();

        return Ok(workTypes);
    }
}