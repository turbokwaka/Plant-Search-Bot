using GardenBot.Models;
using GardenBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GardenBot.Controllers.Commands;

public class SearchCommand : ICommand, IListener
{
    public TelegramBotClient Client => Bot.GetTelegramBot();
    public string Name => "search";
    public CommandExecutor Executor { get; }
    private readonly PlantSearchService _plantSearchService = new();
    private readonly PlantDetailedInfoService _plantDetailedInfoService = new();
    private PlantSearchModelData? _searchPlant;
    private PlantInfoModel? _infoPlant;

    public SearchCommand(CommandExecutor executor)
    {
        Executor = executor;
    }

    private string? _answer1;

    public async Task Execute(Update update)
    {
        long chatId = update.Message?.Chat.Id ?? update.CallbackQuery.Message.Chat.Id;;

        Executor.StartListen(this); 
        
        await Client.SendTextMessageAsync(chatId, "What plant are you looking for?");
    }

    public async Task GetUpdate(Update update)
    {
        long chatId = update.Message?.Chat.Id ?? update.CallbackQuery.Message.Chat.Id;

        if (update.Message != null && _answer1 == null)
        {
            _answer1 = update.Message.Text;
            _searchPlant = await _plantSearchService.Execute(_answer1);

            InlineKeyboardMarkup? buttons;
            if (_searchPlant == null)
            {
                buttons = new InlineKeyboardMarkup(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Back to menu", "menu")
                    }
                });
                
                await Client.SendTextMessageAsync(chatId, "I can't found this plant 😔" +
                                                          "\nPlease, enter other name.",
                    replyMarkup: buttons);
                
                _answer1 = null;
                
                return;
            }

            InputFile photo = InputFile.FromUri(_searchPlant.ImageUrl);
            buttons = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Skip", "next"),
                    InlineKeyboardButton.WithCallbackData("Info", "detailed_info"),
                    InlineKeyboardButton.WithCallbackData("Menu", "menu")
                }
            });

            await Client.SendPhotoAsync(chatId, photo, 
                caption: $"🌿 *{_searchPlant.CommonName}* (Scientific Name: {_searchPlant.ScientificName})\n\n" +
                         $"🔄 *Life Cycle*: {_searchPlant.Cycle}\n\n" +
                         $"💧 *Watering Needs*: {_searchPlant.Watering}\n\n" +
                         $"☀️ *Sunlight Requirements*: {_searchPlant.Sunlight}\n\n" +
                         $"\n\nYou've been searching for this plant?",
                replyMarkup: buttons,
                parseMode: ParseMode.Markdown);
        }
        else if (update.CallbackQuery != null)
        {
            await HandleCallbackQuery(update);
        }
    }

    private async Task HandleCallbackQuery(Update update)
    {
        var callbackQuery = update.CallbackQuery;
        
        long chatId = callbackQuery.Message.Chat.Id;

        if (callbackQuery.Data == "next")
        {
            _searchPlant = await _plantSearchService.GetPlant();
            
            if (_searchPlant == null)
            {
                await Client.SendTextMessageAsync(chatId, "I can't find any more plants 😔" +
                                                          "\nConsider using other keyword.");
                Executor.StopListen();
                return;
            }

            InputFile photo = InputFile.FromUri(_searchPlant.ImageUrl);
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Skip", "next"),
                    InlineKeyboardButton.WithCallbackData("Info", "detailed_info"),
                    InlineKeyboardButton.WithCallbackData("Menu", "menu")
                }
            });
            
            await Client.SendPhotoAsync(chatId, photo, 
                caption: $"🌿 *{_searchPlant.CommonName}* (Scientific Name: {_searchPlant.ScientificName})\n\n" +
                         $"🔄 *Life Cycle*: {_searchPlant.Cycle}\n\n" +
                         $"💧 *Watering Needs*: {_searchPlant.Watering}\n\n" +
                         $"☀️ *Sunlight Requirements*: {_searchPlant.Sunlight}\n\n" +
                         $"\n\nYou've been searching for this plant?",
                replyMarkup: buttons,
                parseMode: ParseMode.Markdown);
        }
        else if (callbackQuery.Data == "detailed_info")
        {
            _infoPlant = await _plantDetailedInfoService.GetPlant(_searchPlant.Id);
            InputFile photo = InputFile.FromUri(_infoPlant.ImageUrl);
            
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Save", "save"),
                    InlineKeyboardButton.WithCallbackData("Menu", "menu")
                }
            });

            await Client.SendPhotoAsync(chatId, photo,
                caption: $"🌿 *{_infoPlant.CommonName}* (Scientific Name: {_infoPlant.ScientificName})\n\n" +
                         $"📝 *Description*: {_infoPlant.Description}\n\n" +
                         $"☀️ *Sunlight Requirements*: {_infoPlant.Sunlight}\n\n" +
                         $"🔄 *Life Cycle*: {_infoPlant.Cycle}\n\n" +
                         $"💧 *Watering Needs*: {_infoPlant.Watering}\n\n" +
                         $"🌱 *Preferred Soil*: {_infoPlant.Soil}\n\n" +
                         $"📏 *Growth Size*: {_infoPlant.GrowthSize}\n\n" +
                         $"🌸 *Blooming Period*: {_infoPlant.Blooming}",
                replyMarkup: buttons,
                parseMode: ParseMode.Markdown);
        }
        else if (callbackQuery.Data == "menu")
        {
            _answer1 = null;
            
            Executor.StopListen();

            var startCommand = new StartCommand();
            await startCommand.Execute(update);
        }
        else if (callbackQuery.Data == "save")
        {
            // will be here soon
        }
        

        await Client.AnswerCallbackQueryAsync(callbackQuery.Id);
    }
}