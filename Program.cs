using Discord;
using Discord.WebSocket;
using EasyNetLog;
using Newtonsoft.Json.Linq;

namespace KostyasLairBot;

internal class Program
{
    public static EasyNetLogger Logger { get; private set; } = new(msg => $"[<color=red>{DateTime.Now:HH:mm:ss.fff}</color>] {msg}", true, new string[] { "latest.log" });

    public static DiscordSocketClient Discord { get; private set; } = new();

    private const string configPath = "config.json";

    private static async Task Main()
    {
        if (!File.Exists(configPath))
        {
            Logger.Log($"Please set up a {configPath} file first.");
            return;
        }

        var config = JObject.Parse(await File.ReadAllTextAsync(configPath));

        var token = (string?)config["token"];
        if (token == null)
        {
            Logger.Log($"Please set the token in the config file.");
            return;
        }

        Discord.Log += OnDiscordLog;

        await Discord.LoginAsync(TokenType.Bot, token);
        await Discord.StartAsync();

        await Task.Delay(-1);
    }

    private static Task OnDiscordLog(LogMessage message)
    {
        Logger.Log($"[<color=blue>Discord</color>] {message.Message}");
        return Task.CompletedTask;
    }
}