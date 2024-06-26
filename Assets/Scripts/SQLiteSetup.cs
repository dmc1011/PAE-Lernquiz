using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System;

public class SQLiteSetup : MonoBehaviour
{
    private const string databaseName = "Quiz_Database";
    private string dbConnectionString;
    private IDbConnection dbConnection;

    public static SQLiteSetup Instance { get; private set; }

    public CatalogueTable catalogueTable { get; private set; }
    public QuestionTable questionTable { get; private set; }
    public AnswerTable answerTable { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dbConnectionString = "URI=file:" + Application.persistentDataPath + "/" + databaseName;
            dbConnection = new SqliteConnection(dbConnectionString);
            dbConnection.Open();

            CreateTables();

            catalogueTable = new CatalogueTable(dbConnection);
            questionTable = new QuestionTable(dbConnection);
            answerTable = new AnswerTable(dbConnection);
        }
        else
        {
            Destroy(gameObject);
        }

        // TODO: delete as soon as editor is implemented

        /*
         string jsonFilePath = Path.Combine(Application.persistentDataPath, "Catalogue/0.json");
         string jsonString = File.ReadAllText(jsonFilePath);
         Catalogue catalogue = JsonUtility.FromJson<Catalogue>(jsonString);

         catalogueTable.AddCatalgoue(catalogue);

         foreach (var question in catalogue.questions)
         {
             questionTable.AddQuestion(question);
             foreach (var answer in question.answers)
             {
                 answerTable.AddAnswer(new Answer(answer.id, answer.text, question.id, answer.isCorrect));
             }
         }

        jsonFilePath = Path.Combine(Application.persistentDataPath, "Catalogue/1.json");
        jsonString = File.ReadAllText(jsonFilePath);
        catalogue = JsonUtility.FromJson<Catalogue>(jsonString);

        catalogueTable.AddCatalgoue(catalogue);

        foreach (var question in catalogue.questions)
        {
            questionTable.AddQuestion(question);
            foreach (var answer in question.answers)
            {
                answerTable.AddAnswer(new Answer(answer.id, answer.text, question.id, answer.isCorrect));
            }
        }*/
    }

    private void CreateTables()
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Catalogue (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT
            );
            CREATE TABLE IF NOT EXISTS Question (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CatalogueId INTEGER,
                Text TEXT,
                FOREIGN KEY(CatalogueId) REFERENCES Catalogue(Id)
            );
            CREATE TABLE IF NOT EXISTS Answer (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                QuestionId INTEGER,
                Text TEXT,
                IsCorrect BOOLEAN,
                FOREIGN KEY(QuestionId) REFERENCES Question(Id)
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
