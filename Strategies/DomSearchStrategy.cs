using System.Xml;
using StudentPerformanceXml.Models;

namespace StudentPerformanceXml.Strategies;

public class DomSearchStrategy : IXmlSearchStrategy
{
    public List<Student> Search(
        string filePath,
        SearchCriteria criteria)
    {
        List<Student> students = new List<Student>();

        XmlDocument document = new XmlDocument();
        document.Load(filePath);

        XmlNodeList? studentNodes =
            document.SelectNodes("/Students/Student");

        if (studentNodes == null)
            return students;

        foreach (XmlNode studentNode in studentNodes)
        {
            Student student = new Student
            {
                Id = int.Parse(
                    studentNode.Attributes?["Id"]?.Value ?? "0"),

                FullName =
                    studentNode.Attributes?["FullName"]?.Value ?? "",

                Faculty =
                    studentNode.Attributes?["Faculty"]?.Value ?? "",

                Department =
                    studentNode.Attributes?["Department"]?.Value ?? "",

                Course = int.Parse(
                    studentNode.Attributes?["Course"]?.Value ?? "0")
            };

            XmlNodeList? subjectNodes =
                studentNode.SelectNodes("Subjects/Subject");

            if (subjectNodes != null)
            {
                foreach (XmlNode subjectNode in subjectNodes)
                {
                    student.Subjects.Add(new Subject
                    {
                        Name =
                            subjectNode.Attributes?["Name"]?.Value ?? "",

                        Grade = int.Parse(
                            subjectNode.Attributes?["Grade"]?.Value ?? "0")
                    });
                }
            }

            if (MatchesCriteria(student, criteria))
            {
                students.Add(student);
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
            return false;

        if (!string.IsNullOrWhiteSpace(criteria.Faculty) &&
            student.Faculty != criteria.Faculty)
            return false;

        if (!string.IsNullOrWhiteSpace(criteria.Department) &&
            student.Department != criteria.Department)
            return false;

        if (criteria.Course.HasValue &&
            student.Course != criteria.Course.Value)
            return false;

        if (!string.IsNullOrWhiteSpace(criteria.SubjectName) &&
            !student.Subjects.Any(
                s => s.Name.Equals(
                    criteria.SubjectName,
                    StringComparison.OrdinalIgnoreCase)))
            return false;

        return true;
    }
}