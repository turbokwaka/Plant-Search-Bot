namespace GardenBot.Services;

public class PropertyCheckService
{
    private readonly HttpClient _httpClient;
    private readonly MyConfiguration _configuration;

    public PropertyCheckService()
    {
        _httpClient = new HttpClient();
        _configuration = new MyConfiguration();
    }
    
    public bool Check(string prop)
    {
        if (string.IsNullOrEmpty(prop))
        {
            return false;
        }

        if (prop.Contains("https"))
        {
            if (prop.Contains("subscription-api-pricing") || prop.Contains("upgrade_access.jpg"))
            {
                return false;
            }
        
            var response = _httpClient.GetAsync(prop).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            
            if (content.Contains($"{_configuration.ApiUrl}storage/image/error_asset/plant.png"))
            {
                return false; // The page contains the error image, so it's a 404 page
            }
        }
        
        return true;
    }
}