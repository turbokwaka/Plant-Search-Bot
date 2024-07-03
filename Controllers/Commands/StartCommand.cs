using GardenBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GardenBot.Controllers.Commands;

public class StartCommand : ICommand
{
    public TelegramBotClient Client => Bot.GetTelegramBot();
    public string Name => "/start";

    public async Task Execute(Update update)
    {
        var db = new DatabaseService();
        await db.PostUser(update);
        
        var menuCommand = new MenuCommand();
        await menuCommand.Execute(update);
    }
}