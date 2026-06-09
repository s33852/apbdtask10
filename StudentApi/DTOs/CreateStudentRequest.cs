namespace StudentApi.DTOs;

public record CreateStudentRequest(string IndexNumber, string FirstName, string LastName, string Email, int Semester);
