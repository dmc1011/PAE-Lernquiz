using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsScreen : MonoBehaviour
{

    [SerializeField] private Transform scrollTransform;
    [SerializeField] private Transform canvasTransform;

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
            entry.SetData(achievement);
            entry.GetComponent<Button>().onClick.AddListener(
                delegate {
                    AchievementPopup example = Instantiate(achievementPopUp, canvasTransform);
                    example.SetData(achievement);
                    example.Fire();
                }
            );
        }
    }

    void Update()
    {
        
    }


}
