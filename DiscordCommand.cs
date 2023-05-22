using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace KostyasLairBot;

public abstract class DiscordCommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    [NotNull] internal List<SlashCommandOptionBuilder>? Parameters { get; private set; } = new();

    public abstract Task OnExecute(SocketSlashCommand interaction);
}

public abstract class DiscordCommand<T> : DiscordCommand
{
    protected abstract DiscordCommandParameter<T> Parameter { get; }

    public DiscordCommand()
    {
        Parameters.Add(Parameter.SlashCommandOptionBuilder);
    }

    public abstract Task OnExecute(SocketSlashCommand interaction, DiscordCommandArgument<T> argument1);

    public sealed override async Task OnExecute(SocketSlashCommand interaction) => await OnExecute(interaction,
        new DiscordCommandArgument<T>(interaction.Data.Options.FirstOrDefault(x => x.Name == Parameters[0].Name)));
}

public abstract class DiscordCommand<T1, T2> : DiscordCommand
{
    protected abstract DiscordCommandParameter<T1> Parameter1 { get; }
    protected abstract DiscordCommandParameter<T2> Parameter2 { get; }

    public DiscordCommand()
    {
        Parameters.Add(Parameter1.SlashCommandOptionBuilder);
        Parameters.Add(Parameter2.SlashCommandOptionBuilder);
    }

    public abstract Task OnExecute(SocketSlashCommand interaction, DiscordCommandArgument<T1> argument1, DiscordCommandArgument<T2> argument2);

    public sealed override async Task OnExecute(SocketSlashCommand interaction) => await OnExecute(interaction,
        new DiscordCommandArgument<T1>(interaction.Data.Options.FirstOrDefault(x => x.Name == Parameters[0].Name)),
        new DiscordCommandArgument<T2>(interaction.Data.Options.FirstOrDefault(x => x.Name == Parameters[1].Name)));
}