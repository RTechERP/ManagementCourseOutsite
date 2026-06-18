using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseLessonCommentReactionRepository : GenericRepository<CourseLessonCommentReaction>
    {
        private readonly RTCCourseContext _context;

        public CourseLessonCommentReactionRepository(RTCCourseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Thêm hoặc cập nhật (Upsert) reaction của học viên cho bình luận.
        /// Trả về true nếu thêm mới/cập nhật, false nếu hủy reaction (xóa).
        /// </summary>
        public bool ToggleReaction(int commentId, int employeeId, string reactionType, out string finalReaction)
        {
            var existing = _context.CourseLessonCommentReactions
                .FirstOrDefault(r => r.CommentId == commentId && r.EmployeeId == employeeId);

            if (existing != null)
            {
                if (existing.ReactionType == reactionType)
                {
                    // Hủy reaction (nếu click lại chính icon cũ)
                    _context.CourseLessonCommentReactions.Remove(existing);
                    _context.SaveChanges();
                    finalReaction = null;
                    return false;
                }
                else
                {
                    // Thay đổi reaction (nếu click icon khác)
                    existing.ReactionType = reactionType;
                    existing.CreatedDate = DateTime.Now;
                    _context.SaveChanges();
                    finalReaction = reactionType;
                    return true;
                }
            }
            else
            {
                // Thêm mới reaction
                var reaction = new CourseLessonCommentReaction
                {
                    CommentId = commentId,
                    EmployeeId = employeeId,
                    ReactionType = reactionType,
                    CreatedDate = DateTime.Now
                };
                _context.CourseLessonCommentReactions.Add(reaction);
                _context.SaveChanges();
                finalReaction = reactionType;
                return true;
            }
        }

        /// <summary>
        /// Lấy thống kê số lượng từng loại cảm xúc cho bình luận.
        /// </summary>
        public List<ReactionSummaryDTO> GetReactionSummary(int commentId)
        {
            return _context.CourseLessonCommentReactions
                .Where(r => r.CommentId == commentId)
                .GroupBy(r => r.ReactionType)
                .Select(g => new ReactionSummaryDTO
                {
                    ReactionType = g.Key,
                    Count = g.Count()
                })
                .ToList();
        }

        /// <summary>
        /// Lấy chi tiết những người đã thả cảm xúc cho bình luận (phục vụ Tooltip).
        /// </summary>
        public List<ReactionDetailDTO> GetReactionDetails(int commentId)
        {
            var reactions = _context.CourseLessonCommentReactions
                .Where(r => r.CommentId == commentId)
                .ToList();

            var employeeIds = reactions.Select(r => r.EmployeeId).Distinct().ToList();
            var employees = _context.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .ToDictionary(e => e.Id);

            return reactions.Select(r =>
            {
                employees.TryGetValue(r.EmployeeId, out var emp);
                return new ReactionDetailDTO
                {
                    EmployeeId = r.EmployeeId,
                    FullName = emp?.FullName ?? "Người dùng không tồn tại",
                    ReactionType = r.ReactionType
                };
            }).ToList();
        }
    }

    public class ReactionSummaryDTO
    {
        public string ReactionType { get; set; }
        public int Count { get; set; }
    }

    public class ReactionDetailDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string ReactionType { get; set; }
    }
}
