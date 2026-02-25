using System.Net;
using System.Net.Http.Json;
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
    
    var newMachineType = new CreateMachineTypeDto 
    {
        Name = "Soustruh CNC",
        MaintenanceIntervalHours = 100
    };
    
    // creating new machine type
    var typeResponse = await _client.PostAsJsonAsync("api/Machines/create-machine-type", newMachineType);
    Assert.Equal(HttpStatusCode.Created, typeResponse.StatusCode);
    
    //get Id
    var typeData = await typeResponse.Content.ReadFromJsonAsync<CreateMachineTypeResponseDto>(); 
    Assert.NotNull(typeData);
    int generatedTypeId = typeData.Id;

    //create new machine with new machine type
    var newMachine = new CreateMachineDto
    {
        Code = "CNC-001",
        Status = "active",
        Location = "Hala A",
        MachineTypeId = generatedTypeId // Tady napojujeme ID z kroku 1!
    };
    
    //create new machine
    var machineResponse = await _client.PostAsJsonAsync("api/Machines/create-machine", newMachine);
    Assert.Equal(HttpStatusCode.Created, machineResponse.StatusCode);

    // get id of new mahcine 
    var machineData = await machineResponse.Content.ReadFromJsonAsync<CreateMachineResponseDto>();
    Assert.NotNull(machineData);
    int generatedMachineId = machineData.Id;

    
    var getResponse = await _client.GetAsync($"api/Machines/{generatedMachineId}");
    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
}
    
    
}
