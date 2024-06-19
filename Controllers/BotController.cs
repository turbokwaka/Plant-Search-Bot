using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GardenBot.Controllers;

[ApiController]
[Route("/")]
public class BotController(UpdateDistributor<CommandExecutor> updateDistributor) : ControllerBase
{
    private TelegramBotClient _bot = Bot.GetTelegramBot();
    
    public async void Post(Update update)
    {
        Console.WriteLine("Update was received.");
        if (update.Message == null)
            return;

        await updateDistributor.GetUpdate(update);
    }

    [HttpGet]
    public string Get()
    {
        return "Telegram bot was started";
    }
}