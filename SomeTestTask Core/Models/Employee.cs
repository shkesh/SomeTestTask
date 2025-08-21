using SomeTestTask.Core.Enums;

namespace SomeTestTask.Core.Models;

public class Employee
{
    public string? FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Gender? Gender { get; set; }

    public int GetAge() => DateTime.Now.Year - DateOfBirth.Year;
}