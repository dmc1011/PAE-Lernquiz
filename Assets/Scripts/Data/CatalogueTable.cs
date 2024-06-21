using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;

public class CatalogueTable
{
    private const string TABLE_NAME = "Catalogue";
    private IDbConnection dbConnection;

    public CatalogueTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public void InsertData(Catalogue catalogue)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (Name) VALUES (@Name)";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", catalogue.name));
        dbcmd.ExecuteNonQuery();
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
            List<Answer> answers = FindAnswersByQuestionId(id);
            questions.Add(new Question(id, text, catalogueId, answers));
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
}
