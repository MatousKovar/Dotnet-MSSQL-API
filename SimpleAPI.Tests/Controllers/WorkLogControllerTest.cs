using SimpleAPI.DTOs;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SimpleAPI.Data;

namespace SimpleAPI.Tests.Controllers;
public class WorkLogsControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();


    [Fact]
    public async Task RegisterWorkSessionEdgeCases()
    {
        var requestDto = new CreateWorkLogDto
        {
            MachineId = 1,
            OperatorId = 1,
            ProjectId = 99 
        };
        
        //testing non existing project
        var response = await _client.PostAsJsonAsync("api/WorkLogs/register-work-session", requestDto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        requestDto.OperatorId = 99;
        requestDto.ProjectId = 1;
        // non existent operator ID
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        requestDto.MachineId = 99; 
        requestDto.OperatorId = 1;
        // non existent Machine ID
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestingRegisterWorkSessionEndToEnd()
    {
        var workLog = new CreateWorkLogDto
        {
            MachineId = 1,
            OperatorId = 1,
            ProjectId = 1
        };
        
        // check if code for registering is ok
        var response = await _client.PostAsJsonAsync("api/WorkLogs/register-work-session", workLog);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        
        //checking if project
        var projectResponse = await _client.GetAsync($"api/Projects/{workLog.ProjectId}");
        Assert.Equal(HttpStatusCode.OK, projectResponse.StatusCode);


        var projectData = await projectResponse.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(projectData);
        Assert.Equal("in_progress", projectData.Status);
        
        var httpResponseMessage = await _client.GetAsync($"api/Projects/work-logs-for-project/{workLog.ProjectId}");
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);


        var returnedLogs = await httpResponseMessage.Content.ReadFromJsonAsync<List<WorkLogDto>>();
        Assert.NotNull(returnedLogs);
        Assert.NotEmpty(returnedLogs);
        
        
    }
}