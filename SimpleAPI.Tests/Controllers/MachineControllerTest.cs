namespace SimpleAPI.Tests.Controllers;

public class MachineControllerTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task TestGetMachine()
    {
        var response = await _client.GetAsync("api/Machines/1");
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }
    
    
}
