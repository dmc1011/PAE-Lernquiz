using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

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
        }
        else
        {
            Destroy(gameObject);
        }

        // TODO: delete as soon as editor is implemented
        //AddCatalogueFromJson("Catalogue/0.json");
        //AddCatalogueFromJson("Catalogue/1.json");

    }

    public void AddCatalogueFromJson(string jsonRelativeFilePath)
    {
        string jsonFilePath = Path.Combine(Application.persistentDataPath, jsonRelativeFilePath);
        string jsonString = File.ReadAllText(jsonFilePath);
        Catalogue catalogue = JsonUtility.FromJson<Catalogue>(jsonString);
        catalogueTable.AddCatalogue(catalogue);
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
                FOREIGN KEY(CurrentQuestionId) REFERENCES Question(Id)
            );
            CREATE TABLE IF NOT EXISTS Question (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CatalogueId INTEGER,
                Text TEXT,
                Name TEXT,
                CorrectAnsweredCount INTEGER DEFAULT 0,
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
                IsCompleted BOOL,
                FOREIGN KEY(CatalogueId) REFERENCES Catalogue(Id) ON DELETE CASCADE
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
}
