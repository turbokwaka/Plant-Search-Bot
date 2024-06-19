using GardenBot.Controllers;

namespace GardenBot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddSingleton<UpdateDistributor<CommandExecutor>>();
        
        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}