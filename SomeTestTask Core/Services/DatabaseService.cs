using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using SomeTestTask.Core.Enums;
using SomeTestTask.Core.Models;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace SomeTestTask.Core.Services;

public class DatabaseService
{
    private readonly string ConnectionString = string.Empty;

    public DatabaseService()
    {
        string appSettings = File.ReadAllText("../../../../SomeTestTask Core/appsettings.json");
        var configuration = JsonSerializer.Deserialize<Configuration>(appSettings);
        if (configuration!.ConnectionString.IsNullOrEmpty())
            throw new Exception("Database service: Invalid configuration file");
        ConnectionString = configuration.ConnectionString!;
    }

    public void CreateStaffTable()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(string.Empty, connection))
            {
                string query = """
                               CREATE TABLE Staff
                               (
                                   full_name NVARCHAR(100),
                                   date_of_birth DATE,
                                   gender NVARCHAR(6),
                                   CHECK (gender = 'Male' OR gender = 'Female')
                               )
                               """;
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
    }

    public void InsertEmployeeToDatabase(Employee employee)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(string.Empty, connection))
            {
                string query = """
                               INSERT INTO Staff VALUES 
                               (
                                   @full_name,
                                   @date_of_birth,
                                   @gender
                               )
                               """;
                command.CommandText = query;
                command.Parameters.AddWithValue("@full_name", employee.FullName);
                command.Parameters.AddWithValue("@date_of_birth", employee.DateOfBirth);
                command.Parameters.AddWithValue("@gender", employee.Gender.ToString());
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Employee> GetAllEmployees()
    {
        var employees = new List<Employee>();
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(string.Empty, connection))
            {
                string query = """
                               SELECT * FROM Staff
                               """;
                command.CommandText = query;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string? fullName = reader["full_name"].ToString();
                        if (!DateTime.TryParse(reader["date_of_birth"].ToString(), null, DateTimeStyles.None, out DateTime dateOfBirth))
                            throw new Exception("Invalid date of birth");
                        var gender = reader["gender"].ToString() switch
                        {
                            "Male" => Gender.Male,
                            "Female" => Gender.Female,
                            _ => throw new Exception("Invalid gender")
                        };
                        employees.Add(new Employee()
                        {
                            FullName = fullName,
                            DateOfBirth = DateOnly.FromDateTime(dateOfBirth),
                            Gender = gender
                        });
                    }
                }
            }
        }
        return employees;
    }

    public void BulkInsertEmployees(List<Employee> employees)
    {
        var table = new DataTable();
        table.Columns.Add("full_name", typeof(string));
        table.Columns.Add("date_of_birth", typeof(DateOnly));
        table.Columns.Add("gender", typeof(string));

        foreach (var emp in employees)
        {
            table.Rows.Add(emp.FullName, emp.DateOfBirth, emp.Gender.ToString());
        }

        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = "Staff";
                    bulkCopy.BatchSize = 5000;

                    bulkCopy.ColumnMappings.Add("full_name", "full_name");
                    bulkCopy.ColumnMappings.Add("date_of_birth", "date_of_birth");
                    bulkCopy.ColumnMappings.Add("gender", "gender");

                    bulkCopy.WriteToServer(table);
                }
                transaction.Commit();
            }
        }
    }

    public TimeSpan GetMillisecondsOfSelectEmployeesOrderedByFirstFullNameSymbolAndGender(char symbol, Gender gender)
    {
        var stopwatch = new Stopwatch();
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(string.Empty, connection))
            {
                string query = """
                               SELECT * FROM Staff
                               WHERE LEFT(full_name, 1) = @symbol
                               AND gender = @gender
                               """;
                command.CommandText = query;
                command.Parameters.AddWithValue("@symbol", symbol);
                command.Parameters.AddWithValue("@gender", gender.ToString());
                stopwatch.Start();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // ������...
                }
                stopwatch.Stop();
            }
        }
        return stopwatch.Elapsed;
    }

    public async Task<TimeSpan> GetMillisecondsOfSelectEmployeesOrderedByFirstFullNameSymbolAndGenderAsync(char symbol, Gender gender)
    {
        var stopwatch = new Stopwatch();
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(string.Empty, connection))
            {
                string query = """
                               SELECT * FROM Staff
                               WHERE LEFT(full_name, 1) = @symbol
                               AND gender = @gender
                               """;
                command.CommandText = query;
                command.Parameters.AddWithValue("@symbol", symbol);
                command.Parameters.AddWithValue("@gender", gender.ToString());
                stopwatch.Start();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    // ������...
                }
                stopwatch.Stop();
            }
        }
        return stopwatch.Elapsed;
    }
}