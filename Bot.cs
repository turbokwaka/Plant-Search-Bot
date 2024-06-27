using Telegram.Bot;

namespace GardenBot;

public class Bot
{
    private static TelegramBotClient client { get; set; }

    public static TelegramBotClient GetTelegramBot()
    {
        var configuration = new MyConfiguration();
        if (client != null)
        {
            return client;
        }
        client = new TelegramBotClient(configuration.TelegramToken);
        return client;
    }
}