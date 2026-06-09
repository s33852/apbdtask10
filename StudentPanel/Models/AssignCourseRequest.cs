using System.ComponentModel.DataAnnotations;

namespace StudentPanel.Models;

public class AssignCourseRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a course.")]
    public int CourseId { get; set; }
}
