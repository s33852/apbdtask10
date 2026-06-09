namespace StudentPanel.Models;

public class StudentDto
{
    public int Id { get; set; }
    public string IndexNumber { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public int Semester { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
