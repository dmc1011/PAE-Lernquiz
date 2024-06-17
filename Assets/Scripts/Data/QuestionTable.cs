using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public class QuestionTable : SQLiteHelper
{
    private const string TABLE_NAME = "Question";
    
    public QuestionTable() : base()
    {
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
            "id TEXT PRIMARY KEY, " +
            "catalogueId TEXT, " +
            "text TEXT )";
        dbcmd.ExecuteNonQuery();
    }

    public void InsertData(Question question)
    {
        IDbCommand dbcmd = getDbCommand();
        
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (id, catalogueId, text) VALUES (@id, @catalogueId, @text)";
        dbcmd.Parameters.Add(new SqliteParameter("@id", question.id));
        dbcmd.Parameters.Add(new SqliteParameter("@catalogueId", question.catalogueId));
        dbcmd.Parameters.Add(new SqliteParameter("@text", question.text));
        dbcmd.ExecuteNonQuery();
        
    }
}
