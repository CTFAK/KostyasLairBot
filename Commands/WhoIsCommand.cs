using Discord;
using Discord.WebSocket;

namespace KostyasLairBot.Commands;

internal class WhoIsCommand : DiscordCommand<SocketUser>
{
    public override string Name => "whois";

    public override string Description => "Gets info about a user";

    protected override DiscordCommandParameter<SocketUser> Parameter => new()
    {
        Name = "User"
    };

    public override async Task OnExecute(SocketSlashCommand interaction, DiscordCommandArgument<SocketUser> user)
    {
        var embed = new EmbedBuilder()
        {
            Title = $"{user.Argument.Username}#{user.Argument.Discriminator}",
            ImageUrl = user.Argument.GetAvatarUrl()
        };

        embed.Fields.Add(new()
        {
            Name = "Id",
            Value = user.Argument.Id
        });

        await interaction.RespondAsync(embeds: new Embed[] { embed.Build() });
    }
}
