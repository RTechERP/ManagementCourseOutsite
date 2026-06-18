using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseLessonLikeRepository : GenericRepository<CourseLessonLike>
    {
        private readonly RTCCourseContext _context;

        public CourseLessonLikeRepository(RTCCourseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy record like của employee cho bài học (null nếu chưa like)
        /// </summary>
        public CourseLessonLike GetLike(int lessonId, int employeeId)
        {
            return _context.CourseLessonLikes
                .FirstOrDefault(x => x.LessonId == lessonId && x.EmployeeId == employeeId);
        }

        /// <summary>
        /// Đếm tổng lượt like của một bài học
        /// </summary>
        public int CountByLesson(int lessonId)
        {
            return _context.CourseLessonLikes.Count(x => x.LessonId == lessonId);
        }

        /// <summary>
        /// Đếm tổng lượt like của tất cả bài học trong khoá học
        /// </summary>
        public int CountByCourse(int courseId)
        {
            return _context.CourseLessonLikes
                .Join(_context.CourseLessons.Where(l => l.CourseId == courseId && l.IsDeleted != true),
                      like => like.LessonId,
                      lesson => lesson.Id,
                      (like, lesson) => like)
                .Count();
        }
    }
}
