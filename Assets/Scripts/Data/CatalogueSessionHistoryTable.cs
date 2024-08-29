using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;

public class CatalogueSessionHistoryTable
{
    private const string TABLE_NAME = "CatalogueSessionHistory";
    private IDbConnection dbConnection;

    public CatalogueSessionHistoryTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public int AddCatalogueSessionHistory(int catalogueId, int timeSpent, bool isCompleted)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (CatalogueId, SessionDate, TimeSpent, IsCompleted) VALUES(@CatalogueId, @SessionDate, @TimeSpent, @IsCompleted)";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", catalogueId));
        dbcmd.Parameters.Add(new SqliteParameter("@SessionDate", DateTime.Now));
        dbcmd.Parameters.Add(new SqliteParameter("@TimeSpent", timeSpent));
        dbcmd.Parameters.Add(new SqliteParameter("@IsCompleted", isCompleted));
        dbcmd.ExecuteNonQuery();

        dbcmd.CommandText = "SELECT last_insert_rowid()";
        int newSessionId = Convert.ToInt32(dbcmd.ExecuteScalar());

        // check number of history entries for catalgoue
        dbcmd.CommandText = "SELECT COUNT(*) FROM " + TABLE_NAME + " WHERE CatalogueId = @CatalogueId";
        dbcmd.Parameters.Clear();
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", catalogueId));
        long count = (long)dbcmd.ExecuteScalar();

        if (count > 5)
        {
            dbcmd.CommandText = "DELETE FROM " + TABLE_NAME + " WHERE Id IN (SELECT Id FROM " + TABLE_NAME + " WHERE CatalogueId = @CatalogueId ORDER BY SessionDate ASC LIMIT 1)";
            dbcmd.ExecuteNonQuery();
        }
        return newSessionId;
    }

    public void UpdateCatalogueSessionHistory(int sessionId, int timeSpent, bool isCompleted, bool sessionIsErrorFree)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET TimeSpent = @TimeSpent, IsCompleted = @IsCompleted, IsErrorFree = @IsErrorFree WHERE Id = @SessionId";
        dbcmd.Parameters.Add(new SqliteParameter("@TimeSpent", timeSpent));
        dbcmd.Parameters.Add(new SqliteParameter("@IsCompleted", isCompleted));
        dbcmd.Parameters.Add(new SqliteParameter("@SessionId", sessionId));
        dbcmd.Parameters.Add(new SqliteParameter("@IsErrorFree", sessionIsErrorFree));

        dbcmd.ExecuteNonQuery();
    }

    public CatalogueSessionHistory FindCatalogueSessionHistoryById(int sessionId)
    {
        CatalogueSessionHistory catalogueSessionHistory = null;
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE Id = @Id LIMIT 1";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", sessionId));

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            int catalogueId = Convert.ToInt32(reader["CatalogueId"]);
            int timeSpent = Convert.ToInt32(reader["TimeSpent"]);
            DateTime sessionDate = (DateTime)reader["SessionDate"];
            bool isCompleted = (bool)reader["IsCompleted"];
            bool isErrorFree = (bool)reader["IsErrorFree"];
            catalogueSessionHistory = new CatalogueSessionHistory(id, catalogueId, sessionDate, timeSpent, isCompleted, isErrorFree);
        }
        return catalogueSessionHistory;
    }

    public List<CatalogueSessionHistory> FindCatalogueSessionHistoryByCatalogueId(int catalogueId)
    {
        List<CatalogueSessionHistory> catalogueSessionHistories = new List<CatalogueSessionHistory>();
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE CatalogueId = @CatalogueId ORDER BY SessionDate DESC";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", catalogueId));

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            int timeSpent = Convert.ToInt32(reader["TimeSpent"]);
            DateTime sessionDate = (DateTime)reader["SessionDate"];
            bool isCompleted = (bool)reader["IsCompleted"];
            bool isErrorFree = (bool)reader["IsErrorFree"];
            catalogueSessionHistories.Add(new CatalogueSessionHistory(id, catalogueId, sessionDate, timeSpent, isCompleted, isErrorFree));
        }
        return catalogueSessionHistories;
    }

    public CatalogueSessionHistory FindLatestCatalogueSessionHistoryByCatalogueId(int catalogueId)
    {
        CatalogueSessionHistory catalogueSessionHistory = null;
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE CatalogueId = @CatalogueId ORDER BY SessionDate DESC LIMIT 1";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", catalogueId));

        IDataReader reader = dbcmd.ExecuteReader();
        if (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            int timeSpent = Convert.ToInt32(reader["TimeSpent"]);
            DateTime sessionDate = (DateTime)reader["SessionDate"];
            bool isCompleted = (bool)reader["IsCompleted"];
            bool isErrorFree = (bool)reader["IsErrorFree"];
            catalogueSessionHistory = new CatalogueSessionHistory(id, catalogueId, sessionDate, timeSpent, isCompleted, isErrorFree);
        }
        return catalogueSessionHistory;
    }

    public int FindTimeSpentInCompletedSessionsToday()
    {
        int timeSpent = 0;
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT SUM(TimeSpent) FROM " + TABLE_NAME + " WHERE DATE(SessionDate) = DATE('now')";

        object result = dbcmd.ExecuteScalar();
        if (result != DBNull.Value)
            timeSpent = Convert.ToInt32(result);
        
        return timeSpent;
    }
}
