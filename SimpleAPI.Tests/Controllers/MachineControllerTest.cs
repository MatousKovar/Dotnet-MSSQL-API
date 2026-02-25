using System.Net;
using SimpleAPI.DTOs;

namespace SimpleAPI.Tests.Controllers;

public class MachineControllerTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task TestGetMachine()
    {
        var response = await _client.GetAsync("api/Machines/1");
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestMachinesEndToEnd()
    {
        var newMachine = MachineDto
        {
            
        }
        
    }
    
    
}
