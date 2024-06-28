using Telegram.Bot;
using Telegram.Bot.Types;

namespace GardenBot.Controllers;

public class LoveCommand : ICommand, IListener
{
    public TelegramBotClient Client => Bot.GetTelegramBot();

    public string Name => "/лоарівллоівпрвіло";

    public CommandExecutor Executor { get; }

    public LoveCommand(CommandExecutor executor)
    {
        Executor = executor;
    }

    private string? _answer1 = null;
    private string? _answer2 = null;

    public async Task Execute(Update update)
    {
        long chatId = update.Message.Chat.Id;
        if (update.Message.Text == null) 
            return;
        
        Executor.StartListen(this);
        
        await Client.SendTextMessageAsync(chatId, "Як тебе звати?");
    }

    public async Task GetUpdate(Update update)
    {
        long chatId = update.Message.Chat.Id;
        if (update.Message.Text == null) 
            return;

        if (_answer1 == null)
        {
            if (update.Message.Text.ToLower() == "аделіна" || update.Message.Text.ToLower() == "ліна")
            {
                _answer1 = update.Message.Text;
                await Client.SendTextMessageAsync(chatId, "Привіт, кицунь)) Ти мене кохаєш?");
            }
            else
            {
                await Client.SendTextMessageAsync(chatId, "Відповідь неправильна. Як тебе звати?");
            }
        }
        else if (_answer2 == null)
        {
            string lowerText = update.Message.Text.ToLower();
            if (lowerText == "так" || lowerText == "звісно" || lowerText == "канєшно" || lowerText == "да")
            {
                _answer2 = update.Message.Text;
                await Client.SendTextMessageAsync(chatId, "І я тебе кохаю, сонечко <3");
                Executor.StopListen();
            }
            else
            {
                await Client.SendTextMessageAsync(chatId, "Відповідь неправильна. Привіт, кицунь)) Ти мене кохаєш?");
            }
        }
    }
}