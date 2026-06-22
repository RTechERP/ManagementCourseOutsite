using ManagementCourse.Common;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq;

namespace ManagementCourse.Controllers
{
    public class CourseController : Controller
    {
        CourseRepository courseRepo;

        public CourseController(CourseRepository courseRepository)
        {
            courseRepo = courseRepository;
        }


        [HttpGet]
        public JsonResult GetListCourse(int id, string filterText, int departmentID, int catalogType)
        {
            //int departmentID = (int)HttpContext.Session.GetInt32("department_id");
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");


            int kpiPositionTypeID = 0;
            //DataTable kpiPositionType = LoadDataFromSP.GetDataTableSP("spGetKPIPositionTypeByEmployeeID",
            //            new string[] { "@EmployeeID" }, new object[] { employeeID });
            //if (kpiPositionType.Rows.Count > 0)
            //{
            //    kpiPositionTypeID = TextUtils.ToInt(kpiPositionType.Rows[0]["ID"]);
            //}

            var listCourse = courseRepo.ListCourses(id, filterText, employeeID, catalogType,false, false);

            return Json(listCourse, new System.Text.Json.JsonSerializerOptions());
        }

        [HttpGet]
        public JsonResult GetMyCourses(int skip = 0, int take = 10)
        {
            var employeeId = HttpContext.Session.GetInt32("employeeid");
            if (employeeId == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            // Lấy toàn bộ khóa học (catalog=0, filter="")
            var allCourses = courseRepo.ListCourses(0, "", employeeId.Value, 0, false, false);

            // Lọc khóa học đã bắt đầu học
            var participatingCourses = allCourses
                .Where(c => c.TotalHistoryLession > 0 || c.TotalTimeLearned > 0)
                .Select(c => new
                {
                    c.ID,
                    c.NameCourse,
                    c.TotalHistoryLession,
                    c.NumberLesson,
                    c.ThumbnailUrl,
                    progressPct = c.NumberLesson > 0 ? (int)System.Math.Round((double)c.TotalHistoryLession * 100 / c.NumberLesson) : 0,
                    IsCompleted = c.NumberLesson > 0 && c.TotalHistoryLession >= c.NumberLesson
                })
                .OrderBy(c => c.IsCompleted ? 1 : 0) // Chưa hoàn thành lên trước
                .ThenByDescending(c => c.ID) // Mới truy cập / ID lớn hơn ưu tiên
                .Skip(skip)
                .Take(take)
                .ToList();

            return Json(new { success = true, data = participatingCourses }, new System.Text.Json.JsonSerializerOptions());
        }

    }
}
