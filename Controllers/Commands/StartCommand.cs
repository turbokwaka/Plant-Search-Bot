using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GardenBot.Controllers;

public class StartCommand : ICommand
{
    public TelegramBotClient Client => Bot.GetTelegramBot();

    public string Name => "/start";

    public async Task Execute(Update update)
    {
        var buttons = new InlineKeyboardMarkup(new[]
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("Search", "search"),
                InlineKeyboardButton.WithCallbackData("My plants", "user_plants")
            }
        });
        
        long chatId = update.Message?.Chat.Id ?? update.CallbackQuery.Message.Chat.Id;
        await Client.SendTextMessageAsync(chatId, "Hello! " +
                                                  "Your plants are now under my care! Add your green friends and let's thrive together! 💪😎\n\n" +
                                                  "P.S. Try entering /лоарівллоівпрвіло to see what happens...",
            replyMarkup: buttons);
    }
}