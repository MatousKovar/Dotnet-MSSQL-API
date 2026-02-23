using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleAPI.Data;
using SimpleAPI.Models;

namespace SimpleAPI.Tests.Controllers;

// Creates database with basic data
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MachineDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<MachineDbContext>(options => 
                options.UseInMemoryDatabase("GlobalSharedTestDb"));

            
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            
            var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();
                
            
            if (!db.Machines.Any())
            {
                db.Machines.Add(new Machine { Id = 1, Code = "M1", Status = "active" });
                db.Operators.Add(new Operator { Id = 1, FirstName = "Jan", LastName = "Novak" });
                db.Projects.Add(new Project { Id = 1, Name = "Valid Project", Status = "planned" });
                db.SaveChanges(); 
            }
        });
    }
}