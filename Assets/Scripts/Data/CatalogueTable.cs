using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;

public class CatalogueTable
{
    private const string TABLE_NAME = "Catalogue";
    private IDbConnection dbConnection;
    private QuestionTable questionTable;
    private AnswerTable answerTable;
    private AnswerHistoryTable answerHistoryTable;
    private CatalogueSessionHistoryTable catalogueSessionHistoryTable;

    public CatalogueTable(IDbConnection dbConnection, QuestionTable questionTable, AnswerTable answerTable, AnswerHistoryTable answerHistoryTable, CatalogueSessionHistoryTable catalogueSessionHistoryTable)
    {
        this.dbConnection = dbConnection;
        this.questionTable = questionTable;
        this.answerTable = answerTable;
        this.answerHistoryTable = answerHistoryTable;
        this.catalogueSessionHistoryTable = catalogueSessionHistoryTable;
    }

    public void AddCatalogue(Catalogue catalogue)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (Name, CurrentQuestionId) VALUES (@Name, @CurrentQuestionId)";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", catalogue.name));
        dbcmd.Parameters.Add(new SqliteParameter("@CurrentQuestionId", DBNull.Value));
        dbcmd.ExecuteNonQuery();
        dbcmd.CommandText = "SELECT last_insert_rowid()";
        int catalogueId = Convert.ToInt32(dbcmd.ExecuteScalar());

        foreach (var question in catalogue.questions)
        {
            question.catalogueId = catalogueId;
            questionTable.AddQuestion(question);
            dbcmd.CommandText = "SELECT last_insert_rowid()";
            int questionId = Convert.ToInt32(dbcmd.ExecuteScalar());
            foreach (var answer in question.answers)
            {
                answerTable.AddAnswer(new Answer(answer.id, answer.text, questionId, answer.isCorrect));
            }
        }
    }

    public void AddQuestion(int catalogueId, Question question)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        question.catalogueId = catalogueId;
        questionTable.AddQuestion(question);
        dbcmd.CommandText = "SELECT last_insert_rowid()";
        int questionId = Convert.ToInt32(dbcmd.ExecuteScalar());
        foreach (var answer in question.answers)
        {
            answerTable.AddAnswer(new Answer(answer.id, answer.text, questionId, answer.isCorrect));
        }
    }

    public List<Catalogue> FindAllCatalogues()
    {
        List<Catalogue> catalogues = new List<Catalogue>();
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM Catalogue order by Id";

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string name = reader["Name"].ToString();
            int currentQuestionIdIndex = reader.GetOrdinal("CurrentQuestionId");
            int currentQuestionId = reader.IsDBNull(currentQuestionIdIndex)
                                    ? -1
                                    : reader.GetInt32(currentQuestionIdIndex);
            int totalTimeSpent = Convert.ToInt32(reader["TotalTimeSpent"]);
            int sessionCount = Convert.ToInt32(reader["SessionCount"]);
            int errorFreeSessionCount = Convert.ToInt32(reader["ErrorFreeSessionCount"]);
            int completedRandomQuizCount = Convert.ToInt32(reader["CompletedRandomQuizCount"]);
            int errorFreeRandomQuizCount = Convert.ToInt32(reader["ErrorFreeRandomQuizCount"]);
            List<Question> questions = FindQuestionsByCatalogueId(id);
            List<CatalogueSessionHistory> sessionHistories = catalogueSessionHistoryTable.FindCatalogueSessionHistoryByCatalogueId(id);
            catalogues.Add(new Catalogue(id, name, currentQuestionId, totalTimeSpent, sessionCount, errorFreeSessionCount, completedRandomQuizCount, errorFreeRandomQuizCount, questions, sessionHistories));
        }
        return catalogues;
    }

    public Catalogue FindCatalogueByName(string catalogueName)
    {
        Catalogue catalogue = null;
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE Name = @Name";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", catalogueName));
        IDataReader reader = dbcmd.ExecuteReader();

        if (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string name = reader["Name"].ToString();
            int currentQuestionIdIndex = reader.GetOrdinal("CurrentQuestionId");
            int currentQuestionId = reader.IsDBNull(currentQuestionIdIndex)
                                    ? -1
                                    : reader.GetInt32(currentQuestionIdIndex);
            int totalTimeSpent = Convert.ToInt32(reader["TotalTimeSpent"]);
            int sessionCount = Convert.ToInt32(reader["SessionCount"]);
            int errorFreeSessionCount = Convert.ToInt32(reader["ErrorFreeSessionCount"]);
            int completedRandomQuizCount = Convert.ToInt32(reader["CompletedRandomQuizCount"]);
            int errorFreeRandomQuizCount = Convert.ToInt32(reader["ErrorFreeRandomQuizCount"]);
            List<Question> questions = FindQuestionsByCatalogueId(id);
            List<CatalogueSessionHistory> sessionHistories = catalogueSessionHistoryTable.FindCatalogueSessionHistoryByCatalogueId(id);
            catalogue = new Catalogue(id, name, currentQuestionId, totalTimeSpent, sessionCount, errorFreeSessionCount, completedRandomQuizCount, errorFreeRandomQuizCount, questions, sessionHistories);
        }
        return catalogue;
    }

    public Catalogue FindCatalogueById(int catalogueId)
    {
        Catalogue catalogue = null;
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", catalogueId));
        IDataReader reader = dbcmd.ExecuteReader();

        if (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string name = reader["Name"].ToString();
            int currentQuestionIdIndex = reader.GetOrdinal("CurrentQuestionId");
            int currentQuestionId = reader.IsDBNull(currentQuestionIdIndex)
                                    ? -1
                                    : reader.GetInt32(currentQuestionIdIndex);
            int totalTimeSpent = Convert.ToInt32(reader["TotalTimeSpent"]);
            int sessionCount = Convert.ToInt32(reader["SessionCount"]);
            int errorFreeSessionCount = Convert.ToInt32(reader["ErrorFreeSessionCount"]);
            int completedRandomQuizCount = Convert.ToInt32(reader["CompletedRandomQuizCount"]);
            int errorFreeRandomQuizCount = Convert.ToInt32(reader["ErrorFreeRandomQuizCount"]);
            List<Question> questions = FindQuestionsByCatalogueId(catalogueId);
            List<CatalogueSessionHistory> sessionHistories = catalogueSessionHistoryTable.FindCatalogueSessionHistoryByCatalogueId(id);
            catalogue = new Catalogue(id, name, currentQuestionId, totalTimeSpent, sessionCount, errorFreeSessionCount, completedRandomQuizCount, errorFreeRandomQuizCount, questions, sessionHistories);
        }
        return catalogue;
    }

    public void DeleteCatalogueById(int catalogueId)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "DELETE FROM " + TABLE_NAME + " WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", catalogueId));
        dbcmd.ExecuteNonQuery();
    }

    public void DeleteQuestionById(int questionId)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "DELETE FROM " + "Question" + " WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", questionId));
        dbcmd.ExecuteNonQuery();
    }

    public void UpdateCatalogue(Catalogue catalogue)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = @"
            UPDATE " + TABLE_NAME + @"
            SET Name = @Name,
                CurrentQuestionId = @CurrentQuestionId,
                TotalTimeSpent = @TotalTimeSpent,
                SessionCount = @SessionCount,
                ErrorFreeSessionCount = @ErrorFreeSessionCount,
                CompletedRandomQuizCount = @CompletedRandomQuizCount,
                ErrorFreeRandomQuizCount = @ErrorFreeRandomQuizCount
            WHERE Id = @Id;
        ";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", catalogue.id));
        dbcmd.Parameters.Add(new SqliteParameter("@Name", catalogue.name));
        dbcmd.Parameters.Add(new SqliteParameter("@CurrentQuestionId", catalogue.currentQuestionId == -1 ? DBNull.Value : catalogue.currentQuestionId));
        dbcmd.Parameters.Add(new SqliteParameter("@TotalTimeSpent", catalogue.totalTimeSpent));
        dbcmd.Parameters.Add(new SqliteParameter("@SessionCount", catalogue.sessionCount));
        dbcmd.Parameters.Add(new SqliteParameter("@ErrorFreeSessionCount", catalogue.errorFreeSessionCount));
        dbcmd.Parameters.Add(new SqliteParameter("@CompletedRandomQuizCount", catalogue.completedRandomQuizCount));
        dbcmd.Parameters.Add(new SqliteParameter("@ErrorFreeRandomQuizCount", catalogue.errorFreeRandomQuizCount));
        dbcmd.ExecuteNonQuery();
    }

    // HD TODO: integrate both Functions in updateQuestion in QuestionTable
    public void UpdateQuestionNameByID(int questionID, string newName)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + "Question" + " SET Name = @Name WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", questionID));
        dbcmd.Parameters.Add(new SqliteParameter("@Name", newName));
        dbcmd.ExecuteNonQuery();
    }

    public void UpdateQuestionTextByID(int questionID, string newText)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + "Question" + " SET Text = @Text WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", questionID));
        dbcmd.Parameters.Add(new SqliteParameter("@Text", newText));
        dbcmd.ExecuteNonQuery();
    }

    public void UpdateAnswerTextByID(int answerID, string newText)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + "Answer" + " SET Text = @Text WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", answerID));
        dbcmd.Parameters.Add(new SqliteParameter("@Text", newText));
        dbcmd.ExecuteNonQuery();
    }

    public List<Question> FindQuestionsByCatalogueId(int catalogueId)
    {
        List<Question> questions = new List<Question>();
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM Question WHERE CatalogueId = @CatalogueId order by Id";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", catalogueId));

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string text = reader["Text"].ToString();
            string name = reader["Name"].ToString();
            int correctAnsweredCount = Convert.ToInt32(reader["CorrectAnsweredCount"]);
            bool enabledForPractice = (bool)reader["EnabledForPractice"];
            int totalAnsweredCount = Convert.ToInt32(reader["TotalAnsweredCount"]);
            List<Answer> answers = FindAnswersByQuestionId(id);
            List<AnswerHistory> answerHistory = answerHistoryTable.FindAnswerHistoryByQuestionId(id);
            questions.Add(new Question(id, text, name, correctAnsweredCount, totalAnsweredCount, catalogueId, enabledForPractice, answers, answerHistory));
        }
        return questions;
    }

    // needs to be moved to QuestionTable, but was not possible for the moment without bigger changes in SQLiteSetup
    public Question FindQuestionById(int questionId)
    {
        Question question = null;
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM Question WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", questionId));

        IDataReader reader = dbcmd.ExecuteReader();
        if (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            int catalogueId = Convert.ToInt32(reader["CatalogueId"]);
            string text = reader["Text"].ToString();
            string name = reader["Name"].ToString();
            int correctAnsweredCount = Convert.ToInt32(reader["CorrectAnsweredCount"]);
            bool enabledForPractice = (bool)reader["EnabledForPractice"];
            int totalAnsweredCount = Convert.ToInt32(reader["TotalAnsweredCount"]);
            List<Answer> answers = FindAnswersByQuestionId(id);
            List<AnswerHistory> answerHistory = answerHistoryTable.FindAnswerHistoryByQuestionId(id);
            question = new Question(id, text, name, correctAnsweredCount, totalAnsweredCount, catalogueId, enabledForPractice, answers, answerHistory);
        }
        return question;
    }

    public List<Answer> FindAnswersByQuestionId(int questionId)
    {
        List<Answer> answers = new List<Answer>();
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM Answer WHERE QuestionId = @QuestionId order by Id";
        dbcmd.Parameters.Add(new SqliteParameter("@QuestionId", questionId));

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string text = reader["Text"].ToString();
            bool isCorrect = (bool)reader["IsCorrect"];
            answers.Add(new Answer(id, text, questionId, isCorrect));
        }
        return answers;
    }

    public Catalogue FindRandomCatalogue()
    {
        Catalogue catalogue = null;
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " ORDER BY RANDOM() LIMIT 1";
        IDataReader reader = dbcmd.ExecuteReader();
        if (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string name = reader["Name"].ToString();
            int currentQuestionIdIndex = reader.GetOrdinal("CurrentQuestionId");
            int currentQuestionId = reader.IsDBNull(currentQuestionIdIndex)
                                    ? -1
                                    : reader.GetInt32(currentQuestionIdIndex);
            int totalTimeSpent = Convert.ToInt32(reader["TotalTimeSpent"]);
            int sessionCount = Convert.ToInt32(reader["SessionCount"]);
            int errorFreeSessionCount = Convert.ToInt32(reader["ErrorFreeSessionCount"]);
            int completedRandomQuizCount = Convert.ToInt32(reader["CompletedRandomQuizCount"]);
            int errorFreeRandomQuizCount = Convert.ToInt32(reader["ErrorFreeRandomQuizCount"]);
            List<Question> questions = FindQuestionsByCatalogueId(id);
            List<CatalogueSessionHistory> sessionHistories = catalogueSessionHistoryTable.FindCatalogueSessionHistoryByCatalogueId(id);
            catalogue = new Catalogue(id, name, currentQuestionId, totalTimeSpent, sessionCount, errorFreeSessionCount, completedRandomQuizCount, errorFreeRandomQuizCount, questions, sessionHistories);
        }
        return catalogue;
    }

    public int FindCorrectAnsweredQuestionsCountByCatalogueId(int catalogueId)
    {
        int correctAnsweredQuestionsCount = 0;

        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = @"
            SELECT COUNT(*) AS CorrectAnsweredQuestionsCount
            FROM Question
            WHERE CatalogueId = @CatalogueId AND CorrectAnsweredCount > 0;
        ";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", catalogueId));

        object result = dbcmd.ExecuteScalar();
        if (result != null)
        {
            correctAnsweredQuestionsCount = Convert.ToInt32(result);
        }

        return correctAnsweredQuestionsCount;
    }
}
