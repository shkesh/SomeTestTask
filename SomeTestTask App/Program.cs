using System.Globalization;
using SomeTestTask.Core.Enums;
using SomeTestTask.Core.Models;
using SomeTestTask.Core.Services;

namespace SomeTestTask.App;

public class Program
{
    private static readonly DatabaseService DatabaseService = new();

    public static async Task Main(string[] args)
    {
        try
        {
            if (args.Length == 0 || args.Length > 4)
                throw new Exception("Invalid count of arguments");

            if (!int.TryParse(args[0], out int mode) && (mode < 1 || mode > 5))
                throw new Exception("Invalid first argument (Mode)");

            Console.WriteLine($"Selected mode: {mode}");
            switch (mode)
            {
                case 1:
                    ActionOnFirstArgument();
                    break;
                case 2:
                    ActionOnSecondArgument(args);
                    break;
                case 3:
                    ActionOnThirdArgument();
                    break;
                case 4:
                    ActionOnFourthArgument();
                    break;
                case 5:
                    ActionOnFifthArgument();
                    break;
                case 6:
                    await ActionOnSixthArgument();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static void ActionOnFirstArgument()
    {
        DatabaseService.CreateStaffTable();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success: Staff table created");
        Console.ResetColor();
    }

    private static void ActionOnSecondArgument(string[] args)
    {
        if (args.Length < 4)
            throw new Exception("Invalid count of arguments");

        string fullName = args[1];
        if (!DateTime.TryParse(args[2], null, DateTimeStyles.None, out DateTime dateOfBirth))
            throw new Exception("Invalid date of birth");
        var gender = args[3] switch
        {
            "Male" => Gender.Male,
            "Female" => Gender.Female,
            _ => throw new Exception("Invalid gender")
        };
        var employeeToInsert = new Employee()
        {
            FullName = fullName,
            DateOfBirth = DateOnly.FromDateTime(dateOfBirth),
            Gender = gender
        };
        DatabaseService.InsertEmployeeToDatabase(employeeToInsert);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success: Employee created");
        Console.ResetColor();
    }

    private static void ActionOnThirdArgument()
    {
        var listOfEmployees = DatabaseService.GetAllEmployees()
            .OrderBy(emp => emp.FullName)
            .GroupBy(p => new { p.FullName, p.DateOfBirth })
            .Select(g => g.First())
            .ToList();

        Console.WriteLine($"Number of employees: {listOfEmployees.Count}");
        foreach (var emp in listOfEmployees)
        {
            Console.WriteLine(
                $"Full name: {emp.FullName} | Date of birth: {emp.DateOfBirth} | Gender: {emp.Gender} | Age: {emp.GetAge()}");
        }
    }

    private static void ActionOnFourthArgument()
    {
        var generator = new RandomEmployeesGenerator();
        int recordCount = 1_000_000;
        var employees = generator.GenerateRandomEmployees(recordCount);
        DatabaseService.BulkInsertEmployees(employees);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Success: {recordCount.ToString()} employees generated and added to database");
        Console.ResetColor();
    }

    private static void ActionOnFifthArgument()
    {
        char symbolToOrder = 'F';
        Gender genderToOrder = Gender.Male;
        TimeSpan elapsed = DatabaseService.GetMillisecondsOfSelectEmployeesOrderedByFirstFullNameSymbolAndGender(symbolToOrder, genderToOrder);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Success: {elapsed.TotalMilliseconds} ms elapsed");
        Console.ResetColor();
    }

    private static async Task ActionOnSixthArgument()
    {
        char symbolToOrderWithAsync = 'F';
        Gender genderToOrderWithAsyncAsync = Gender.Male;
        TimeSpan elapsedWithAsync = await DatabaseService.GetMillisecondsOfSelectEmployeesOrderedByFirstFullNameSymbolAndGenderAsync(symbolToOrderWithAsync, genderToOrderWithAsyncAsync);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Success: {elapsedWithAsync.TotalMilliseconds} ms elapsed");
        Console.ResetColor();
    }
}


