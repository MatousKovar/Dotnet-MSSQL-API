using System.Net;
using System.Net.Http.Json;
using SimpleAPI.Data;
using SimpleAPI.DTOs;

namespace SimpleAPI.Tests.Controllers;

public class OperatorControllerTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task TestEndToEnd()
    {
        var newOperator = new CreateOperatorDto
        {
            FirstName = "John",
            LastName = "Doe",
            BadgeNumber = "123",
        };

        //test creating
        var response = await _client.PostAsJsonAsync("api/Operators/register-operator",newOperator);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        //get the id of returned operator
        var responseId = await response.Content.ReadFromJsonAsync<CreateOperatorResponseDto>();
        Assert.NotNull(responseId);
        
        //check if operator exists
        var responseSearch = await _client.GetAsync($"api/Operators/{responseId.Id}");
        Assert.Equal(HttpStatusCode.OK, responseSearch.StatusCode);
        var responseContent = await responseSearch.Content.ReadFromJsonAsync<OperatorDto>();
        Assert.NotNull(responseContent);
    }
}