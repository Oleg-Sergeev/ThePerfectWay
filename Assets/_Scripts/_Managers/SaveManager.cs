using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager
{
    public float soundVolume, musicVolume, totalDistance;
    public int coin, emerald, coinsCollected, emeraldsCollected, currentLevel, completedLevel, levelsCount, BestScore, BestScore_Hardcore, giftDays;
    public DateTime dateTime;
    public string gameMode, language, quality, name_FigureOrTrail, notify, date;
    public bool IsTutorialPassed, IsPayed;
    public bool[] IsBonusCodesEntered;
    public Color colorbttnMusic, colorbttnSound, colorFigure, colorTrail;
    public Levels[] levels;
    public Times[] times;
    public Boosts[] boosts;
    public Tickets[] tickets;
    public Achievements[] achievements;
    public Dictionary<string, float> time;
    public Dictionary<string, Boosts> boost;
    public Dictionary<string, Tickets> ticket;
    public Dictionary<string, Achievements> achievement;



    public void Load(SaveManager sv, string str)
    {
        SaveData sd = new SaveData();
        sd = JsonUtility.FromJson<SaveData>(str);

        sv.time = new Dictionary<string, float>();
        sv.boost = new Dictionary<string, Boosts>();
        sv.ticket = new Dictionary<string, Tickets>();
        sv.achievement = new Dictionary<string, Achievements>();

        sv.times = new Times[sd.times.Length];

        sv.boosts = new Boosts[_UserInterface.Instance.powerUps.Length > sd.boosts.Length ? _UserInterface.Instance.powerUps.Length : sd.boosts.Length];

        sv.tickets = new Tickets[_UserInterface.Instance.ticketParent.transform.childCount - 1];

        int achievementLength = sd.achievements != null ? sd.achievements.Length : 0;
        if (_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.childCount - 2 > achievementLength)
            achievementLength = _UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.childCount - 2;

        sv.achievements = new Achievements[achievementLength];

        sv.levels = new Levels[sd.levels.Length];
        for (int i = 0; i < sv.levels.Length; i++) sv.levels[i].stars = new int[3];

        #region Data loading
        for (int i = 0; i < sd.levels.Length; i++)
        {
            for (int j = 0; j < sd.levels[0].stars.Length; j++)
            {
                sv.levels[i].stars[j] = sd.levels[i].stars[j];
            }
        }
        for (int i = 0; i < sd.times.Length; i++)
        {
            sv.times[i].key = sd.times[i].key;
            sv.times[i].value = sd.times[i].value;
            sv.time.Add(times[i].key, times[i].value);
        }
        for (int i = 0; i < sd.boosts.Length; i++)
        {
            sv.boosts[i] = sd.boosts[i];
            sv.boost.Add(sv.boosts[i].name, sv.boosts[i]);
        }
        for (int i = sd.boosts.Length; i < sv.boosts.Length; i++)
        {
            sv.boosts[i] = new Boosts
            {
                name = _UserInterface.Instance.powerUps[i].transform.GetChild(1).GetChild(0).name,
                cost = int.Parse(_UserInterface.Instance.powerUps[i].transform.GetChild(4).GetChild(1).GetComponent<UnityEngine.UI.Text>().text)
            };
            if (_UserInterface.Instance.powerUps[i].transform.GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Text>().text == "")
            {
                sv.boosts[i].amount = -1;
                sv.boosts[i].upgrade = 0;
            }
            else
            {
                sv.boosts[i].amount = 0;
                sv.boosts[i].upgrade = -1;
            }
            sv.boost.Add(sv.boosts[i].name, sv.boosts[i]);
        }
        for (int i = 0; i < (sd.achievements != null ? sd.achievements.Length : 0); i++)
        {
            sv.achievements[i] = sd.achievements[i];
            sv.achievement.Add(sv.achievements[i].name, sv.achievements[i]);
        }
        for (int i = (sd.achievements != null ? sd.achievements.Length : 0); i < sv.achievements.Length; i++)
        {
            sv.achievements[i] = new Achievements()
            {
                name = _UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).name,
                startProgress = 0,
                currentProgress = 0,
                endProgress = float.Parse(_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).GetChild(4).GetChild(2).GetComponent<Text>().text),
                coins = int.Parse(_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).GetChild(3).GetChild(0).GetComponent<Text>().text),
                emeralds = int.Parse(_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).GetChild(3).GetChild(2).GetComponent<Text>().text),
                IsUnlocked = false
            };
            sv.achievement.Add(sv.achievements[i].name, sv.achievements[i]);
        }

        if (sd.tickets != null)
        {
            for (int i = 0; i < ((sd.tickets.Length < sv.tickets.Length) ? sd.tickets.Length : sv.tickets.Length); i++) sv.tickets[i] = sd.tickets[i];
        }

        if (_UserInterface.bonusCodes.Length > (sd.IsBonusCodesEntered != null ? sd.IsBonusCodesEntered.Length : 0))
        {
            bool[] temp = sd.IsBonusCodesEntered;
            sv.IsBonusCodesEntered = new bool[_UserInterface.bonusCodes.Length];
            for (int i = 0; i < temp.Length; i++) sv.IsBonusCodesEntered[i] = temp[i];
        }
        else
        {
            sv.IsBonusCodesEntered = sd.IsBonusCodesEntered;
        }

        sv.dateTime = DateTime.Parse(sd.date ?? DateTime.Now.ToString());
        sv.musicVolume = sd.musicVolume;
        sv.soundVolume = sd.soundVolume;
        sv.totalDistance = sd.totalDistance;
        sv.coin = sd.coin;
        sv.emerald = sd.emerald;
        sv.coinsCollected = sd.coinsCollected;
        sv.emeraldsCollected = sd.emeraldsCollected;
        sv.currentLevel = sd.currentLevel;
        sv.completedLevel = sd.completedLevel;
        sv.BestScore = sd.BestScore;
        sv.BestScore_Hardcore = sd.BestScore_Hardcore;
        sv.gameMode = sd.gameMode;
        sv.language = sd.language;
        sv.quality = sd.quality;
        sv.notify = sd.notify;
        sv.name_FigureOrTrail = sd.name_FigureOrTrail;
        sv.IsPayed = sd.IsPayed;
        sv.IsTutorialPassed = sd.IsTutorialPassed;
        sv.colorbttnMusic = sd.colorbttnMusic;
        sv.colorbttnSound = sd.colorbttnSound;
        sv.colorFigure = sd.colorFigure;
        sv.colorTrail = sd.colorTrail;
        sv.giftDays = sd.giftDays;
        #endregion
    }

    public void Load(SaveManager sv)
    {
        string json, path;

#if UNITY_EDITOR
        path = Application.dataPath + "/Save.json";
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Application.persistentDataPath + "/Save.json";
#endif
        if (File.Exists(path))
        {
            json = File.ReadAllText(path);
        }
        else
        {
            TextAsset textAsset = Resources.Load<TextAsset>("DefaultSave");
            json = textAsset.text;
        }

        SaveData sd = new SaveData();
        sd = JsonUtility.FromJson<SaveData>(json);

        sv.time = new Dictionary<string, float>();
        sv.boost = new Dictionary<string, Boosts>();
        sv.ticket = new Dictionary<string, Tickets>();
        sv.achievement = new Dictionary<string, Achievements>();

        sv.times = new Times[sd.times.Length];
        
        sv.boosts = new Boosts[_UserInterface.Instance.powerUps.Length > sd.boosts.Length ? _UserInterface.Instance.powerUps.Length : sd.boosts.Length];

        sv.tickets = new Tickets[_UserInterface.Instance.ticketParent.transform.childCount - 1];

        int achievementLength = sd.achievements != null ? sd.achievements.Length : 0;
        if (_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.childCount - 2 > achievementLength)
            achievementLength = _UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.childCount - 2;

        sv.achievements = new Achievements[achievementLength];

        sv.levels = new Levels[sd.levels.Length];
        for (int i = 0; i < sv.levels.Length; i++) sv.levels[i].stars = new int[3];

        #region Data loading
        for (int i = 0; i < sd.levels.Length; i++)
        {
            for (int j = 0; j < sd.levels[0].stars.Length; j++)
            {
                sv.levels[i].stars[j] = sd.levels[i].stars[j];
            }
        }
        for (int i = 0; i < sd.times.Length; i++)
        {
            sv.times[i].key = sd.times[i].key;
            sv.times[i].value = sd.times[i].value;
            sv.time.Add(times[i].key, times[i].value);
        }
        for (int i = 0; i < sd.boosts.Length; i++)
        {
            sv.boosts[i] = sd.boosts[i];
            sv.boost.Add(sv.boosts[i].name, sv.boosts[i]);
        }
        for (int i = sd.boosts.Length; i < sv.boosts.Length; i++)
        {
            sv.boosts[i] = new Boosts
            {
                name = _UserInterface.Instance.powerUps[i].transform.GetChild(1).GetChild(0).name,
                cost = int.Parse(_UserInterface.Instance.powerUps[i].transform.GetChild(4).GetChild(1).GetComponent<UnityEngine.UI.Text>().text)
            };
            if (_UserInterface.Instance.powerUps[i].transform.GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Text>().text == "")
            {
                sv.boosts[i].amount = -1;
                sv.boosts[i].upgrade = 0;
            }
            else
            {
                sv.boosts[i].amount = 0;
                sv.boosts[i].upgrade = -1;
            }
            sv.boost.Add(sv.boosts[i].name, sv.boosts[i]);
        }
        for (int i = 0; i < (sd.achievements != null ? sd.achievements.Length : 0); i++)
        {
            sv.achievements[i] = sd.achievements[i];
            sv.achievement.Add(sv.achievements[i].name, sv.achievements[i]);
        }
        for (int i = (sd.achievements != null ? sd.achievements.Length : 0); i < sv.achievements.Length; i++)
        {
            sv.achievements[i] = new Achievements()
            {
                name = _UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).name,
                startProgress = 0,
                currentProgress = 0,
                endProgress = float.Parse(_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).GetChild(4).GetChild(2).GetComponent<Text>().text),
                coins = int.Parse(_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).GetChild(3).GetChild(0).GetComponent<Text>().text),
                emeralds = int.Parse(_UserInterface.GetDictionaryValue(_UserInterface.Instance.panelsDict, "PanelAchievements").transform.GetChild(i + 1).GetChild(3).GetChild(2).GetComponent<Text>().text),
                IsUnlocked = false
            };
            sv.achievement.Add(sv.achievements[i].name, sv.achievements[i]);
        }

        if (sd.tickets != null)
        {
            for (int i = 0; i < ((sd.tickets.Length < sv.tickets.Length) ? sd.tickets.Length : sv.tickets.Length); i++) sv.tickets[i] = sd.tickets[i];
        }

        if (_UserInterface.bonusCodes.Length > (sd.IsBonusCodesEntered != null? sd.IsBonusCodesEntered.Length : 0) )
        {
            bool[] temp = sd.IsBonusCodesEntered;
            sv.IsBonusCodesEntered = new bool[_UserInterface.bonusCodes.Length];
            for (int i = 0; i < temp.Length; i++) sv.IsBonusCodesEntered[i] = temp[i];
        }
        else
        {
            sv.IsBonusCodesEntered = sd.IsBonusCodesEntered;
        }

        sv.dateTime = DateTime.Parse(sd.date ?? DateTime.Now.ToString());


        sv.musicVolume = sd.musicVolume;
        sv.soundVolume = sd.soundVolume;
        sv.totalDistance = sd.totalDistance;
        sv.coin = sd.coin;
        sv.emerald = sd.emerald;
        sv.coinsCollected = sd.coinsCollected;
        sv.emeraldsCollected = sd.emeraldsCollected;
        sv.currentLevel = sd.currentLevel;
        sv.completedLevel = sd.completedLevel;
        sv.BestScore = sd.BestScore;
        sv.BestScore_Hardcore = sd.BestScore_Hardcore;
        sv.gameMode = sd.gameMode;
        sv.language = sd.language;
        sv.quality = sd.quality;
        sv.notify = sd.notify;
        sv.name_FigureOrTrail = sd.name_FigureOrTrail;
        sv.IsPayed = sd.IsPayed;
        sv.IsTutorialPassed = sd.IsTutorialPassed;
        sv.colorbttnMusic = sd.colorbttnMusic;
        sv.colorbttnSound = sd.colorbttnSound;
        sv.colorFigure = sd.colorFigure;
        sv.colorTrail = sd.colorTrail;
        sv.giftDays = sd.giftDays;
        #endregion
    }

    public void Save(bool autosave = false)
    {
        string path;

#if UNITY_EDITOR
        path = Application.dataPath + "/Save.json";
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Application.persistentDataPath + "/Save.json";
#endif
        for (int i = 0; i < _UserInterface.sv.time.Count; i++)
        {
            _UserInterface.sv.times[i].value = _UserInterface.sv.time[_UserInterface.sv.times[i].key];
        }
        _UserInterface.sv.date = _UserInterface.sv.dateTime.ToString();

        File.WriteAllText(path, JsonUtility.ToJson(_UserInterface.sv, true));
        if (!autosave)
        {
            if (GPGamesManager.allowSaving) GPGamesManager.WriteSaveData(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(_UserInterface.sv)), DateTime.Now.ToString().
            Replace(" ", "").
            Replace(".", "").
            Replace(":", "").
            Replace("/", ""));
            else Debug.LogWarning("Saving is unavailable");
        }
        else GPGamesManager.WriteSaveData(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(_UserInterface.sv)));
    }
}

public class SaveData
{
    public Levels[] levels;
    public Times[] times;
    public Boosts[] boosts;
    public Tickets[] tickets;
    public Achievements[] achievements;
    public float soundVolume, musicVolume, totalDistance;
    public int coin, emerald, coinsCollected, emeraldsCollected, currentLevel, completedLevel, BestScore, BestScore_Hardcore, giftDays;
    public string gameMode, language, quality, name_FigureOrTrail, notify, date, d1,d2,d3;
    public bool IsTutorialPassed, IsPayed;
    public bool[] IsBonusCodesEntered;
    public Color colorbttnMusic, colorbttnSound, colorFigure, colorTrail;
}

[Serializable]
public struct Levels
{
    public int[] stars;
}

[Serializable]
public struct Times
{
    public string key;
    public float value;
}

[Serializable]
public class Boosts
{
    public string name;
    public int cost, amount, upgrade;
}

[Serializable]
public class Tickets
{
    public string name;
    public int cost, amount, rarity;
}

[Serializable]
public class Achievements
{
    public string name;
    public float startProgress, endProgress, currentProgress;
    public int coins, emeralds;
    public bool IsUnlocked;
}