using SomeTestTask.Core.Models;
using SomeTestTask.Core.Enums;

namespace SomeTestTask.Core.Services;

public class RandomEmployeesGenerator
{
    private readonly Random _random;

    private readonly List<string> _maleNames;
    private readonly List<string> _maleSurnames;
    private readonly List<string> _maleMiddleNames;

    private readonly List<string> _femaleNames;
    private readonly List<string> _femaleSurnames;
    private readonly List<string> _femaleMiddleNames;

    public RandomEmployeesGenerator()
    {
        _random = new Random();

        _maleNames = File.ReadAllLines("Resources/Male/male_names.txt").ToList();
        _maleSurnames = File.ReadAllLines("Resources/Male/male_surnames.txt").ToList();
        _maleMiddleNames = File.ReadAllLines("Resources/Male/male_middle_names.txt").ToList();

        _femaleNames = File.ReadAllLines("Resources/Female/female_names.txt").ToList();
        _femaleSurnames = File.ReadAllLines("Resources/Female/female_surnames.txt").ToList();
        _femaleMiddleNames = File.ReadAllLines("Resources/Female/female_middle_names.txt").ToList();
    }

    public List<Employee> GenerateRandomEmployees(int count)
    {
        var employees = new List<Employee>();
        for (int i = 0; i < count; i++)
        {
            employees.Add(GenerateRandomEmployee());
        }
        return employees;
    }

    private Employee GenerateRandomEmployee()
    {
        var gender = (Gender)_random.Next(0, 2);
        string fullName = string.Empty;
        switch (gender)
        {
            case Gender.Male:
                fullName = GenerateRandomMaleName();
                break;
            case Gender.Female:
                fullName = GenerateRandomFemaleName();
                break;
        }
        return new Employee()
        {
            FullName = fullName,
            DateOfBirth = GenerateRandomDateOfBirth(),
            Gender = gender
        };
    }

    private string GenerateRandomMaleName()
    {
        string surname = _maleSurnames[_random.Next(0, _maleSurnames.Count)];
        string name = _maleNames[_random.Next(0, _maleNames.Count)];
        string middleName = _maleMiddleNames[_random.Next(0, _maleMiddleNames.Count)];
        return $"{surname} {name} {middleName}";
    }

    private string GenerateRandomFemaleName()
    {
        string surname = _femaleSurnames[_random.Next(0, _femaleSurnames.Count)];
        string name = _femaleNames[_random.Next(0, _femaleNames.Count)];
        string middleName = _femaleMiddleNames[_random.Next(0, _femaleMiddleNames.Count)];
        return $"{surname} {name} {middleName}";
    }

    public DateOnly GenerateRandomDateOfBirth()
    {
        return new DateOnly(
            _random.Next(1950, 2006),
            _random.Next(1, 13),
            _random.Next(1, 29));
    }
}