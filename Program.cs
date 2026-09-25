using TestJob.Data;
using TestJob.Repositories;
using TestJob.Services;

namespace TestJob;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        
        builder.Services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
        
        builder.Services.AddScoped<IElementsRepository, ElementsRepository>();

        builder.Services.AddScoped<IParseService, ParseService>();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}