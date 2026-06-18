using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using ManagementCourse.Models.DTO;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseRatingRepository : GenericRepository<CourseRating>
    {
        private readonly RTCCourseContext _context;

        public CourseRatingRepository(RTCCourseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy đánh giá của employee cho khoá học (null nếu chưa đánh giá)
        /// </summary>
        public CourseRating GetRating(int courseId, int employeeId)
        {
            return _context.CourseRatings
                .FirstOrDefault(x => x.CourseId == courseId && x.EmployeeId == employeeId);
        }

        /// <summary>
        /// Tổng hợp thống kê đánh giá của khoá học
        /// </summary>
        public CourseRatingSummaryDTO GetSummary(int courseId, int? employeeId = null)
        {
            var ratings = _context.CourseRatings.Where(x => x.CourseId == courseId).ToList();

            var summary = new CourseRatingSummaryDTO
            {
                TotalRatings = ratings.Count,
                AvgStars     = ratings.Any() ? (decimal)ratings.Average(r => r.Stars) : 0,
                Star1        = ratings.Count(r => r.Stars == 1),
                Star2        = ratings.Count(r => r.Stars == 2),
                Star3        = ratings.Count(r => r.Stars == 3),
                Star4        = ratings.Count(r => r.Stars == 4),
                Star5        = ratings.Count(r => r.Stars == 5),
            };

            if (employeeId.HasValue)
            {
                var userRating = ratings.FirstOrDefault(r => r.EmployeeId == employeeId.Value);
                if (userRating != null)
                {
                    summary.UserStars   = userRating.Stars;
                    summary.UserComment = userRating.Comment;
                }
            }

            return summary;
        }
    }
}
