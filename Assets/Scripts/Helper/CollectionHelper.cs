using UnityEngine;
using System.Linq;

public static class CollectionHelper
{
    public static readonly string[] collectionNames = new string[] { "Ring of Eternity", "Ring of Wizard", "Ring of Strength" };

    public static bool IsCollectionCollected(string collectionName)
    {
        if (CollectionHelper.collectionNames.All(name => name != collectionName))
        {
            throw new System.Exception("Unknown collection name");
        }
        return PlayerPrefs.GetInt(collectionName, 0) == 1;
    }

    public static void CollectCollection(string collectionName)
    {
        if (CollectionHelper.collectionNames.All(name => name != collectionName))
        {
            throw new System.Exception("Unknown collection name");
        }
        PlayerPrefs.SetInt(collectionName, 1);
    }
}
