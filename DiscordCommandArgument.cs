using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace KostyasLairBot;

public class DiscordCommandArgument<T>
{
    public bool IsSet { get; private set; }

    [NotNull] public T? Argument { get; private set; }

    public DiscordCommandArgument(SocketSlashCommandDataOption? option)
    {
        IsSet = option != null;
        if (option != null)
            Argument = (T)option.Value;
    }
}
