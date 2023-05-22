using Discord.WebSocket;

namespace KostyasLairBot;

internal abstract class DiscordCommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract Task OnExecute(SocketSlashCommand interaction);
}

internal abstract class DiscordCommand<T> : DiscordCommand
{

}

internal abstract class DiscordCommand<T, T2> : DiscordCommand
{
}