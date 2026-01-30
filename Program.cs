using System.Data;
using SimpleAPI.Data;
using Microsoft.EntityFrameworkCore;


namespace SimpleAPI;

public class Program
{
     public static void Main(string[] args)
    {
       
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")  // sets from connection string from config file. AddDbContext is handled by MachineDbContext constructor
            ?? throw new InvalidOperationException("Connection string"
            + "'DefaultConnection' not found.");
        builder.Services.AddDbContext<MachineDbContext>(options => options.UseSqlServer());

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();        

        builder.Services.AddControllers(); // scans directory for classes with ending Controller


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection(); // for redirecting http to https

        app.MapControllers();

        app.Run();
    }

}