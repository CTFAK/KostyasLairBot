using Discord;
using Discord.WebSocket;

namespace KostyasLairBot.Commands;

internal class SetStatusCommand : DiscordCommand<string>
{
    public override string Name => "setstatus";

    public override string Description => "Sets the bot status";

    public override GuildPermission? RequiredPermissions => GuildPermission.Administrator;

    protected override DiscordCommandParameter<string> Parameter => new()
    {
        Name = "Status"
    };

    public override async Task OnExecute(SocketSlashCommand interaction, DiscordCommandArgument<string> status)
    {
        await Program.Discord.SetGameAsync(status.Argument);
    }
}
