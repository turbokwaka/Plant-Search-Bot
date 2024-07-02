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
        return ParseJsonDataAsync(response);
    }

    private PlantSearchModel ParseJsonDataAsync(string json)
    {
        var jsonObject = JObject.Parse(json);
        var returnObject = new PlantSearchModel();
        var propertyCheckService = new PropertyCheckService();

        foreach (var item in jsonObject["data"])
        {
            int invalidPropertiesCount = 0;

            var cycle = (string)item["cycle"];
            var watering = (string)item["watering"];
            var sunlight = String.Join(", ", item["sunlight"]);
            var imageUrl = (string)item["default_image"]["original_url"];

            bool cycleIsValid = propertyCheckService.Check(cycle);
            bool wateringIsValid = propertyCheckService.Check(watering);
            bool sunlightIsValid = propertyCheckService.Check(sunlight);
            bool imageIsValid = propertyCheckService.Check(imageUrl);

            if (!cycleIsValid) invalidPropertiesCount++;
            if (!wateringIsValid) invalidPropertiesCount++;
            if (!sunlightIsValid) invalidPropertiesCount++;
            if (!imageIsValid) invalidPropertiesCount++;

            if (invalidPropertiesCount > 2) continue;

            returnObject.Data.Add(new PlantSearchModelData
            {
                PlantId = (int)item["id"],
                CommonName = (string)item["common_name"],
                ScientificName = string.Join(", ", item["scientific_name"]),
                Cycle = cycleIsValid ? cycle : "No information.",
                Watering = wateringIsValid ? watering : "No information.",
                Sunlight = sunlightIsValid ? sunlight : "No information.",
                ImageUrl = imageIsValid ? imageUrl : "https://postimg.cc/0rBn2kDn"
            });
        }


        return returnObject;
    }
}