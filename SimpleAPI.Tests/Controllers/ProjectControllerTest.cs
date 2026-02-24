using SimpleAPI.DTOs;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SimpleAPI.Data;

namespace SimpleAPI.Tests.Controllers;

public class ProjectControllerTest(CustomWebApplicationFactory factory) :  IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task TestCreateProject()
    {
        var createProjectDto = new CreateProjectDto
        {
            Name = "TestProject",
        };
        
        var response = await _client.PostAsJsonAsync("/api/Projects/create-project", createProjectDto);
        // Created succesfully
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        // Ziskani id z response
        var responseData = await response.Content.ReadFromJsonAsync<CreateWorkLogResponseDto>();
        Assert.NotNull(responseData);
        var createdId  = responseData.Id;
        
        using (var scope = factory.Server.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();
            
            var newProject = await db.Projects.FindAsync(createdId);
            //new project exists
            Assert.NotNull(newProject);
            Assert.Equal("planned", newProject.Status);
        }
        
    }

    [Fact]
    public async Task TestWorkLogsForProject()
    {
        var reponse = await _client.PostAsync("/api/Projects/work-logs-for-project", new StringContent("1"));
        
    }
}