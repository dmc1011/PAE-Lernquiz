using System.Collections.Generic;
using UnityEngine;

public class AchievementsScreen : MonoBehaviour
{

    [SerializeField] private Transform scrollTransform;
    [SerializeField] private AchievementPopup achievementPopUp;
    private AchievementTable achievementTable;
    List<Achievement> achievements = new();

    void Start()
    {
        achievementTable = SQLiteSetup.Instance.achievementTable;
        achievements = achievementTable.FindAllAchievements();

        foreach (Achievement achievement in achievements)
        {
            AchievementPopup entry = Instantiate(achievementPopUp, scrollTransform);
            entry.SetData(
                AchievementPopup.Grade.None,
                achievement.name + " " + achievement.grade,
                achievement.description
            );
        }
    }

    void Update()
    {
        
    }
}
