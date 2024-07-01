using Telegram.Bot;
using Telegram.Bot.Types;

namespace GardenBot.Controllers;

public class StartCommand : ICommand
{
    public TelegramBotClient Client => Bot.GetTelegramBot();

    public string Name => "/start";

    public async Task Execute(Update update)
    {
        long chatId = update.Message.Chat.Id;
        await Client.SendTextMessageAsync(chatId, "Hello! " +
                                                  "Your plants are now under my care! Add your green friends and let's thrive together! 💪😎\n\n" +
                                                  "P.S. Try entering /лоарівллоівпрвіло to see what happens...");
    }
}