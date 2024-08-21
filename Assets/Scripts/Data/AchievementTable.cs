using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;

public class AchievementTable
{
    private const string TABLE_NAME = "Achievement";
    private IDbConnection dbConnection;

    private QuestionTable questionTable;
    private CatalogueTable catalogueTable;
    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;

    public AchievementTable(IDbConnection dbConnection, QuestionTable questionTable, CatalogueTable catalogueTable, CatalogueSessionHistoryTable catalogueSessionHistoryTable)
    {
        this.dbConnection = dbConnection;
        this.questionTable = questionTable;
        this.catalogueTable = catalogueTable;
        this.catalogueSessionHistoryTable = catalogueSessionHistoryTable;
    }

    public void AddAchievement(Achievement achievement)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();

        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (Name, Description, IsAchieved) VALUES (@Name, @Description, @IsAchieved)";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", achievement.name));
        dbcmd.Parameters.Add(new SqliteParameter("@Description", achievement.description));
        dbcmd.Parameters.Add(new SqliteParameter("@IsAchieved", achievement.isAchieved));
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

    public bool CheckAchievementCondition(string achievementName, Catalogue catalogue = null)
    {
        switch (achievementName)
        {
            // Besserwisser
            case "Besserwisser Bronze":
                return CheckCorrectAnsweredQuestionCountAchievement(50);

            case "Besserwisser Silber":
                return CheckCorrectAnsweredQuestionCountAchievement(500);

            case "Besserwisser Gold":
                return CheckCorrectAnsweredQuestionCountAchievement(1000);

            // Fokus
            case "Fokus Bronze":
                return CheckFocusAchievement(catalogue, 30);

            case "Fokus Silber":
                return CheckFocusAchievement(catalogue, 60);

            case "Fokus Gold":
                return CheckFocusAchievement(catalogue, 120);

            // Zeitmanagement
            case "Zeitmanagement Bronze":
                return CheckTimeManagementAchievement(300);

            case "Zeitmanagement Silber":
                return CheckTimeManagementAchievement(600);

            case "Zeitmanagement Gold":
                return CheckTimeManagementAchievement(1200);

            // Fleiﬂig
            case "Fleiﬂig Bronze":
                return CheckCompletedSessionsAchievement(25);

            case "Fleiﬂig Silber":
                return CheckCompletedSessionsAchievement(50);

            case "Fleiﬂig Gold":
                return CheckCompletedSessionsAchievement(100);

            // Flawless
            case "Flawless Bronze":
                return CheckFlawlessAchievement(catalogue, 1);

            case "Flawless Silber":
                return CheckFlawlessAchievement(catalogue, 5);

            case "Flawless Gold":
                return CheckFlawlessAchievement(catalogue, 10);

            // Multitalent
            case "Multitalent Bronze":
                return CheckMultitalentAchievement(5);

            case "Multitalent Silber":
                return CheckMultitalentAchievement(10);

            case "Multitalent Gold":
                return CheckMultitalentAchievement(25);

            // Intensiv
            case "Intensiv Bronze":
                return CheckIntensiveAchievement(15);

            case "Intensiv Silber":
                return CheckIntensiveAchievement(30);

            case "Intensiv Gold":
                return CheckIntensiveAchievement(60);

            // Hartn‰ckig 
            case "Hartn‰ckig  Bronze":
                return CheckTotalQuestionCountAchievement(1000);

            case "Hartn‰ckig  Silber":
                return CheckTotalQuestionCountAchievement(5000);

            case "Hartn‰ckig  Gold":
                return CheckTotalQuestionCountAchievement(10000);


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
}
