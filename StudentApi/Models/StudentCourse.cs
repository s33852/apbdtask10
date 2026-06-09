namespace StudentApi.Models;

public class StudentCourse
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime AssignedAt { get; set; }
    public Course Course { get; set; } = null!;
}
