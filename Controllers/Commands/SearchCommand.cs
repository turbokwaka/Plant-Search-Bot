using GardenBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GardenBot.Controllers.Commands;

public class SearchCommand : ICommand, IListener
{
    public TelegramBotClient Client => Bot.GetTelegramBot();
    public string Name => "/search";
    public CommandExecutor Executor { get; }
    private PlantSearchService _plantSearchService = new();

    public SearchCommand(CommandExecutor executor)
    {
        Executor = executor;
    }

    private string? _answer1;

    public async Task Execute(Update update)
    {
        long chatId = update.Message!.Chat.Id;
        if (update.Message.Text == null) 
            return;

        Executor.StartListen(this); 
        
        await Client.SendTextMessageAsync(chatId, "Введи назву рослини.");
    }

    public async Task GetUpdate(Update update)
    {
        long chatId = update.Message?.Chat.Id ?? update.CallbackQuery.Message.Chat.Id;

        if (update.Message != null && _answer1 == null)
        {
            _answer1 = update.Message.Text;
            var plant = await _plantSearchService.Execute(_answer1);

            if (plant == null)
            {
                await Client.SendTextMessageAsync(chatId, "Я не зміг знайти рослинку по цьому запиту :(" +
                                                          "\nСпробуйте використати іншу назву.");
                Executor.StopListen();
                return;
            }

            InputFile photo = InputFile.FromUri(plant.ImageUrl);
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Далі", "next"),
                    InlineKeyboardButton.WithCallbackData("Детальніше", "detailed_info"),
                    InlineKeyboardButton.WithCallbackData("Зупинити пошук", "stop_search")
                }
            });

            await Client.SendPhotoAsync(chatId, photo, 
                caption: $"Here is your plant - {plant.CommonName}" +
                         $"\n(scientific name: {plant.ScientificName})" +
                         $"\n\nCycle: {plant.Cycle}" +
                         $"\nWatering: {plant.Watering}" +
                         $"\nSunlight: {plant.Sunligt}" +
                         $"\n\nYou've been searching for this plant?",
                replyMarkup: buttons);
        }
        else if (update.CallbackQuery != null)
        {
            await HandleCallbackQuery(update.CallbackQuery);
        }
    }

    private async Task HandleCallbackQuery(CallbackQuery callbackQuery)
    {
        long chatId = callbackQuery.Message.Chat.Id;

        if (callbackQuery.Data == "next")
        {
            var plant = await _plantSearchService.GetPlant();
            
            if (plant == null)
            {
                await Client.SendTextMessageAsync(chatId, "Більше рослинок по цьому запиту нема :(" +
                                                          "\nСпробуйте використати іншу назву.");
                Executor.StopListen();
                return;
            }

            InputFile photo = InputFile.FromUri(plant.ImageUrl);
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Далі", "next"),
                    InlineKeyboardButton.WithCallbackData("Детальна інформація", "detailed_info"),
                    InlineKeyboardButton.WithCallbackData("Зупинити пошук", "stop_search")
                }
            });
            
            await Client.SendPhotoAsync(chatId, photo, 
                caption: $"Here is your plant - {plant.CommonName}" +
                         $"\n(scientific name: {plant.ScientificName})" +
                         $"\n\nCycle: {plant.Cycle}" +
                         $"\nWatering: {plant.Watering}" +
                         $"\nSunlight: {plant.Sunligt}" +
                         $"\n\nYou've been searching for this plant?",
                replyMarkup: buttons);
        }
        else if (callbackQuery.Data == "detailed_info")
        {
            await Client.SendTextMessageAsync(chatId, "Here is the detailed information about the plant...");
        }
        else if (callbackQuery.Data == "stop_search")
        {
            await Client.SendTextMessageAsync(chatId, "Stopping the search...");
            Executor.StopListen();
            return;
        }

        await Client.AnswerCallbackQueryAsync(callbackQuery.Id);
    }
}