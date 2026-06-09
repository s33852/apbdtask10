namespace StudentApi.Models;

public class Student
{
    public int Id { get; set; }
    public string IndexNumber { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public int Semester { get; set; }
    public List<StudentCourse> StudentCourses { get; set; } = new();
}
