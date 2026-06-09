using StudentPanel.Models;

namespace StudentPanel.Services;

public class StudentsApiClient(HttpClient http)
{
    public async Task<List<StudentDto>> GetStudentsAsync() =>
        await http.GetFromJsonAsync<List<StudentDto>>("/api/students") ?? new List<StudentDto>();

    public Task<StudentDto?> GetStudentAsync(int id) =>
        http.GetFromJsonAsync<StudentDto?>($"/api/students/{id}");

    public async Task<StudentDto> CreateStudentAsync(CreateStudentRequest request)
    {
        var response = await http.PostAsJsonAsync("/api/students", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StudentDto>())!;
    }

    public async Task<List<StudentCourseDto>> GetStudentCoursesAsync(int studentId) =>
        await http.GetFromJsonAsync<List<StudentCourseDto>>($"/api/students/{studentId}/courses") ?? new List<StudentCourseDto>();

    public async Task<List<CourseDto>> GetCoursesAsync() =>
        await http.GetFromJsonAsync<List<CourseDto>>("/api/courses") ?? new List<CourseDto>();

    public async Task AssignCourseAsync(int studentId, AssignCourseRequest request)
    {
        var response = await http.PostAsJsonAsync($"/api/students/{studentId}/courses", request);
        response.EnsureSuccessStatusCode();
    }
}
