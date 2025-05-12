using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Entities;


public class SQLiteSetup : MonoBehaviour
{
    // SQLite Database
    private const string databaseName = "Quiz_Database";
    private string dbConnectionString;
    private IDbConnection dbConnection;
    public static SQLiteSetup Instance { get; private set; }

    // Tables
    public CatalogueTable catalogueTable { get; private set; }
    public QuestionTable questionTable { get; private set; }
    public AnswerTable answerTable { get; private set; }
    public AnswerHistoryTable answerHistoryTable { get; private set; }
    public CatalogueSessionHistoryTable catalogueSessionHistoryTable { get; private set; }
    public AchievementTable achievementTable { get; private set; }
    public DailyTaskHistoryTable dailyTaskHistoryTable { get; private set; }

    // SQLite Methods and Config
    [DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_config(int config);

    [DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_shutdown();

    [DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_initialize();

    private const int SQLITE_CONFIG_SERIALIZED = 3;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sqlite3_shutdown();

            int result = sqlite3_config(SQLITE_CONFIG_SERIALIZED);
            if (result != 0)
            {
                Debug.LogError("Fehler beim Setzen des Threading-Modus für SQLite! Result: " + result);
            }
            else
            {
                Debug.Log("SQLite Threading-Modus erfolgreich auf SERIALIZED gesetzt.");
            }

            sqlite3_initialize();

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
            dailyTaskHistoryTable = new DailyTaskHistoryTable(dbConnection);
            achievementTable = new AchievementTable(dbConnection, dailyTaskHistoryTable, catalogueTable, catalogueSessionHistoryTable);

            AddInitialAchievementsIfNeeded();
            LoadExampleCatalogues();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void InjectSupabase()
    {
        /*// create Gesundheitslehre catalogue
        TextAsset catalogue1 = Resources.Load<TextAsset>("Gesundheitslehre_Beispielkatalog");
        if (catalogue1 != null)
        {
            Debug.Log("Start Cat1");
            Catalogue catalogue = JsonConvert.DeserializeObject<Catalogue>(catalogue1.text);
            DataInjectionService.InjectQuestions(1, catalogue);
            Debug.Log("Done Cat1");
        }
        
        TextAsset catalogue2 = Resources.Load<TextAsset>("Erziehungswissenschaft_Beispielkatalog");
        if (catalogue2 != null)
        {
            Debug.Log("Start Cat2");
            Catalogue catalogue = JsonConvert.DeserializeObject<Catalogue>(catalogue2.text);
            DataInjectionService.InjectQuestions(0, catalogue);
            Debug.Log("Done Cat2");
        }
        */

        //DataInjectionService.InjectAchievements();
    }

    private void LoadExampleCatalogues()
    {
        List<Catalogue> catalogues = catalogueTable.FindAllCatalogues();
        if (catalogues != null && catalogues.Count != 0)
            return;

        // create Gesundheitslehre catalogue
        TextAsset catalogue1 = Resources.Load<TextAsset>("Gesundheitslehre_Beispielkatalog");
        if(catalogue1 != null)
        {
            Catalogue catalogue = JsonConvert.DeserializeObject<Catalogue>(catalogue1.text);
            CreateNewCatalogue(catalogue);
        }

        // create Erziehungswissenschaft catalogue
        TextAsset catalogue2 = Resources.Load<TextAsset>("Erziehungswissenschaft_Beispielkatalog");
        if (catalogue2 != null)
        {
            Catalogue catalogue = JsonConvert.DeserializeObject<Catalogue>(catalogue2.text);
            CreateNewCatalogue(catalogue);
        }
    }

    private void CreateNewCatalogue(Catalogue catalogue)
    {
        bool isCatalogueValid = false;
        int i = 2;

        while (!isCatalogueValid)
        {
            if (catalogueTable.FindCatalogueByName(catalogue.name) == null)
            {
                isCatalogueValid = true;
                catalogueTable.AddCatalogue(catalogue);
            }
            else
            {
                catalogue.name = catalogue.name + " " + i;
                i++;
            }
        }
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
                CompletedRandomQuizCount INTEGER DEFAULT 0,
                ErrorFreeRandomQuizCount INTEGER DEFAULT 0,
                FOREIGN KEY(CurrentQuestionId) REFERENCES Question(Id)
            );
            CREATE TABLE IF NOT EXISTS Question (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CatalogueId INTEGER,
                Text TEXT,
                Name TEXT,
                CorrectAnsweredCount INTEGER DEFAULT 0,
                EnabledForPractice BOOLEAN DEFAULT 0,
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
                Name TEXT NOT NULL,
                Grade TEXT NOT NULL,
                Description TEXT NOT NULL,
                PopupText TEXT NOT NULL,
                IsAchieved BOOLEAN DEFAULT FALSE,
                AchievedAt DATETIME,
                UNIQUE(Name, Grade)
            );
            CREATE TABLE IF NOT EXISTS DailyTaskHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TaskDate DATE NOT NULL UNIQUE,
                CorrectAnswersCount INTEGER DEFAULT 0,
                TaskCompleted BOOLEAN DEFAULT FALSE
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
        dbcmd.CommandText = "SELECT COUNT(*) FROM Achievement";
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
            new Achievement("Flawless", AchievementPopup.Grade.Bronze, "Schließe zum ersten Mal einen gesamten Katalog ohne einen einzigen Fehler ab", "Du hast zum ersten Mal einen Katalog ohne Fehler abgeschlossen!", false, null),
            new Achievement("Flawless", AchievementPopup.Grade.Silver, "Meistere einen Katalog fünfmal ohne Fehler", "Du hast einen Katalog fünfmal ohne Fehler abgeschlossen!", false, null),
            new Achievement("Flawless", AchievementPopup.Grade.Gold, "Erreiche Perfektion, indem du einen Katalog zehnmal fehlerfrei abschließt", "Du hast einen Katalog zehnmal ohne Fehler abgeschlossen!", false, null),
           
            new Achievement("Multitalent", AchievementPopup.Grade.Bronze, "Beweise deine Vielseitigkeit, indem du fünf Katalog-Durchläufe ohne Fehler meisterst", "Du hast fünf Katalog-Durchläufe ohne Fehler abgeschlossen!", false, null),
            new Achievement("Multitalent", AchievementPopup.Grade.Silver, "Zeige deine umfassende Expertise, indem du zehn Katalog-Durchläufe fehlerfrei abschließt", "Du hast zehn Katalog-Durchläufe ohne Fehler abgeschlossen!", false, null),
            new Achievement("Multitalent", AchievementPopup.Grade.Gold, "Demonstriere dein Können, indem du 25 Katalog-Durchläufe ohne einen einzigen Fehler meisterst", "Du hast 25 Katalog-Durchläufe ohne Fehler abgeschlossen!", false, null),

            new Achievement("Besserwisser", AchievementPopup.Grade.Bronze, "Beantworte 50 Fragen korrekt und zeige, dass du auf dem richtigen Weg bist", "Du hast 50 Fragen richtig beantwortet!", false, null),
            new Achievement("Besserwisser", AchievementPopup.Grade.Silver, "Beweise dein Wissen mit 500 richtig beantworteten Fragen", "Du hast 500 Fragen richtig beantwortet!", false, null),
            new Achievement("Besserwisser", AchievementPopup.Grade.Gold, "Zeige deine herausragenden Kenntnisse, indem du 1000 Fragen korrekt beantwortest", "Du hast 1000 Fragen richtig beantwortet!", false, null),

            new Achievement("Daylies", AchievementPopup.Grade.Bronze, "Schließe fünfmal den Daily Task erfolgreich ab", "Du hast den Daily Task fünfmal abgeschlossen!", false, null),
            new Achievement("Daylies", AchievementPopup.Grade.Silver, "Erreiche das Ziel, indem du fünfzehnmal den Daily Task meisterst", "Du hast den Daily Task fünfzehnmal abgeschlossen!", false, null),
            new Achievement("Daylies", AchievementPopup.Grade.Gold, "Zeige Ausdauer, indem du dreißigmal den Daily Task erfolgreich abschließt", "Du hast den Daily Task dreißigmal abgeschlossen!", false, null),

            new Achievement("Randomat", AchievementPopup.Grade.Bronze, "Schließe zehn Random Quiz Runden erfolgreich ab", "Du hast zehn Random Quiz Runden abgeschlossen!", false, null),
            new Achievement("Randomat", AchievementPopup.Grade.Silver, "Beweise deine Flexibilität, indem du fünfzig Random Quiz Runden meisterst", "Du hast 50 Random Quiz Runden abgeschlossen!", false, null),
            new Achievement("Randomat", AchievementPopup.Grade.Gold, "Zeige deine Ausdauer, indem du hundert Random Quiz Runden erfolgreich abschließt", "DU hast 100 Random Quiz Runden abgeschlossen!", false, null),

            new Achievement("Hartnäckig", AchievementPopup.Grade.Bronze, "Beantworte insgesamt 1000 Fragen", "Du hast 1000 Fragen beantwortet!", false, null),
            new Achievement("Hartnäckig", AchievementPopup.Grade.Silver, "Beantworte insgesamt 5000 Fragen", "Du hast 5000 Fragen beantwortet!", false, null),
            new Achievement("Hartnäckig", AchievementPopup.Grade.Gold, "Beantworte insgesamt 10000 Fragen", "Du hast 10000 Fragen beantwortet!", false, null),

            new Achievement("Fokus", AchievementPopup.Grade.Bronze, "Verbringe 30 Minuten in einem einzigen Katalog", "Du hast 30 Minuten in einem Katalog verbracht!", false, null),
            new Achievement("Fokus", AchievementPopup.Grade.Silver, "Verbringe 60 Minuten in einem einzigen Katalog", "Du hast 60 Minuten in einem Katalog verbracht!", false, null),
            new Achievement("Fokus", AchievementPopup.Grade.Gold, "Verbringe 120 Minuten in einem einzigen Katalog", "Du hast 120 Minuten in einem Katalog verbracht!", false, null),

            new Achievement("Zeitmanagement", AchievementPopup.Grade.Bronze, "Verbringe insgesamt 300 Minuten in Katalogen", "Du hast insgesamt 300 Minuten in Katalogen verbracht!", false, null),
            new Achievement("Zeitmanagement", AchievementPopup.Grade.Silver, "Verbringe insgesamt 600 Minuten in Katalogen", "Du hast insgesamt 600 Minuten in Katalogen verbracht!", false, null),
            new Achievement("Zeitmanagement", AchievementPopup.Grade.Gold, "Verbringe insgesamt 1200 Minuten in Katalogen", "Du hast insgesamt 1200 Minuten in Katalogen verbracht!", false, null),

            new Achievement("Random Flawless", AchievementPopup.Grade.Bronze, "Schließe zehn Random Quiz Runden ohne Fehler ab", "Du hast zehn Random Quiz Runden ohne Fehler abgeschlossen!", false, null),
            new Achievement("Random Flawless", AchievementPopup.Grade.Silver, "Schließe 25 Random Quiz Runden ohne Fehler ab", "Du hast 25 Random Quiz Runden ohne Fehler abgeschlossen!", false, null),
            new Achievement("Random Flawless", AchievementPopup.Grade.Gold, "Schließe 50 Random Quiz Runden ohne Fehler ab", "Du hast 50 Random Quiz Runden ohne Fehler abgeschlossen!", false, null),

            new Achievement("Intensiv", AchievementPopup.Grade.Bronze, "Verbringe 15 Minuten an einem Tag in Katalogen", "Du hast 15 Minuten an einem Tag in Katalogen verbracht!", false, null),
            new Achievement("Intensiv", AchievementPopup.Grade.Silver, "Verbringe 30 Minuten an einem Tag in Katalogen", "Du hast 30 Minuten an einem Tag in Katalogen verbracht!", false, null),
            new Achievement("Intensiv", AchievementPopup.Grade.Gold, "Verbringe 60 Minuten an einem Tag in Katalogen", "Du hast 60 Minuten an einem Tag in Katalogen verbracht!", false, null),

            new Achievement("Fleißig", AchievementPopup.Grade.Bronze, "Schließe 25 Quiz-Durchläufe ab", "Du hast 25 Quiz-Durchläufe abgeschlossen!", false, null),
            new Achievement("Fleißig", AchievementPopup.Grade.Silver, "Schließe 50 Quiz-Durchläufe ab", "Du hast 25 Quiz-Durchläufe abgeschlossen!", false, null),
            new Achievement("Fleißig", AchievementPopup.Grade.Gold, "Schließe 100 Quiz-Durchläufe ab", "Du hast 25 Quiz-Durchläufe abgeschlossen!", false, null)
        };

        foreach (Achievement achievement in achievements)
        {
            achievementTable.AddAchievement(achievement);
        }
    }
}
