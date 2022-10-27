using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// JSON轉換幫手，提供Array型別的轉換
/// </summary>
public static class JsonHelper
{
    private class Wrapper<T>
    {
        public T[] content;
    }

    public static T[] FromJson<T>(string rawString)
    {
        string validString = $"{{\"content\":{rawString}}}";
        try
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(validString);
            return wrapper.content;
        }
        catch
        {
            Debug.Log(validString);
            throw;
        }
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>()
        {
            content = array
        };
        Regex parsingContent = new Regex("{\"content\":(.+)}");
        return parsingContent.Match(JsonUtility.ToJson(wrapper)).Groups[1].Value;
    }
}
