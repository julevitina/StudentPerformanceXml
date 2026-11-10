using System.Xml.Linq;
using StudentPerformanceXml.Models;
using StudentPerformanceXml.Services;
using StudentPerformanceXml.Strategies;

namespace StudentPerformanceXml;

public partial class MainPage : ContentPage
{
    private readonly XmlSearchContext _searchContext;

    private string? _filePath;

    public MainPage()
    {
        InitializeComponent();

        _searchContext = new XmlSearchContext();

        strategyPicker.ItemsSource = new List<string>
        {
            "DOM",
            "SAX",
            "LINQ to XML"
        };

        strategyPicker.SelectedIndex = 0;
    }

    private async void OnOpenFileClicked(
        object sender,
        EventArgs e)
    {
        FileResult? result =
            await FilePicker.Default.PickAsync();

        if (result == null)
        {
            return;
        }

        _filePath = result.FullPath;

        fileLabel.Text =
            $"Selected file: {result.FileName}";

        LoadSearchValues(_filePath);
    }

    private void LoadSearchValues(string filePath)
    {
        XDocument document =
            XDocument.Load(filePath);

        List<XElement> students =
            document.Descendants("Student").ToList();

        facultyPicker.ItemsSource = students
            .Select(student =>
                (string?)student.Attribute("Faculty"))
            .Where(value =>
                !string.IsNullOrWhiteSpace(value))
            .Distinct()
            .OrderBy(value => value)
            .ToList();

        departmentPicker.ItemsSource = students
            .Select(student =>
                (string?)student.Attribute("Department"))
            .Where(value =>
                !string.IsNullOrWhiteSpace(value))
            .Distinct()
            .OrderBy(value => value)
            .ToList();

        coursePicker.ItemsSource = students
            .Select(student =>
                (int?)student.Attribute("Course"))
            .Where(value =>
                value.HasValue)
            .Select(value =>
                value!.Value)
            .Distinct()
            .OrderBy(value => value)
            .ToList();

        subjectPicker.ItemsSource = document
            .Descendants("Subject")
            .Select(subject =>
                (string?)subject.Attribute("Name"))
            .Where(value =>
                !string.IsNullOrWhiteSpace(value))
            .Distinct()
            .OrderBy(value => value)
            .ToList();
    }

    private async void OnSearchClicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            await DisplayAlert(
                "Error",
                "Select an XML file first.",
                "OK");

            return;
        }

        SetSelectedStrategy();

        SearchCriteria criteria =
            new SearchCriteria
            {
                FullName =
                    string.IsNullOrWhiteSpace(fullNameEntry.Text)
                        ? null
                        : fullNameEntry.Text,

                Faculty =
                    facultyPicker.SelectedItem?.ToString(),

                Department =
                    departmentPicker.SelectedItem?.ToString(),

                Course =
                    coursePicker.SelectedItem is int course
                        ? course
                        : null,

                SubjectName =
                    subjectPicker.SelectedItem?.ToString()
            };

        List<Student> students =
            _searchContext.Search(
                _filePath,
                criteria);

        DisplayResults(students);
    }

    private void SetSelectedStrategy()
    {
        string? selectedStrategy =
            strategyPicker.SelectedItem?.ToString();

        switch (selectedStrategy)
        {
            case "DOM":
                _searchContext.SetStrategy(
                    new DomSearchStrategy());
                break;

            case "SAX":
                _searchContext.SetStrategy(
                    new SaxSearchStrategy());
                break;

            case "LINQ to XML":
                _searchContext.SetStrategy(
                    new LinqSearchStrategy());
                break;
        }
    }

    private void DisplayResults(
        List<Student> students)
    {
        if (students.Count == 0)
        {
            resultsEditor.Text =
                "No students found.";

            return;
        }

        List<string> resultLines =
            new List<string>();

        foreach (Student student in students)
        {
            string subjects =
                string.Join(
                    ", ",
                    student.Subjects.Select(
                        subject =>
                            $"{subject.Name}: {subject.Grade}"));

            resultLines.Add(
                $"ID: {student.Id}\n" +
                $"Name: {student.FullName}\n" +
                $"Faculty: {student.Faculty}\n" +
                $"Department: {student.Department}\n" +
                $"Course: {student.Course}\n" +
                $"Subjects: {subjects}\n");
        }

        resultsEditor.Text =
            string.Join(
                "\n--------------------\n",
                resultLines);
    }

    private void OnClearClicked(
        object sender,
        EventArgs e)
    {
        fullNameEntry.Text = string.Empty;

        facultyPicker.SelectedIndex = -1;
        departmentPicker.SelectedIndex = -1;
        coursePicker.SelectedIndex = -1;
        subjectPicker.SelectedIndex = -1;

        resultsEditor.Text = string.Empty;
    }
}