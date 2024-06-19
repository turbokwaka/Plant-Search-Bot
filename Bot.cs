using Telegram.Bot;

namespace GardenBot;

public class Bot
{
    private static TelegramBotClient client { get; set; }

    public static TelegramBotClient GetTelegramBot()
    {
        if (client != null)
        {
            return client;
        }
        client = new TelegramBotClient("YOUR_TOKEN");
        return client;
    }
}