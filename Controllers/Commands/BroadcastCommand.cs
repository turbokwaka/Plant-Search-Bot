using GardenBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GardenBot.Controllers.Commands
{
    public class BroadcastCommand : ICommand, IListener
    {
        public TelegramBotClient Client => Bot.GetTelegramBot();
        public string Name => "/broadcast";
        public CommandExecutor Executor { get; }
        private List<int> AllUsers { get; set; }
        private string? _pendingMessageText;
        private List<PhotoSize>? _pendingMessagePhotos;

        public BroadcastCommand(CommandExecutor executor)
        {
            Executor = executor;
        }

        public async Task Execute(Update update)
        {
            long chatId = update.Message.Chat.Id;
            var configuration = new MyConfiguration();

            if (!configuration.AdminUsers.Contains((int)chatId))
            {
                await Client.SendTextMessageAsync(chatId, "You cannot use this command");
                return;
            }

            var db = new DatabaseService();
            AllUsers = await db.GetAllUsers();

            var buttons = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Cancel", "menu")
            });

            string message = "Send me a message that I should broadcast to all users.\n" +
                             "Please group everything into one message. Do not use more than one photo.";

            await Client.SendTextMessageAsync(chatId, message, replyMarkup: buttons);

            Executor.StartListen(this);
        }

public async Task GetUpdate(Update update)
{
    long chatId = update.Message?.Chat.Id ?? update.CallbackQuery.Message.Chat.Id;
    
    if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Data == "menu")
    {
        await Client.SendTextMessageAsync(chatId, "Broadcast canceled.");
        Executor.StopListen();

        var menuCommand = new MenuCommand();
        await menuCommand.Execute(update);
        return;
    }

    if (update.Message != null)
    {
        _pendingMessageText = update.Message.Caption ?? update.Message.Text ;
        _pendingMessagePhotos = update.Message.Photo?.ToList();

        Console.WriteLine(_pendingMessageText);

        var buttons = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Yes", "broadcast_yes"),
                InlineKeyboardButton.WithCallbackData("No", "broadcast_no")
            }
        });

        string message = "Do you want to broadcast this message?";
        await Client.SendTextMessageAsync(chatId, message, replyMarkup: buttons);
        return;
    }

    if (update.Type == UpdateType.CallbackQuery)
    {
        if (update.CallbackQuery.Data == "broadcast_yes")
        {
            if (_pendingMessagePhotos != null && _pendingMessagePhotos.Any())
            {
                var photo = InputFile.FromFileId(_pendingMessagePhotos[0].FileId);

                foreach (var userId in AllUsers)
                {
                    await Client.SendPhotoAsync(userId, photo, caption: _pendingMessageText);
                }
            }
            else
            {
                foreach (var userId in AllUsers)
                {
                    await Client.SendTextMessageAsync(userId, _pendingMessageText);
                }
            }

            await Client.SendTextMessageAsync(chatId, "Message was broadcast to all users.");
            Executor.StopListen();
        }
        else if (update.CallbackQuery.Data == "broadcast_no")
        {
            var buttons = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Cancel", "menu")
            });
            
            await Client.SendTextMessageAsync(chatId, "Please send the message again."
                , replyMarkup: buttons);
            _pendingMessageText = null;
            _pendingMessagePhotos = null;
        }
    }
}
    }
}
