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
    public async Task TestChangingProjectState()
    {
        var workLog = new CreateWorkLogDto
        {
            MachineId = 1,
            OperatorId = 1,
            ProjectId = 1
        };
        
        var response = await _client.PostAsJsonAsync("api/WorkLogs/register-work-session", workLog);
        
        // Vraci spravny code
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        
        var responseData = await response.Content.ReadFromJsonAsync<CreateWorkLogResponseDto>();
        
        // Vraci validni Id noveho work logu
        Assert.NotNull(responseData);
        Assert.True(responseData.Id > 0);

        // muze se stat ze kdybychom toto delali mimo scope, tak nam vrati na
        // database je vzdy scoped - tvori se objekt DbContext pro kazdy request
        // pokud by to bylo v jednom scopu, tak ackoliv muze EF mit tu zmenu provedenou, tak muze mit zacachovanou nejakou verze tech jednotlivych objekut
        // kde se ty zmeny jeste nepropsaly
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();
            
            var updatedProject = await db.Projects.FindAsync(workLog.ProjectId);
            
            // kontrola, ze se prepsal status projektu - jakmile je u projektu alespon jeden WorkLog, tak je automaticky 'in_progress'
            Assert.NotNull(updatedProject);
            Assert.Equal("in_progress", updatedProject.Status);
        }
        
    }
}