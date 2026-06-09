namespace StudentApi.DTOs;

public record StudentCourseDto(int StudentId, int CourseId, string CourseName, int Ects, DateTime AssignedAt);
