using System;
using System.Collections.Generic;

namespace ManagementCourse.Models;

public partial class CourseLessonHistory
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? Status { get; set; }

    public int? LessonId { get; set; }

    public DateTime? ViewDate { get; set; }

    public int? VideoDuration { get; set; }

    public int? LastWatchedSecond { get; set; }

    public int? MaxWatchedSecond { get; set; }

    public decimal? WatchedPercent { get; set; }
}
