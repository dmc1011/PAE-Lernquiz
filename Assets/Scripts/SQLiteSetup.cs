using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System;
using System.Collections.Generic;

public class SQLiteSetup : MonoBehaviour
{
    private const string databaseName = "Quiz_Database";
    private string dbConnectionString;
    private IDbConnection dbConnection;

    public static SQLiteSetup Instance { get; private set; }

    public CatalogueTable catalogueTable { get; private set; }
    public QuestionTable questionTable { get; private set; }
    public AnswerTable answerTable { get; private set; }
    public AnswerHistoryTable answerHistoryTable { get; private set; }
    public CatalogueSessionHistoryTable catalogueSessionHistoryTable { get; private set; }
    public AchievementTable achievementTable { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dbConnectionString = "URI=file:" + Application.persistentDataPath + "/" + databaseName;
            dbConnection = new SqliteConnection(dbConnectionString);
            dbConnection.Open();

            IDbCommand enableForeignKeyCommand = dbConnection.CreateCommand();
            enableForeignKeyCommand.CommandText = "PRAGMA foreign_keys = ON;";
            enableForeignKeyCommand.ExecuteNonQuery();

            CreateTables();

            answerHistoryTable = new AnswerHistoryTable(dbConnection);
            questionTable = new QuestionTable(dbConnection);
            answerTable = new AnswerTable(dbConnection);
            catalogueSessionHistoryTable = new CatalogueSessionHistoryTable(dbConnection);
            catalogueTable = new CatalogueTable(dbConnection, questionTable, answerTable, answerHistoryTable, catalogueSessionHistoryTable);
            achievementTable = new AchievementTable(dbConnection, questionTable, catalogueTable, catalogueSessionHistoryTable);
        }
        else
        {
            Destroy(gameObject);
        }
        AddInitialAchievementsIfNeeded();
    }

    private void CreateTables()
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Catalogue (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                CurrentQuestionId INTEGER,
                TotalTimeSpent INTEGER DEFAULT 0,
                SessionCount INTEGER DEFAULT 0,
                ErrorFreeSessionCount INTEGER DEFAULT 0,
                FOREIGN KEY(CurrentQuestionId) REFERENCES Question(Id)
            );
            CREATE TABLE IF NOT EXISTS Question (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CatalogueId INTEGER,
                Text TEXT,
                Name TEXT,
                CorrectAnsweredCount INTEGER DEFAULT 0,
                TotalAnsweredCount INTEGER DEFAULT 0,
                FOREIGN KEY(CatalogueId) REFERENCES Catalogue(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS Answer (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                QuestionId INTEGER,
                Text TEXT,
                IsCorrect BOOLEAN,
                FOREIGN KEY(QuestionId) REFERENCES Question(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS AnswerHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                QuestionId INTEGER,
                AnswerDate DATETIME,
                WasCorrect BOOLEAN,
                SessionId INTEGER,
                FOREIGN KEY(QuestionId) REFERENCES Question(Id) ON DELETE CASCADE,
                FOREIGN KEY(SessionId) REFERENCES CatalogueSessionHistory(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS CatalogueSessionHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CatalogueId INTEGER,
                SessionDate DATETIME,
                TimeSpent INTEGER,
                IsCompleted BOOLEAN,
                IsErrorFree BOOLEAN DEFAULT TRUE,
                FOREIGN KEY(CatalogueId) REFERENCES Catalogue(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS Achievement (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                Description TEXT NOT NULL,
                IsAchieved BOOLEAN DEFAULT FALSE,
                AchievedAt DATETIME
            );
        ";
        dbCommand.ExecuteNonQuery();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            dbConnection.Close();

        }
    }

    private void AddInitialAchievementsIfNeeded()
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT COUNT(*) FROM Achievement;";
        int achievementCount = Convert.ToInt32(dbcmd.ExecuteScalar());

        if (achievementCount == 0)
        {
            AddInitialAchievements();
        }
    }

    private void AddInitialAchievements()
    {
        List<Achievement> achievements = new()
        {
            new Achievement("Flawless Bronze", "Zum ersten Mal einen Katalog ohne Fehler abschlieﬂen", false, DateTime.Now),
            new Achievement("Flawless Silber", "Einen Katalog 5 mal ohne Fehler abschlieﬂen", false, DateTime.Now),
            new Achievement("Flawless Gold", "Einen Katalog 10 mal ohne Fehler abschlieﬂen", false, DateTime.Now),
            new Achievement("Multitalent Bronze", "5 Kataloge ohne Fehler abschlieﬂen", false, DateTime.Now),
            new Achievement("Multitalent Silber", "10 Kataloge ohne Fehler abschlieﬂen", false, DateTime.Now),
            new Achievement("Multitalent Gold", "25 Kataloge ohne Fehler abschlieﬂen", false, DateTime.Now),
            new Achievement("Besserwisser Bronze", "50 Fragen richtig beantwortet", false, DateTime.Now),
            new Achievement("Besserwisser Silber", "500 Fragen richtig beantwortet", false, DateTime.Now),
            new Achievement("Besserwisser Gold", "1000 Fragen richtig beantwortet", false, DateTime.Now),
            new Achievement("Streak Bronze", "10 Tage in Folge 5 Fragen beantwortet", false, DateTime.Now),
            new Achievement("Streak Silber", "25 Tage in Folge 5 Fragen beantwortet", false, DateTime.Now),
            new Achievement("Streak Gold", "50 Tage in Folge 5 Fragen beantwortet", false, DateTime.Now),
            new Achievement("Daylies Bronze", "10 mal alle Daylies abgeschlossen", false, DateTime.Now),
            new Achievement("Daylies Silber", "25 mal alle Daylies abgeschlossen", false, DateTime.Now),
            new Achievement("Daylies Gold", "50 mal alle Daylies abgeschlossen", false, DateTime.Now),
            new Achievement("Level Bronze", "Level 10 erreicht", false, DateTime.Now),
            new Achievement("Level Silber", "Level 25 erreicht", false, DateTime.Now),
            new Achievement("Level Gold", "Level 50 erreicht", false, DateTime.Now),
            new Achievement("Ersteller Bronze", "1 Katalog erstellt", false, DateTime.Now),
            new Achievement("Ersteller Silber", "5 Kataloge erstellt", false, DateTime.Now),
            new Achievement("Ersteller Gold", "10 Kataloge erstellt", false, DateTime.Now),
            new Achievement("Vernetzt Bronze", "1 Katalog exportiert", false, DateTime.Now),
            new Achievement("Vernetzt Silber", "5 Kataloge exportiert", false, DateTime.Now),
            new Achievement("Vernetzt Gold", "10 Kataloge exportiert", false, DateTime.Now),
            new Achievement("Randomat Bronze", "10 Random Quiz Runden abgeschlossen", false, DateTime.Now),
            new Achievement("Randomat Silber", "50 Random Quiz Runden abgeschlossen", false, DateTime.Now),
            new Achievement("Randomat Gold", "100 Random Quiz Runden abgeschlossen", false, DateTime.Now),
            new Achievement("Importeur Bronze", "1 Katalog importiert", false, DateTime.Now),
            new Achievement("Importeur Silber", "5 Kataloge importiert", false, DateTime.Now),
            new Achievement("Importeur Gold", "10 Kataloge importiert", false, DateTime.Now),
            new Achievement("Hartn‰ckig Bronze", "1000 Fragen beantwortet", false, DateTime.Now),
            new Achievement("Hartn‰ckig Silber", "5000 Fragen beantwortet", false, DateTime.Now),
            new Achievement("Hartn‰ckig Gold", "10000 Fragen beantwortet", false, DateTime.Now),
            new Achievement("Fokus Bronze", "In einem Katalog 30 Minuten verbracht", false, DateTime.Now),
            new Achievement("Fokus Silber", "In einem Katalog 60 Minuten verbracht", false, DateTime.Now),
            new Achievement("Fokus Gold", "In einem Katalog 120 Minuten verbracht", false, DateTime.Now),
            new Achievement("Zeitmanagement Bronze", "300 Minuten in Katalogen verbracht", false, DateTime.Now),
            new Achievement("Zeitmanagement Silber", "600 Minuten in Katalogen verbracht", false, DateTime.Now),
            new Achievement("Zeitmanagement Gold", "1200 Minuten in Katalogen verbracht", false, DateTime.Now),
            new Achievement("Random Flawless Bronze", "10 Random Quiz Runden ohne Fehler abgeschlossen", false, DateTime.Now),
            new Achievement("Random Flawless Silber", "25 Random Quiz Runden ohne Fehler abgeschlossen", false, DateTime.Now),
            new Achievement("Random Flawless Gold", "50 Random Quiz Runden ohne Fehler abgeschlossen", false, DateTime.Now),
            new Achievement("Intensiv Bronze", "15 Minuten an einem Tag in Katalogen verbracht", false, DateTime.Now),
            new Achievement("Intensiv Silber", "30 Minuten an einem Tag in Katalogen verbracht", false, DateTime.Now),
            new Achievement("Intensiv Gold", "60 Minuten an einem Tag in Katalogen verbracht", false, DateTime.Now),
            new Achievement("Fleiﬂig Bronze", "25 Durchl‰ufe abgeschlossen", false, DateTime.Now),
            new Achievement("Fleiﬂig Silber", "50 Durchl‰ufe abgeschlossen", false, DateTime.Now),
            new Achievement("Fleiﬂig Gold", "100 Durchl‰ufe abgeschlossen", false, DateTime.Now)
        };

        foreach (Achievement achievement in achievements)
        {
            achievementTable.AddAchievement(achievement);
        }
    }
}
