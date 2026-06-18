using System;
using System.Collections.Generic;

namespace ManagementCourse.Models.DTO;

/// <summary>
/// DTO cho một bình luận bài học, bao gồm replies
/// </summary>
public class CourseLessonCommentDTO
{
    public int ID { get; set; }
    public int LessonID { get; set; }
    public int EmployeeID { get; set; }
    public string FullName { get; set; }
    public string ImagePath { get; set; }
    public int? ParentID { get; set; }
    public string Content { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int ReplyCount { get; set; }
    public int TotalRootComments { get; set; }

    public string ReplyToName { get; set; }

    public string ImageOriginalName { get; set; }
    public string ImageServerName { get; set; }
    public string AttachmentOriginalName { get; set; }
    public string AttachmentServerName { get; set; }

    public List<ManagementCourse.Reposiory.ReactionSummaryDTO> ReactionSummary { get; set; } = new();
    public List<ManagementCourse.Reposiory.ReactionDetailDTO> ReactionDetails { get; set; } = new();
    public string UserCurrentReaction { get; set; }

    /// <summary>
    /// Có thể chỉnh sửa: chủ comment trong 15 phút đầu, hoặc admin
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// Có thể xóa: chủ comment hoặc admin
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Danh sách reply của comment này (load riêng khi mở rộng)
    /// </summary>
    public List<CourseLessonCommentDTO> Replies { get; set; } = new();

    /// <summary>
    /// Thời gian hiển thị dạng relative: "2 phút trước"
    /// </summary>
    public string TimeAgo => GetTimeAgo(CreatedDate);

    private static string GetTimeAgo(DateTime dt)
    {
        var diff = DateTime.Now - dt;
        if (diff.TotalSeconds < 60) return "vừa xong";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} phút trước";
        if (diff.TotalHours < 24)   return $"{(int)diff.TotalHours} giờ trước";
        if (diff.TotalDays < 30)    return $"{(int)diff.TotalDays} ngày trước";
        if (diff.TotalDays < 365)   return $"{(int)(diff.TotalDays / 30)} tháng trước";
        return $"{(int)(diff.TotalDays / 365)} năm trước";
    }
}
