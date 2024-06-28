using Telegram.Bot.Types;

namespace GardenBot.Controllers;

public class UpdateDistributor<T> where T : ITelegramUpdateListener, new()
{
    private Dictionary<long, T> listeners;

    public UpdateDistributor()
    {
        listeners = new Dictionary<long, T>();
    }

    public async Task GetUpdate(Update update)
    {
        long chatId;
        if (update.Message != null)
            chatId = update.Message.Chat.Id;
        else if (update.CallbackQuery != null)
            chatId = update.CallbackQuery.Message.Chat.Id;
        else
            return;
        
        Console.WriteLine("Distributing update...");
        T? listener = listeners.GetValueOrDefault(chatId);
        if (listener is null)
        {
            listener = new T();
            listeners.Add(chatId, listener);
            await listener.GetUpdate(update);
            return;
        }

        Console.WriteLine("Getting update from listener");
        await listener.GetUpdate(update);
    }
}