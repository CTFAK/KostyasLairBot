using Discord;
using Discord.WebSocket;

namespace KostyasLairBot.Commands;

internal class SendCommand : DiscordCommand<SocketTextChannel, string>
{
    public override string Name => "send";

    public override string Description => "Sends a message to the specified channel";

    public override GuildPermission? RequiredPermissions => GuildPermission.Administrator;

    protected override DiscordCommandParameter<SocketTextChannel> Parameter1 => new()
    {
        Name = "Channel",
        IsRequired = false
    };

    protected override DiscordCommandParameter<string> Parameter2 => new()
    {
        Name = "Message"
    };

    public override async Task OnExecute(SocketSlashCommand interaction, DiscordCommandArgument<SocketTextChannel> argument1, DiscordCommandArgument<string> argument2)
    {
        if (argument1.IsSet)
        {
            await argument1.Argument.SendMessageAsync(argument2.Argument);
            await interaction.RespondAsync("Done");
        }
        else
        {
            await interaction.RespondAsync(argument2.Argument);
        }
    }
}
