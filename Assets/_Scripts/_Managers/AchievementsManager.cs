using UnityEngine;

public static class AchievementsManager
{
    public static void IncrementAchievement(string key, int steps)
    {
        if (!_UserInterface.sv.achievement.ContainsKey(key))
        {
            Debug.LogError("Key not found. Key: " + key);
            return;
        }

        _UserInterface.sv.achievement[key].currentProgress += steps;

        if (_UserInterface.sv.achievement[key].currentProgress >= _UserInterface.sv.achievement[key].endProgress)
        {
            if(!Social.localUser.authenticated) BaseUI.Instance.OnUnlockAchievement($"{_UserInterface.sv.achievement[key].name} - unlocked", NotifyType.Success);
        }
    }
}
