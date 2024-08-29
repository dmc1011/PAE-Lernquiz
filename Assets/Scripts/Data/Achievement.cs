using System;

[System.Serializable]
public class Achievement
{
    public int id;
    public string name;
    public AchievementPopup.Grade grade;
    public string description;
    public string popupText;
    public bool isAchieved;
    public DateTime? achievedAt;

    public Achievement(string name, AchievementPopup.Grade grade, string description, string popupText, bool isAchieved, DateTime? achievedAt)
    {
        this.name = name;
        this.description = description;
        this.isAchieved = isAchieved;
        this.achievedAt = achievedAt;
        this.popupText = popupText;
        this.grade = grade;
    }
}
