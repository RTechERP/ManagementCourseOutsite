using System;

namespace ManagementCourse.Models.DTO;

/// <summary>
/// DTO cho danh sách người tham gia khoá học
/// </summary>
public class CourseParticipantDTO
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; }
    public string ImagePath { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLesson { get; set; }
    public decimal ProgressPct { get; set; }
    public string StatusText { get; set; }
    public DateTime? LastActivity { get; set; }
    public int TotalParticipants { get; set; }
}
