using System;

namespace ManagementCourse.Models.DTO
{
    public class CourseCatalogDTO
    {
        public int Id { get; set; }
        public int? Stt { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
        public bool? DeleteFlag { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CatalogType { get; set; }
        public string? CourseCatalogTypeName { get; set; }
        public string? CourseCatalogTypeCode { get; set; }
    }
}
