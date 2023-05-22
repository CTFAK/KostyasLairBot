using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KostyasLairBot;

internal abstract class DiscordCommandParameter
{
    internal SlashCommandOptionBuilder SlashCommandOptionBuilder { get; private set; } = new();

    protected DiscordCommandParameter(ApplicationCommandOptionType type)
    {
        SlashCommandOptionBuilder.Type = type;
    }

    public string Name
    {
        get => SlashCommandOptionBuilder.Name;
        set => SlashCommandOptionBuilder.Name = value;
    }

    public string Description
    {
        get => SlashCommandOptionBuilder.Description;
        set => SlashCommandOptionBuilder.Description = value;
    }
}

internal sealed class DiscordCommandParameter<T> : DiscordCommandParameter
{
    public DiscordCommandParameter() : base(GetTypeFromT())
    {

    }

    private static ApplicationCommandOptionType GetTypeFromT()
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

        throw new NotSupportedException($"The argument type {type} is not supported");
    }
}