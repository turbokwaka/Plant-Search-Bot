using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GardenBot.Models.Database;

public class PlantEntity
{
    [Key] 
    public int PlantId { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    
    public string CommonName { get; set; }
    public string ScientificName { get; set; }
    public string Description { get; set; }
    public string Sunlight { get; set; }
    public string Cycle { get; set; }
    public string Watering { get; set; }
    public string Soil { get; set; }
    public string GrowthSize { get; set; }
    public string Blooming { get; set; }
    public string ImageUrl { get; set; }
}