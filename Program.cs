using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace wht
{
    internal class Program
    {
        // Entry point of the application
        static async Task Main(string[] args)
        {
            // Loop to display menu and handle user input
            while (true)
            {
                Console.Clear();
                Console.SetWindowSize(80, 20);
                Console.SetBufferSize(80, 20);
                Console.Title = " ";

                DisplayBanner(); // Show the application banner
                DisplayMenu();   // Show the available options

                // Read user input
                ConsoleKeyInfo input = Console.ReadKey();
                char option = input.KeyChar;

                // Handle user selection
                switch (option)
                {
                    case 'a':
                        await SendWebhookMessage(); // Send a single webhook message
                        break;
                    case 'b':
                        await SpamWebhookMessages(); // Spam messages to a webhook
                        break;
                    case 'c':
                        await GetGuildInfo(); // Get information about a guild
                        break;
                    case 'd':
                        await GetUserStatusById(); // Get user status by ID
                        break;
                    case 'e':
                        return; // Exit the application
                }
            }
        }

        // Displays the application banner
        static void DisplayBanner()
        {
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"      ███████  ███                    █████       █████       ███  ███████ ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"     ███░░░███░███                   ░░███       ░░███       ░███ ███░░░███");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(@"    ░░░   ░███░███    █████ ███ █████ ░███████   ███████     ░███░░░   ░███");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"      ███████ ░███   ░░███ ░███░░███  ░███░░███ ░░░███░      ░███  ███████ ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(@"     ░███░░░  ░███    ░███ ░███ ░███  ░███ ░███   ░███       ░███ ░███░░░  ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"      ███      ███     ░░████░████    ████ █████  ░░█████     ███  ███ ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"     ░░░      ░░░       ░░░░ ░░░░    ░░░░ ░░░░░    ░░░░░     ░░░  ░░░    ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ");
            Console.WriteLine(@"              wht, @nildontsleep | v1, * = needs bot token");
            Console.ResetColor();
        }

        // Displays the menu options
        static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ");
            Console.WriteLine(@"  " + Environment.UserName + " | webhook sender (a)");
            Console.WriteLine(@"  " + Environment.UserName + " | webhook spammer (b)");
            Console.WriteLine(@"  " + Environment.UserName + " | guild info* (c)");
            Console.WriteLine(@"  " + Environment.UserName + " | user status* (d)");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("");
            Console.WriteLine(@"  " + Environment.UserName + " | exit (e)");
            Console.WriteLine(" ");
        }

        // Asynchronous method to retrieve and display guild information
        static async Task GetGuildInfo()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Clear();

            // Prompt for the guild ID and bot token
            Console.WriteLine(@"  " + Environment.UserName + " | Enter Guild ID: ");
            string guildId = Console.ReadLine();

            Console.WriteLine(@"  " + Environment.UserName + " | Enter Bot Token: ");
            string botToken = Console.ReadLine();

            using (HttpClient client = new HttpClient())
            {
                // Set the authorization header with the bot token
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", botToken);

                try
                {
                    // Make a GET request to fetch guild information
                    HttpResponseMessage response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}");
                    response.EnsureSuccessStatusCode(); // Ensure a successful response

                    // Read and deserialize the response content
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic guildInfo = JsonConvert.DeserializeObject(jsonResponse);

                    // Extract and display relevant guild information
                    int memberCount = guildInfo.member_count != null ? (int)guildInfo.member_count : 0;
                    string guildName = guildInfo.name ?? "Unknown";
                    bool isCommunity = guildInfo.community != null ? (bool)guildInfo.community : false;
                    string ownerId = guildInfo.owner_id ?? "Unknown";
                    string iconUrl = guildInfo.icon != null ? $"https://cdn.discordapp.com/icons/{guildId}/{guildInfo.icon}.png" : "No icon available";

                    // Display the guild information
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"  Guild Name: {guildName}");
                    Console.WriteLine($"  Member Count: {memberCount}");
                    Console.WriteLine($"  Is Community: {isCommunity}");
                    Console.WriteLine($"  Guild Owner ID: {ownerId}");
                    Console.WriteLine($"  Guild Icon URL: {iconUrl}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Unexpected Error: {ex.Message}");
                }
            }

            Console.Write(" ");
            Console.ReadKey();
        }

        // Asynchronous method to get user status by ID
        static async Task GetUserStatusById()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Clear();

            // Prompt for user ID and bot token
            Console.WriteLine(@"  " + Environment.UserName + " | Enter User ID: ");
            string userId = Console.ReadLine();

            Console.WriteLine(@"  " + Environment.UserName + " | Enter Bot Token: ");
            string botToken = Console.ReadLine();

            using (HttpClient client = new HttpClient())
            {
                // Set the authorization header with the bot token
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", botToken);

                try
                {
                    // Make a GET request to fetch user information
                    HttpResponseMessage response = await client.GetAsync($"https://discord.com/api/v10/users/{userId}");
                    response.EnsureSuccessStatusCode(); // Ensure a successful response

                    // Read and deserialize the response content
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse); // Log the raw response for debugging

                    dynamic userInfo = JsonConvert.DeserializeObject(jsonResponse);

                    // Check if userInfo is null
                    if (userInfo == null)
                    {
                        Console.WriteLine(@"  " + Environment.UserName + " | User information not found.");
                        return;
                    }

                    // Extracting values safely with null checks
                    string username = $"{userInfo.username}#{userInfo.discriminator}";
                    string userIdValue = userInfo.id.ToString();
                    long createdAtTimestamp = userInfo.created_at != null ? (long)userInfo.created_at : 0; // Default to 0 if not present
                    DateTime createdAt = createdAtTimestamp != 0 ? DateTimeOffset.FromUnixTimeMilliseconds(createdAtTimestamp).DateTime : DateTime.MinValue;

                    // Handling avatar and banner
                    string avatarId = userInfo.avatar != null ? userInfo.avatar.ToString() : null;
                    string avatarUrl = avatarId != null ? $"https://cdn.discordapp.com/avatars/{userIdValue}/{avatarId}.png" : "No avatar available";
                    string gifAvatarUrl = avatarId != null && avatarId.StartsWith("a_") ? $"https://cdn.discordapp.com/avatars/{userIdValue}/{avatarId}.gif" : avatarUrl;

                    string bannerId = userInfo.banner != null ? userInfo.banner.ToString() : null;
                    string bannerUrl = bannerId != null ? $"https://cdn.discordapp.com/banners/{userIdValue}/{bannerId}.png" : "No banner available";
                    if (bannerId != null && bannerId.StartsWith("a_"))
                    {
                        bannerUrl = $"https://cdn.discordapp.com/banners/{userIdValue}/{bannerId}.gif";
                    }

                    string bannerColor = userInfo.banner_color != null ? userInfo.banner_color.ToString() : "No color";

                    // Display user information
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"  Username: {username}");
                    Console.WriteLine($"  User ID: {userIdValue}");
                    Console.WriteLine($"  Created At: {createdAt}");
                    Console.WriteLine($"  Avatar URL: {avatarUrl}");
                    Console.WriteLine($"  GIF Avatar URL: {gifAvatarUrl}");
                    Console.WriteLine($"  Banner URL: {bannerUrl}");
                    Console.WriteLine($"  Banner Color: {bannerColor}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Unexpected Error: {ex.Message}");
                }
            }

            Console.Write(" ");
            Console.ReadKey();
        }

        // Asynchronous method to send a single webhook message
        static async Task SendWebhookMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            // Prompt for webhook URL and message
            Console.WriteLine(@"  " + Environment.UserName + " | Enter Webhook URL: ");
            string webhookUrl = Console.ReadLine();
            Console.WriteLine(@"  " + Environment.UserName + " | Enter Message: ");
            string messageContent = Console.ReadLine();

            // Create the payload
            var payload = new
            {
                content = messageContent,
                username = "wht",
                avatar_url = "https://imgur.com/a/h29uogN" // Default avatar URL
            };

            // Serialize payload to JSON
            var jsonPayload = JsonConvert.SerializeObject(payload);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Make a POST request to send the webhook message
                    var response = await client.PostAsync(webhookUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode(); // Ensure a successful response
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("  " + Environment.UserName + " | Message sent successfully.");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Unexpected Error: {ex.Message}");
                }
            }

            Console.Write(" ");
            Console.ReadKey();
        }

        // Asynchronous method to spam messages to a webhook
        static async Task SpamWebhookMessages()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            // Prompt for webhook URL, message, and number of messages
            Console.WriteLine(@"  " + Environment.UserName + " | Enter Webhook URL: ");
            string webhookUrl = Console.ReadLine();
            Console.WriteLine(@"  " + Environment.UserName + " | Enter Message: ");
            string messageContent = Console.ReadLine();

            Console.WriteLine(@"  " + Environment.UserName + " | Enter the number of messages: ");
            string numMessagesInput = Console.ReadLine();

            // Attempt to parse the number of messages
            if (!int.TryParse(numMessagesInput, out int numMessages) || numMessages <= 0)
            {
                Console.WriteLine("  " + Environment.UserName + " | Please enter a valid number greater than 0.");
                return;
            }

            // Create the payload
            var payload = new
            {
                content = messageContent,
                username = Environment.UserName,
                avatar_url = "https://i.imgur.com/Gz4RZ84.jpg" // Default avatar URL
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    for (int i = 0; i < numMessages; i++)
                    {
                        // Make a POST request for each message
                        var response = await client.PostAsync(webhookUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                        response.EnsureSuccessStatusCode(); // Ensure a successful response
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        int testar = i + 1;
                        Console.WriteLine("  " + Environment.UserName + " | Message " + testar + "/" + numMessages + "sent.");
                        Thread.Sleep(250); // Optional delay between messages
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {Environment.UserName} | Unexpected Error: {ex.Message}");
                }
            }

            Console.Write(" ");
            Console.ReadKey();
        }
    }
}
