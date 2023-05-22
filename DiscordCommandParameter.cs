using Discord;
using Discord.WebSocket;

namespace KostyasLairBot;

public abstract class DiscordCommandParameter
{
    internal SlashCommandOptionBuilder SlashCommandOptionBuilder { get; private set; } = new();

    protected DiscordCommandParameter(Type innerType, ApplicationCommandOptionType type)
    {
        SlashCommandOptionBuilder.Type = type;
        IsRequired = true;
        Description = "No description.";

        if (type == ApplicationCommandOptionType.Channel)
        {
            if (innerType != typeof(SocketChannel) && innerType != typeof(SocketGuildChannel))
            {
                SlashCommandOptionBuilder.ChannelTypes = new();
                if (innerType == typeof(SocketTextChannel))
                {
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.Text);
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.PublicThread);
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.PrivateThread);
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.NewsThread);
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.News);
                }
                else if (innerType == typeof(SocketVoiceChannel))
                {
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.Voice);
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.Stage);
                }
                else if (innerType == typeof(SocketForumChannel))
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.Forum);
                else if (innerType == typeof(SocketCategoryChannel))
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.Category);
                else if (innerType == typeof(SocketStageChannel))
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.Stage);
                else if (innerType == typeof(SocketNewsChannel))
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.News);
                else if (innerType == typeof(SocketThreadChannel))
                {
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.PublicThread);
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.PrivateThread);
                    SlashCommandOptionBuilder.ChannelTypes.Add(ChannelType.NewsThread);
                }
                else
                    throw new NotSupportedException($"The channel type {innerType} is unsupported.");
            }
        }
    }

    public string Name
    {
        get => SlashCommandOptionBuilder.Name;
        set => SlashCommandOptionBuilder.Name = value.ToLower();
    }

    public string Description
    {
        get => SlashCommandOptionBuilder.Description;
        set => SlashCommandOptionBuilder.Description = value;
    }

    public int? MinStringLength
    {
        get => SlashCommandOptionBuilder.MinLength;
        set => SlashCommandOptionBuilder.MinLength = value;
    }

    public int? MaxStringLength
    {
        get => SlashCommandOptionBuilder.MaxLength;
        set => SlashCommandOptionBuilder.MaxLength = value;
    }

    public double? MinValue
    {
        get => SlashCommandOptionBuilder.MinValue;
        set => SlashCommandOptionBuilder.MinValue = value;
    }

    public double? MaxValue
    {
        get => SlashCommandOptionBuilder.MaxValue;
        set => SlashCommandOptionBuilder.MaxValue = value;
    }

    public bool IsRequired
    {
        get => SlashCommandOptionBuilder.IsRequired ?? false;
        set => SlashCommandOptionBuilder.IsRequired = value;
    }
}

public sealed class DiscordCommandParameter<T> : DiscordCommandParameter
{
    public DiscordCommandParameter() : base(typeof(T), GetCommandTypeFromT())
    {

    }

    private static ApplicationCommandOptionType GetCommandTypeFromT()
    {
        var type = typeof(T);
        var typeCode = Type.GetTypeCode(type);

        if (typeCode >= TypeCode.SByte && typeCode <= TypeCode.UInt64)
            return ApplicationCommandOptionType.Integer;

        switch (typeCode)
        {
            case TypeCode.Boolean:
                return ApplicationCommandOptionType.Boolean;

            case TypeCode.Decimal:
            case TypeCode.Single:
            case TypeCode.Double:
                return ApplicationCommandOptionType.Number;

            case TypeCode.String:
                return ApplicationCommandOptionType.String;
        }

        if (typeCode == TypeCode.Object)
        {
            if (type == typeof(SocketUser))
                return ApplicationCommandOptionType.User;

            if (type.IsAssignableTo(typeof(SocketChannel)))
                return ApplicationCommandOptionType.Channel;

            if (type == typeof(IAttachment))
                return ApplicationCommandOptionType.Attachment;

            if (type == typeof(SocketRole))
                return ApplicationCommandOptionType.Role;
        }

        throw new NotSupportedException($"The argument type {type} is not supported");
    }
}