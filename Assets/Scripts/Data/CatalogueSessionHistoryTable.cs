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

    public void AddCatalogueSessionHistory(int catalogueId, int timeSpent)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (CatalogueId, SessionDate, TimeSpent) VALUES(@CatalogueId, @SessionDate, @TimeSpent)";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", catalogueId));
        dbcmd.Parameters.Add(new SqliteParameter("@SessionDate", DateTime.Now));
        dbcmd.Parameters.Add(new SqliteParameter("@TimeSpent", timeSpent));
        dbcmd.ExecuteNonQuery();

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
            catalogueSessionHistories.Add(new CatalogueSessionHistory(id, catalogueId, sessionDate, timeSpent));
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
            catalogueSessionHistory = new CatalogueSessionHistory(id, catalogueId, sessionDate, timeSpent);
        }
        return catalogueSessionHistory;
    }
}
