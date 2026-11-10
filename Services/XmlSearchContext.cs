using StudentPerformanceXml.Models;
using StudentPerformanceXml.Strategies;

namespace StudentPerformanceXml.Services;

public class XmlSearchContext
{
    private IXmlSearchStrategy? _strategy;

    public void SetStrategy(IXmlSearchStrategy strategy)
    {
        _strategy = strategy;
    }

    public List<Student> Search(
        string filePath,
        SearchCriteria criteria)
    {
        if (_strategy == null)
        {
            throw new InvalidOperationException(
                "Search strategy is not selected.");
        }

        return _strategy.Search(filePath, criteria);
    }
}