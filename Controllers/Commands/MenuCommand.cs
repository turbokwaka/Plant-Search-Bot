using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GardenBot.Controllers;

public class MenuCommand : ICommand
{
    public TelegramBotClient Client => Bot.GetTelegramBot();

    public string Name => "menu";

    public async Task Execute(Update update)
    {
        var buttons = new InlineKeyboardMarkup(new[]
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("Search", "search"),
            }
        });

        string? firstName = update.Message?.Chat.FirstName ?? update.CallbackQuery!.Message!.Chat.FirstName;
        long chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.Message!.Chat.Id;
        string greetingMessage = string.IsNullOrEmpty(firstName) 
            ? "Hello!"
            : $"Hello, {firstName}!";
        
        await Client.SendTextMessageAsync(chatId, $"{greetingMessage}\n" +
                                                  $"Your plants are now under my care! " +
                                                  $"Add your green friends and let's thrive together! 💪😎",
            replyMarkup: buttons);
    }
}