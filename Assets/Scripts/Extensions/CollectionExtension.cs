using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectionExtension
{
    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        var randomElementNumber = Random.Range(0, collection.Count());
        return collection.ElementAt(randomElementNumber);
    }

    public static T GetRandomOrDefault<T>(this IEnumerable<T> collection)
    {
        if (!collection.Any())
        {
            return default(T);
        }

        var randomElementNumber = Random.Range(0, collection.Count());
        return collection.ElementAt(randomElementNumber);
    }
}

