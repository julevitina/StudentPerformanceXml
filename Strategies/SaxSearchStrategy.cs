using System.Xml;
using StudentPerformanceXml.Models;

namespace StudentPerformanceXml.Strategies;

public class SaxSearchStrategy : IXmlSearchStrategy
{
    public List<Student> Search(
        string filePath,
        SearchCriteria criteria)
    {
        List<Student> students = new List<Student>();

        using XmlReader reader = XmlReader.Create(filePath);

        Student? currentStudent = null;

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element &&
                reader.Name == "Student")
            {
                currentStudent = new Student
                {
                    Id = int.Parse(reader.GetAttribute("Id") ?? "0"),
                    FullName = reader.GetAttribute("FullName") ?? string.Empty,
                    Faculty = reader.GetAttribute("Faculty") ?? string.Empty,
                    Department = reader.GetAttribute("Department") ?? string.Empty,
                    Course = int.Parse(reader.GetAttribute("Course") ?? "0")
                };
            }

            if (reader.NodeType == XmlNodeType.Element &&
                reader.Name == "Subject" &&
                currentStudent != null)
            {
                Subject subject = new Subject
                {
                    Name = reader.GetAttribute("Name") ?? string.Empty,
                    Grade = int.Parse(reader.GetAttribute("Grade") ?? "0")
                };

                currentStudent.Subjects.Add(subject);
            }

            if (reader.NodeType == XmlNodeType.EndElement &&
                reader.Name == "Student" &&
                currentStudent != null)
            {
                if (MatchesCriteria(currentStudent, criteria))
                {
                    students.Add(currentStudent);
                }

                currentStudent = null;
            }
        }

        return students;
    }

    private bool MatchesCriteria(
        Student student,
        SearchCriteria criteria)
    {
        if (!string.IsNullOrWhiteSpace(criteria.FullName) &&
            !student.FullName.Contains(
                criteria.FullName,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(criteria.Faculty) &&
            student.Faculty != criteria.Faculty)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(criteria.Department) &&
            student.Department != criteria.Department)
        {
            return false;
        }

        if (criteria.Course.HasValue &&
            student.Course != criteria.Course.Value)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(criteria.SubjectName) &&
            !student.Subjects.Any(subject =>
                subject.Name.Equals(
                    criteria.SubjectName,
                    StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        return true;
    }
}