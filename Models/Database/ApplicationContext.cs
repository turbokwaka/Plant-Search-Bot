
using Microsoft.EntityFrameworkCore;

namespace GardenBot.Models.Database;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<PlantEntity> Plants { get; set; }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasMany(e => e.Plants)
            .WithOne()
            .HasForeignKey(e => e.UserId);
    }
}