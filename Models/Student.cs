namespace StudentPerformanceXml.Models;

public class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Faculty { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public int Course { get; set; }

    public List<Subject> Subjects { get; set; } = new();
}