using Discord;
using Discord.WebSocket;
using EasyNetLog;
using LibGit2Sharp;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace KostyasLairBot;

internal class Program
{
    public static EasyNetLogger Logger { get; private set; } = new(msg => $"[<color=red>{DateTime.Now:HH:mm:ss.fff}</color>] {msg}", true, new string[] { "latest.log" });

    public static DiscordSocketClient Discord { get; private set; } = new();

    private const string configPath = "config.json";

    private static async Task Main()
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        Logger.Log($"Current dir set to {Directory.GetCurrentDirectory()}");

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
        Discord.Ready += OnBotStart;

        await Discord.LoginAsync(TokenType.Bot, token);
        await Discord.StartAsync();

        await GitCheckLoopAsync();

        Logger.Log("<color=red>Exiting...</color>");
    }

    private static async Task OnBotStart()
    {
        await Discord.SetGameAsync("amongus", type: ActivityType.Watching);
    }

    private static async Task GitCheckLoopAsync()
    {
        Directory.SetCurrentDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));
        Logger.Log($"Current dir set to {Directory.GetCurrentDirectory()}");

        var repo = new Repository(Directory.GetCurrentDirectory());

        for (; ; )
        {
            repo.Network.Fetch("origin", new string[] { "production" });

            if (repo.Refs.First(x => x.CanonicalName == "refs/heads/production").TargetIdentifier != repo.Refs.First(x => x.CanonicalName == "refs/remotes/origin/production").TargetIdentifier)
            {
                Logger.Log("New update detected. Pulling and restarting...");

                await Process.Start("git", "pull origin production").WaitForExitAsync();
                Process.Start("dotnet", "run");

                return;
            }

            await Task.Delay(1 * 60000);
        }
    }

    private static Task OnDiscordLog(LogMessage message)
    {
        Logger.Log($"[<color=blue>Discord</color>] {message.Message}");
        return Task.CompletedTask;
    }
}