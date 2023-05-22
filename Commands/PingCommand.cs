using Discord.WebSocket;

namespace KostyasLairBot.Commands;

internal class PingCommand : DiscordCommand
{
    public override string Name => "ping";

    public override string Description => "Checks if the bot is alive";

    public override async Task OnExecute(SocketSlashCommand interaction)
    {
        await interaction.RespondAsync("Pong");
    }
}
