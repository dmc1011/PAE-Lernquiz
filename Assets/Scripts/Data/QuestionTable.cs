using Mono.Data.Sqlite;
using System.Collections.Generic;
using System;
using System.Data;

public class QuestionTable
{
    private const string TABLE_NAME = "Question";
    private IDbConnection dbConnection;

    public QuestionTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public void AddQuestion(Question question)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (CatalogueId, Text, Name) VALUES (@CatalogueId, @Text, @Name)";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", question.catalogueId));
        dbcmd.Parameters.Add(new SqliteParameter("@Text", question.text));
        dbcmd.Parameters.Add(new SqliteParameter("@Name", question.name));
        dbcmd.ExecuteNonQuery();
        
    }

    public void UpdateQuestion(Question question)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET CorrectAnsweredCount = @CorrectAnsweredCount, EnabledForPractice = @EnabledForPractice, TotalAnsweredCount = @TotalAnsweredCount WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", question.id));
        dbcmd.Parameters.Add(new SqliteParameter("@CorrectAnsweredCount", question.correctAnsweredCount));
        dbcmd.Parameters.Add(new SqliteParameter("@EnabledForPractice", question.enabledForPractice));
        dbcmd.Parameters.Add(new SqliteParameter("@TotalAnsweredCount", question.totalAnsweredCount));
        dbcmd.ExecuteNonQuery();
    }
}
