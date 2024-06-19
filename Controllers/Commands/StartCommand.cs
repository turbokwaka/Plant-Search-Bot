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
        await Client.SendTextMessageAsync(chatId, "Привіт! Я Чіназес Бот \ud83c\udf3f Ваші рослини тепер під моїм крилом! " +
                                                  "Додайте зелених друзів і вперед до процвітання! \ud83d\udcaa\ud83d\ude0e!" +
                                                  "\n\n...пс... спробуй ввести /лоарівллоівпрвіло .........");
    }
}