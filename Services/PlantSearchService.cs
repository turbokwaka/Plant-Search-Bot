using GardenBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GardenBot.Services;

public class PlantSearchService
{
    private readonly MyConfiguration _configuration = new MyConfiguration();
    private PlantSearchModel? _plantData = null;
    private string? _keyword = null;
    public async Task<PlantSearchModelData?> Execute(string? keyword)
    {
        _keyword = keyword;
        _plantData = await Search(keyword);

        // Check if _plantData or _plantData.Data is null or empty
        if (_plantData?.Data != null && _plantData.Data.Any())
        {
            return _plantData.Data.First();
        }
        
        return null;
    }

    public async Task<PlantSearchModelData?> GetPlant()
    {
        _plantData.Data.RemoveAt(0);
        
        if (_plantData?.Data != null && _plantData.Data.Any())
        {
            return _plantData.Data.First();
        }
        
        return null;
    }
    
    private async Task<PlantSearchModel?> Search(string keyword)
    {
        var httpClient = new HttpClient();
        var url = $"{_configuration.ApiUrl}api/species-list?key={_configuration.ApiTokens.First()}&q={keyword}&indoor=1";

        var response = await httpClient.GetStringAsync(url);
        return await ParseJson(response);
    }

    private async Task<PlantSearchModel> ParseJson(string json)
    {
        var jsonObject = JObject.Parse(json);
        var returnObject = new PlantSearchModel();
        var propertyCheckService = new PropertyCheckService();

        foreach (var item in jsonObject["data"])
        {
            var invalidPropertiesCount = 0;

            bool cycleIsValid = await propertyCheckService.Check((string)item["cycle"]);
            bool wateringIsValid = await propertyCheckService.Check((string)item["watering"]);
            bool sunlightIsValid = await propertyCheckService.Check(String.Join(", ", item["sunlight"]));
            bool imageIsValid = await propertyCheckService.Check((string) item["default_image"]["original_url"]);
            
            returnObject.Data.Add(new PlantSearchModelData
            {
                CommonName = (string)item["common_name"],
                ScientificName = string.Join(", ", item["scientific_name"]),
                Cycle = cycleIsValid ? (string) item["cycle"] : "No information.",
                Watering = wateringIsValid ? (string) item["watering"] : "No information.",
                Sunligt = sunlightIsValid ? String.Join(", ", item["sunlight"]) : "No information.",
                ImageUrl = imageIsValid ? (string) item["default_image"]["original_url"] : "https://postimg.cc/0rBn2kDn"
            });
        }

        return returnObject;
    }
}