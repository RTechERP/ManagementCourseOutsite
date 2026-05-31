using System;
using System.Collections.Generic;

namespace ManagementCourse.Models;

public partial class CourseExamPractice
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? EmployeeId { get; set; }

    public decimal? PracticePoints { get; set; }

    public bool? Evaluate { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string Note { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
