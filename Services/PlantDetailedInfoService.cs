using GardenBot.Models;
using Newtonsoft.Json.Linq;

namespace GardenBot.Services;

public class PlantDetailedInfoService
{
    private readonly MyConfiguration _configuration = new();
    private readonly PlantInfoModel _plantData;

    public async Task<PlantInfoModel> GetPlant(int id)
    {
        return await Search(id);
    }

    private async Task<PlantInfoModel> Search(int id)
    {
        var httpClient = new HttpClient();
        var url = $"{_configuration.ApiUrl}api/species/details/{id}?key={_configuration.ApiTokens.First()}";

        var response = await httpClient.GetStringAsync(url);
        return ParseJsonData(response); 
    }

    private PlantInfoModel ParseJsonData(string json)
    {
        var jsonObject = JObject.Parse(json);
        var propertyCheckService = new PropertyCheckService();
        
        bool imageIsValid = propertyCheckService.Check((string)jsonObject["default_image"]?["original_url"]);
        
        var dimensions = jsonObject["dimensions"];
        var minDimension = (int?)dimensions?["min_value"];
        var maxDimension = (int?)dimensions?["max_value"];
        var dimensionUnit = (string)dimensions?["unit"];
        
        var commonName = (string)jsonObject["common_name"] ?? "No information.";
        var scientificName = string.Join(", ", jsonObject["scientific_name"] ?? new JArray());
        var description = (string)jsonObject["description"] ?? "No description available.";
        var sunlight = string.Join(", ", jsonObject["sunlight"] ?? new JArray());
        var cycle = (string)jsonObject["cycle"] ?? "No information.";
        var watering = ParseWateringBenchmark(jsonObject["watering_general_benchmark"]);
        var soil = jsonObject["soil"] != null
            ? string.Join(", ", jsonObject["soil"])
            : "No information.";

        var growthSize = minDimension.HasValue && maxDimension.HasValue && minDimension == maxDimension
            ? $"{minDimension} to {maxDimension} {dimensionUnit}"
            : $"{minDimension} {dimensionUnit}";

        var blooming = ParseBloomingInfo(jsonObject);

        var imageUrl = imageIsValid
            ? (string)jsonObject["default_image"]["original_url"]
            : "https://postimg.cc/0rBn2kDn";

        return new PlantInfoModel
        {
            CommonName = commonName,
            ScientificName = scientificName,
            Description = description,
            Sunlight = sunlight,
            Cycle = cycle,
            Watering = watering,
            Soil = soil,
            GrowthSize = growthSize,
            Blooming = blooming,
            ImageUrl = imageUrl
        };
    }

    private string ParseBloomingInfo(JObject plantInfo)
    {
        if ((bool)plantInfo["flowers"])
        {
            string flowerColor = plantInfo["flower_color"]?.ToString().ToLower();
            string floweringSeason = plantInfo["flowering_season"]?.ToString().ToLower();

            if (!string.IsNullOrEmpty(flowerColor))
            {
                return !string.IsNullOrEmpty(floweringSeason)
                    ? $"Has {flowerColor} flowers. Blooming season is {floweringSeason}."
                    : $"Has {flowerColor} flowers.";
            }
        }
        return "No information";
    }
    private string ParseWateringBenchmark(JToken wateringBenchmark)
    {
        if (wateringBenchmark is JObject benchmark)
        {
            string value = benchmark["value"]?.ToString();
            string unit = benchmark["unit"]?.ToString();

            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(unit))
            {
                return $"{value} {unit}";
            }
        }
        return "No information";
    }
}