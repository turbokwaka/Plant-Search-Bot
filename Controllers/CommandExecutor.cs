using GardenBot.Controllers;
using GardenBot.Controllers.Commands;
using Telegram.Bot.Types;

public class CommandExecutor : ITelegramUpdateListener
{
    private List<ICommand> commands;
    private IListener? listener = null;
    private ICommand notFoundCommand;
    

    public CommandExecutor()
    {
        commands = new List<ICommand>()
        {
            new StartCommand(),
            new NotFoundCommand(),
            new MenuCommand(),
            new LoveCommand(this),
            new SearchCommand(this)
        };
        notFoundCommand = new NotFoundCommand();
    }

    public async Task GetUpdate(Update update)
    {
        if (listener == null)
        {
            await ExecuteCommand(update);
        }
        else
        {
            await listener.GetUpdate(update);
        }
    }

    private async Task ExecuteCommand(Update update)
    {
        Message msg = update.Message;
        bool commandFound = false;
        foreach (var command in commands)
        {
            if (command.Name == msg?.Text || command.Name == update.CallbackQuery?.Data)
            {
                commandFound = true;
                await command.Execute(update);
            }
        }

        if (!commandFound)
            notFoundCommand.Execute(update);
    }

    public void StartListen(IListener newListener)
    {
        listener = newListener;
    }

    public void StopListen()
    {
        listener = null;
    }
}