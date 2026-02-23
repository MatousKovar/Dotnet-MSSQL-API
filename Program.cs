using SimpleAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection; 


namespace SimpleAPI;

public class Program
{
     public static void Main(string[] args)
    {
       
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")  // sets from connection string from config file. AddDbContext is handled by MachineDbContext constructor
            ?? throw new InvalidOperationException("Connection string"
            + "'DefaultConnection' not found.");


        //dependency injection can easily swap for test database if needed
        builder.Services.AddDbContext<MachineDbContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddEndpointsApiExplorer();

        // Pass path to XML file - for swagger to see comments
        builder.Services.AddSwaggerGen(options =>
            {
                //XML file named same as project
                //located in bin folder
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
            });     

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
