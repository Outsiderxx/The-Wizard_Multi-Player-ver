using UnityEngine;
using System;
using System.IO;
using System.Linq;

public static class RecordHelper
{
    public static void SaveRecord(SavedRecord data)
    {
        string fileContent = JsonUtility.ToJson(data);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, data.time.ToString() + ".json"), fileContent);
    }

    public static SavedRecord[] LoadAllRecord()
    {
        string[] allFilePath = Directory.GetFiles(Application.streamingAssetsPath).Where(filePath => Path.GetExtension(filePath) == ".json").ToArray();
        return allFilePath.Select(filePath =>
        {
            string fileContent = File.ReadAllText(filePath);
            return JsonUtility.FromJson<SavedRecord>(fileContent);
        }).ToArray();
    }
}
