using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;

public class SQLiteHelper
{
    private const string databaseName = "Quiz_Database";
    public string dbConnectionString;
    public IDbConnection dbConnection;

    public SQLiteHelper()
    {
        dbConnectionString = "URI=file:" + Application.persistentDataPath + "/" + databaseName;
        dbConnection = new SqliteConnection(dbConnectionString);
        dbConnection.Open();
    }

    ~SQLiteHelper()
    {
        dbConnection.Close();
    }

    public IDbCommand getDbCommand()
    {
        return dbConnection.CreateCommand();
    }

    public IDataReader getDataById(string id, string tableName)
    {
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText = "SELECT * FROM " + tableName + " WHERE id = @id";
        dbcmd.Parameters.Add(new SqliteParameter("@id", id));
        return dbcmd.ExecuteReader();
    }

    public IDataReader getAllData(string table_name)
    {
        IDbCommand dbcmd = getDbCommand();
        dbcmd.CommandText =
            "SELECT * FROM " + table_name;
        IDataReader reader = dbcmd.ExecuteReader();
        return reader;
    }

    public void close()
    {
        dbConnection.Close();
    }
}
