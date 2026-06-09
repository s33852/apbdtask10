using StudentPanel.Models;

namespace StudentPanel.Services;

public class ObservedStudentsState
{
    private readonly List<StudentDto> _observed = new();

    public IReadOnlyList<StudentDto> Observed => _observed.AsReadOnly();

    public int Count => _observed.Count;

    public event Action? OnChange;

    public bool IsObserved(int studentId) => _observed.Any(s => s.Id == studentId);

    public void Add(StudentDto student)
    {
        if (!IsObserved(student.Id))
        {
            _observed.Add(student);
            OnChange?.Invoke();
        }
    }

    public void Remove(int studentId)
    {
        var item = _observed.FirstOrDefault(s => s.Id == studentId);
        if (item is not null)
        {
            _observed.Remove(item);
            OnChange?.Invoke();
        }
    }

    public void Toggle(StudentDto student)
    {
        if (IsObserved(student.Id)) Remove(student.Id);
        else Add(student);
    }
}
