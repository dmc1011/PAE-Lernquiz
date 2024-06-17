using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public class AnswerTable : SQLiteHelper
{
    private const string TABLE_NAME = "Answer";

    public AnswerTable() : base()
    {
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
            "id TEXT PRIMARY KEY, " +
            "text TEXT, " +
            "questionId TEXT, " +
            "isCorrect BOOLEAN)";
        dbcmd.ExecuteNonQuery();
    }

    public void InsertData(Answer answer)
    {
        IDbCommand dbcmd = getDbCommand();

        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (id, text, questionId, isCorrect) VALUES (@id, @text, @questionId, @isCorrect)";
        dbcmd.Parameters.Add(new SqliteParameter("@id", answer.id));
        dbcmd.Parameters.Add(new SqliteParameter("@text", answer.text));
        dbcmd.Parameters.Add(new SqliteParameter("@questionId", answer.questionId));
        dbcmd.Parameters.Add(new SqliteParameter("@isCorrect", answer.isCorrect));
        dbcmd.ExecuteNonQuery();
    }
}
