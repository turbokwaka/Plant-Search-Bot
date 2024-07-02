using GardenBot.Controllers;
using GardenBot.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace GardenBot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddSingleton<UpdateDistributor<CommandExecutor>>();
        builder.Services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql("DatabaseConnectionString"));
        
        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}