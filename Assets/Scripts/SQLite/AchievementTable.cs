using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Entities;


public class AchievementTable
{
    private const string TABLE_NAME = "Achievement";
    private IDbConnection dbConnection;

    private CatalogueTable catalogueTable;
    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;
    private DailyTaskHistoryTable dailyTaskHistoryTable;

    public AchievementTable(IDbConnection dbConnection, DailyTaskHistoryTable dailyTaskHistoryTable, CatalogueTable catalogueTable, CatalogueSessionHistoryTable catalogueSessionHistoryTable)
    {
        this.dbConnection = dbConnection;
        this.dailyTaskHistoryTable = dailyTaskHistoryTable;
        this.catalogueTable = catalogueTable;
        this.catalogueSessionHistoryTable = catalogueSessionHistoryTable;
    }

    public void AddAchievement(Achievement achievement)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();

        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (Name, Description, IsAchieved, PopupText, Grade) VALUES (@Name, @Description, @IsAchieved, @PopupText, @Grade)";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", achievement.name));
        dbcmd.Parameters.Add(new SqliteParameter("@Description", achievement.description));
        dbcmd.Parameters.Add(new SqliteParameter("@IsAchieved", achievement.isAchieved));
        dbcmd.Parameters.Add(new SqliteParameter("@PopupText", achievement.popupText));
        dbcmd.Parameters.Add(new SqliteParameter("@Grade", achievement.grade.ToString()));
        dbcmd.ExecuteNonQuery();
    }

    public void UpdateAchievement(Achievement achievement)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET IsAchieved = @IsAchieved, AchievedAt = @AchievedAt WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", achievement.id));
        dbcmd.Parameters.Add(new SqliteParameter("@IsAchieved", achievement.isAchieved));
        dbcmd.Parameters.Add(new SqliteParameter("@AchievedAt", DateTime.Now));
        dbcmd.ExecuteNonQuery();
    }

    public List<Achievement> FindAllAchievements()
    {
        List<Achievement> achievements = new List<Achievement>();
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = @"
        SELECT * FROM Achievement 
        ORDER BY Name,
            CASE Grade
                WHEN 'Bronze' THEN 1
                WHEN 'Silver' THEN 2
                WHEN 'Gold' THEN 3
                ELSE 4
            END";
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string name = reader["Name"].ToString();
            string description = reader["Description"].ToString();
            string popUpText = reader["PopupText"].ToString();
            bool isAchieved = (bool)reader["IsAchieved"];
            DateTime? achievedAt = reader["AchievedAt"] != DBNull.Value ? (DateTime?)reader["AchievedAt"] : null;
            AchievementPopup.Grade grade = (AchievementPopup.Grade)Enum.Parse(typeof(AchievementPopup.Grade), reader["Grade"].ToString());
            Achievement achievement = new Achievement(name, grade, description, popUpText, isAchieved, achievedAt);
            achievement.id = id;
            achievements.Add(achievement);

        }
        return achievements;
    }

    public Achievement FindAchievementByNameAndGrade(string name, AchievementPopup.Grade grade)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE Name = @Name AND Grade = @Grade";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", name));
        dbcmd.Parameters.Add(new SqliteParameter("@Grade", grade.ToString()));

        IDataReader reader = dbcmd.ExecuteReader();

        if (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string achievementName = reader["Name"].ToString();
            string description = reader["Description"].ToString();
            string popUpText = reader["PopupText"].ToString();
            bool isAchieved = Convert.ToBoolean(reader["IsAchieved"]);
            DateTime? achievedAt = reader["AchievedAt"] != DBNull.Value ? (DateTime?)reader["AchievedAt"] : null;

            Achievement achievement = new Achievement(achievementName, grade, description, popUpText, isAchieved, achievedAt);
            return achievement;
        }
        return null;
    }

    public void MarkAchievementAsAchieved(string achievementName, AchievementPopup.Grade grade)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET IsAchieved = 1, AchievedAt = @AchievedAt WHERE Name = @Name AND Grade = @Grade";

        dbcmd.Parameters.Add(new SqliteParameter("@Name", achievementName));
        dbcmd.Parameters.Add(new SqliteParameter("@Grade", grade.ToString()));
        dbcmd.Parameters.Add(new SqliteParameter("@AchievedAt", DateTime.Now));

        dbcmd.ExecuteNonQuery();
    }

    public bool CheckAchievementCondition(string achievementName, AchievementPopup.Grade grade, Catalogue catalogue = null)
    {
        Debug.Log(achievementName + " " + grade.ToString());
        switch (achievementName + " " + grade.ToString())
        {
            // Besserwisser
            case "Besserwisser Bronze":
                return CheckCorrectAnsweredQuestionCountAchievement(50);

            case "Besserwisser Silver":
                return CheckCorrectAnsweredQuestionCountAchievement(500);

            case "Besserwisser Gold":
                return CheckCorrectAnsweredQuestionCountAchievement(1000);

            // Fokus
            case "Fokus Bronze":
                return CheckFocusAchievement(catalogue, 30);

            case "Fokus Silver":
                return CheckFocusAchievement(catalogue, 60);

            case "Fokus Gold":
                return CheckFocusAchievement(catalogue, 120);

            // Zeitmanagement
            case "Zeitmanagement Bronze":
                return CheckTimeManagementAchievement(300);

            case "Zeitmanagement Silver":
                return CheckTimeManagementAchievement(600);

            case "Zeitmanagement Gold":
                return CheckTimeManagementAchievement(1200);

            // Fleiﬂig
            case "Fleiﬂig Bronze":
                return CheckCompletedSessionsAchievement(25);

            case "Fleiﬂig Silver":
                return CheckCompletedSessionsAchievement(50);

            case "Fleiﬂig Gold":
                return CheckCompletedSessionsAchievement(100);

            // Flawless
            case "Flawless Bronze":
                return CheckFlawlessAchievement(catalogue, 1);

            case "Flawless Silver":
                return CheckFlawlessAchievement(catalogue, 5);

            case "Flawless Gold":
                return CheckFlawlessAchievement(catalogue, 10);

            // Multitalent
            case "Multitalent Bronze":
                return CheckMultitalentAchievement(5);

            case "Multitalent Silver":
                return CheckMultitalentAchievement(10);

            case "Multitalent Gold":
                return CheckMultitalentAchievement(25);

            // Intensiv
            case "Intensiv Bronze":
                return CheckIntensiveAchievement(15);

            case "Intensiv Silver":
                return CheckIntensiveAchievement(30);

            case "Intensiv Gold":
                return CheckIntensiveAchievement(60);

            // Hartn‰ckig 
            case "Hartn‰ckig Bronze":
                return CheckTotalQuestionCountAchievement(1000);

            case "Hartn‰ckig Silver":
                return CheckTotalQuestionCountAchievement(5000);

            case "Hartn‰ckig Gold":
                return CheckTotalQuestionCountAchievement(10000);

            // Daylies 
            case "Daylies Bronze":
                return GetConsecutiveCompletedDailyTasks(5);

            case "Daylies Silver":
                return GetConsecutiveCompletedDailyTasks(15);

            case "Daylies Gold":
                return GetConsecutiveCompletedDailyTasks(30);

            // Randomat  
            case "Randomat Bronze":
                return CheckRandomatAchievement(10);

            case "Randomat Silver":
                return CheckRandomatAchievement(50);

            case "Randomat Gold":
                return CheckRandomatAchievement(100);

            // Random Flawless  
            case "Random Flawless Bronze":
                return CheckRandomFlawlessAchievement(10);

            case "Random Flawless Silver":
                return CheckRandomFlawlessAchievement(25);

            case "Random Flawless Gold":
                return CheckRandomFlawlessAchievement(50);


            default:
                return false;
        }
    }

    private bool CheckCorrectAnsweredQuestionCountAchievement(int requiredCorrectCount)
    {
        List<Catalogue> allCatalogues = catalogueTable.FindAllCatalogues();
        int correctCount = 0;

        foreach (Catalogue catalogue in allCatalogues)
        {
            int correctAnsweredQuestionsForCatalogue = catalogueTable.FindCorrectAnsweredQuestionsCountByCatalogueId(catalogue.id);
            correctCount += correctAnsweredQuestionsForCatalogue;
        }

        return correctCount >= requiredCorrectCount;
    }

    private bool CheckFocusAchievement(Catalogue catalogue, int requiredMinutes)
    {
        if (catalogue != null)
        {
            return catalogue.totalTimeSpent / 60 >= requiredMinutes;
        }
        return false;
    }

    private bool CheckTimeManagementAchievement(int requiredMinutes)
    {
        List<Catalogue> allCatalogues = catalogueTable.FindAllCatalogues();
        int totalTimeSpentCount = 0;

        foreach (Catalogue catalogue in allCatalogues)
        {
            totalTimeSpentCount += catalogue.totalTimeSpent;
        }

        return totalTimeSpentCount / 60 >= requiredMinutes;
    }

    private bool CheckCompletedSessionsAchievement(int requiredSessions)
    {
        List<Catalogue> allCatalogues = catalogueTable.FindAllCatalogues();
        int totalSessionCount = 0;

        foreach (Catalogue catalogue in allCatalogues)
        {
            totalSessionCount += catalogue.sessionCount;
        }

        return totalSessionCount >= requiredSessions;
    }

    private bool CheckFlawlessAchievement(Catalogue catalogue, int requiredSessions)
    {
        if (catalogue != null)
        {
            return catalogue.errorFreeSessionCount >= requiredSessions;
        }
        return false;
    }

    private bool CheckMultitalentAchievement(int requiredSessions)
    {
        List<Catalogue> allCatalogues = catalogueTable.FindAllCatalogues();
        int totalErrorFreeSessionCount = 0;

        foreach (Catalogue catalogue in allCatalogues)
        {
            totalErrorFreeSessionCount += catalogue.errorFreeSessionCount;
        }

        return totalErrorFreeSessionCount >= requiredSessions;
    }

    private bool CheckIntensiveAchievement(int requiredMinutes)
    {
        int timeSpent = catalogueSessionHistoryTable.FindTimeSpentInCompletedSessionsToday();

        return timeSpent / 60 >= requiredMinutes;
    }

    private bool CheckTotalQuestionCountAchievement(int requiredCount)
    {
        int count = 0;
        List<Catalogue> allCatalogues = catalogueTable.FindAllCatalogues();

        foreach (Catalogue catalogue in allCatalogues)
        {
            foreach (Question question in catalogue.questions)
            {
                count += question.totalAnsweredCount;
            }
        }

        return count >= requiredCount;
    }

    public bool GetConsecutiveCompletedDailyTasks(int requiredDays)
    {
        List<DailyTaskHistory> dailyTaskEntries = dailyTaskHistoryTable.FindAllDailyTaskEntries();
        int consecutiveDays = 0;

        DateTime? previousDate = null;

        foreach (DailyTaskHistory entry in dailyTaskEntries)
        {
            if (entry.taskCompleted)
            {
                DateTime currentDate = entry.taskDate;

                if (previousDate == null)
                {
                    previousDate = currentDate;
                    consecutiveDays = 1;
                }
                else
                {
                    if (previousDate.Value.AddDays(-1) == currentDate)
                    {
                        consecutiveDays++;
                        previousDate = currentDate;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        Debug.Log(consecutiveDays);
        return consecutiveDays >= requiredDays;
    }

    private bool CheckRandomatAchievement(int requiredCount)
    {
        List<Catalogue> allCatalogues = catalogueTable.FindAllCatalogues();
        int completedRandomCount = 0;

        foreach (Catalogue catalogue in allCatalogues)
        {
            completedRandomCount += catalogue.completedRandomQuizCount;
        }

        return completedRandomCount >= requiredCount;
    }

    private bool CheckRandomFlawlessAchievement(int requiredCount)
    {
        List<Catalogue> allCatalogues = catalogueTable.FindAllCatalogues();
        int errorFreeRandomQuizCount = 0;

        foreach (Catalogue catalogue in allCatalogues)
        {
            errorFreeRandomQuizCount += catalogue.errorFreeRandomQuizCount;
        }

        return errorFreeRandomQuizCount >= requiredCount;
    }

}
