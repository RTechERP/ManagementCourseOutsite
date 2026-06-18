using AspNetCoreHero.ToastNotification.Abstractions;
using ManagementCourse.Common;
using ManagementCourse.Hubs;
using ManagementCourse.Models;
using ManagementCourse.Models.DTO;
using ManagementCourse.Models.ViewModel;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;


namespace ManagementCourse.Controllers
{
    public class LessonController : Controller
    {
        LessonRepository _lessonRepo;
        CourseRepository _courseRepo;
        FileCourseRepository _fileCourseRepo;
        CourseLessonHistoryRepository _lessonHistoryRepo;
        GenericRepository<CourseExamResult> _cousrseExamResult = new GenericRepository<CourseExamResult>();
        GenericRepository<CourseExam> _cousrseExam = new GenericRepository<CourseExam>();
        private readonly IConfiguration _configuration;
        private readonly IHubContext<CommentHub> _hubContext;

        CourseExamRepository _examRepo = new CourseExamRepository(null);
        CourseLessonLikeRepository _likeRepo;
        CourseLessonCommentRepository _commentRepo;
        CourseLessonCommentReactionRepository _commentReactionRepo;
        CourseNotificationRepository _notificationRepo;
        ConfigSystemRepository _configSystemRepository;
        public INotyfService _notyfService { get; set; }
        private string _pathServer = "";
        public LessonController(LessonRepository lessonRepository, CourseRepository courseRepository,
            FileCourseRepository fileCourseRepository, CourseLessonHistoryRepository lessonHistoryRepo,
            INotyfService notyfService, IConfiguration configuration,
            CourseLessonLikeRepository likeRepo,
            CourseLessonCommentRepository commentRepo,
            CourseLessonCommentReactionRepository commentReactionRepo,
            CourseNotificationRepository notificationRepo,
            ConfigSystemRepository configSystemRepository,
        IHubContext<CommentHub> hubContext)
        {
            _lessonRepo = lessonRepository;
            _courseRepo = courseRepository;
            _fileCourseRepo = fileCourseRepository;
            _lessonHistoryRepo = lessonHistoryRepo;
            _notyfService = notyfService;
            _configuration = configuration;
            _likeRepo = likeRepo;
            _commentRepo = commentRepo;
            _commentReactionRepo = commentReactionRepo;
            _notificationRepo = notificationRepo;
            _hubContext = hubContext;
            _configSystemRepository = configSystemRepository;
            _pathServer = GetPathServer("CourseLessonComment");

        }

        public IActionResult Index(int courseId)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var currentUserName = HttpContext.Session.GetString("fullname");
            var isAdmin = HttpContext.Session.GetInt32("isadmin") == 1?"true":"false";
            var avatarLetter = !string.IsNullOrEmpty(currentUserName) ? char.ToUpper(currentUserName[0]) : 'U';
            ViewBag.CourseID = courseId;
            ViewBag.AvatarLetter = avatarLetter;
            ViewBag.IsAdmin = isAdmin;
            ViewBag.PathCommentImages = BuildUrl(_pathServer, "images");
            ViewBag.PathCommentFiles = BuildUrl(_pathServer, "files");
            return View();

            //try
            //{
            //    string[] nameFileShow;
            //    string[] idFileShow;
            //    int empoyeeId = (int)HttpContext.Session.GetInt32("employeeid");
            //    var nameCourse = _courseRepo.GetByID(courseId)?.NameCourse;
            //    CourseLessonViewModel courseLessonViewModel = new CourseLessonViewModel();
            //    if (lessionId == 0)
            //    {
            //        courseLessonViewModel = _lessonRepo.GetCourseLessonFromCourse(courseId, empoyeeId);
            //    }
            //    else
            //    {
            //        courseLessonViewModel = _lessonRepo.GetCourseLessonById(courseId, empoyeeId, lessionId);
            //    }

            //    if (courseLessonViewModel == null)
            //        return Redirect("~/Home/Error");

            //    var allLessons = _lessonRepo.ListCourseLessonFromCourse(courseId, empoyeeId);
            //    ViewBag.ListLesson = allLessons;
            //    ViewBag.NameCourse = nameCourse;

            //    ViewBag.LessonID = lessionId;

            //    //DataTable dt = LoadDataFromSP.GetDataTableSP("spGetNameFileLesson", new string[] { "@LessonID" }, new object[] { courseLessonViewModel.Id });
            //    //if (dt.Rows.Count > 0)
            //    //{
            //    //    string allNameFile = dt.Rows[0]["NameFile"].ToString();
            //    //    string allIdFile = dt.Rows[0]["ID"].ToString();
            //    //    if (allNameFile != "" && allIdFile != null)
            //    //    {
            //    //        nameFileShow = allNameFile.Split(',');
            //    //        idFileShow = allIdFile.Split(',');
            //    //        List<FileShow> fileShows = new List<FileShow>();

            //    //        for (int i = 0; i < nameFileShow.Length && i < idFileShow.Length; i++)
            //    //        {
            //    //            fileShows.Add(new FileShow { ID = idFileShow[i] + Path.GetExtension(nameFileShow[i]), NameFile = nameFileShow[i] });
            //    //        }

            //    //        ViewBag.FileCourse = fileShows;
            //    //    }
            //    //}

            //    if (_cousrseExam.GetAll().FirstOrDefault(p => p.CourseId == courseId) != null)
            //        ViewBag.CourseExamID = _cousrseExam.GetAll().FirstOrDefault(p => p.CourseId == courseId).Id;
            //    else ViewBag.CourseExamID = 0;

            //    ViewBag.EmployeeID = HttpContext.Session.GetInt32("employeeid");

            //    //Check đã học xong các bài học chưa
            //    DataTable dtLessonHistory = LoadDataFromSP.GetDataTableSP("spGetCourseLessonHistory", new string[] { "@CourseID", "@EmployeeID" }, new object[] {courseId,empoyeeId });
            //    var statusHistory = dtLessonHistory.Select("Status = 0");
            //    ViewBag.StatusHistory = dtLessonHistory.Rows.Count <= 0 ? "disabled" : (statusHistory.Length > 0 ? "disabled":"");


            //}
            //catch
            //{

            //    return Redirect("~/Home/Error");
            //}

        }
        [HttpPost]
        public IActionResult CheckHistoryLesson(int lessonId)
        {
            try
            {
                var empoyeeId = HttpContext.Session.GetInt32("employeeid").Value;
                if (lessonId == 0/* || empoyeeId == 0*/)
                {
                    return Json(-1);
                }
                CourseLessonHistory courseLessonHis = _lessonHistoryRepo.GetCourseLessonHistory(empoyeeId, lessonId);
                if (courseLessonHis == null)
                {
                    CourseLessonHistory lessonHistory = new CourseLessonHistory();
                    lessonHistory.ViewDate = DateTime.Now;
                    lessonHistory.EmployeeId = empoyeeId;
                    lessonHistory.LessonId = lessonId;
                    lessonHistory.Status = 1;
                    _lessonHistoryRepo.Create(lessonHistory);
                    return Json(1);  //bằng 1 là đã  xem
                }
                else
                {
                    courseLessonHis.ViewDate = DateTime.Now;
                    courseLessonHis.EmployeeId = empoyeeId;
                    courseLessonHis.LessonId = lessonId;
                    courseLessonHis.Status = courseLessonHis.Status == 1 ? 0 : 1;  //Đã xem
                    _lessonHistoryRepo.Update(courseLessonHis);
                    return Json(courseLessonHis.Status);  //bằng 1 là đã  xem, bằng 0 là chưa xem
                }
            }
            catch
            {
                return Json(-1);
            }


        }
        public class FileShow
        {
            public string ID { get; set; }
            public string NameFile { get; set; }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetBlobDownload(string file_name)
        //{
        //    try
        //    {
        //        //file_name = "6.pdf";
        //        var tempPath = "http://192.168.1.2:8083/api/Upload/Course/" + file_name;
        //        int idFile = TextUtils.ToInt(Path.GetFileNameWithoutExtension(file_name));
        //        CourseFile fileCourse = _fileCourseRepo.GetAllList().FirstOrDefault(c => c.Id == idFile);
        //        using (var httpClient = new HttpClient())
        //        {
        //            using (var response = await httpClient.GetAsync(tempPath))
        //            {
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var content = new StreamContent(await response.Content.ReadAsStreamAsync());
        //                    var contentType = "APPLICATION/octet-stream";
        //                    return File(await content.ReadAsByteArrayAsync(), contentType, fileCourse.NameFile);
        //                }
        //                else
        //                {
        //                    ErrorViewModel errorView = new ErrorViewModel()
        //                    {
        //                        Message = $"{response.StatusCode.ToString()} [{fileCourse.NameFile}]",
        //                    };
        //                    return RedirectToAction("Error", "Home", errorView);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorViewModel errorView = new ErrorViewModel()
        //        {
        //            Message = ex.Message
        //        };
        //        return RedirectToAction("Error", "Home", errorView);
        //    }

        //}
        [HttpGet]
        public async Task<IActionResult> GetBlobDownload(string file_name)
        {
            try
            {
                //file_name = "6.pdf";
                //var tempPath = "http://192.168.1.2:8083/api/Upload/Course/" + file_name;
                int idFile = TextUtils.ToInt(Path.GetFileNameWithoutExtension(file_name));
                CourseFile fileCourse = _fileCourseRepo.GetAllList().FirstOrDefault(c => c.Id == idFile);
                string urlPDF = BuildPdfUrl(fileCourse.ServerPath);
                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(baseUrl + urlPDF))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = new StreamContent(await response.Content.ReadAsStreamAsync());
                            var contentType = "APPLICATION/octet-stream";
                            return File(await content.ReadAsByteArrayAsync(), contentType, fileCourse.NameFile);
                        }
                        else
                        {
                            ErrorViewModel errorView = new ErrorViewModel()
                            {
                                Message = "Lỗi không tìm thấy file!",
                            };
                            //return RedirectToAction("Error", "Home", errorView);

                            HttpContext.Session.SetString("ErrorMessage", errorView.Message);
                            return Redirect(Request.Headers["Referer"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = "Lỗi không tìm thấy file!"
                };

                HttpContext.Session.SetString("ErrorMessage", errorView.Message);
                return Redirect(Request.Headers["Referer"].ToString());
            }

        }
        [HttpGet]
        public string BuildUrl(string? url, string? subPath)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;
            string host = $"/api/share/";

            string urlFile = url.Replace(@"\\192.168.1.190\", "");
            
            if (!string.IsNullOrWhiteSpace(subPath))
            {
                urlFile = Path.Combine(urlFile,subPath);
            }
            urlFile = urlFile.Replace("\\", "/");

            return host + urlFile;
        }
        [HttpGet]
        public string BuildPdfUrl(string? urlPDF)
        {
            if (string.IsNullOrWhiteSpace(urlPDF))
                return string.Empty;
            //var apiUrl = _configuration["APIUrl"];
            //string host = $"{apiUrl}api/share/";
            string host = $"/api/share/";

            string urlFile = urlPDF.Replace(@"\\192.168.1.190\", "");
            urlFile = urlFile.Replace("\\", "/");

            return host + urlFile;
        }
        public JsonResult CreateExamResult([FromBody] CourseExamResult examResult)
        {
            var employeeID = HttpContext.Session.GetInt32("employeeid");
            var fullname = HttpContext.Session.GetString("fullname");
            var model = _cousrseExamResult.GetAll().FirstOrDefault(p => p.CourseExamId == examResult.CourseExamId && p.EmployeeId == employeeID);
            JsonResult jsonResult;

            if (model != null)
            {
                model.UpdatedBy = fullname;
                model.UpdatedDate = DateTime.Now;
                var obj = _cousrseExamResult.Update(model);
                jsonResult = Json(obj, new System.Text.Json.JsonSerializerOptions());
            }
            else
            {
                CourseExamResult result = new CourseExamResult()
                {
                    CourseExamId = examResult.CourseExamId,
                    EmployeeId = employeeID,
                    TotalCorrect = 0,
                    TotalIncorrect = 0,
                    PercentageCorrect = 0,
                    CreatedBy = fullname,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = fullname,
                    UpdatedDate = DateTime.Now
                };
                var obj = _cousrseExamResult.Create(result);

                jsonResult = Json(obj, new System.Text.Json.JsonSerializerOptions());
            }

            return jsonResult;

        }
        public JsonResult GetExamResult(int courseID, int employeeID)
        {
            return Json(_cousrseExamResult.GetAll().FirstOrDefault(p => p.CourseExamId == courseID && p.EmployeeId == employeeID));
        }

        //Get danh sách bài học
        public JsonResult GetCourseLesson(int courseId)
        {
            //try
            //{
            //    int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            //    List<CourseLessonDTO> listLesson = SQLHelper<CourseLessonDTO>.ProcedureToList("spGetCourseLessonByCourseID",
            //                                                                                    new string[] { "@CourseID", "@EmployeeID" },
            //                                                                                    new object[] { courseId, employeeID });
            //    return Json(listLesson, new System.Text.Json.JsonSerializerOptions());
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            try
            {
                int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
                List<CourseLessonDTO> listLesson = SQLHelper<CourseLessonDTO>.ProcedureToList("spGetCourseLessonByCourseID",
                                                                                                new string[] { "@CourseID", "@EmployeeID" },
                                                                                                new object[] { courseId, employeeID });
                //========================================================= lee min khooi update 19/09/2024 ===========================================
                List<object> lstData = new List<object>();
                foreach (CourseLessonDTO item in listLesson)
                {
                    List<CourseExam> lstExam = _examRepo.GetAll().Where(p => p.LessonId == item.ID).ToList();

                    lstData.Add(new { lstLesson = item, lstExam = lstExam });
                }




                return Json(lstData, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get danh sách file đính kèm
        public JsonResult GetCourseFile(int lessonID)
        {
            try
            {
                List<CourseFileDTO> listFile = SQLHelper<CourseFileDTO>.SqlToList($"SELECT * FROM dbo.CourseFile WHERE LessonID = {lessonID} AND IsDeleted <> 1");
                if (listFile.Count > 0)
                {
                    foreach (var item in listFile)
                    {
                        item.FileName = item.ID + Path.GetExtension(item.NameFile);
                    }
                }

                return Json(listFile, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IActionResult GetCourseExam(int courseId)
        {
            try
            {
                var list = _cousrseExam.GetAll().Where(p => p.CourseId == courseId);
                return Json(list, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        #region Like bài học

        [HttpPost]
        public JsonResult ToggleLike(int lessonId)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                if (employeeId == null) return Json(new { success = false });

                var existing = _likeRepo.GetLike(lessonId, employeeId.Value);
                bool liked;
                if (existing != null)
                {
                    _likeRepo.Remove(existing);
                    liked = false;
                }
                else
                {
                    _likeRepo.Create(new Models.CourseLessonLike
                    {
                        LessonId    = lessonId,
                        EmployeeId  = employeeId.Value,
                        CreatedDate = DateTime.Now
                    });
                    liked = true;
                }

                int total = _likeRepo.CountByLesson(lessonId);
                return Json(new { success = true, liked, totalLikes = total });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetLikeStatus(int lessonId)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                bool liked = employeeId.HasValue && _likeRepo.GetLike(lessonId, employeeId.Value) != null;
                int  total = _likeRepo.CountByLesson(lessonId);
                return Json(new { liked, totalLikes = total });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Người tham gia khoá học

        [HttpGet]
        public JsonResult GetParticipants(int courseId, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var list = _commentRepo.GetParticipants(courseId, pageIndex, pageSize);
                return Json(new { success = true, data = list, total = list.FirstOrDefault()?.TotalParticipants ?? 0 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Bình luận bài học

        [HttpGet]
        public JsonResult GetComments(int lessonId, int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid") ?? 0;
                bool isAdmin   = HttpContext.Session.GetString("isadmin") == "true";
                var list = _commentRepo.GetRootComments(lessonId, employeeId, isAdmin, pageIndex, pageSize);
                int total = list.FirstOrDefault()?.TotalRootComments ?? 0;
                return Json(new { success = true, data = list, total });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetReplies(int parentId)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid") ?? 0;
                bool isAdmin   = HttpContext.Session.GetString("isadmin") == "true";
                var list = _commentRepo.GetReplies(parentId, employeeId, isAdmin);
                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public string GetPathServer(string keyName)
        {
            try
            {
                if (string.IsNullOrEmpty(keyName))
                {
                    return "";
                }
                var pathUpload = _configSystemRepository.GetUploadPathByKey(keyName);
                string path = pathUpload;
                return  path ;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        [HttpPost]
        public async Task<JsonResult> AddComment([FromForm] CommentRequest req)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                if (employeeId == null) return Json(new { success = false, message = "Chưa đăng nhập" });
                if (string.IsNullOrWhiteSpace(req.Content) && req.CommentImage == null && req.CommentFile == null)
                {
                    return Json(new { success = false, message = "Nội dung bình luận hoặc tệp đính kèm không được để trống" });
                }

                string imgOrig = null, imgServer = null;
                string attachOrig = null, attachServer = null;
                string pathUploadImage = Path.Combine(_pathServer, "images");
                string pathUploadFiles = Path.Combine(_pathServer, "files");
                if (string.IsNullOrWhiteSpace(_pathServer) )
                {
                    return Json(new { success = false, message = $"Không tìm thấy cấu hình đường dẫn" });
                }
                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(pathUploadImage))
                {
                    Directory.CreateDirectory(pathUploadImage);
                }
                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(pathUploadFiles))
                {
                    Directory.CreateDirectory(pathUploadFiles);
                }


                if (req.CommentImage != null && req.CommentImage.Length > 0)
                {
                    // Tạo tên file unique để tránh trùng lặp
                    var fileExtension = Path.GetExtension(req.CommentImage.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(req.CommentImage.FileName);
                     imgServer = $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                    var fullPath = Path.Combine(pathUploadImage, imgServer);



                    imgOrig = req.CommentImage.FileName;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await req.CommentImage.CopyToAsync(stream);
                    }
                }

                if (req.CommentFile != null && req.CommentFile.Length > 0)
                {
                    // Tạo tên file unique để tránh trùng lặp
                    var fileExtension = Path.GetExtension(req.CommentFile.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(req.CommentFile.FileName);
                    attachServer = $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                    var fullPath = Path.Combine(pathUploadFiles, attachServer);

                    attachOrig = req.CommentFile.FileName;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await req.CommentFile.CopyToAsync(stream);
                    }
                }

                var comment = new Models.CourseLessonComment
                {
                    LessonId    = req.LessonId,
                    EmployeeId  = employeeId.Value,
                    ParentId    = req.ParentId,
                    Content     = req.Content?.Trim() ?? "",
                    IsDeleted   = false,
                    CreatedDate = DateTime.Now,
                    ReplyToName = req.ParentId.HasValue ? req.ReplyToName?.Trim() : null,
                    ImageOriginalName = imgOrig,
                    ImageServerName = imgServer,
                    AttachmentOriginalName = attachOrig,
                    AttachmentServerName = attachServer
                };
                _commentRepo.Create(comment);

                // Tạo thông báo cho người nhận (nếu phản hồi bình luận của người khác)
                if (comment.ParentId.HasValue)
                {
                    var parentComment = _commentRepo.GetByID(comment.ParentId.Value);
                    if (parentComment != null && parentComment.EmployeeId != employeeId.Value)
                    {
                        var triggerName = HttpContext.Session.GetString("fullname") ?? "Học viên";
                        var lesson = _lessonRepo.GetByID(req.LessonId);
                        var courseId = lesson?.CourseId ?? 0;
                        var notification = _notificationRepo.AddCommentReplyNotification(
                            parentComment.EmployeeId,
                            employeeId.Value,
                            triggerName,
                            comment.ParentId.Value,
                            comment.Id,
                            req.LessonId,
                            courseId
                        );

                        // Broadcast SignalR tới người nhận
                        var unreadCount = _notificationRepo.GetUnreadCount(parentComment.EmployeeId);
                        await _hubContext.Clients.Group($"User_{parentComment.EmployeeId}").SendAsync("ReceiveNotification", new {
                            id = notification.Id,
                            content = notification.Content,
                            targetUrl = notification.TargetUrl,
                            unreadCount = unreadCount
                        });
                    }
                }

                // Broadcast SignalR
                var isAdmin = HttpContext.Session.GetString("isadmin") == "true";
                var dto = _commentRepo.GetCommentDTO(comment.Id, employeeId.Value, isAdmin);
                if (dto != null)
                {
                    await _hubContext.Clients.Group($"Lesson_{req.LessonId}").SendAsync("ReceiveNewComment", dto);
                }

                return Json(new { success = true, id = comment.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ToggleCommentReaction(int commentId, string reactionType)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                if (employeeId == null) return Json(new { success = false, message = "Chưa đăng nhập" });

                bool active = _commentReactionRepo.ToggleReaction(commentId, employeeId.Value, reactionType, out string finalReaction);

                var summary = _commentReactionRepo.GetReactionSummary(commentId);
                var details = _commentReactionRepo.GetReactionDetails(commentId);

                var comment = _commentRepo.GetByID(commentId);
                if (comment != null)
                {
                    await _hubContext.Clients.Group($"Lesson_{comment.LessonId}").SendAsync("ReceiveCommentReaction", new {
                        commentId = commentId,
                        summary = summary,
                        details = details
                    });

                    // Tạo thông báo nếu thả cảm xúc lên bình luận của người khác và hành động là thêm (active == true)
                    if (active && comment.EmployeeId != employeeId.Value)
                    {
                        var triggerName = HttpContext.Session.GetString("fullname") ?? "Học viên";
                        var lesson = _lessonRepo.GetByID(comment.LessonId);
                        var courseId = lesson?.CourseId ?? 0;
                        var notification = _notificationRepo.AddOrGroupCommentReactionNotification(
                            comment.EmployeeId,
                            employeeId.Value,
                            triggerName,
                            commentId,
                            comment.LessonId,
                            courseId
                        );

                        // Broadcast SignalR tới người nhận
                        var unreadCount = _notificationRepo.GetUnreadCount(comment.EmployeeId);
                        await _hubContext.Clients.Group($"User_{comment.EmployeeId}").SendAsync("ReceiveNotification", new {
                            id = notification.Id,
                            content = notification.Content,
                            targetUrl = notification.TargetUrl,
                            unreadCount = unreadCount
                        });
                    }
                }

                return Json(new { success = true, active = active, finalReaction = finalReaction, summary = summary, details = details });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult DownloadCommentAttachment(string serverName, string originalName)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "comments", serverName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Không tìm thấy tệp tin.");
                }
                var bytes = System.IO.File.ReadAllBytes(filePath);
                return File(bytes, "application/octet-stream", originalName);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi tải tệp: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditComment([FromBody] CommentEditRequest req)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                bool isAdmin   = HttpContext.Session.GetInt32("isadmin") == 1;
                var comment = _commentRepo.GetByID(req.CommentId);
                if (comment == null) return Json(new { success = false, message = "Không tìm thấy" });

                bool canEdit = !comment.IsDeleted &&
                    (comment.EmployeeId == employeeId && (DateTime.Now - comment.CreatedDate).TotalMinutes <= 15 || isAdmin);

                if (!canEdit) return Json(new { success = false, message = "Không có quyền sửa" });

                comment.Content     = req.Content.Trim();
                comment.UpdatedDate = DateTime.Now;
                _commentRepo.Update(comment);

                // Broadcast SignalR
                await _hubContext.Clients.Group($"Lesson_{comment.LessonId}").SendAsync("ReceiveEditedComment", req.CommentId, req.Content);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteComment(int commentId)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                bool isAdmin   = HttpContext.Session.GetInt32("isadmin") == 1;
                var comment = _commentRepo.GetByID(commentId);
                if (comment == null) return Json(new { success = false, message = "Không tìm thấy" });

                bool canDelete = comment.EmployeeId == employeeId || isAdmin;
                if (!canDelete) return Json(new { success = false, message = "Không có quyền xóa" });

                int lessonId = comment.LessonId;

                if (isAdmin)
                {
                    // Hard delete root comment and all its replies
                    var replies = _commentRepo.GetAllList().Where(c => c.ParentId == commentId).ToList();
                    var replyIds = replies.Select(r => r.Id).ToList();
                    var subReplies = _commentRepo.GetAllList().Where(c => c.ParentId != null && replyIds.Contains(c.ParentId.Value)).ToList();
                    
                    if (subReplies.Any()) _commentRepo.RemoveRange(subReplies);
                    if (replies.Any()) _commentRepo.RemoveRange(replies);
                    _commentRepo.Delete(commentId);
                }
                else
                {
                    // Soft delete
                    comment.IsDeleted   = true;
                    comment.UpdatedDate = DateTime.Now;
                    _commentRepo.Update(comment);
                }

                // Broadcast SignalR
                await _hubContext.Clients.Group($"Lesson_{lessonId}").SendAsync("ReceiveDeletedComment", commentId, isAdmin);

                return Json(new { success = true, isHardDeleted = isAdmin });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetNotifications()
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                if (employeeId == null) return Json(new { success = false, message = "Chưa đăng nhập" });

                var list = _notificationRepo.GetLatestNotifications(employeeId.Value);
                var unreadCount = _notificationRepo.GetUnreadCount(employeeId.Value);

                return Json(new { success = true, list = list, unreadCount = unreadCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult MarkNotificationsAsRead()
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                if (employeeId == null) return Json(new { success = false, message = "Chưa đăng nhập" });

                _notificationRepo.MarkAllAsRead(employeeId.Value);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult MarkSingleNotificationAsRead(int id)
        {
            try
            {
                var employeeId = HttpContext.Session.GetInt32("employeeid");
                if (employeeId == null) return Json(new { success = false, message = "Chưa đăng nhập" });

                bool updated = _notificationRepo.MarkAsRead(id, employeeId.Value);
                return Json(new { success = true, updated = updated });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class CommentRequest
        {
            public int     LessonId    { get; set; }
            public int?    ParentId    { get; set; }
            public string  Content     { get; set; }
            /// <summary>Tên người được reply — hiển thị "A ▶ B" kiểu TikTok</summary>
            public string  ReplyToName { get; set; }
            public IFormFile CommentImage { get; set; }
            public IFormFile CommentFile { get; set; }
        }

        public class CommentEditRequest
        {
            public int    CommentId { get; set; }
            public string Content   { get; set; }
        }

        #endregion
    }
}
