using System;

[System.Serializable]
public class Achievement
{
    public int id;
    public string name;
    public string description;
    public bool isAchieved;
    public DateTime achievedAt;

    public Achievement(string name, string description, bool isAchieved, DateTime achievedAt)
    {
        this.name = name;
        this.description = description;
        this.isAchieved = isAchieved;
        this.achievedAt = achievedAt;
    }
}
