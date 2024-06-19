using Telegram.Bot;
using Telegram.Bot.Types;

namespace GardenBot.Controllers.Commands;

public class NotFoundCommand : ICommand
{
    public TelegramBotClient Client => Bot.GetTelegramBot();
    public string Name => "";

    public async Task Execute(Update update)
    {
        long chatId = update.Message.Chat.Id;
        await Client.SendTextMessageAsync(chatId, "Я не знаю такої команди :(. Можливо, ти ввів її неправильно..");
    }
}