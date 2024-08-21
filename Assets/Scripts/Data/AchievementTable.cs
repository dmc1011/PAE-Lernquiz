using Mono.Data.Sqlite;
using System;
using System.Data;

public class AchievementTable
{
    private const string TABLE_NAME = "Achievement";
    private IDbConnection dbConnection;

    public AchievementTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public void AddAchievement(Achievement achievement)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();

        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (Name, Description, IsAchieved) VALUES (@Name, @Description, @IsAchieved)";
        dbcmd.Parameters.Add(new SqliteParameter("@Name", achievement.name));
        dbcmd.Parameters.Add(new SqliteParameter("@Description", achievement.description));
        dbcmd.Parameters.Add(new SqliteParameter("@IsAchieved", achievement.isAchieved));
        dbcmd.ExecuteNonQuery();
    }

    public void UpdateAchievement(Achievement achievement)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "UPDATE " + TABLE_NAME + " SET IsAchieved = @IsAchieved, AchievedAt = @AchievedAt WHERE Id = @Id";
        dbcmd.Parameters.Add(new SqliteParameter("@Id", achievement.id));
        dbcmd.Parameters.Add(new SqliteParameter("@IsAchieved", achievement.isAchieved));
        dbcmd.Parameters.Add(new SqliteParameter("@AchievedAt", DateTime.Now));
        dbcmd.ExecuteNonQuery();
    }
}
