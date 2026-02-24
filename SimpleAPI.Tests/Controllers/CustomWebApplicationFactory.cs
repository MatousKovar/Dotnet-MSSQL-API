using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SimpleAPI.Data;
using SimpleAPI.Models;

namespace SimpleAPI.Tests.Controllers;

/**
 * Je potreva vytvorit custom factory, protoze pri spusteni se pomoci DI injectuje hlavni databaze
 * A zde v testech by se injecotavala druha - nelze
 * Tady se jen provaci to ze se smaze pripojena DB a potom se pripoji ke specialni InMemory databazi, ktera je pouzivana na testy
 * zaroven jsou vlozeny zakladni data - test vstupu
 */
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MachineDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            
            var inMemoryServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            
            services.AddDbContext<MachineDbContext>(options => 
            {
                options.UseInMemoryDatabase("GlobalSharedTestDb");
                options.UseInternalServiceProvider(inMemoryServiceProvider);
            });
        });
    }
    
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Necháme .NET postavit celou aplikaci (tím se finálně aplikuje In-Memory)
        var host = base.CreateHost(builder);

        // Teď už můžeme bezpečně vytáhnout databázi a naplnit ji
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();
            
            db.Database.EnsureCreated();
                
            // Vložíme naše základní data
            if (!db.Machines.Any())
            {
                db.Machines.Add(new Machine { Id = 1, Code = "M1", Status = "active" });
                db.Operators.Add(new Operator { Id = 1, FirstName = "Jan", LastName = "Novak",BadgeNumber = "C1"});
                db.Projects.Add(new Project { Id = 1, Name = "Valid Project", Status = "planned" });
                
                db.SaveChanges(); 
            }
        }

        return host;
    }
}