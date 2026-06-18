using System;

namespace ManagementCourse.Models.DTO
{
    public class CourseNotificationDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? TriggerEmployeeId { get; set; }
        public string TriggerFullName { get; set; }
        public string TriggerAvatar { get; set; }
        public string NotificationType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string TargetUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted { get; set; }
        public int? CommentId { get; set; }
    }
}
