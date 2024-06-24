using GardenBot.Models;
using Newtonsoft.Json;

namespace GardenBot.Services;

public class PlantSearchService
{
    private readonly MyConfiguration _configuration = new MyConfiguration();
    private PlantSearchModel? _plantData = null;
    private string? _keyword = null;
    private int _plantIndex = 0;

    private async Task<PlantSearchModel?> Search(string keyword)
    {
        var httpClient = new HttpClient();
        var url = $"{_configuration.ApiUrl}species-list?key={_configuration.ApiTokens.First()}&q={keyword}&indoor=1";

        var response = await httpClient.GetAsync(url);
        var json = response.ToString();

        _plantData = JsonConvert.DeserializeObject<PlantSearchModel>(json);
        return _plantData;
    }

    public async Task<PlantSearchModelData?> GetPlant(string? keyword)
    {
        if (keyword == null)
        {
            return null;
        }
        
        if (_plantData == null || _keyword != keyword)
        {
            _keyword = keyword;
            _plantData = await Search(keyword);
        }

        var plant = _plantData?.Data[_plantIndex];
        _plantIndex++;
        
        return plant;
    }
}