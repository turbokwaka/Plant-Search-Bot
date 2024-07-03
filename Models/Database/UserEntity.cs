using System.ComponentModel.DataAnnotations;
using GardenBot.Models.Database;

namespace GardenBot.Models;

public class UserEntity
{
    [Key]
    public int UserId { get; set; }
    public string? FirstName { get; set; }
}