
var listLesson = [];
var listExam = [];
var lis

$(document).ready(async function () {
    $(`#menu_link_items_${catalogId}`).parent().addClass("active");
    try {
        listExam = await GetCourseExam();
        GetAllLesson();
    } catch (error) {
        MessageError(error);
    }
});


function CheckHistoryLess(event, id) {
    var status = $(`#historyCheckbox_${id}`).is(':checked');
    var title = `Bạn có muốn đổi trạng thái thành ${status == true ? 'đã học' : 'chưa học'} không?`;
    event.preventDefault();
    this.blur();
    Swal.fire({
        title: title,
        showCancelButton: true,
        confirmButtonText: 'OK',
        overlay: true,
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Lesson/CheckHistoryLesson',
                type: 'POST',
                dataType: 'json',
                data: {
                    lessonId: id,
                },
                traditional: true,
                success: function (result) {
                    if (result == 1) {
                        $('#historyCheckbox_' + id).prop('checked', true); // Thay đổi giá trị checked thành true
                        if ($('#id_lesson').val() == id) {
                            $('#title-check-less').text('Đã học');
                        }
                    } else if (result == 0) {
                        $('#historyCheckbox_' + id).prop('checked', false);
                        if ($('#id_lesson').val() == id) {
                            $('#title-check-less').text('Chưa học');
                        }
                    } else {
                        MessageError("Lỗi ");
                    }

                    GetAllLesson();
                },

                error: function (err) {
                    MessageError(err.responseText);
                }
            });

        }
    })
}

//Get danh sách file đính kèm
function GetCourseFile(lessonid) {
    $.ajax({
        url: '/Lesson/GetCourseFile',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: {
            lessonID: lessonid,
        },
        success: function (data) {

            var html = '<h5>Download tài liệu đính kèm:</h5>';
            $.each(data, function (key, item) {
                html += `<form action="/Lesson/GetBlobDownload" method="get">
                            <input type="hidden" class="form-control" name="file_name" value="${item.FileName}">
                            <button type="submit" class="btn btn-success m-0 p-0 text-start"
                                    style="color:#000;background-color:transparent;border-color:transparent;box-shadow:none; text-decoration:underline;">
                                ${item.NameFile}
                            </button>
                        </form>`;
            })

            $('#download_file_lesson_container').html(html);
        },

        error: function (err) {
            MessageError(err.responseText);
        }
    });
}


//Sự kiện khi click khoá học
function onClickLesson(event, id) {

    // Chỉ remove class active trong danh sách bài học, không ảnh hưởng menu trái
    $('#list_group_lesson .active').removeClass("active");
    //Set active
    $(event.target).parent().addClass("active");
    //Load nội dung
    var lesson = listLesson.find(x => x.ID == id);
    var stringUrl = "";
    $('#course_name_lesson').text(lesson.NameCourse);

    var textClass = lesson.Status == 1 ? 'text-success' : '';

    var htmlIframeVideo = lesson.VideoURL == '' ? '' : `<iframe src="${lesson.VideoURL}" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowfullscreen style="width:100%; height:700px;"></iframe>`;
    var htmlContentHeader = `<div class="row m-0">
                                <div class="col-sm-10 p-0">
                                    <h5 class="card-title m-0">${lesson.LessonTitle}</h5>
                                </div>

                                <div class="col-sm-2 p-0">
                                    <h6 class="card-subtitle ${textClass} m-0 card-subtitle-status">${lesson.StatusText}</h6>
                                </div>
                            </div>
                            ${htmlIframeVideo}`;
    var htmlContentBody = lesson.LessonContent;



    $('#lesson_content_header').html(htmlContentHeader);
    $('#lesson_content_body').html(htmlContentBody);
    $.ajax({
        url: '/Lesson/BuildPdfUrl',
        type: 'GET',
        dataType: 'text',
        data: {
            urlPDF: lesson.UrlPDF
        },
        success: function (res) {
            console.log('Dữ liệu bài học:', res);
            stringUrl = res;
            var htmlIframePdf = lesson.UrlPDF == '' ? '' : `<iframe src="${stringUrl}" style="width:100%; height:700px;"></iframe>`;
            var htmlContentPdf = `
                            ${htmlIframePdf}`;
            $('#lesson_content_pdf').html(htmlContentPdf);

        },
        error: function (xhr, status, error) {
            console.error('Lỗi:', error);
            console.error('Response:', xhr.responseText);
        }
    });
    //Get danh sách file đính kèm
    GetCourseFile(id)
}

//Get danh sách khoá học
function GetAllLesson() {
    $.ajax({
        url: '/Lesson/GetCourseLesson',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: {
            courseId: courseId,
        },

        // =============================== lee min khooi update 19/09/2024 ====================================================================
        success: function (data) {
            console.log(data);
            let dataLesson = data.map(item => item.lstLesson);
            listLesson = dataLesson;
            var html = '';
            for (var i = 0; i < dataLesson.length; i++) {
                let htmlLessonExam = "";


                $.each(data[i].lstExam, (key, exam) => {
                    if (exam.ExamType == 1) {
                        htmlLessonExam += `<button class="dropdown-item" onclick="startExam(0, ${exam.LessonId}, 1)">Trắc nghiệm</button>`
                    } else if (exam.ExamType == 2) {
                        htmlLessonExam += `<button class="dropdown-item" onclick="startExam(0, ${exam.LessonId}, 2)">Thực hành</button>`
                    } else if (exam.ExamType == 3) {
                        htmlLessonExam += `<button class="dropdown-item" onclick="startExam(0, ${exam.LessonId}, 3)">Bài tập</button>`
                    }
                });

                let examDrop = `
                                <div class="dropdown">
                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" id="growthReportId" data-bs-toggle="dropdown" aria-haspopup="true"  aria-expanded="false">
                                    Bài test
                                </button>
                                <div class="dropdown-menu dropdown-menu-end" aria-labelledby="growthReportId">
                                    ${htmlLessonExam}
                                </div>
                            </div> `;
                var checked = dataLesson[i].Status == 1 ? 'checked' : '';
                html += `<li class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
                            <span class="badge bg-info me-2">${dataLesson[i].STT}</span>
                            <a href="#" class="text-dark" id="lesson_item_${dataLesson[i].ID}" onclick="return onClickLesson(event,${dataLesson[i].ID});" style="width: 100%;">${dataLesson[i].LessonTitle}</a>
                             ${data[i].lstExam.length > 0 ? examDrop : ''}
                            <input class="form-check-input ms-3 p-2" type="checkbox" id="historyCheckbox_${dataLesson[i].ID}" onclick="return CheckHistoryLess(event,${dataLesson[i].ID})" ${checked}>
                        </li>\n`;
            }



            //var status = data.find(x => x.Status == 0);

            //if (status == null && courseId > 0) {
            //    html += `<li class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
            //                <span class="badge bg-info me-2"></span>
            //                <a href="/CourseExamResult/Index?courseId=${courseId}" style="width: 100%;">Bài kiểm tra</a>
            //            </li>`;
            //}

            //html += `<li class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
            //                <span class="badge bg-info me-2"></span>
            //                <a href="/CourseExamResult/Index?courseId=${courseId}" style="width: 100%;">Bài kiểm tra</a>
            //            </li>`;
            var htmlExam = '';
            $.each(listExam, (key, item) => {
                if (item.ExamType == 1) {
                    htmlExam += `<li class="nav-item m-1">
                                    <button class="btn btn-primary btn-sm w-100" onclick="startExam(${courseId}, 0, 1)">Trắc nghiệm</button>
                                </li>`
                } else if (item.ExamType == 2) {
                    htmlExam += ` <li class="nav-item m-1">
                                    <button class="btn btn-warning btn-sm w-100" onclick="startExam(${courseId}, 0, 2)">Thực hành</button>
                                </li>`
                } else if (item.ExamType == 3) {
                    htmlExam += `<li class="nav-item m-1">
                                    <button class="btn btn-info btn-sm w-100" onclick="startExam(${courseId}, 0, 3)">Bài tập</button>
                                </li>`
                } else {

                }
            });

            // Also update dropdown exam links
            $.each(data, (idx, cat) => {
                $.each(cat.lstExam, (key, exam) => {
                    if (exam.ExamType == 1) {
                        $(`a[href="/CourseExamResult/LessonExamQuiz?lessonID=${exam.LessonId}"]`).replaceWith(`<button class="dropdown-item" onclick="startExam(0, ${exam.LessonId}, 1)">Trắc nghiệm</button>`);
                    } else if (exam.ExamType == 2) {
                        $(`a[href="/CourseExamResult/Practice?lessonID=${exam.LessonId}"]`).replaceWith(`<button class="dropdown-item" onclick="startExam(0, ${exam.LessonId}, 2)">Thực hành</button>`);
                    } else if (exam.ExamType == 3) {
                        $(`a[href="/CourseExamResult/Exercise?lessonID=${exam.LessonId}"]`).replaceWith(`<button class="dropdown-item" onclick="startExam(0, ${exam.LessonId}, 3)">Bài tập</button>`);
                    }
                });
            });

            html += `<li class="list-group-item list-group-item-action d-flex justify-content-between align-items-center p-0">
                            <ul class="nav nav-tabs nav-fill w-100" role="tablist">
                                ${htmlExam}
                            </ul>
                        </li>`;
            $('#list_group_lesson').html(html);

            $(`#lesson_item_${dataLesson[0].ID}`).click();
        },

        error: function (err) {
            MessageError(err.responseText);
        }
    });
}


function GetCourseExam() {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/Lesson/GetCourseExam',
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            data: {
                courseId: courseId,
            },
            success: function (data) {
                resolve(data);

            },
            error: function (err) {
                reject(err.responseText);
            }
        });
    });
}

// Validate exam and redirect - không reload khi validation fail
function startExam(courseId, lessonId, examType) {
    // Chọn API tương ứng với loại exam
    var apiUrl = '/CourseExamResult/ValidateExam';
    if (examType === 2) {
        apiUrl = '/CourseExamResult/ValidatePractice';
    } else if (examType === 3) {
        apiUrl = '/CourseExamResult/ValidateExercise';
    }

    $.ajax({
        url: apiUrl,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { courseId: courseId, lessonID: lessonId },
        success: function (result) {
            if (result.success) {
                // Validation OK - redirect sang trang thi
                if (examType === 1) {
                    if (lessonId > 0) {
                        window.location.href = `/CourseExamResult/LessonExamQuiz?lessonID=${lessonId}`;
                    } else {
                        window.location.href = `/CourseExamResult/Index?courseId=${courseId}`;
                    }
                } else if (examType === 2) {
                    window.location.href = `/CourseExamResult/Practice?courseId=${courseId}&lessonID=${lessonId}`;
                } else if (examType === 3) {
                    window.location.href = `/CourseExamResult/Exercise?courseId=${courseId}&lessonID=${lessonId}`;
                }
            } else {
                // Validation fail - hiện popup lỗi, KHÔNG reload
                Swal.fire({
                    icon: 'error',
                    title: 'Không thể thi',
                    text: result.message,
                    confirmButtonText: 'Đóng',
                    confirmButtonColor: '#667eea',
                    allowOutsideClick: true,
                    allowEscapeKey: true,
                    heightAuto: false
                });
            }
        },
        error: function (err) {
            console.log('AJAX Error:', err);
            var errorMsg = err.responseJSON ? err.responseJSON.message : (err.responseText || 'Đã xảy ra lỗi khi kiểm tra. Vui lòng thử lại!');
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: errorMsg,
                confirmButtonText: 'Đóng',
                confirmButtonColor: '#667eea',
                heightAuto: false
            });
        }
    });
}