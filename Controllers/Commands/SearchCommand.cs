using GardenBot.Models;
using GardenBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GardenBot.Controllers.Commands;

public class SearchCommand : ICommand, IListener
{
    public TelegramBotClient Client => Bot.GetTelegramBot();
    public string Name => "Знайти рослинку\ud83c\udf31";
    public CommandExecutor Executor { get; }
    private PlantSearchService _plantSearchService = new PlantSearchService();

    public SearchCommand(CommandExecutor executor)
    {
        Executor = executor;
    }

    private string? _answer1 = null;

    public async Task Execute(Update update)
    {
        long chatId = update.Message!.Chat.Id;
        if (update.Message.Text == null) 
            return;

        Executor.StartListen(this); 
        
        await Client.SendTextMessageAsync(chatId, "Введи назву рослини");
    }

    public async Task GetUpdate(Update update)
    {
        long chatId = update.Message!.Chat.Id;
        if (update.Message.Text == null) 
            return;

        _answer1 = update.Message.Text;
        var plant = await _plantSearchService.GetPlant(_answer1);

        if (plant == null)
        {
            await Client.SendTextMessageAsync(chatId, "Please, enter a name of a plant.");
            return;
        }
        
        InputFile photo = InputFile.FromUri(plant.ImageUrl);
        await Client.SendPhotoAsync(chatId, photo, 
            caption: $"Here is your plant - {plant.CommonName}" +
                     $"\n(scientific name: {plant.ScientificName})" +
                     $"\n\nCycle: {plant.Cycle}" +
                     $"\nWatering: {plant.Watering}" +
                     $"\nSunlight: {plant.Sunligt}" +
                     $"\n\nYou've been searching for this plant?");
    }
}