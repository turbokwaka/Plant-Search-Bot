# Plant Search Telegram Bot

## Overview
This is a C# Telegram bot designed to help users search for plant information by keyword. It includes a broadcast command for administrators and utilizes a PostgreSQL database along with an external API to retrieve plant data.

## Features
- **Keyword Search**: Users can search for plant information using keywords.
- **Broadcast Command**: Administrators can send broadcast messages to all users.
- **PostgreSQL Integration**: Bot uses PostgreSQL for data storage.
- **External API**: Retrieves detailed plant information from an external API.

## Installation
1. **Clone the repository**
   ```bash
   git clone https://github.com/turbokwaka/Plant-Search-Bot.git
   cd plant-search-bot
   ```

2. **Set up PostgreSQL**
   - Create a database for the bot.
   - Update the connection string in `appsettings.json`.

3. **Configure the bot**
   - Update the bot token and API keys in `appsettings.json`.

4. **Run the bot**
   ```bash
   dotnet run
   ```

## Configuration
Ensure your `appsettings.json` is configured with the correct values:
```json
{
   "Logging": {
      "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
      }
   },
   "AllowedHosts": "*",

   "MyConfiguration" : {
      "TelegramToken": "Your telegram bot token, get it with BotFather.",
      "ApiUrl": "https://perenual.com/",
      "ApiTokens": {
         "FirstToken": "TOKEN",
         "SecondToken": "NOT NECESSARY"
      },
      "DatabaseConnectionString": "Connection string to postgres database.",
      "AdminUsers" : "Chat id of admin users, separated by coma."
   }
}
```

## Usage
- **Search for a plant**: Press `Search` button to find information about a plant.
- **Admin broadcast**: Admins can use `/broadcast <message>` to send a message to all users.

## Contributing
Feel free to fork this repository and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.

---

Enjoy searching for plants with the Plant Search Telegram Bot! 🌿