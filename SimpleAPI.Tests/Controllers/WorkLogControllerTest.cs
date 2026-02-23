using SimpleAPI.DTOs;
using System.Net;
using System.Net.Http.Json;

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
}