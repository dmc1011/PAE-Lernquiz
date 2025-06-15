using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    // player prefs keys for daily task
    private const string LastResetDateKey = "LastResetDate";
    private const string IsDailyTaskCompletedKey = "IsDailyTaskCompleted";
    private const string DailyTaskCatalogueIdKey = "DailyTaskCatalogueId";
    private const string EvaluationTypeKey = "evaluationFor";


    public static void SetDailyTaskCatalogueId() {
        PlayerPrefs.SetInt(DailyTaskCatalogueIdKey, Global.CurrentDailyTask.catalogueIndex);
        PlayerPrefs.Save();
    }

    public static string GetLastResetDate() {
        return PlayerPrefs.GetString(LastResetDateKey, Strings.Empty);
    }

    public static void SetLastResetDate(string date) {
        PlayerPrefs.SetString(LastResetDateKey, date);
        PlayerPrefs.Save();
    }

    public static bool GetIsDailyTaskCompleted() {
        return PlayerPrefs.GetString(IsDailyTaskCompletedKey) == Strings.True;
    }

    public static void SetIsDailyTaskCompleted(bool isCompleted) {
        PlayerPrefs.SetString(IsDailyTaskCompletedKey, isCompleted ? Strings.True : Strings.False);
        PlayerPrefs.Save();
    }

    public static void SetEvaluationType(string evaluationType)
    {
        PlayerPrefs.SetString(EvaluationTypeKey, evaluationType);
        PlayerPrefs.Save();
    }
}
