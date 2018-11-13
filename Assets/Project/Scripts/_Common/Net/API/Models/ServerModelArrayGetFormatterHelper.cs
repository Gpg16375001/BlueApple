using SmileLab.Net.API;

using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

/// <summary>
/// Special Resolver for ServerModel Arrays
/// </summary>
public class ServerModelArrayResolver : IFormatterResolver
{
    public static IFormatterResolver Instance = new ServerModelArrayResolver();

    ServerModelArrayResolver()
    {

    }

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        return FormatterCache<T>.formatter;
    }

    static class FormatterCache<T>
    {
        public static readonly IMessagePackFormatter<T> formatter;

        static FormatterCache()
        {
            formatter = (IMessagePackFormatter<T>)ServerModelArrayGetFormatterHelper.GetFormatter(typeof(T));
        }
    }
}

internal static class ServerModelArrayGetFormatterHelper
{
    static readonly Dictionary<Type, Type> formatterMap = new Dictionary<Type, Type>()
    {
        {typeof(CardData[]), typeof(ArrayFormatter<CardData>)},
        {typeof(BoardData[]), typeof(ArrayFormatter<BoardData>)},
    };

    internal static object GetFormatter(Type t)
    {
        Type formatterType;
        if (formatterMap.TryGetValue(t, out formatterType))
        {
            return Activator.CreateInstance(formatterType);
        }

        return null;
    }
}
