using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SimpleAPI.Data;
using SimpleAPI.DTOs;
using SimpleAPI.Models;

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

    [Fact]
    public async Task GetMachinesWithOverdueMaintenance_ReturnsCorrectMachines()
    {
        // Overdue Logic
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();
            var now = DateTime.UtcNow;


            var maintenanceWorkType = new WorkType { Id = 99, WorkName = "Maintenance" };
            if (!db.WorkTypes.Any(wt => wt.Id == 99)) db.WorkTypes.Add(maintenanceWorkType);


            var machineType = new MachineType { Id = 99, Name = "Typ s údržbou", MaintenanceIntervalHours = 10 };
            if (!db.MachineTypes.Any(mt => mt.Id == 99)) db.MachineTypes.Add(machineType);

            // machine has indefined type - no maintenance needed
            db.Machines.Add(new Machine { Id = 101, Code = "BEZ-TYPU", MachineTypeId = null });

            // no maintenance
            db.Machines.Add(new Machine { Id = 102, Code = "NIKDY", MachineTypeId = 99 });

            // Machine was maintained 2 hours ago, 10 is limit
            db.Machines.Add(new Machine { Id = 103, Code = "OK", MachineTypeId = 99 });
            db.WorkLogs.Add(new WorkLog
            {
                Id = 103, MachineId = 103, WorkTypeId = 99,
                StartTime = now.AddHours(-2), EndTime = now.AddHours(-1)
            });

            // machine is overdue
            db.Machines.Add(new Machine { Id = 104, Code = "POZDE", MachineTypeId = 99 });
            db.WorkLogs.Add(new WorkLog
            {
                Id = 104, MachineId = 104, WorkTypeId = 99,
                StartTime = now.AddHours(-21), EndTime = now.AddHours(-20) // Konec údržby před 20h
            });

            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/Machines/maintenance-overdue");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var overdueMachines = await response.Content.ReadFromJsonAsync<List<MachineMaintenanceOverdueDto>>();
        Assert.NotNull(overdueMachines);


        Assert.Contains(overdueMachines, m => m.Code == "NIKDY");
        Assert.Contains(overdueMachines, m => m.Code == "POZDE");

        Assert.DoesNotContain(overdueMachines, m => m.Code == "BEZ-TYPU");
        Assert.DoesNotContain(overdueMachines, m => m.Code == "OK");

        
        var pozdeStroj = overdueMachines.First(m => m.Code == "POZDE");
        
        Assert.True(pozdeStroj.OverdueByHours > 9.5 && pozdeStroj.OverdueByHours < 10.5);
    }
}
