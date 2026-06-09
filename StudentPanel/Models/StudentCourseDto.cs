namespace StudentPanel.Models;

public class StudentCourseDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public int Ects { get; set; }
    public DateTime AssignedAt { get; set; }
}
