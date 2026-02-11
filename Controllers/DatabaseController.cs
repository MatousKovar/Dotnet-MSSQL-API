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

    // All endpoints should return ActionResult<T> - contains HTTP status codes and data
    // Can return IActionResult, but that hides response type for swagger docs
    // Get all machines with their types
    // ActionResult class is a wrapper for HTTP responses - contains code, data and so on
    // almost always better to return async, worker does not have to wait for DB response
    // /api/Database/machines
    [HttpGet("machines")]
    public async Task<ActionResult<List<MachineDto>>> getMachines()
    {
        //Include looks for related data based on foreign keys - like JOIN in SQL
        //Can create cycles - that is what MachineDto is for, it is essentially a simplified version of the Machine model that only contains the data we want to return, and does not include navigation properties that could cause cycles
        List<MachineDto> machines = await _context.Machines
            .OrderBy(m => m.Code)
            .Include(m => m.MachineType)
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

    
    [HttpPost("registerWorkSession")]
    public async Task<ActionResult<CreateWorkLogResponseDto>> RegisterWorkSession(CreateWorkLogDto workLog)
    {
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
            StartTime = workLog.StartTime,
            EndTime = workLog.EndTime,
            OutputQuantity = workLog.OutputQuantity,
            Notes = workLog.Notes
        };

        _context.WorkLogs.Add(newLog);
        await _context.SaveChangesAsync();

        //Best practice to return Created code 201 with location of new resource and its ID, so that client can easily access it
        return CreatedAtAction(
            nameof(GetLogById), // method to find inserted log
            new { id = newLog.Id }, // URL parameters for GetLogById
            new CreateWorkLogResponseDto{ Id = newLog.Id }); // return object
    }

    [HttpGet("workLogs/{id}")]
    public async Task<ActionResult<WorkLogDto>> GetLogById([FromRoute] int id)
    {
        var logDto = await _context.WorkLogs
        .Where(log => log.Id == id)
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
        }).FirstOrDefaultAsync();

        if (logDto == null)
        {
            return NotFound($"Work log with ID {id} not found.");
        }

        return Ok(logDto);
    }


    // function sets end time of session when receives HttpPut request with valid work_log_id
    [HttpPut("endWorkSession")]
    public async Task<ActionResult<WorkLogDto>> EndWorkSession(int workLogId)
    {
       
        WorkLog? workLog = await _context.WorkLogs.FindAsync(workLogId);

        if(workLog == null)
        {
            return NotFound($"Work session with id: {workLogId} not found.");
        }

        workLog.EndTime = DateTime.UtcNow; // timezone running on server
        await _context.SaveChangesAsync();

        var resultDto = new WorkLogDto 
        {
            Id = workLog.Id,
            MachineId = workLog.MachineId,
            OperatorId = workLog.OperatorId,

            StartTime = workLog.StartTime,
            EndTime = workLog.EndTime,
            OutputQuantity = workLog.OutputQuantity,
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
    [HttpGet("getRecentWorklogs")]
    public async Task<ActionResult<List<WorkLogDto>>> GetRecentWorkLogs([FromQuery] int take = 50, [FromQuery] int skip = 0)
    {
        var skip_take = ValidateSkipAndTake(skip, take);
        if(skip_take != null)
        {
            return BadRequest(skip_take);
        }

        var logs = await _context.WorkLogs
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
               OutputQuantity = log.OutputQuantity,
               Notes = log.Notes,
        
               MachineCode = log.Machine.Code,
               OperatorFirstname = log.Operator.FirstName,
               OperatorLastname = log.Operator.LastName,
               ProjectName = log.Project.Name
           }).ToListAsync();
        return Ok(logs);
    }

    [HttpGet("operators")]
    public async Task<ActionResult<List<OperatorDto>>>GetOperators()
    {
        var dto = await _context.Operators
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

    [HttpGet("projects")]
    public async Task<ActionResult<List<ProjectDto>>>GetProjects()
    {
        var dto = await _context.Projects
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

    [HttpGet("workLogsForProject/{projectId}")]
    public async Task<ActionResult<List<WorkLogDto>>>GetWorkLogsForProject(int projectId, [FromQuery] int take = 50, [FromQuery] int skip = 0)
    {
        var skip_take = ValidateSkipAndTake(skip,take);
        if(skip_take != null)
            return BadRequest(skip_take);

        var workLogs = await _context.WorkLogs
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

    private string ValidateSkipAndTake(int skip, int take)
    {
        if(skip < 0)
        {
            return "Skip parameter cannot be negative number.";
        }

        if (take <= 0 )
        {
            return "Take parameter cannot be negative number or zero.";
        }

        if(take > 1000)
        {
            return "Take parameter can be at most 1000.";
        }

        return null;
    }

    // Generic function for checking if IDs exist in table T 
    private async Task<bool> IdExists<T>(int id) where T : class
    {
        // Cannot call e.Id - need to use EF.Property to access the property by name, because T is not known at compile time
        // EF.Property is a function which can tell compiler that there is property "Id" on object e 
        return await _context.Set<T>().AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }

    // checks input, returns null if input is in order
    private async Task<string?> ValidateWorkLogAsync(CreateWorkLogDto workLog)
    {
        if (!await IdExists<Machine>(workLog.MachineId))
        {
            return $"Machine ID {workLog.MachineId} does not exist.";
        }

        if (!await IdExists<Operator>(workLog.OperatorId))
        {
            return $"Operator ID {workLog.OperatorId} does not exist.";
        }

        if (!await IdExists<Project>(workLog.ProjectId))
        {
            return $"Project ID {workLog.ProjectId} does not exist.";
        }

        if (workLog.StartTime > DateTime.Now)
        {
            return "Start time cannot be in the future.";
        }

        if (workLog.EndTime.HasValue && workLog.EndTime > DateTime.Now)
        {
            return "End time cannot be in the future.";
        }

        return null;
    }

}