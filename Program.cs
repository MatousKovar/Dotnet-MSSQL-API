using System.Data;

namespace SimpleAPI;

public class Program
{
     public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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