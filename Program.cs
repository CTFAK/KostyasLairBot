using Discord;
using Discord.WebSocket;
using EasyNetLog;
using Newtonsoft.Json.Linq;
using Octokit;
using System.Diagnostics;

namespace KostyasLairBot;

internal class Program
{
    public static EasyNetLogger Logger { get; private set; } = new(msg => $"[<color=red>{DateTime.Now:HH:mm:ss.fff}</color>] {msg}", true, new string[] { "latest.log" });

    public static DiscordSocketClient Discord { get; private set; } = new();

    public static GitHubClient GitHub { get; private set; } = new(new ProductHeaderValue("KostyasLairBot"));

    private const string configPath = "config.json";
    private const string commitShaPath = "commit.txt";

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
        Logger.Log("The bot has started");
    }

    private static async Task GitCheckLoopAsync()
    {
        Directory.SetCurrentDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));
        Logger.Log($"Current dir set to {Directory.GetCurrentDirectory()}");

        await Process.Start("git", "checkout production").WaitForExitAsync();

        for (; ; )
        {
            var branch = await GitHub.Git.Reference.Get("CTFAK", "KostyasLairBot", "heads/production");
            if (!File.Exists(commitShaPath) || await File.ReadAllTextAsync(commitShaPath) != branch.Object.Sha)
            {
                Logger.Log($"New update detected ({branch.Object.Sha}). Restarting...");

                await File.WriteAllTextAsync(commitShaPath, branch.Object.Sha);

                await Process.Start("git", "fetch").WaitForExitAsync();
                await Process.Start("git", "pull origin production").WaitForExitAsync();

                Process.Start("dotnet", "run");

                return;
            }

            await Task.Delay(10000);
        }
    }

    private static Task OnDiscordLog(LogMessage message)
    {
        Logger.Log($"[<color=blue>Discord</color>] {message.Message}");
        return Task.CompletedTask;
    }
}