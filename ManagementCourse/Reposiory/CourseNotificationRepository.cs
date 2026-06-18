using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using ManagementCourse.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseNotificationRepository : GenericRepository<CourseNotification>
    {
        private readonly RTCCourseContext _context;

        public CourseNotificationRepository(RTCCourseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách 10 thông báo mới nhất kèm thông tin người kích hoạt
        /// </summary>
        public List<CourseNotificationDTO> GetLatestNotifications(int employeeId, int limit = 10)
        {
            var list = _context.CourseNotifications
                .Where(n => n.EmployeeId == employeeId)
                .OrderByDescending(n => n.CreatedDate)
                .Take(limit)
                .ToList();

            var triggerEmployeeIds = list
                .Where(n => n.TriggerEmployeeId.HasValue)
                .Select(n => n.TriggerEmployeeId.Value)
                .Distinct()
                .ToList();

            var employees = _context.Employees
                .Where(e => triggerEmployeeIds.Contains(e.Id))
                .ToDictionary(e => e.Id);

            return list.Select(n =>
            {
                Employee emp = null;
                if (n.TriggerEmployeeId.HasValue)
                {
                    employees.TryGetValue(n.TriggerEmployeeId.Value, out emp);
                }

                return new CourseNotificationDTO
                {
                    Id = n.Id,
                    EmployeeId = n.EmployeeId,
                    TriggerEmployeeId = n.TriggerEmployeeId,
                    TriggerFullName = emp?.FullName ?? "Hệ thống",
                    TriggerAvatar = emp?.ImagePath,
                    NotificationType = n.NotificationType,
                    Title = n.Title,
                    Content = n.Content,
                    TargetUrl = n.TargetUrl,
                    IsRead = n.IsRead,
                    CreatedDate = n.CreatedDate,
                    CreatedDateFormatted = FormatCreatedDate(n.CreatedDate),
                    CommentId = n.CommentId
                };
            }).ToList();
        }

        /// <summary>
        /// Đếm số lượng thông báo chưa đọc
        /// </summary>
        public int GetUnreadCount(int employeeId)
        {
            return _context.CourseNotifications
                .Count(n => n.EmployeeId == employeeId && !n.IsRead);
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo của người dùng là đã đọc
        /// </summary>
        public bool MarkAllAsRead(int employeeId)
        {
            var unread = _context.CourseNotifications
                .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                .ToList();

            if (unread.Any())
            {
                foreach (var item in unread)
                {
                    item.IsRead = true;
                }
                _context.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Đánh dấu một thông báo cụ thể là đã đọc
        /// </summary>
        public bool MarkAsRead(int id, int employeeId)
        {
            var notif = _context.CourseNotifications
                .FirstOrDefault(n => n.Id == id && n.EmployeeId == employeeId);

            if (notif != null && !notif.IsRead)
            {
                notif.IsRead = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        private string GetCommentHashChain(int commentId)
        {
            var ids = new List<int>();
            var current = _context.CourseLessonComments.Find(commentId);
            while (current != null)
            {
                ids.Insert(0, current.Id);
                if (current.ParentId.HasValue && current.ParentId.Value != 0)
                {
                    current = _context.CourseLessonComments.Find(current.ParentId.Value);
                }
                else
                {
                    break;
                }
            }
            return string.Join("-", ids);
        }

        /// <summary>
        /// Thêm thông báo mới khi có phản hồi bình luận (Không gộp, mỗi phản hồi là một thông báo riêng)
        /// </summary>
        public CourseNotification AddCommentReplyNotification(int receiverId, int triggerId, string triggerName, int parentId, int replyCommentId, int lessonId, int courseId)
        {
            var lesson = _context.CourseLessons.Find(lessonId);
            string lessonTitle = lesson?.LessonTitle ?? "";
            string safeTriggerName = System.Net.WebUtility.HtmlEncode(triggerName);
            string safeLessonTitle = System.Net.WebUtility.HtmlEncode(lessonTitle);
            string lessonSuffix = string.IsNullOrEmpty(lessonTitle) ? "" : $" trong bài học: <b>{safeLessonTitle}</b>";

            string newContent = $"{safeTriggerName} đã trả lời bình luận của bạn{lessonSuffix}.";
            string hashChain = GetCommentHashChain(replyCommentId);
            string targetUrl = $"/Lesson/Index?courseId={courseId}&lessonId={lessonId}#comment-{hashChain}";

            var newNotif = new CourseNotification
            {
                EmployeeId = receiverId,
                TriggerEmployeeId = triggerId,
                NotificationType = "CommentReply",
                Title = "Phản hồi bình luận",
                Content = newContent,
                TargetUrl = targetUrl,
                IsRead = false,
                CreatedDate = DateTime.Now,
                CommentId = parentId
            };
            _context.CourseNotifications.Add(newNotif);
            _context.SaveChanges();
            return newNotif;
        }

        /// <summary>
        /// Thêm hoặc gộp thông báo khi có lượt bày tỏ cảm xúc (Reaction)
        /// </summary>
        public CourseNotification AddOrGroupCommentReactionNotification(int receiverId, int triggerId, string triggerName, int commentId, int lessonId, int courseId)
        {
            // Tìm thông báo reaction CHƯA ĐỌC liên quan đến bình luận này
            var existing = _context.CourseNotifications
                .FirstOrDefault(n => n.EmployeeId == receiverId && n.CommentId == commentId && n.NotificationType == "CommentReaction" && !n.IsRead);

            // Tìm tất cả các tác giả thả cảm xúc trên bình luận này (trừ người nhận)
            var reactors = _context.CourseLessonCommentReactions
                .Where(r => r.CommentId == commentId && r.EmployeeId != receiverId)
                .Select(r => r.EmployeeId)
                .Distinct()
                .ToList();

            // Đảm bảo có người gửi hiện tại
            if (!reactors.Contains(triggerId))
            {
                reactors.Add(triggerId);
            }

            var lesson = _context.CourseLessons.Find(lessonId);
            string lessonTitle = lesson?.LessonTitle ?? "";
            string safeTriggerName = System.Net.WebUtility.HtmlEncode(triggerName);
            string safeLessonTitle = System.Net.WebUtility.HtmlEncode(lessonTitle);
            string lessonSuffix = string.IsNullOrEmpty(lessonTitle) ? "" : $" trong bài học: <b>{safeLessonTitle}</b>";

            string newContent = "";
            if (reactors.Count <= 1)
            {
                newContent = $"{safeTriggerName} đã bày tỏ cảm xúc về bình luận của bạn{lessonSuffix}.";
            }
            else if (reactors.Count == 2)
            {
                var otherId = reactors.First(id => id != triggerId);
                var otherEmp = _context.Employees.Find(otherId);
                string otherName = otherEmp?.FullName ?? "Học viên khác";
                string safeOtherName = System.Net.WebUtility.HtmlEncode(otherName);
                newContent = $"{safeTriggerName} và {safeOtherName} đã bày tỏ cảm xúc về bình luận của bạn{lessonSuffix}.";
            }
            else
            {
                newContent = $"{safeTriggerName} và {reactors.Count - 1} người khác đã bày tỏ cảm xúc về bình luận của bạn{lessonSuffix}.";
            }

            string hashChain = GetCommentHashChain(commentId);
            string targetUrl = $"/Lesson/Index?courseId={courseId}&lessonId={lessonId}#comment-{hashChain}";

            if (existing != null)
            {
                // Cập nhật thông báo cũ
                existing.Content = newContent;
                existing.TriggerEmployeeId = triggerId;
                existing.CreatedDate = DateTime.Now;
                existing.TargetUrl = targetUrl;
                _context.SaveChanges();
                return existing;
            }
            else
            {
                // Tạo thông báo mới
                var newNotif = new CourseNotification
                {
                    EmployeeId = receiverId,
                    TriggerEmployeeId = triggerId,
                    NotificationType = "CommentReaction",
                    Title = "Biểu cảm bình luận",
                    Content = newContent,
                    TargetUrl = targetUrl,
                    IsRead = false,
                    CreatedDate = DateTime.Now,
                    CommentId = commentId
                };
                _context.CourseNotifications.Add(newNotif);
                _context.SaveChanges();
                return newNotif;
            }
        }

        private string FormatCreatedDate(DateTime date)
        {
            var diff = DateTime.Now - date;
            if (diff.TotalMinutes < 1) return "Vừa xong";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} phút trước";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} giờ trước";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} ngày trước";
            return date.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
