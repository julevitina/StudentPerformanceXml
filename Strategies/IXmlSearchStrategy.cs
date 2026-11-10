using StudentPerformanceXml.Models;

namespace StudentPerformanceXml.Strategies;

public interface IXmlSearchStrategy
{
    List<Student> Search(
        string filePath,
        SearchCriteria criteria);
}