using System.Xml.Linq;
using StudentPerformanceXml.Models;

namespace StudentPerformanceXml.Strategies;

public class LinqSearchStrategy : IXmlSearchStrategy
{
    public List<Student> Search(
        string filePath,
        SearchCriteria criteria)
    {
        XDocument document = XDocument.Load(filePath);

        List<Student> students = document
            .Descendants("Student")
            .Select(studentElement => new Student
            {
                Id = (int?)studentElement.Attribute("Id") ?? 0,

                FullName =
                    (string?)studentElement.Attribute("FullName") ?? string.Empty,

                Faculty =
                    (string?)studentElement.Attribute("Faculty") ?? string.Empty,

                Department =
                    (string?)studentElement.Attribute("Department") ?? string.Empty,

                Course =
                    (int?)studentElement.Attribute("Course") ?? 0,

                Subjects = studentElement
                    .Element("Subjects")?
                    .Elements("Subject")
                    .Select(subjectElement => new Subject
                    {
                        Name =
                            (string?)subjectElement.Attribute("Name")
                            ?? string.Empty,

                        Grade =
                            (int?)subjectElement.Attribute("Grade")
                            ?? 0
                    })
                    .ToList()
                    ?? new List<Subject>()
            })
            .Where(student => MatchesCriteria(student, criteria))
            .ToList();

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