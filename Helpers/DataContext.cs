namespace WebApi.Helpers;

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

public class DataContext
{
    private DbSettings _dbSettings;

    public DataContext(IOptions<DbSettings> dbSettings)
    {
        _dbSettings = dbSettings.Value;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(BuildConnectionString());
    }

    public async Task Init()
    {
        await _initDatabase();
        await _initTables();
    }

    private string BuildConnectionString()
    {
        var connectionString = $"Server={_dbSettings.Server}; Database={_dbSettings.Database}; User Id={_dbSettings.UserId}; Password={_dbSettings.Password};";
        if (_dbSettings.Dev.HasValue && _dbSettings.Dev.Value)
        {
            connectionString += " TrustServerCertificate=true; Encrypt=False";
        }
        return connectionString;
    }
    private async Task _initDatabase()
    {
        
        using var connection = new SqlConnection(BuildConnectionString());
        var sql = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{_dbSettings.Database}') CREATE DATABASE [{_dbSettings.Database}];";
        await connection.ExecuteAsync(sql);
    }

    private async Task _initTables()
    {
        // create tables if they don't exist
        using var connection = CreateConnection();
        await _initUsers();

        async Task _initUsers()
        {
            var sql = """
                IF OBJECT_ID('Users', 'U') IS NULL
                CREATE TABLE Users (
                    Id INT NOT NULL PRIMARY KEY IDENTITY,
                    Title NVARCHAR(MAX),
                    FirstName NVARCHAR(MAX),
                    LastName NVARCHAR(MAX),
                    Email NVARCHAR(MAX),
                    PhoneNumber NVARCHAR(MAX),
                    Role INT,
                    PasswordHash NVARCHAR(MAX)
                );
            """;
            await connection.ExecuteAsync(sql);
        }
    }
}