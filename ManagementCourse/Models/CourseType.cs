using System;
using System.Collections.Generic;

namespace ManagementCourse.Models;

public partial class CourseType
{
    public int Id { get; set; }

    public string CourseTypeCode { get; set; }

    public string CourseTypeName { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? Stt { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsLearnInTurn { get; set; }
}
