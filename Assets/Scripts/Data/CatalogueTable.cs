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

    public CatalogueTable(IDbConnection dbConnection, QuestionTable questionTable, AnswerTable answerTable)
    {
        this.dbConnection = dbConnection;
        this.questionTable = questionTable;
        this.answerTable = answerTable;
    }

    public void AddCatalogue(Catalogue catalogue)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (Name) VALUES (@Name)";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", catalogue.name));
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
            List<Question> questions = FindQuestionsByCatalogueId(id);
            catalogues.Add(new Catalogue(id, name, questions));
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
            List<Question> questions = FindQuestionsByCatalogueId(id);
            catalogue = new Catalogue(id, name, questions);
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
            List<Question> questions = FindQuestionsByCatalogueId(catalogueId);
            catalogue = new Catalogue(id, name, questions);
        }
        return catalogue;
    }

    // HD TODO: also delete questions + answers for the selected catalogue
    public void DeleteCatalogueById(int catalogueId)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "DELETE FROM " + TABLE_NAME + " WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", catalogueId));
        dbcmd.ExecuteNonQuery();
    }

    // HD TODO: also delete answers for the selected question
    public void DeleteQuestionById(int questionId)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "DELETE FROM " + "Question" + " WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", questionId));
        dbcmd.ExecuteNonQuery();
    }

    public void UpdateCatalogueById(int catalogueId, string newName)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET Name = @Name WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", catalogueId));
        dbcmd.Parameters.Add(new SqliteParameter("@Name", newName));
        dbcmd.ExecuteNonQuery();
    }

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
            List<Answer> answers = FindAnswersByQuestionId(id);
            questions.Add(new Question(id, text, name, catalogueId, answers));
        }
        return questions;
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
            List<Question> questions = FindQuestionsByCatalogueId(id);
            catalogue = new Catalogue(id, name, questions);
        }
        return catalogue;
    }
}
