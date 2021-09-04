using System.Collections.Generic;
using UnityEngine;

public class LanguageManager
{ 
    private static Dictionary<string, string> localizations;


    public static void ChangeLanguage()
    {
        localizations = new Dictionary<string, string>();
        
        TextAsset textAsset = Resources.Load<TextAsset>(_UserInterface.sv.language);
        string json = textAsset.text;
        LocalizationData localData = JsonUtility.FromJson<LocalizationData>(json);

        for (int i = 0; i < localData.items.Length; i++) localizations.Add(localData.items[i].key, localData.items[i].value);
    }

    public static string GetLocalizedValue(string key)
    {
        if (localizations.ContainsKey(key))
        {
            return localizations[key];
        }
        else
        {
            Debug.LogError("Key not found. Key: " + key);
            return "/Error/";
        }
    }
}

public class LocalizationData
{
    public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}

