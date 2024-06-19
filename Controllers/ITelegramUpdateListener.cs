using Telegram.Bot.Types;

namespace GardenBot.Controllers;

public interface ITelegramUpdateListener
{
    public async Task GetUpdate(Update update) { }
}