using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;

public class AnswerHistoryTable
{
    private const string TABLE_NAME = "AnswerHistory";
    private IDbConnection dbConnection;

    public AnswerHistoryTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public void AddAnswerHistory(int questionId, bool wasCorrect)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (QuestionId, AnswerDate, WasCorrect) VALUES(@QuestionId, @AnswerDate, @WasCorrect)";
        dbcmd.Parameters.Add(new SqliteParameter("@QuestionId", questionId));
        dbcmd.Parameters.Add(new SqliteParameter("@AnswerDate", DateTime.Now));
        dbcmd.Parameters.Add(new SqliteParameter("@QuestionId", wasCorrect));
        dbcmd.ExecuteNonQuery();

        // check number of history entried for question
        dbcmd.CommandText = "SELECT COUNT(*) FROM " + TABLE_NAME + " WHERE QuestionId = @QuestionId";
        dbcmd.Parameters.Clear();
        dbcmd.Parameters.Add(new SqliteParameter("@QuestionId", questionId));
        long count = (long)dbcmd.ExecuteScalar();

        if (count > 5)
        {
            dbcmd.CommandText = "DELETE FROM " + TABLE_NAME + " WHERE Id IN (SELECT Id FROM " + TABLE_NAME + " WHERE QuestionId = @QuestionId ORDER BY AnswerDate ASC LIMIT 1)";
            dbcmd.ExecuteNonQuery();
        }
    }

    public List<AnswerHistory> FindAnswerHistoryByQuestionId(int questionId)
    {
        List<AnswerHistory> answerHistory = new List<AnswerHistory>();
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE QuestionId = @QuestionId";
        dbcmd.Parameters.Add(new SqliteParameter("@QuestionId", questionId));

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            bool wasCorrect = (bool)reader["WasCorrect"];
            DateTime answerDate = (DateTime)reader["answerDate"];
            answerHistory.Add(new AnswerHistory(id, questionId, answerDate, wasCorrect));
        }
        return answerHistory;
    }
}
