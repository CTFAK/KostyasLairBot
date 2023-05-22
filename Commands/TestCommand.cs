using Discord.WebSocket;

namespace KostyasLairBot.Commands;

internal class TestCommand : DiscordCommand
{
    public override string Name => "test";

    public override string Description => "Just a test";

    public override async Task OnExecute(SocketSlashCommand interaction)
    {
        await interaction.RespondAsync("This is indeed a test.");
    }
}
