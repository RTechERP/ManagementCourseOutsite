using ManagementCourse.Common;
using ManagementCourse.Models;
using System.Collections.Generic;
using System.Data;
using ManagementCourse.Models.DTO;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseRepository : GenericRepository<Course>
    {
        public List<CourseDTO> ListCourses( int courseCatalogID, string filterText, int employeeID, int catalogType,bool isAdmin, bool canLearnAhead)
        {
            //List<CourseDTO> list = new List<CourseDTO>();
            //DataSet dataSet = LoadDataFromSP.GetDataSetSP("spGetCourse",
            //                                                new string[] { "@DepartmentID", "@CourseCatalogID", "@FilterText", "@EmployeeID" },
            //                                                new object[] { departmentID, courseCatalogID, filterText, employeeID });
            //DataTable dt = dataSet.Tables[0];
            //if (dt.Rows.Count > 0)
            //{
            //    list = TextUtils.ConvertDataTable<CourseDTO>(dt);

            //}

            List<CourseDTO> listCourse = SQLHelper<CourseDTO>.ProcedureToList("spGetCourseNew",
                                                        new string[] { "@CourseCatalogID", "@FilterText", "@EmployeeID", "@CatalogType", "@IsShowAll" },
                                                        new object[] { courseCatalogID, filterText, employeeID, catalogType, 1});

            var orderedCourses = listCourse
    .OrderBy(c => c.CatalogType)
    .ThenByDescending(c => c.CatalogID)
    .ThenBy(c => c.ID)
    .ToList();


            // Admin mở khóa toàn bộ khóa học
            if (isAdmin || canLearnAhead)
            {
                foreach (var course in orderedCourses)
                {
                    course.Status = 1;
                }

                return orderedCourses;
            }

            var groups = orderedCourses
                .GroupBy(c => new { c.CatalogType, c.CatalogID })
                .ToList();

            foreach (var group in groups)
            {
                var coursesInCatalog = group.ToList();

                for (int i = 0; i < coursesInCatalog.Count; i++)
                {
                    var course = coursesInCatalog[i];

                    if (i == 0)
                    {
                        course.Status = 1; // Bài đầu tiên trong danh mục luôn mở
                        continue;
                    }

                    var prevCourse = coursesInCatalog[i - 1];

                    bool prevCompleted = prevCourse.NumberLesson >= prevCourse.TotalHistoryLession
                                         && prevCourse.Evaluate == 1;

                    course.Status = prevCompleted ? 1 : 0;
                }
            }

            return orderedCourses;
            //return listCourse;
        }
    }
}
