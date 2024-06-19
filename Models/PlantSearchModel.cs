namespace GardenBot.Models;

public class PlantSearchModel
{
    public List<PlantSearchModel> data { get; set; }
}

public class PlantSearchModelData
{
    public string CommonName { get; set; }
    public string ScientificName { get; set; }
    public string Cycle { get; set; }
    public string Watering { get; set; }
    public string Sunligt { get; set; }
    public string ImageUrl { get; set; }
}