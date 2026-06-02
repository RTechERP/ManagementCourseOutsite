
using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using ManagementCourse.Models.DTO;

namespace ManagementCourse.Controllers
{
    public class HomeController : Controller
    {

        CourseCatalogRepository _courseCatalogRepo;
        CourseRepository _courseRepo;
        LessonRepository _lessonRepo;
        UsersRepository _usersRepo;
        GenericRepository<CourseExam> _cousrseExam = new GenericRepository<CourseExam>();
        public readonly EmailHelper _emailHelper;

        public HomeController(CourseCatalogRepository courseCatalogRepository, CourseRepository courseRepository, LessonRepository lessonRepository, EmailHelper emailHelper, UsersRepository usersRepository)
        {
            _courseCatalogRepo = courseCatalogRepository;
            _courseRepo = courseRepository;
            _lessonRepo = lessonRepository;
            _emailHelper = emailHelper;
            _usersRepo = usersRepository;
        }

        public IActionResult Index(int courseCatalogID, int catalogType = 1)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            int userID = (int)HttpContext.Session.GetInt32("userid");
            User user = _usersRepo.GetByID(userID);
            bool isAdmin = user.IsAdmin ?? false;
            //int kpiPositionTypeID = 0;
            //DataTable kpiPositionType = LoadDataFromSP.GetDataTableSP("spGetKPIPositionTypeByEmployeeID",
            //            new string[] { "@EmployeeID" }, new object[] { employeeID });
            //if (kpiPositionType.Rows.Count > 0)
            //{
            //    kpiPositionTypeID = TextUtils.ToInt(kpiPositionType.Rows[0]["ID"]);
            //}


            ViewBag.CoureCatalogID = courseCatalogID;
            ViewBag.CatalogType = catalogType;

            //var listCourseChile = new List<CourseDTO>();
            //if (!(kpiPositionTypeID <= 0))
            //{
            //    listCourseChile = _courseRepo.ListCourses( 0, "", employeeID, catalogType);
            //}

            var listCourseParent = _courseRepo.ListCourses(courseCatalogID, "", employeeID, catalogType, isAdmin);
            if (listCourseParent.Count > 0)
            {
                ViewBag.TitleCourse = courseCatalogID == 0 ? "" : $"danh sách khoá học {listCourseParent.FirstOrDefault().NameCourseCatalog}";
            }

            ViewBag.ListExam = _cousrseExam.GetAll().ToList();
            //int leader = TextUtils.ToInt(HttpContext.Session.GetInt32("isleader"));
            //int isAdmin = TextUtils.ToInt(HttpContext.Session.GetInt32("isAdmin"));

            //for (int i = 0; i < listCourseParent.Count; i++)
            //{

            //    if (i == 0) continue;

            //    CourseDTO course = listCourseParent[i - 1];

            //    if (course.CatalogType != 2)
            //    {
            //        if ((course.NumberLesson == course.TotalHistoryLession && course.Evaluate == 1) || leader > 0 || isAdmin > 0 || employeeID == 55)
            //        {
            //            listCourseParent[i].Status = 1;
            //        }
            //        else
            //        {
            //            listCourseParent[i].Status = 0;
            //        }
            //    }
            //}


            //if (!listCourseChile.Any(x => x.Evaluate <= 0))
            //{
            //    return View(listCourseParent);
            //}

            //var childIds = new HashSet<int>(listCourseChile.Select(c => c.ID));

            //foreach (var parent in listCourseParent)
            //{
            //    if (parent.Evaluate == 1 || childIds.Contains(parent.ID) || leader > 0 || isAdmin > 0 || employeeID == 55)
            //    {
            //        parent.Status = 1; // Đạt hoặc nằm trong listCourseChild → có thể làm
            //    }
            //    else
            //    {
            //        parent.Status = 0; // Còn lại → không thể làm
            //    }
            //}

            return View(listCourseParent);

        }


        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập username và mật khẩu !";
                return View();
            }
            password = string.IsNullOrEmpty(password) ? "" : Common.MaHoaMD5.EncryptPassword(password);
            DataTable user = LoadDataFromSP.GetDataTableSP("spLogin",
                                            new string[] { "@LoginName", "@Password" }, new object[] { username, password });
            if (user.Rows.Count > 0)
            {
                // Kiem tra trang thai tai khoan
                int statusUser = TextUtils.ToInt(user.Rows[0]["StatusUser"]);

                if (statusUser == -1)
                {
                    ViewBag.Error = "Tài khoản của bạn đang chờ xét duyệt. Vui lòng liên hệ quản trị viên.";
                    return View();
                }
                else if (statusUser == 1)
                {
                    ViewBag.Error = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.";
                    return View();
                }

                //int isAdmin = Convert.ToInt32(TextUtils.ToInt(user.Rows[0]["IsAdmin"]));
                HttpContext.Session.SetInt32("userid", TextUtils.ToInt(user.Rows[0]["ID"]));
                HttpContext.Session.SetInt32("employeeid", TextUtils.ToInt(user.Rows[0]["EmployeeID"]));
                HttpContext.Session.SetString("loginname", TextUtils.ToString(user.Rows[0]["LoginName"]));
                HttpContext.Session.SetString("fullname", TextUtils.ToString(user.Rows[0]["FullName"]));
                //HttpContext.Session.SetInt32("isAdmin", TextUtils.ToInt(user.Rows[0]["IsAdmin"]));
                //HttpContext.Session.SetInt32("isAdminSale", TextUtils.ToInt(user.Rows[0]["IsAdminSale"]));
                //HttpContext.Session.SetInt32("department_id", TextUtils.ToInt(user.Rows[0]["DepartmentID"]));
                //HttpContext.Session.SetString("department", TextUtils.ToString(user.Rows[0]["DepartmentName"]));
                //HttpContext.Session.SetInt32("headOfDepartment", TextUtils.ToInt(user.Rows[0]["HeadofDepartment"]));
                //HttpContext.Session.SetInt32("role_id", TextUtils.ToInt(user.Rows[0]["RoleID"]));
                //HttpContext.Session.SetString("role", TextUtils.ToString(user.Rows[0]["RoleName"]));
                //HttpContext.Session.SetString("img", TextUtils.ToString(user.Rows[0]["AnhCBNV"]));
                //HttpContext.Session.SetInt32("userteamid", TextUtils.ToInt(user.Rows[0]["UserTeamID"]));
                //HttpContext.Session.SetInt32("isleader", TextUtils.ToInt(user.Rows[0]["IsLeader"]));
                //HttpContext.Session.SetInt32("UserGroupID", TextUtils.ToInt(user.Rows[0]["UserGroupID"]));
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu !";
            }

            return View(user);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(string fullName, string birthDate, int gender, string email,
            string Organization, string Position, string password, string confirmPassword,
            string ReferralSource, string OtherReferralSource)
        {
            // Lưu lại giá trị form để hiển thị lại khi có lỗi
            ViewData["FullName"] = fullName;
            ViewData["BirthDate"] = birthDate;
            ViewData["Gender"] = gender;
            ViewData["Email"] = email;
            ViewData["Organization"] = Organization;
            ViewData["Position"] = Position;

            // Validate required fields
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(birthDate) ||
                gender == 0 || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(Organization) || string.IsNullOrEmpty(Position) ||
                string.IsNullOrEmpty(ReferralSource) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            // Validate gender (1 = Nam, 0 = Nu)
            if (gender != 1 && gender != 0)
            {
                ViewBag.Error = "Giới tính không hợp lệ!";
                return View();
            }

            // Validate email format
            if (!email.Contains("@") || !email.Contains("."))
            {
                ViewBag.Error = "Email không hợp lệ!";
                return View();
            }

            // Validate password match
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp!";
                return View();
            }

            // Validate password length
            if (password.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự!";
                return View();
            }

            // Xử lý nguồn giới thiệu - nếu chọn "Khác" thì lấy giá trị từ textbox
            string finalReferralSource = ReferralSource;
            if (ReferralSource == "Khác" && !string.IsNullOrEmpty(OtherReferralSource))
            {
                finalReferralSource = OtherReferralSource;
            }

            // Hash password
            string hashedPassword = Common.MaHoaMD5.EncryptPassword(password);

            // Đăng ký: Tạo User → Tạo Employee (trong 1 SP)
            try
            {
                DataTable result = LoadDataFromSP.GetDataTableSP("spRegister",
                    new string[] { "@FullName", "@BirthDate", "@Gender", "@Email", "@Organization", "@Position", "@ReferralSource", "@Password" },
                    new object[] { fullName, birthDate, gender, email, Organization, Position, finalReferralSource, hashedPassword });

                if (result.Rows.Count > 0 && Convert.ToInt32(result.Rows[0]["UserID"]) > 0)
                {
                    // Gửi email thông báo tài khoản mới chờ phê duyệt
                    await _emailHelper.SendAsync(fullName, email);

                    ViewBag.Success = "Đăng ký thành công! Vui lòng chờ xét duyệt tài khoản.";
                    return View();
                }
                else
                {
                    ViewBag.Error = result.Rows[0]["Message"].ToString();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }
        #endregion

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string email, string oldPassword, string newPassword, string confirmPassword)
        {
            ViewData["Email"] = email;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(oldPassword) ||
                string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới nhập lại không khớp!";
                return View();
            }

            if (newPassword.Length < 6)
            {
                ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return View();
            }

            try
            {
                string hashedOldPassword = Common.MaHoaMD5.EncryptPassword(oldPassword);
                string hashedNewPassword = Common.MaHoaMD5.EncryptPassword(newPassword);

                DataTable result = LoadDataFromSP.GetDataTableSP("spChangePassword",
                    new string[] { "@Email", "@OldPassword", "@NewPassword" },
                    new object[] { email, hashedOldPassword, hashedNewPassword });

                if (result.Rows.Count > 0)
                {
                    int resultCode = Convert.ToInt32(result.Rows[0]["ResultCode"]);
                    if (resultCode > 0)
                    {
                        TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.Error = result.Rows[0]["Message"].ToString();
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Không thể xử lý yêu cầu.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult CheckNullCourse(int id)
        {
            var lstLess = _lessonRepo.GetAllList().Where(c => c.CourseId == id);
            if (lstLess.Count() < 1)
                return Json(0, new System.Text.Json.JsonSerializerOptions());
            return Json(1, new System.Text.Json.JsonSerializerOptions());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(ErrorViewModel errorView)
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return View(errorView);
        }

        [HttpPost]
        public IActionResult LoginToCourse(string userName, string passwordHash, int registerIdeaTypeID, int courseID)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passwordHash))
            {
                return RedirectToAction("Login", "Home");
            }
            //string password = MaHoaMD5.DecryptPassword(login.PasswordHash);
            DataTable user = LoadDataFromSP.GetDataTableSP("spLogin",
                                            new string[] { "@LoginName", "@Password" }, new object[] { userName, passwordHash });
            if (user.Rows.Count > 0)
            {
                HttpContext.Session.SetInt32("userid", TextUtils.ToInt(user.Rows[0]["ID"]));
                HttpContext.Session.SetInt32("employeeid", TextUtils.ToInt(user.Rows[0]["EmployeeID"]));
                HttpContext.Session.SetString("loginname", TextUtils.ToString(user.Rows[0]["LoginName"]));
                HttpContext.Session.SetString("fullname", TextUtils.ToString(user.Rows[0]["FullName"]));
                HttpContext.Session.SetInt32("isAdmin", TextUtils.ToInt(user.Rows[0]["IsAdmin"]));
                HttpContext.Session.SetInt32("department_id", TextUtils.ToInt(user.Rows[0]["DepartmentID"]));
                HttpContext.Session.SetString("department", TextUtils.ToString(user.Rows[0]["DepartmentName"]));
                HttpContext.Session.SetInt32("isleader", TextUtils.ToInt(user.Rows[0]["IsLeader"]));
                HttpContext.Session.SetInt32("UserGroupID", TextUtils.ToInt(user.Rows[0]["UserGroupID"]));
                DataTable kpiPositionType = LoadDataFromSP.GetDataTableSP("spGetKPIPositionTypeByEmployeeID",
                                            new string[] { "@EmployeeID" }, new object[] { user.Rows[0]["ID"] });
                if (kpiPositionType.Rows.Count > 0)
                {
                    HttpContext.Session.SetInt32("KPIPositionTypeID", TextUtils.ToInt(kpiPositionType.Rows[0]["ID"]));
                }
                else
                {
                    HttpContext.Session.SetInt32("KPIPositionTypeID", TextUtils.ToInt(0));
                }

                if (courseID <= 0)
                {
                    CourseCatalog catalogModel = _courseCatalogRepo.GetByID(registerIdeaTypeID) ?? new CourseCatalog();
                    return Redirect($"Index?departmentID={catalogModel.DepartmentId}&courseCatalogID={catalogModel.Id}&catalogType={catalogModel.CatalogType}");
                }
                else
                {
                    return RedirectToAction("Index", "Lesson", new { courseId = courseID, catalogId = registerIdeaTypeID });
                }
            }
            return RedirectToAction("Login", "Home");
        }


    }
}
