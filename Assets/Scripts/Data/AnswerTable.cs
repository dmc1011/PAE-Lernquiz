using Mono.Data.Sqlite;
using System.Data;

public class AnswerTable
{
    private const string TABLE_NAME = "Answer";
    private IDbConnection dbConnection;

    public AnswerTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public void InsertData(Answer answer)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();

        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (Text, QuestionId, IsCorrect) VALUES (@Text, @QuestionId, @IsCorrect)";
        dbcmd.Parameters.Add(new SqliteParameter("@Text", answer.text));
        dbcmd.Parameters.Add(new SqliteParameter("@QuestionId", answer.questionId));
        dbcmd.Parameters.Add(new SqliteParameter("@IsCorrect", answer.isCorrect));
        dbcmd.ExecuteNonQuery();
    }
}
