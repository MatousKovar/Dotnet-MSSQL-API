using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Data;
using SimpleAPI.DTOs;
using SimpleAPI.Models;

namespace SimpleAPI.Tests.Controllers;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

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
        var response = await _client.PostAsJsonAsync("/api/worklogs/register-work-session", requestDto);
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