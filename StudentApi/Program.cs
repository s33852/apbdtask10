using StudentApi.DTOs;
using StudentApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5100")
     .AllowAnyHeader()
     .AllowAnyMethod()));

var app = builder.Build();

app.UseCors();

var courses = new List<Course>
{
    new() { Id = 1, Name = "Mathematics", Ects = 6 },
    new() { Id = 2, Name = "Algorithms and Data Structures", Ects = 5 },
    new() { Id = 3, Name = "Databases", Ects = 4 },
    new() { Id = 4, Name = "Object-Oriented Programming", Ects = 5 },
    new() { Id = 5, Name = "Operating Systems", Ects = 4 },
};

var students = new List<Student>
{
    new() { Id = 1, IndexNumber = "s12345", FirstName = "Anna",  LastName = "Kowalska",   Email = "anna.kowalska@example.com",    Semester = 3 },
    new() { Id = 2, IndexNumber = "s12346", FirstName = "Piotr", LastName = "Nowak",      Email = "piotr.nowak@example.com",      Semester = 2 },
    new() { Id = 3, IndexNumber = "s12347", FirstName = "Marta", LastName = "Wisniawska", Email = "marta.wisniawska@example.com", Semester = 5 },
};

students[0].StudentCourses.Add(new StudentCourse { StudentId = 1, CourseId = 1, AssignedAt = DateTime.UtcNow.AddDays(-30), Course = courses[0] });
students[0].StudentCourses.Add(new StudentCourse { StudentId = 1, CourseId = 3, AssignedAt = DateTime.UtcNow.AddDays(-20), Course = courses[2] });
students[1].StudentCourses.Add(new StudentCourse { StudentId = 2, CourseId = 2, AssignedAt = DateTime.UtcNow.AddDays(-10), Course = courses[1] });

int nextStudentId = students.Max(s => s.Id) + 1;

app.MapGet("/api/students", () =>
    students.Select(s => new StudentDto(s.Id, s.IndexNumber, s.FirstName, s.LastName, s.Email, s.Semester)));

app.MapGet("/api/students/{id:int}", (int id) =>
{
    var s = students.FirstOrDefault(x => x.Id == id);
    return s is null ? Results.NotFound() : Results.Ok(new StudentDto(s.Id, s.IndexNumber, s.FirstName, s.LastName, s.Email, s.Semester));
});

app.MapPost("/api/students", (CreateStudentRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.IndexNumber) ||
        string.IsNullOrWhiteSpace(req.FirstName) ||
        string.IsNullOrWhiteSpace(req.LastName) ||
        string.IsNullOrWhiteSpace(req.Email) ||
        req.Semester < 1 || req.Semester > 8)
        return Results.BadRequest("Invalid student data.");

    var student = new Student
    {
        Id = nextStudentId++,
        IndexNumber = req.IndexNumber,
        FirstName = req.FirstName,
        LastName = req.LastName,
        Email = req.Email,
        Semester = req.Semester,
    };
    students.Add(student);
    var dto = new StudentDto(student.Id, student.IndexNumber, student.FirstName, student.LastName, student.Email, student.Semester);
    return Results.Created($"/api/students/{student.Id}", dto);
});

app.MapGet("/api/courses", () =>
    courses.Select(c => new CourseDto(c.Id, c.Name, c.Ects)));

app.MapGet("/api/students/{id:int}/courses", (int id) =>
{
    var s = students.FirstOrDefault(x => x.Id == id);
    if (s is null) return Results.NotFound();
    var result = s.StudentCourses.Select(sc => new StudentCourseDto(sc.StudentId, sc.CourseId, sc.Course.Name, sc.Course.Ects, sc.AssignedAt));
    return Results.Ok(result);
});

app.MapPost("/api/students/{id:int}/courses", (int id, AssignCourseRequest req) =>
{
    var student = students.FirstOrDefault(x => x.Id == id);
    if (student is null) return Results.NotFound("Student not found.");

    var course = courses.FirstOrDefault(c => c.Id == req.CourseId);
    if (course is null) return Results.BadRequest("Course not found.");

    if (student.StudentCourses.Any(sc => sc.CourseId == req.CourseId))
        return Results.Conflict("Course already assigned.");

    student.StudentCourses.Add(new StudentCourse { StudentId = id, CourseId = course.Id, AssignedAt = DateTime.UtcNow, Course = course });
    return Results.Ok(new StudentCourseDto(id, course.Id, course.Name, course.Ects, DateTime.UtcNow));
});

app.Run();
