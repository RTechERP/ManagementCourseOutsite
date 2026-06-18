namespace ManagementCourse.Models.DTO;

/// <summary>
/// DTO tổng hợp đánh giá sao của khoá học
/// </summary>
public class CourseRatingSummaryDTO
{
    public int TotalRatings { get; set; }
    public decimal AvgStars { get; set; }

    /// <summary>
    /// Số lượng mỗi mức sao [index 0 = 1 sao, ... index 4 = 5 sao]
    /// </summary>
    public int Star1 { get; set; }
    public int Star2 { get; set; }
    public int Star3 { get; set; }
    public int Star4 { get; set; }
    public int Star5 { get; set; }

    /// <summary>
    /// Số sao user hiện tại đã đánh giá (null nếu chưa đánh giá)
    /// </summary>
    public int? UserStars { get; set; }

    public string UserComment { get; set; }
}
