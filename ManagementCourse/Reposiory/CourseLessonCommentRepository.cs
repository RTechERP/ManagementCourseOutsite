using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using ManagementCourse.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseLessonCommentRepository : GenericRepository<CourseLessonComment>
    {
        private readonly RTCCourseContext _context;

        public CourseLessonCommentRepository(RTCCourseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách comment gốc (không có parentId) của bài học, có phân trang
        /// </summary>
        public List<CourseLessonCommentDTO> GetRootComments(int lessonId, int employeeId,
            bool isAdmin, int pageIndex = 0, int pageSize = 10)
        {
            var now = DateTime.Now;

            var rootComments = _context.CourseLessonComments
                .Where(c => c.LessonId == lessonId && c.ParentId == null)
                .OrderByDescending(c => c.CreatedDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            int total = _context.CourseLessonComments
                .Count(c => c.LessonId == lessonId && c.ParentId == null);

            var commentIds = rootComments.Select(c => c.Id).ToList();

            var reactions = _context.CourseLessonCommentReactions
                .Where(r => commentIds.Contains(r.CommentId))
                .ToList();

            var reactorEmployeeIds = reactions.Select(r => r.EmployeeId).Distinct().ToList();
            var employeeIds = rootComments.Select(c => c.EmployeeId).Union(reactorEmployeeIds).Distinct().ToList();

            var employees = _context.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .ToDictionary(e => e.Id);

            return rootComments.Select(c =>
            {
                employees.TryGetValue(c.EmployeeId, out var emp);
                bool canEdit   = !c.IsDeleted && (c.EmployeeId == employeeId && (now - c.CreatedDate).TotalMinutes <= 15 || isAdmin);
                bool canDelete = !c.IsDeleted && (c.EmployeeId == employeeId || isAdmin);
                int replyCount = _context.CourseLessonComments.Count(r => r.ParentId == c.Id);

                var commentReactions = reactions.Where(r => r.CommentId == c.Id).ToList();

                var reactionSummary = commentReactions
                    .GroupBy(r => r.ReactionType)
                    .Select(g => new ReactionSummaryDTO
                    {
                        ReactionType = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                var reactionDetails = commentReactions.Select(r =>
                {
                    employees.TryGetValue(r.EmployeeId, out var reactorEmp);
                    return new ReactionDetailDTO
                    {
                        EmployeeId = r.EmployeeId,
                        FullName = reactorEmp?.FullName ?? "Người dùng không tồn tại",
                        ReactionType = r.ReactionType
                    };
                }).ToList();

                var userCurrentReaction = commentReactions
                    .FirstOrDefault(r => r.EmployeeId == employeeId)?.ReactionType;

                return new CourseLessonCommentDTO
                {
                    ID              = c.Id,
                    LessonID        = c.LessonId,
                    EmployeeID      = c.EmployeeId,
                    FullName        = emp?.FullName ?? "Người dùng không tồn tại",
                    ImagePath       = emp?.ImagePath,
                    ParentID        = c.ParentId,
                    Content         = c.IsDeleted ? "[Đã xóa]" : c.Content,
                    IsDeleted       = c.IsDeleted,
                    CreatedDate     = c.CreatedDate,
                    UpdatedDate     = c.UpdatedDate,
                    ReplyCount      = replyCount,
                    TotalRootComments = total,
                    ReplyToName     = null,  // comment gốc không có reply target
                    CanEdit         = canEdit,
                    CanDelete       = canDelete,
                    ImageOriginalName = c.ImageOriginalName,
                    ImageServerName = c.ImageServerName,
                    AttachmentOriginalName = c.AttachmentOriginalName,
                    AttachmentServerName = c.AttachmentServerName,
                    ReactionSummary = reactionSummary,
                    ReactionDetails = reactionDetails,
                    UserCurrentReaction = userCurrentReaction
                };
            }).ToList();
        }

        /// <summary>
        /// Lấy danh sách reply của một comment cha
        /// </summary>
        public List<CourseLessonCommentDTO> GetReplies(int parentId, int employeeId, bool isAdmin)
        {
            var now = DateTime.Now;

            var replies = _context.CourseLessonComments
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.CreatedDate)
                .ToList();

            var commentIds = replies.Select(c => c.Id).ToList();

            var reactions = _context.CourseLessonCommentReactions
                .Where(r => commentIds.Contains(r.CommentId))
                .ToList();

            var reactorEmployeeIds = reactions.Select(r => r.EmployeeId).Distinct().ToList();
            var employeeIds = replies.Select(c => c.EmployeeId).Union(reactorEmployeeIds).Distinct().ToList();

            var employees = _context.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .ToDictionary(e => e.Id);

            return replies.Select(c =>
            {
                employees.TryGetValue(c.EmployeeId, out var emp);
                bool canEdit   = !c.IsDeleted && (c.EmployeeId == employeeId && (now - c.CreatedDate).TotalMinutes <= 15 || isAdmin);
                bool canDelete = !c.IsDeleted && (c.EmployeeId == employeeId || isAdmin);
                // Đếm số cấp con 2 (sub-replies) để hiện nút "X trả lời" trên cấp con 1
                int replyCount = _context.CourseLessonComments.Count(r => r.ParentId == c.Id);

                var commentReactions = reactions.Where(r => r.CommentId == c.Id).ToList();

                var reactionSummary = commentReactions
                    .GroupBy(r => r.ReactionType)
                    .Select(g => new ReactionSummaryDTO
                    {
                        ReactionType = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                var reactionDetails = commentReactions.Select(r =>
                {
                    employees.TryGetValue(r.EmployeeId, out var reactorEmp);
                    return new ReactionDetailDTO
                    {
                        EmployeeId = r.EmployeeId,
                        FullName = reactorEmp?.FullName ?? "Người dùng không tồn tại",
                        ReactionType = r.ReactionType
                    };
                }).ToList();

                var userCurrentReaction = commentReactions
                    .FirstOrDefault(r => r.EmployeeId == employeeId)?.ReactionType;

                return new CourseLessonCommentDTO
                {
                    ID          = c.Id,
                    LessonID    = c.LessonId,
                    EmployeeID  = c.EmployeeId,
                    FullName    = emp?.FullName ?? "Người dùng không tồn tại",
                    ImagePath   = emp?.ImagePath,
                    ParentID    = c.ParentId,
                    Content     = c.IsDeleted ? "[Đã xóa]" : c.Content,
                    IsDeleted   = c.IsDeleted,
                    CreatedDate = c.CreatedDate,
                    UpdatedDate = c.UpdatedDate,
                    ReplyCount  = replyCount,
                    ReplyToName = c.ReplyToName,
                    CanEdit     = canEdit,
                    CanDelete   = canDelete,
                    ImageOriginalName = c.ImageOriginalName,
                    ImageServerName = c.ImageServerName,
                    AttachmentOriginalName = c.AttachmentOriginalName,
                    AttachmentServerName = c.AttachmentServerName,
                    ReactionSummary = reactionSummary,
                    ReactionDetails = reactionDetails,
                    UserCurrentReaction = userCurrentReaction
                };
            }).ToList();
        }

        /// <summary>
        /// Lấy chi tiết comment theo ID dưới dạng DTO để gửi SignalR
        /// </summary>
        public CourseLessonCommentDTO GetCommentDTO(int commentId, int employeeId, bool isAdmin)
        {
            var now = DateTime.Now;
            var c = _context.CourseLessonComments.FirstOrDefault(x => x.Id == commentId);
            if (c == null) return null;

            var emp = _context.Employees.FirstOrDefault(e => e.Id == c.EmployeeId);
            bool canEdit   = !c.IsDeleted && (c.EmployeeId == employeeId && (now - c.CreatedDate).TotalMinutes <= 15 || isAdmin);
            bool canDelete = !c.IsDeleted && (c.EmployeeId == employeeId || isAdmin);
            int replyCount = _context.CourseLessonComments.Count(r => r.ParentId == c.Id);

            var commentReactions = _context.CourseLessonCommentReactions
                .Where(r => r.CommentId == commentId)
                .ToList();

            var reactorEmployeeIds = commentReactions.Select(r => r.EmployeeId).Distinct().ToList();
            var reactors = _context.Employees
                .Where(e => reactorEmployeeIds.Contains(e.Id))
                .ToDictionary(e => e.Id);

            var reactionSummary = commentReactions
                .GroupBy(r => r.ReactionType)
                .Select(g => new ReactionSummaryDTO
                {
                    ReactionType = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var reactionDetails = commentReactions.Select(r =>
            {
                reactors.TryGetValue(r.EmployeeId, out var reactorEmp);
                return new ReactionDetailDTO
                {
                    EmployeeId = r.EmployeeId,
                    FullName = reactorEmp?.FullName ?? "Người dùng không tồn tại",
                    ReactionType = r.ReactionType
                };
            }).ToList();

            var userCurrentReaction = commentReactions
                .FirstOrDefault(r => r.EmployeeId == employeeId)?.ReactionType;

            return new CourseLessonCommentDTO
            {
                ID              = c.Id,
                LessonID        = c.LessonId,
                EmployeeID      = c.EmployeeId,
                FullName        = emp?.FullName ?? "Người dùng không tồn tại",
                ImagePath       = emp?.ImagePath,
                ParentID        = c.ParentId,
                Content         = c.IsDeleted ? "[Đã xóa]" : c.Content,
                IsDeleted       = c.IsDeleted,
                CreatedDate     = c.CreatedDate,
                UpdatedDate     = c.UpdatedDate,
                ReplyCount      = replyCount,
                ReplyToName     = c.ReplyToName,
                CanEdit         = canEdit,
                CanDelete       = canDelete,
                ImageOriginalName = c.ImageOriginalName,
                ImageServerName = c.ImageServerName,
                AttachmentOriginalName = c.AttachmentOriginalName,
                AttachmentServerName = c.AttachmentServerName,
                ReactionSummary = reactionSummary,
                ReactionDetails = reactionDetails,
                UserCurrentReaction = userCurrentReaction
            };
        }

        /// <summary>
        /// Lấy danh sách người tham gia khoá học từ CourseLessonHistory
        /// </summary>
        public List<CourseParticipantDTO> GetParticipants(int courseId, int pageIndex = 0, int pageSize = 20)
        {
            return SQLHelper<CourseParticipantDTO>.ProcedureToList(
                "spGetCourseParticipants",
                new string[] { "@CourseID", "@PageIndex", "@PageSize" },
                new object[] { courseId, pageIndex, pageSize });
        }
    }
}
