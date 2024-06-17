using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class SQLiteSetup : MonoBehaviour
{
    void Start()
    {
        CatalogueTable catalogueTable = new CatalogueTable();
        QuestionTable questionTable = new QuestionTable();
        AnswerTable answerTable = new AnswerTable();


       /* string jsonFilePath = Path.Combine(Application.persistentDataPath, "Catalogue/0.json");
        string jsonString = File.ReadAllText(jsonFilePath);
        Catalogue catalogue = JsonUtility.FromJson<Catalogue>(jsonString);

        catalogueTable.InsertData(catalogue);

        foreach (var question in catalogue.questions)
        {
            questionTable.InsertData(question);
            foreach (var answer in question.answers)
            {
                answerTable.InsertData(new Answer(answer.id, answer.text, question.id, answer.isCorrect));
            }
        }*/
    }
}
