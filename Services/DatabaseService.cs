using GardenBot.Models;
using GardenBot.Models.Database;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace GardenBot.Services
{
    public class DatabaseService
    {
        private readonly ApplicationContext _context;

        public DatabaseService()
        {
            var configuration = new MyConfiguration();
            var connectionString = configuration.ConnectionString;
            
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseNpgsql(connectionString);

            _context = new ApplicationContext(optionsBuilder.Options);
        }
        
        public async Task PostUser(Update update)
        {
            int userId = (int)update.Message!.Chat.Id;
            string? firstName = update.Message!.Chat.FirstName;

            if (!_context.Users.Any(u => u.UserId == userId))
            {
                var user = new UserEntity()
                {
                    UserId = userId,
                    FirstName = firstName
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}