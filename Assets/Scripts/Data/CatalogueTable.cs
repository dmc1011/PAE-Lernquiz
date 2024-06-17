using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public class CatalogueTable : SQLiteHelper
{
    private const string TABLE_NAME = "Catalogue";

    public CatalogueTable() : base()
    {
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
            "id TEXT PRIMARY KEY, " +
            "name TEXT, " +
            "ownerId TEXT )";
        dbcmd.ExecuteNonQuery();
    }

    public void InsertData(Catalogue catalogue)
    {
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (id, name, ownerId) VALUES (@id, @name, @ownerId)";
        dbcmd.Parameters.Add(new SqliteParameter("@id", catalogue.id));
        dbcmd.Parameters.Add(new SqliteParameter("@name", catalogue.name));
        dbcmd.Parameters.Add(new SqliteParameter("@ownerId", catalogue.ownerId));
        dbcmd.ExecuteNonQuery();
    }

    public Catalogue GetCatalogueById(string catalogueId)
    {
        Debug.Log("Catalogue id: " + catalogueId);
        Catalogue catalogue = null;
        IDataReader reader = getDataById(catalogueId, TABLE_NAME);
        Debug.Log(reader);
        if (reader.Read())
        {
            string id = reader["id"].ToString();
            string name = reader["name"].ToString();
            string ownerId = reader["ownerId"].ToString();
            List<Question> questions = GetQuestionsByCatalogueId(catalogueId);
            catalogue = new Catalogue(id, name, ownerId, questions);
        }
        return catalogue;
    }

    public List<Question> GetQuestionsByCatalogueId(string catalogueId)
    {
        List<Question> questions = new List<Question>();
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText = "SELECT * FROM Question WHERE catalogueId = @catalogueId order by id";
        dbcmd.Parameters.Add(new SqliteParameter("@catalogueId", catalogueId));

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            string id = reader["id"].ToString();
            string text = reader["text"].ToString();
            List<Answer> answers = GetAnswersByQuestionId(id);
            questions.Add(new Question(id, text, catalogueId, answers));
        }
        return questions;
    }

    public List<Answer> GetAnswersByQuestionId(string questionId)
    {
        List<Answer> answers = new List<Answer>();
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText = "SELECT * FROM Answer WHERE questionId = @questionId order by id";
        dbcmd.Parameters.Add(new SqliteParameter("@questionId", questionId));

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            string id = reader["id"].ToString();
            string text = reader["text"].ToString();
            bool isCorrect = (bool)reader["isCorrect"];
            answers.Add(new Answer(id, text, questionId, isCorrect));
        }
        return answers;
    }
}
