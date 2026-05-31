using System;
using System.Collections.Generic;

namespace ManagementCourse.Models;

public partial class CourseCatalogType
{
    public int Id { get; set; }

    public int? Stt { get; set; }

    public string CourseCatalogTypeCode { get; set; }

    public string CourseCatalogTypeName { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
