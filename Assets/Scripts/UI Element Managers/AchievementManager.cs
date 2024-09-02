using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    AchievementTable achievementTable;
    [SerializeField] private AchievementPopup achievementPopup;
    [SerializeField] private Transform parentForRendering;

    // Start is called before the first frame update
    void Awake()
    {
        achievementTable = SQLiteSetup.Instance.achievementTable;
    }

    public void CheckNextAchievementLevelAndSetAchievedStatus(string achievementName, Catalogue catalogue = null)
    {
        List<AchievementPopup.Grade> grades = new List<AchievementPopup.Grade> { AchievementPopup.Grade.Bronze, AchievementPopup.Grade.Silver, AchievementPopup.Grade.Gold };

        foreach (AchievementPopup.Grade grade in grades)
        {
            Achievement achievement = achievementTable.FindAchievementByNameAndGrade(achievementName, grade);
            achievement.isAchieved = true;
            if (!achievement.isAchieved)
            {
                //bool isAchieved = achievementTable.CheckAchievementCondition(achievementName, grade, catalogue);

                if (true)
                {
                    //achievementTable.MarkAchievementAsAchieved(achievementName, grade);
                    var a = Instantiate(achievementPopup, parentForRendering);
                    a.SetData(grade, achievement.name + " " + grade, achievement.popupText);
                    a.Fire();
                }

                return;
                //break;
            }
        }
    }
}
