using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DailyTaskHistoryTable
{
    private const string TABLE_NAME = "DailyTaskHistory";
    private IDbConnection dbConnection;

    public DailyTaskHistoryTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public void AddDailyTaskHistory()
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        DateTime taskDate = DateTime.Now;
        // check id entry already exists for today
        dbcmd.CommandText = "SELECT COUNT(*) FROM " + TABLE_NAME + " WHERE TaskDate = @TaskDate";
        dbcmd.Parameters.Add(new SqliteParameter("@TaskDate", taskDate.ToString("yyyy-MM-dd")));
        long count = (long)dbcmd.ExecuteScalar();

        if (count == 0)
        {
            dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (TaskDate) VALUES (@TaskDate)";
            dbcmd.Parameters.Clear();
            dbcmd.Parameters.Add(new SqliteParameter("@TaskDate", taskDate.ToString("yyyy-MM-dd")));
            dbcmd.ExecuteNonQuery();
        }

        // check number of history entries
        dbcmd.CommandText = "SELECT COUNT(*) FROM " + TABLE_NAME;
        dbcmd.Parameters.Clear();
        long totalCount = (long)dbcmd.ExecuteScalar();

        if (totalCount > 35)
        {
            dbcmd.CommandText = "DELETE FROM " + TABLE_NAME + " WHERE Id IN (SELECT Id FROM " + TABLE_NAME + " ORDER BY TaskDate ASC LIMIT 1)";
            dbcmd.ExecuteNonQuery();
        }
    }

    public List<DailyTaskHistory> FindAllDailyTaskEntries()
    {
        List<DailyTaskHistory> dailyTaskEntries = new List<DailyTaskHistory>();

        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT * FROM DailyTaskHistory ORDER BY TaskDate DESC";
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            bool isCompleted = (bool)reader["TaskCompleted"];
            DateTime taskDate = (DateTime)reader["TaskDate"];
            int correctAnswersCount = Convert.ToInt32(reader["CorrectAnswersCount"]);

            dailyTaskEntries.Add(new DailyTaskHistory(id, taskDate, correctAnswersCount, isCompleted));
        }

        return dailyTaskEntries;
    }


    public void IncrementCorrectAnsweredCount()
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        DateTime taskDate = DateTime.Now;

        dbcmd.CommandText = "SELECT COUNT(*) FROM " + TABLE_NAME + " WHERE TaskDate = @TaskDate";
        dbcmd.Parameters.Add(new SqliteParameter("@TaskDate", taskDate.ToString("yyyy-MM-dd")));
        long count = (long)dbcmd.ExecuteScalar();

        if (count > 0)
        {
            dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET CorrectAnswersCount = CorrectAnswersCount + 1 WHERE TaskDate = @TaskDate";
            dbcmd.Parameters.Clear();
            dbcmd.Parameters.Add(new SqliteParameter("@TaskDate", taskDate.ToString("yyyy-MM-dd")));
            dbcmd.ExecuteNonQuery();
        }
    }

    public void SetTodaysTaskToCompleted()
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        DateTime taskDate = DateTime.Now;

        dbcmd.CommandText = "SELECT COUNT(*) FROM " + TABLE_NAME + " WHERE TaskDate = @TaskDate";
        dbcmd.Parameters.Add(new SqliteParameter("@TaskDate", taskDate.ToString("yyyy-MM-dd")));
        long count = (long)dbcmd.ExecuteScalar();

        if (count > 0)
        {
            dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET TaskCompleted = 1 WHERE TaskDate = @TaskDate";
            dbcmd.Parameters.Clear();
            dbcmd.Parameters.Add(new SqliteParameter("@TaskDate", taskDate.ToString("yyyy-MM-dd")));
            dbcmd.ExecuteNonQuery();
        }
    }
}
