using System;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public static class GPGamesManager
{
    public const string DEFAULT_SAVE_NAME = "Save";

    private static ISavedGameClient savedGameClient;
    private static ISavedGameMetadata currentMetadata;

    private static DateTime startDateTime;

    public static bool allowSaving = false;


    #region Authorization

    public static void Initialize(bool debug)
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = debug;
        PlayGamesPlatform.Activate();

        startDateTime = DateTime.Now;
    }

    public static void Auth(Action<bool, string> onAuth)
    {
        Social.localUser.Authenticate((success, message) =>
        {
            if (success) savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            onAuth(success, message);
        });
    }

    #endregion /Authorization

    #region Achievements

    public static void UnlockAchievement(string id)
    {
        Debug.Log($"Unlocking achievement {id}...");
        Social.ReportProgress(id, 100, OnSuccessfull);
    }

    public static void IncrementAchievement(string id, int steps)
    {
        Debug.Log($"Incrementing {id} progress...");
        PlayGamesPlatform.Instance.IncrementAchievement(id, steps, OnSuccessfull);
    }

    public static void ShowAchievementsUI()
    {
        Debug.Log("Showing achievements UI...");
        Social.ShowAchievementsUI();
    }

    #endregion /Achievements

    #region Leaderboards

    public static void ReportScore(string id, long score)
    {
        Debug.Log($"Reporting {score} scores to {id}");
        Social.ReportScore(score, id, OnSuccessfull);
    }

    public static void ShowLeaderboardUI()
    {
        Debug.Log("Showing leaderboards UI...");

        Social.ShowLeaderboardUI();
    }

    #endregion /Leaderboards

    private static void OnSuccessfull(bool success)
    {
        if (success) Debug.Log("Successfull");
        else Debug.LogError("Failed");
    }

    #region SavedGames

    private static void OpenSaveData(string fileName, Action<SavedGameRequestStatus, ISavedGameMetadata> onDataOpen)
    {
        Debug.Log("*** SaveData opening...");
        if (!Social.localUser.authenticated)
        {
            onDataOpen(SavedGameRequestStatus.AuthenticationError, null);
            return;
        }
        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            onDataOpen);
        Debug.Log("*** SaveData opened");
    }

    public static void ReadSaveData(string fileName, Action<SavedGameRequestStatus, byte[]> onDataRead)
    {
        Debug.Log("*** SaveData reading...");
        if (!Social.localUser.authenticated)
        {
            onDataRead(SavedGameRequestStatus.AuthenticationError, null);
            return;
        }
        OpenSaveData(fileName, (status, metadata) =>
        {
            Debug.Log("*** SaveData opened");
            if (status == SavedGameRequestStatus.Success)
            {
                savedGameClient.ReadBinaryData(metadata, onDataRead);
                currentMetadata = metadata;
                Debug.Log("*** SaveData read " + currentMetadata);
            }
        });
    }

    public static void WriteSaveData(byte[] data, string addSaveName = "")
    {
        bool autosave = false;
        if (addSaveName == "")
        {
            Debug.Log("*** Autosaving...");
            autosave = true;
        }
        else
        {
            Debug.Log("*** Saving...");
            NotifyManager.ShowNotify(_UserInterface.Instance.gameNotify, LanguageManager.GetLocalizedValue("Saving"), NotifyType.Neitral, 1.5f);
        }

        if (!Social.localUser.authenticated || data == null || data.Length == 0)
        {
            if(!autosave) NotifyManager.ShowNotify(_UserInterface.Instance.gameNotify, $"{LanguageManager.GetLocalizedValue("CloudSaveError")}: {SavedGameRequestStatus.AuthenticationError}", NotifyType.Error, 2);
            return;
        }
        Debug.Log("*** Preparing write: opening save data...");

        OpenSaveData(DEFAULT_SAVE_NAME + addSaveName, (status, metadata) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                Debug.Log("*** " + currentMetadata);
                currentMetadata = metadata;
                Debug.Log("*** " + currentMetadata);

                Debug.Log("*** Save data opened... Preparing write...");

                WriteSaveData();
            }
        });

        void WriteSaveData()
        {
            Debug.Log("*** Writing save data");
            Debug.Log("*** 1st");
            TimeSpan totalPlayTime = currentMetadata.TotalTimePlayed + (DateTime.Now - startDateTime);
            Debug.Log("*** 2nd");
            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
            .WithUpdatedDescription(!autosave ? "Saved game at " + DateTime.Now : "Autosaved at " + DateTime.Now)
            .WithUpdatedPlayedTime(totalPlayTime);
            Debug.Log("*** 3rd");
            SavedGameMetadataUpdate updatedMetadata = builder.Build();
            Debug.Log("*** 4th");
            savedGameClient.CommitUpdate(currentMetadata,
                updatedMetadata,
                data,
                (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        Debug.Log("*** Commite update: " + status);
                        currentMetadata = metadata;
                        startDateTime = DateTime.Now;
                    }
                    else
                    {
                        Debug.LogError($"*** Error: status is {status}");
                        NotifyManager.ShowNotify(_UserInterface.Instance.gameNotify, $"{LanguageManager.GetLocalizedValue("CloudSaveError")}: {status}", NotifyType.Error, 4);
                    }
                });
            Debug.Log("*** Save data written");
            if (!autosave) NotifyManager.ShowNotify(_UserInterface.Instance.gameNotify, LanguageManager.GetLocalizedValue("CloudSaveSuccess"), NotifyType.Success);
            allowSaving = false;
        }
    }

    public static void ShowSavesUI(Action<SavedGameRequestStatus, byte[]> onDataRead, Action onDataCreate)
    {
        Debug.Log("*** Saves UI showing...");
        if (!Social.localUser.authenticated)
        {
            onDataRead(SavedGameRequestStatus.AuthenticationError, null);
            NotifyManager.ShowNotify(_UserInterface.Instance.gameNotify, $"{LanguageManager.GetLocalizedValue("CloudLoadError")}: {SavedGameRequestStatus.AuthenticationError}", NotifyType.Error, 2);
            return;
        }

        savedGameClient.ShowSelectSavedGameUI("Select saved game",
            5,
            true,
            true,
            (status, metadata) =>
            {
                if (metadata != null)
                {
                    Debug.Log("*** metadata != null " + metadata);
                    if (string.IsNullOrEmpty(metadata.Filename)) onDataCreate();
                    else ReadSaveData(metadata.Filename, onDataRead);
                }
                else Debug.Log("*** metadata == null");

            });
    }

    #endregion /SavedGames
}
