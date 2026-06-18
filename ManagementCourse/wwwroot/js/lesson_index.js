
var listLesson = [];
var listExam = [];
var currentLessonId = 0;
var commentHubConnection = null;
var commentPage = 0;
var commentTotal = 0;
const COMMENT_PAGE_SIZE = 10;
var selectedStars = 0;
var userHasRated = false;      // true khi đã đánh giá (set từ API)
var progressFirstLoad = true; // bỏ qua lần updateProgress đầu tiên (page load)
var participantPage = 0;
var participantsLoaded = false;
var ratingLabels = ['', 'Tệ', 'Không tốt', 'Bình thường', 'Tốt', 'Xuất sắc'];

var attachedImageFile = null;
var attachedDocFile = null;
var inlineAttachedImages = {};
var inlineAttachedDocs = {};


/**
 * Cập nhật hiển thị các tab dựa theo nội dung bài học.
 * Truyền null để giữ nguyên trạng thái hiện tại của tab đó.
 * @param {boolean|null} hasContent - Có nội dung văn bản không
 * @param {boolean|null} hasPdf     - Có file PDF không
 * @param {boolean|null} hasFiles   - Có file đính kèm không
 */
function updateTabVisibility(hasContent, hasPdf, hasFiles) {
    var $btnContent = $('[data-target="lesson_content_body"]');
    var $btnPdf = $('[data-target="lesson_content_pdf"]');
    var $btnFiles = $('[data-target="download_file_lesson_container"]');

    if (hasContent !== null) $btnContent.toggle(hasContent);
    if (hasPdf !== null) $btnPdf.toggle(hasPdf);
    if (hasFiles !== null) $btnFiles.toggle(hasFiles);

    // Nếu tab đang active bị ẩn → tự switch sang tab đầu tiên còn hiển thị
    var $activeBtn = $('.lesson-tabs-bar .lesson-tab.active');
    if ($activeBtn.length && $activeBtn.is(':hidden')) {
        var $firstVisible = $('.lesson-tabs-bar .lesson-tab:visible').first();
        if ($firstVisible.length) {
            $firstVisible.trigger('click');
        }
    }
}

$(document).ready(async function () {
    $(`#menu_link_items_${catalogId}`).parent().addClass("active");
    try {
        listExam = await GetCourseExam();
        GetAllLesson();
        setupStarPicker();
        loadRatingSummary();
        loadParticipantCount();
        initCommentHub();

        // Lắng nghe thay đổi hash (khi click thông báo khi đang ở cùng trang)
        window.addEventListener('hashchange', function () {
            scrollToAndHighlightComment();
        });
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
            var hasFiles = data && data.length > 0;

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

            // Ẩn/hiển tab File đính kèm sau khi có kết quả AJAX
            updateTabVisibility(null, null, hasFiles);
        },

        error: function (err) {
            MessageError(err.responseText);
        }
    });
}


//Sự kiện khi click khoá học
function onClickLesson(event, id) {
    var oldLessonId = currentLessonId;
    currentLessonId = id;

    if (commentHubConnection && commentHubConnection.state === signalR.HubConnectionState.Connected) {
        if (oldLessonId && oldLessonId !== id) {
            commentHubConnection.invoke("LeaveLessonGroup", oldLessonId);
        }
        if (id) {
            commentHubConnection.invoke("JoinLessonGroup", id);
        }
    }

    // Chỉ remove class active trong danh sách bài học, không ảnh hưởng menu trái
    $('#list_group_lesson .active').removeClass("active");
    //Set active
    $(event.target).parent().addClass("active");

    // Reset tab về "Nội dung bài học" mỗi khi chuyển bài mới
    $('.lesson-tabs-bar .lesson-tab').removeClass('active');
    $('.lesson-tab-content .tab-pane').removeClass('active');
    $('[data-target="lesson_content_body"]').addClass('active');
    $('#lesson_content_body').addClass('active');

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

    function getRawText(html) {
        var div = document.createElement('div');
        div.innerHTML = html || '';

        // Xóa các phần không phải nội dung
        div.querySelectorAll('style, script, head').forEach(function (el) {
            el.remove();
        });

        return (div.textContent || div.innerText || '')
            .replace(/\u00A0/g, ' ')
            .trim();
    }


    // Kiểm tra nội dung và PDF — ẩn tab nếu rỗng
    var rawText = getRawText(lesson.LessonContent);
    var hasContent = rawText.length > 0;
    var hasPdf = !!(lesson.UrlPDF && lesson.UrlPDF.trim() !== '');
    updateTabVisibility(hasContent, hasPdf, null);

    $('#lesson_content_header').html(htmlContentHeader);

    // Ẩn vùng trống khi bài học không có video
    if (!lesson.VideoURL || lesson.VideoURL.trim() === '') {
        $('.lesson-video-card').addClass('no-video');
    } else {
        $('.lesson-video-card').removeClass('no-video');
    }

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
            var htmlIframePdf = lesson.UrlPDF == '' ? '' : `<iframe src="${stringUrl}" style="width:100%; height:780px;"></iframe>`;
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
    GetCourseFile(id);
    loadLikeStatus(id);
    // Reset tab comment mỗi khi chuyển bài
    $('#comment-list').html('');
    cancelReply();
    clearAllAttachments();
    commentPage = 0;
    commentTotal = 0;
    $('#comment-badge').text('0');
    if ($('#tab-comment').hasClass('active')) {
        loadComments(true);
    } else {
        scrollToAndHighlightComment();
    }
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


            updateProgress();

            var urlParams = new URLSearchParams(window.location.search);
            var urlLessonId = urlParams.get('lessonId');
            if (urlLessonId && $(`#lesson_item_${urlLessonId}`).length > 0) {
                $(`#lesson_item_${urlLessonId}`).click();
            } else {
                $(`#lesson_item_${dataLesson[0].ID}`).click();
            }
        },

        error: function (err) {
            MessageError(err.responseText);
        }
    });
}


// Cập nhật thanh tiến độ khoá học
function updateProgress() {
    var done = listLesson.filter(function (x) { return x.Status === 1; }).length;
    var total = listLesson.length;
    var pct = total > 0 ? Math.round(done / total * 100) : 0;
    $('#progress_fill').css('width', pct + '%');
    $('#progress_pct').text(pct + '%');
    var desc = 'Bạn đã hoàn thành ' + done + '/' + total + ' bài học';
    if (pct === 100) desc += ' của khoá học này.';
    else desc += '.';
    $('#progress_desc').text(desc);
    // Chỉ popup khi user vừa hoàn thành bài cuối, không popup lúc load trang
    if (!progressFirstLoad && pct === 100 && !userHasRated) {
        var sessionKey = 'ratingShown_' + courseId;
        if (!sessionStorage.getItem(sessionKey)) {
            sessionStorage.setItem(sessionKey, '1');
            userHasRated = true;
            setTimeout(showRatingModal, 800);
        }
    }
    progressFirstLoad = false;
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

// ===== LIKE BÀI HỌC =====
function loadLikeStatus(lessonId) {
    $.get('/Lesson/GetLikeStatus', { lessonId: lessonId }, function (res) {
        updateLikeUI(res.liked, res.totalLikes);
    });
}

let likeDebounceTimer;
function toggleLike() {
    if (!currentLessonId) return;

    var $btn = $('#btn-like-lesson');
    var isCurrentlyLiked = $btn.hasClass('liked');
    var newLikedState = !isCurrentlyLiked;
    var count = parseInt($('#like-count').text()) || 0;
    count = newLikedState ? count + 1 : Math.max(0, count - 1);

    // Cập nhật UI ngay lập tức
    updateLikeUI(newLikedState, count);

    // Chống spam: clear timer cũ
    clearTimeout(likeDebounceTimer);

    // Đặt timer mới 500ms
    likeDebounceTimer = setTimeout(function () {
        $.post('/Lesson/ToggleLike', { lessonId: currentLessonId }, function (res) {
            if (res.success) {
                updateLikeUI(res.liked, res.totalLikes); // Đồng bộ lại với server
            } else {
                updateLikeUI(isCurrentlyLiked, isCurrentlyLiked ? count + 1 : Math.max(0, count - 1)); // Hoàn tác nếu lỗi
            }
        }).fail(function () {
            updateLikeUI(isCurrentlyLiked, isCurrentlyLiked ? count + 1 : Math.max(0, count - 1)); // Hoàn tác nếu lỗi mạng
        });
    }, 500);
}

function updateLikeUI(liked, count) {
    var $btn = $('#btn-like-lesson');
    var $icon = $('#icon-like');
    $btn.toggleClass('liked', liked);
    $icon.attr('class', liked ? 'bx bxs-heart' : 'bx bx-heart');
    $('#like-count').text(count || 0);
}

// ===== BÌNH LUẬN BÀI HỌC =====
function onTabComment() {
    if ($('#comment-list').html() === '') {
        loadComments(true);
    }
}

function loadComments(reset) {
    if (!currentLessonId) return;
    if (reset) { commentPage = 0; $('#comment-list').html(''); }
    $.get('/Lesson/GetComments', {
        lessonId: currentLessonId,
        pageIndex: commentPage,
        pageSize: COMMENT_PAGE_SIZE
    }, function (res) {
        if (!res.success) return;
        commentTotal = res.total;
        res.data.forEach(function (c) {
            $('#comment-list').append(renderComment(c, 0));
        });
        $('#comment-badge').text(commentTotal);
        var loaded = commentPage * COMMENT_PAGE_SIZE + res.data.length;
        $('#comment-load-more').toggle(loaded < commentTotal);

        if (reset) {
            scrollToAndHighlightComment();
        }
    });
}

function loadMoreComments() {
    commentPage++;
    loadComments(false);
}

// level: 0 = cấp cha, 1 = cấp con 1, 2 = cấp con 2
function renderComment(c, level, isReplyToAbove, hasReplyDown) {
    var avatar = (c.imagePath && c.imagePath.trim() !== '')
        ? '/api/share/' + c.imagePath
        : '/assets/images/avatar_default.png';
    var content = c.isDeleted
        ? '<span class="comment-deleted">[\u0110\u00e3 x\u00f3a]</span>'
        : highlightMentions(escapeHtml(c.content));
    var edited = (c.updatedDate && !c.isDeleted) ? '<span class="comment-edited">(\u0111\u00e3 ch\u1ec9nh s\u1eeda)</span>' : '';

    // Chỉ hiện “A ▶ B” ở cấp con 2 trở lên
    var replyIndicator = (level >= 2 && c.replyToName && !c.isDeleted)
        ? '<span class="reply-to-indicator"><i class="bx bx-chevron-right"></i>'
        + '<span class="reply-to-name">' + escapeHtml(c.replyToName) + '</span></span>'
        : '';
    var actions = '';
    if (!c.isDeleted) {
        var targetParentId = (level >= 2) ? (c.parentId || c.parentID) : c.id;
        actions += '<button class="btn-reply-action" onclick="replyTo(' + c.id + ',\'' + escapeHtml(c.fullName) + '\',' + level + ',' + targetParentId + ')">' +
            '<i class="bx bx-reply"></i> Trả lời</button>';

        // Reactions button with popup picker
        var activeClass = c.userCurrentReaction ? 'has-reacted ' + c.userCurrentReaction : '';
        var btnText = c.userCurrentReaction
            ? getReactionEmoji(c.userCurrentReaction) + ' ' + getReactionLabel(c.userCurrentReaction)
            : '<i class="bx bx-like"></i> Thích';

        actions += '<button class="comment-reaction-btn ' + activeClass + '" onclick="event.stopPropagation(); toggleReaction(' + c.id + ', \'like\')">'
            + '<span class="reaction-btn-text">' + btnText + '</span>'
            + '<div class="reaction-picker-popup" onclick="event.stopPropagation()">'
            + '  <span class="reaction-emoji-option" onclick="toggleReaction(' + c.id + ', \'like\')" title="Thích">👍</span>'
            + '  <span class="reaction-emoji-option" onclick="toggleReaction(' + c.id + ', \'love\')" title="Yêu thích">❤️</span>'
            + '  <span class="reaction-emoji-option" onclick="toggleReaction(' + c.id + ', \'haha\')" title="Haha">😆</span>'
            + '  <span class="reaction-emoji-option" onclick="toggleReaction(' + c.id + ', \'wow\')" title="Wow">😮</span>'
            + '  <span class="reaction-emoji-option" onclick="toggleReaction(' + c.id + ', \'sad\')" title="Buồn">😢</span>'
            + '  <span class="reaction-emoji-option" onclick="toggleReaction(' + c.id + ', \'angry\')" title="Phẫn nộ">😡</span>'
            + '</div>'
            + '</button>';

        if (c.canEdit) actions += '<button class="btn-edit-action" onclick="editComment(' + c.id + ')"><i class="bx bx-edit"></i> Sửa</button>';
        if (c.canDelete || isAdmin) actions += '<button class="btn-delete-action" onclick="deleteComment(' + c.id + ')"><i class="bx bx-trash"></i> Xóa</button>';
    }

    // Nút "X trả lời": hiện cho cấp cha và cấp con 1 nếu có sub-replies
    var nextLevel = level + 1;
    var repliesBtn = (level < 2 && c.replyCount > 0)
        ? '<button class="btn-show-replies" data-count="' + c.replyCount + '" onclick="toggleReplies(' + c.id + ', this, ' + nextLevel + ')"><i class="bx bx-chevron-down"></i> ' + c.replyCount + ' trả lời</button>'
        : '';

    // repliesContainer chỉ tạo cho cấp cha và cấp con 1
    var repliesContainer = (level < 2)
        ? '<div class="replies-container" id="replies-' + c.id + '"></div>'
        : '';

    // Render attachments HTML
    var attachmentsHtml = '';
    if (!c.isDeleted) {
        if (c.imageServerName) {
            var imgUrl = PathCommentImages + '/' + c.imageServerName;
            attachmentsHtml += '<div class="comment-image-attachment" onclick="openLightbox(\'' + imgUrl + '\')">'
                + '<img src="' + imgUrl + '" />'
                + '</div>';
        }
        if (c.attachmentServerName) {
            var downloadUrl = PathCommentFiles + '/'
                + encodeURIComponent(c.attachmentServerName)
                ;
            attachmentsHtml += '<a href="' + downloadUrl + '" class="comment-file-attachment" title="' + escapeHtml(c.attachmentOriginalName) + '">'
                + '<i class="bx bx-paperclip"></i> <span class="file-name-text">' + escapeHtml(c.attachmentOriginalName) + '</span>'
                + '</a>';
        }
    }

    // Render reactions summary HTML
    var reactionsSummaryHtml = '';
    if (!c.isDeleted && c.reactionSummary && c.reactionSummary.length > 0) {
        reactionsSummaryHtml += '<div class="comment-reactions-summary-container">';
        c.reactionSummary.forEach(function (sum) {
            var icon = getReactionEmoji(sum.reactionType);
            var shortCount = formatReactionCount(sum.count);

            var reactorNames = c.reactionDetails
                .filter(d => d.reactionType === sum.reactionType)
                .map(d => d.fullName)
                .join(', ');

            var isUserReacted = c.reactionDetails.some(d => d.reactionType === sum.reactionType && d.employeeId === currentEmployeeId);
            var activeClass = isUserReacted ? 'active' : '';

            reactionsSummaryHtml += '<div class="reaction-summary-pill ' + activeClass + '" onclick="event.stopPropagation(); toggleReaction(' + c.id + ', \'' + sum.reactionType + '\')">'
                + '<span>' + icon + ' ' + shortCount + '</span>'
                + '<span class="tooltip-text">' + escapeHtml(reactorNames) + '</span>'
                + '</div>';
        });
        reactionsSummaryHtml += '</div>';
    } else {
        reactionsSummaryHtml += '<div class="comment-reactions-summary-container"></div>';
    }

    // CSS class theo cấp
    var levelClass = level === 0 ? '' : (level === 1 ? 'is-reply' : 'is-reply-2');
    if (isReplyToAbove) levelClass += ' connect-to-above';
    if (hasReplyDown) levelClass += ' has-reply-down';
    var avatarLetter = c.fullName?.trim()
        ? c.fullName.charAt(0).toUpperCase()
        : 'U';
    return '<div class="comment-item ' + levelClass + '" id="comment-' + c.id + '">'
        + '<div class="comment-main">'           // wrapper flex: avatar + body (no replies)
        + '<div class="topbar-avatar">' + avatarLetter + '</div>'
        + '<div class="comment-body">'
        + '<div class="comment-header">'
        + '<span class="comment-author">' + escapeHtml(c.fullName) + '</span>'
        + replyIndicator
        + '<span class="comment-time">' + c.timeAgo + '</span>' + edited
        + '</div>'
        + '<div class="comment-content" id="comment-content-' + c.id + '">' + content + '</div>'
        + attachmentsHtml
        + reactionsSummaryHtml
        + '<div class="comment-footer">' + actions + repliesBtn + '</div>'
        + '</div>'                               // close comment-body
        + '</div>'                               // close comment-main (thread line ::after stops here)
        + repliesContainer                       // OUTSIDE comment-main — line never enters here
        + '</div>';
}

function toggleReplies(commentId, btn, nextLevel) {
    var $container = $('#replies-' + commentId);
    var $commentItem = $('#comment-' + commentId);
    if ($container.hasClass('open')) {
        $container.removeClass('open').html('');
        $commentItem.removeClass('replies-open'); // ẩn thread line
        $(btn).html('<i class="bx bx-chevron-down"></i> ' + $(btn).data('count') + ' trả lời');
    } else {
        reloadRepliesSilently(commentId);
        $container.addClass('open');
        $commentItem.addClass('replies-open'); // hiện thread line
    }
}

function replyTo(commentId, fullName, level, targetParentId) {
    // Đóng toàn bộ các ô nhập inline khác đang mở trên trang
    $('.inline-reply-wrapper').remove();

    var paddingLeft = level === 0 ? '46px' : '0px';

    // Tạo HTML cho ô nhập inline
    var html = '<div class="inline-reply-wrapper" id="reply-box-' + commentId + '" style="padding-left: ' + paddingLeft + '">'
        + '<div class="topbar-avatar">' + avatarLetter + '</div>'
        + '<div class="reply-input-wrap">'
        + '<div class="reply-editor-container">'
        + '<textarea class="reply-textarea" id="reply-text-' + commentId + '" placeholder="Nhập câu trả lời của bạn..." maxlength="1000"></textarea>'
        + '</div>'
        // Attachment preview container for inline reply
        + '<div id="inline-attachment-preview-' + commentId + '" class="comment-attachment-preview-container" style="display:none">'
        + '  <div id="inline-image-preview-wrapper-' + commentId + '" class="preview-item" style="display:none">'
        + '    <img id="inline-image-preview-' + commentId + '" src="" alt="Image preview" />'
        + '    <button type="button" class="btn-remove-preview" onclick="removeInlineAttachedImage(' + commentId + ')">&times;</button>'
        + '  </div>'
        + '  <div id="inline-file-preview-wrapper-' + commentId + '" class="preview-item" style="display:none">'
        + '    <i class="bx bx-paperclip"></i> <span id="inline-file-preview-name-' + commentId + '" class="preview-file-name">file.txt</span>'
        + '    <button type="button" class="btn-remove-preview" onclick="removeInlineAttachedFile(' + commentId + ')">&times;</button>'
        + '  </div>'
        + '</div>'
        + '<input type="file" id="inline-image-input-' + commentId + '" accept="image/*" style="display:none" onchange="handleInlineImageSelected(this, ' + commentId + ')" />'
        + '<input type="file" id="inline-file-input-' + commentId + '" style="display:none" onchange="handleInlineFileSelected(this, ' + commentId + ')" />'
        + '<div class="reply-actions-row">'
        + '<div style="display:flex; gap:8px; align-items:center; margin-right:auto;">'
        + '  <button type="button" class="btn-attach" onclick="triggerInlineImageUpload(' + commentId + ')" title="Đính kèm ảnh"><i class="bx bx-image"></i> Ảnh</button>'
        + '  <button type="button" class="btn-attach" onclick="triggerInlineFileUpload(' + commentId + ')" title="Đính kèm tài liệu"><i class="bx bx-paperclip"></i> Tài liệu</button>'
        + '</div>'
        + '<button class="btn-inline-cancel" onclick="closeReplyBox(' + commentId + ')">Hủy</button>'
        + '<button class="btn-inline-submit" id="btn-inline-submit-' + commentId + '" onclick="submitInlineReply(' + commentId + ', \'' + escapeHtml(fullName) + '\', ' + targetParentId + ')">Bình luận</button>'
        + '</div>'
        + '</div>'
        + '</div>';

    // Chèn trực tiếp dưới comment-main của comment được click
    $('#comment-' + commentId + ' > .comment-main').after(html);

    // Focus vào textarea mới tạo
    $('#reply-text-' + commentId).focus();
}

function closeReplyBox(commentId) {
    // Clear attachment maps for this commentId
    delete inlineAttachedImages[commentId];
    delete inlineAttachedDocs[commentId];
    $('#reply-box-' + commentId).remove();
}

function submitInlineReply(commentId, replyToName, targetParentId) {
    var content = $('#reply-text-' + commentId).val().trim();
    var imgFile = inlineAttachedImages[commentId];
    var docFile = inlineAttachedDocs[commentId];

    if (!content && !imgFile && !docFile) return;

    var formData = new FormData();
    formData.append('LessonId', currentLessonId);
    formData.append('ParentId', targetParentId);
    formData.append('Content', content);
    if (replyToName) formData.append('ReplyToName', replyToName);

    if (imgFile) formData.append('CommentImage', imgFile);
    if (docFile) formData.append('CommentFile', docFile);

    var $btn = $('#btn-inline-submit-' + commentId);
    $btn.prop('disabled', true);

    $.ajax({
        url: '/Lesson/AddComment',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            $btn.prop('disabled', false);
            if (!res.success) {
                alert(res.message || 'Lỗi gửi bình luận');
                return;
            }
            // Clear maps
            delete inlineAttachedImages[commentId];
            delete inlineAttachedDocs[commentId];
            closeReplyBox(commentId);

            // Tìm comment gốc để reload lại các replies của nó
            var parentCommentItem = $('#comment-' + targetParentId);
            var repliesBtn = parentCommentItem.find('.btn-show-replies');
            var container = $('#replies-' + targetParentId);

            if (container.hasClass('open')) {
                // Tải lại replies
                container.removeClass('open').html('');
                parentCommentItem.removeClass('replies-open');

                $.get('/Lesson/GetReplies', { parentId: targetParentId }, function (replyRes) {
                    if (!replyRes.success) return;
                    var html = '';
                    var nextLevel = parentCommentItem.hasClass('is-reply') ? 2 : 1;
                    replyRes.data.forEach(function (r, index) {
                        var isReplyToAbove = false;
                        var hasReplyDown = false;
                        if (nextLevel >= 2) {
                            if (index > 0) {
                                var prevReply = replyRes.data[index - 1];
                                if (r.replyToName && r.replyToName === prevReply.fullName) {
                                    isReplyToAbove = true;
                                }
                            } else {
                                isReplyToAbove = true;
                            }
                        }
                        if (index < replyRes.data.length - 1) {
                            var nextReply = replyRes.data[index + 1];
                            if (nextReply.replyToName && nextReply.replyToName === r.fullName) {
                                hasReplyDown = true;
                            }
                        }
                        html += renderComment(r, nextLevel, isReplyToAbove, hasReplyDown);
                    });
                    container.html(html).addClass('open');
                    parentCommentItem.addClass('replies-open');

                    if (repliesBtn.length > 0) {
                        repliesBtn.data('count', replyRes.data.length);
                        repliesBtn.html('<i class="bx bx-chevron-up"></i> Thu gọn');
                    }
                });
            } else {
                if (repliesBtn.length > 0) {
                    repliesBtn.click();
                } else {
                    // Nếu chưa có nút, reload toàn bộ danh sách để cập nhật số lượng và render nút
                    loadComments(true);
                }
            }
        },
        error: function () {
            $btn.prop('disabled', false);
            alert('Lỗi kết nối. Vui lòng thử lại!');
        }
    });
}

function cancelReply() {
    $('#comment-parent-id').val('');
    $('#comment-reply-to-name').val('');
    $('#comment-reply-info').hide();
    $('#btn-cancel-reply').hide();
    $('#comment-content').val('');
}

function submitComment() {
    var content = $('#comment-content').val().trim();
    var parentId = parseInt($('#comment-parent-id').val()) || null;
    var replyToName = $('#comment-reply-to-name').val().trim() || null;

    if (!content && !attachedImageFile && !attachedDocFile) return;

    var formData = new FormData();
    formData.append('LessonId', currentLessonId);
    if (parentId !== null) formData.append('ParentId', parentId);
    if (replyToName !== null) formData.append('ReplyToName', replyToName);
    formData.append('Content', content);

    if (attachedImageFile) {
        formData.append('CommentImage', attachedImageFile);
    }
    if (attachedDocFile) {
        formData.append('CommentFile', attachedDocFile);
    }

    var $btn = $('.btn-submit-comment');
    $btn.prop('disabled', true);

    $.ajax({
        url: '/Lesson/AddComment',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            $btn.prop('disabled', false);
            if (!res.success) {
                alert(res.message || 'Lỗi gửi bình luận');
                return;
            }
            cancelReply();
            clearAllAttachments();
            loadComments(true);
        },
        error: function () {
            $btn.prop('disabled', false);
            alert('Lỗi kết nối. Vui lòng thử lại!');
        }
    });
}

function editComment(commentId) {
    if ($('#edit-form-' + commentId).length) return;
    var $content = $('#comment-content-' + commentId);
    var rawText = $content.text().trim();
    if (rawText === '[Đã xóa]') rawText = '';

    var editHtml = '<div class="edit-form" id="edit-form-' + commentId + '">'
        + '<textarea class="comment-textarea edit-textarea" rows="3" maxlength="1000">'
        + escapeHtml(rawText)
        + '</textarea>'
        + '<div class="comment-actions-row" style="margin-top:6px">'
        + '<button class="btn-submit-comment" onclick="saveEdit(' + commentId + ')">'
        + '<i class="bx bx-check"></i> Lưu</button>'
        + '<button class="btn-cancel-reply" onclick="cancelEdit(' + commentId + ')" '
        + 'style="display:inline-block;color:#666">Hủy</button>'
        + '</div></div>';

    $content.hide().after(editHtml);
    var $ta = $('#edit-form-' + commentId + ' textarea');
    var len = $ta.val().length;
    $ta[0].setSelectionRange(len, len);
    $ta.focus();
}

function saveEdit(commentId) {
    var newText = $('#edit-form-' + commentId + ' textarea').val().trim();
    if (!newText) { alert('Nội dung không được để trống!'); return; }
    $.ajax({
        url: '/Lesson/EditComment',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ commentId: commentId, content: newText }),
        success: function (res) {
            if (res.success) {
                var $c = $('#comment-content-' + commentId);
                $c.html(highlightMentions(escapeHtml(newText))).show();
                $('#edit-form-' + commentId).remove();
                var $header = $('#comment-' + commentId + ' .comment-header');
                if (!$header.find('.comment-edited').length) {
                    $header.append('<span class="comment-edited">(đã chỉnh sửa)</span>');
                }
            } else {
                alert(res.message || 'Lưu thất bại!');
            }
        },
        error: function () { alert('Lỗi kết nối. Vui lòng thử lại!'); }
    });
}

function cancelEdit(commentId) {
    $('#edit-form-' + commentId).remove();
    $('#comment-content-' + commentId).show();
}

function deleteComment(commentId) {
    var title = 'Xóa bình luận?';
    var text = isAdmin ? 'Bình luận này và toàn bộ các phản hồi liên quan sẽ bị xóa vĩnh viễn!' : 'Bình luận sẽ được ẩn đi.';

    Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy',
        confirmButtonColor: '#f5222d'
    }).then(function (r) {
        if (!r.isConfirmed) return;
        $.post('/Lesson/DeleteComment', { commentId: commentId }, function (res) {
            if (res.success) {
                if (res.isHardDeleted) {
                    $('#comment-' + commentId).fadeOut(300, function () {
                        $(this).remove();
                    });
                } else {
                    $('#comment-content-' + commentId).html('<span class="comment-deleted">[Đã xóa]</span>');
                    var $item = $('#comment-' + commentId);
                    $item.find('.comment-image-attachment').remove();
                    $item.find('.comment-file-attachment').remove();
                    $item.find('.comment-reactions-summary-container').remove();
                    $item.find('.comment-edited').remove();
                    $item.find('.comment-footer').html('');
                }
            }
        });
    });
}

function highlightMentions(text) {
    // Format m\u1edbi: @[H\u1ecd T\u00ean] \u2014 match to\u00e0n b\u1ed9 t\u00ean c\u00f3 d\u1ea5u c\u00e1ch
    text = text.replace(/@\[([^\]]+)\]/g, '<span class="mention">@$1</span>');
    // Fallback format c\u0169: @T\u1eeb (kh\u00f4ng c\u00f3 d\u1ea5u c\u00e1ch) \u2014 d\u00e0nh cho data \u0111\u00e3 l\u01b0u tr\u01b0\u1edbc
    text = text.replace(/@([^\s<&\[]+)/g, '<span class="mention">@$1</span>');
    return text;
}
function escapeHtml(str) {
    return String(str || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

// ===== ĐÁNH GIÁ SAO =====
function setupStarPicker() {
    $(document).on('mouseenter', '.star-btn', function () {
        var val = parseInt($(this).data('val'));
        $('.star-btn').each(function (i) { $(this).toggleClass('hovered', i < val); });
        $('#rating-label').text(ratingLabels[val]);
    }).on('mouseleave', '.star-btn', function () {
        $('.star-btn').removeClass('hovered');
        $('#rating-label').text(selectedStars ? ratingLabels[selectedStars] : 'Chọn số sao');
    }).on('click', '.star-btn', function () {
        selectedStars = parseInt($(this).data('val'));
        $('.star-btn').each(function (i) { $(this).toggleClass('selected', i < selectedStars); });
        $('#rating-label').text(ratingLabels[selectedStars]);
    });
}

function loadRatingSummary() {
    $.get('/Home/GetRating', { courseId: courseId }, function (res) {
        var avg = parseFloat(res.avgStars) || 0;
        var stars = '';
        for (var i = 1; i <= 5; i++) stars += (i <= Math.round(avg) ? '★' : '☆');
        $('#rating-stars-display').text(stars);
        var txt = res.totalRatings > 0
            ? avg.toFixed(1) + ' sao (' + res.totalRatings + ' đánh giá)'
            : 'Chưa có đánh giá';
        $('#rating-avg-text').text(txt);
        if (res.userStars) {
            userHasRated = true; // Đã đánh giá rồi → không popup
            selectedStars = res.userStars;
            $('#rating-comment').val(res.userComment || '');
            $('.star-btn').each(function (i) { $(this).toggleClass('selected', i < selectedStars); });
            $('#rating-label').text(ratingLabels[selectedStars]);
        }
    });
}

function showRatingModal() {
    $('#rating-course-name').text($('#course_name_lesson').text());
    $('#modal-rating').fadeIn(200);
}
function closeRatingModal(e) {
    if ($(e.target).is('#modal-rating')) $('#modal-rating').fadeOut(200);
}
function closeRatingModalBtn() { $('#modal-rating').fadeOut(200); }

function submitRating() {
    if (!selectedStars) { alert('Vui lòng chọn số sao!'); return; }
    $.ajax({
        url: '/Home/RateCourse',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ courseId: courseId, stars: selectedStars, comment: $('#rating-comment').val() }),
        success: function (res) {
            if (!res.success) return;
            Swal.fire({ icon: 'success', title: 'Cảm ơn bạn!', text: 'Đánh giá đã được lưu.', timer: 2000, showConfirmButton: false });
            $('#modal-rating').fadeOut(200);
            loadRatingSummary();
        }
    });
}

// Load số người tham gia ngay khi khởi tạo (hiển thị count trước khi mở panel)
function loadParticipantCount() {
    $.get('/Lesson/GetParticipants', {
        courseId: courseId,
        pageIndex: 0,
        pageSize: 1
    }, function (res) {
        if (res.success) {
            $('#participant-count').text(res.total || 0);
        }
    });
}

// ===== NGƯỜI THAM GIA =====
function toggleParticipants() {
    var $body = $('#participants-body');
    var isOpen = $body.is(':visible');
    $body.slideToggle(200);
    $('#icon-participants-toggle').attr('class', isOpen ? 'bx bx-chevron-down' : 'bx bx-chevron-up');
    if (!isOpen && !participantsLoaded) { participantsLoaded = true; loadParticipants(); }
}

function loadParticipants() {
    $.get('/Lesson/GetParticipants', { courseId: courseId, pageIndex: participantPage, pageSize: 10 }, function (res) {
        if (!res.success) return;
        $('#participant-count').text(res.total);
        res.data.forEach(function (p) { $('#participants-list').append(renderParticipant(p)); });
        var loaded = (participantPage + 1) * 10;
        $('#participants-load-more').toggle(loaded < res.total);
        participantPage++;
    });
}

function renderParticipant(p) {
    var avatar = (p.imagePath && p.imagePath.trim() !== '')
        ? '/api/share/' + p.imagePath
        : 'https://ui-avatars.com/api/?name=' + encodeURIComponent(p.fullName) + '&size=32&background=e8e8e8&color=7c3aed';
    var statusClass = p.progressPct >= 100 ? 'done' : 'inprogress';
    return '<div class="participant-item">'
        + '<img class="participant-avatar" src="' + avatar + '" alt="">'
        + '<div class="participant-info">'
        + '<div class="participant-name">' + escapeHtml(p.fullName) + '</div>'
        + '<div class="participant-progress">' + p.completedLessons + '/' + p.totalLesson + ' b\u00e0i \u00b7 ' + p.progressPct + '%</div>'
        + '</div>'
        + '<span class="participant-status ' + statusClass + '">' + escapeHtml(p.statusText) + '</span>'
        + '</div>';
}

// ===== REALTIME COMMENTS SIGNALR =====

function initCommentHub() {
    if (!globalHubConnection) {
        console.warn("Global SignalR connection is not initialized.");
        return;
    }

    commentHubConnection = globalHubConnection;

    commentHubConnection.on("ReceiveNewComment", function (dto) {
        if (!currentLessonId || dto.lessonID !== currentLessonId) return;
        if ($('#comment-' + dto.id).length > 0) return;

        // Recalculate canEdit and canDelete for the current client
        var itemEmployeeId = dto.employeeID || dto.employeeId || 0;
        dto.canEdit = !dto.isDeleted && (itemEmployeeId === currentEmployeeId && (new Date() - new Date(dto.createdDate)) / 60000 <= 15);
        dto.canDelete = !dto.isDeleted && (itemEmployeeId === currentEmployeeId || isAdmin);

        var parentId = dto.parentID !== undefined ? dto.parentID : dto.parentId;

        if (parentId === null || parentId === undefined) {
            // Root comment
            $('#comment-list').prepend(renderComment(dto, 0));
            commentTotal++;
            $('#comment-badge').text(commentTotal);
        } else {
            // Reply
            var container = $('#replies-' + parentId);
            var parentCommentItem = $('#comment-' + parentId);

            if (container.length > 0) {
                if (container.hasClass('open')) {
                    reloadRepliesSilently(parentId);
                } else {
                    var repliesBtn = parentCommentItem.find('.btn-show-replies');
                    if (repliesBtn.length > 0) {
                        var count = parseInt(repliesBtn.data('count')) || 0;
                        count++;
                        repliesBtn.data('count', count);
                        repliesBtn.html('<i class="bx bx-chevron-down"></i> ' + count + ' trả lời');
                    } else {
                        var nextLevel = parentCommentItem.hasClass('is-reply') ? 2 : 1;
                        var btnHtml = '<button class="btn-show-replies" data-count="1" onclick="toggleReplies(' + parentId + ', this, ' + nextLevel + ')"><i class="bx bx-chevron-down"></i> 1 trả lời</button>';
                        parentCommentItem.find('.comment-footer').append(btnHtml);
                    }
                }
            }
        }
    });

    commentHubConnection.on("ReceiveCommentReaction", function (data) {
        updateCommentReactionsUI(data.commentId, data.summary, data.details);
    });

    commentHubConnection.on("ReceiveEditedComment", function (commentId, content) {
        var $content = $('#comment-content-' + commentId);
        if ($content.length > 0) {
            $content.html(highlightMentions(escapeHtml(content))).show();
            $('#edit-form-' + commentId).remove();
            var $header = $('#comment-' + commentId + ' .comment-header');
            if (!$header.find('.comment-edited').length) {
                $header.append('<span class="comment-edited">(đã chỉnh sửa)</span>');
            }
        }
    });

    commentHubConnection.on("ReceiveDeletedComment", function (commentId, isHardDeleted) {
        var $item = $('#comment-' + commentId);
        if ($item.length > 0) {
            if (isHardDeleted) {
                $item.fadeOut(300, function () {
                    $(this).remove();
                });
            } else {
                $('#comment-content-' + commentId).html('<span class="comment-deleted">[Đã xóa]</span>');
                $item.find('.comment-image-attachment').remove();
                $item.find('.comment-file-attachment').remove();
                $item.find('.comment-reactions-summary-container').remove();
                $item.find('.comment-edited').remove();
                $item.find('.comment-footer').html('');
            }
        }
    });

    function joinLesson() {
        if (currentLessonId) {
            commentHubConnection.invoke("JoinLessonGroup", currentLessonId)
                .catch(function (err) { console.error("JoinLessonGroup error: ", err); });
        }
    }

    if (commentHubConnection.state === signalR.HubConnectionState.Connected) {
        joinLesson();
    } else {
        commentHubConnection.start().then(joinLesson).catch(function (err) {
            if (commentHubConnection.state === signalR.HubConnectionState.Connected) {
                joinLesson();
            } else {
                console.error("SignalR start error in lesson_index.js: ", err);
            }
        });
    }
}

function reloadRepliesSilently(parentId) {
    var $container = $('#replies-' + parentId);
    var $commentItem = $('#comment-' + parentId);
    var $btn = $commentItem.find('.btn-show-replies');
    var nextLevel = $commentItem.hasClass('is-reply') ? 2 : 1;

    $.get('/Lesson/GetReplies', { parentId: parentId }, function (res) {
        if (!res.success) return;
        var html = '';
        res.data.forEach(function (r, index) {
            var isReplyToAbove = false;
            var hasReplyDown = false;

            if (nextLevel >= 2) {
                if (index > 0) {
                    var prevReply = res.data[index - 1];
                    if (r.replyToName && r.replyToName === prevReply.fullName) {
                        isReplyToAbove = true;
                    }
                } else {
                    isReplyToAbove = true;
                }
            }

            if (index < res.data.length - 1) {
                var nextReply = res.data[index + 1];
                if (nextReply.replyToName && nextReply.replyToName === r.fullName) {
                    hasReplyDown = true;
                }
            }

            // Recalculate canEdit and canDelete client-side
            var itemEmployeeId = r.employeeID || r.employeeId || 0;
            r.canEdit = !r.isDeleted && (itemEmployeeId === currentEmployeeId && (new Date() - new Date(r.createdDate)) / 60000 <= 15);
            r.canDelete = !r.isDeleted && (itemEmployeeId === currentEmployeeId || isAdmin);

            html += renderComment(r, nextLevel, isReplyToAbove, hasReplyDown);
        });
        $container.html(html);
        if ($btn.length > 0) {
            $btn.data('count', res.data.length);
            $btn.html('<i class="bx bx-chevron-up"></i> Thu gọn');
        }

        // Tự động kiểm tra và chạy tiếp chuỗi định vị khi nhận phản hồi con AJAX xong
        if (window.commentTargetChain && window.commentTargetChain.includes(parentId.toString())) {
            processCommentTargetChain();
        }
    });
}

// ===== HELPER FUNCTIONS FOR ATTACHMENTS & REACTIONS =====
function triggerImageUpload() {
    $('#comment-image-input').click();
}

function triggerFileUpload() {
    $('#comment-file-input').click();
}

function handleImageSelected(input) {
    if (input.files && input.files[0]) {
        attachedImageFile = input.files[0];
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#image-preview').attr('src', e.target.result);
            $('#image-preview-wrapper').show();
            $('#comment-attachment-preview').show();
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function handleFileSelected(input) {
    if (input.files && input.files[0]) {
        attachedDocFile = input.files[0];
        $('#file-preview-name').text(attachedDocFile.name).attr('title', attachedDocFile.name);
        $('#file-preview-wrapper').show();
        $('#comment-attachment-preview').show();
    }
}

function removeAttachedImage() {
    attachedImageFile = null;
    $('#comment-image-input').val('');
    $('#image-preview-wrapper').hide();
    if (!$('#file-preview-wrapper').is(':visible')) {
        $('#comment-attachment-preview').hide();
    }
}

function removeAttachedFile() {
    attachedDocFile = null;
    $('#comment-file-input').val('');
    $('#file-preview-wrapper').hide();
    if (!$('#image-preview-wrapper').is(':visible')) {
        $('#comment-attachment-preview').hide();
    }
}

function clearAllAttachments() {
    removeAttachedImage();
    removeAttachedFile();
}

// Inline attachment triggers
function triggerInlineImageUpload(commentId) {
    $('#inline-image-input-' + commentId).click();
}

function triggerInlineFileUpload(commentId) {
    $('#inline-file-input-' + commentId).click();
}

function handleInlineImageSelected(input, commentId) {
    if (input.files && input.files[0]) {
        inlineAttachedImages[commentId] = input.files[0];
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#inline-image-preview-' + commentId).attr('src', e.target.result);
            $('#inline-image-preview-wrapper-' + commentId).show();
            $('#inline-attachment-preview-' + commentId).show();
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function handleInlineFileSelected(input, commentId) {
    if (input.files && input.files[0]) {
        inlineAttachedDocs[commentId] = input.files[0];
        $('#inline-file-preview-name-' + commentId).text(input.files[0].name).attr('title', input.files[0].name);
        $('#inline-file-preview-wrapper-' + commentId).show();
        $('#inline-attachment-preview-' + commentId).show();
    }
}

function removeInlineAttachedImage(commentId) {
    delete inlineAttachedImages[commentId];
    $('#inline-image-input-' + commentId).val('');
    $('#inline-image-preview-wrapper-' + commentId).hide();
    if (!$('#inline-file-preview-wrapper-' + commentId).is(':visible')) {
        $('#inline-attachment-preview-' + commentId).hide();
    }
}

function removeInlineAttachedFile(commentId) {
    delete inlineAttachedDocs[commentId];
    $('#inline-file-input-' + commentId).val('');
    $('#inline-file-preview-wrapper-' + commentId).hide();
    if (!$('#inline-image-preview-wrapper-' + commentId).is(':visible')) {
        $('#inline-attachment-preview-' + commentId).hide();
    }
}

// Reactions functions
function toggleReaction(commentId, reactionType) {
    $.post('/Lesson/ToggleCommentReaction', { commentId: commentId, reactionType: reactionType }, function (res) {
        if (!res.success) {
            console.error(res.message);
        }
    });
}

function updateCommentReactionsUI(commentId, summary, details) {
    var $commentBody = $('#comment-' + commentId + ' > .comment-main .comment-body');
    if ($commentBody.length === 0) return;

    var $summaryContainer = $commentBody.find('.comment-reactions-summary-container');
    if ($summaryContainer.length === 0) {
        $summaryContainer = $('<div class="comment-reactions-summary-container"></div>');
        $commentBody.find('.comment-content').after($summaryContainer);
    }

    $summaryContainer.empty();

    if (summary && summary.length > 0) {
        summary.forEach(function (sum) {
            var icon = getReactionEmoji(sum.reactionType);
            var shortCount = formatReactionCount(sum.count);

            var reactorNames = details
                .filter(d => d.reactionType === sum.reactionType)
                .map(d => d.fullName)
                .join(', ');

            var isUserReacted = details.some(d => d.reactionType === sum.reactionType && d.employeeId === currentEmployeeId);
            var activeClass = isUserReacted ? 'active' : '';

            var pill = '<div class="reaction-summary-pill ' + activeClass + '" onclick="event.stopPropagation(); toggleReaction(' + commentId + ', \'' + sum.reactionType + '\')">'
                + '<span>' + icon + ' ' + shortCount + '</span>'
                + '<span class="tooltip-text">' + escapeHtml(reactorNames) + '</span>'
                + '</div>';

            $summaryContainer.append(pill);
        });
    }

    var userReaction = details.find(d => d.employeeId === currentEmployeeId)?.reactionType;
    var $reactionBtn = $('#comment-' + commentId + ' .comment-reaction-btn');
    if ($reactionBtn.length > 0) {
        $reactionBtn.removeClass('has-reacted like love haha wow sad angry');
        if (userReaction) {
            $reactionBtn.addClass('has-reacted ' + userReaction);
            $reactionBtn.find('.reaction-btn-text').html(getReactionEmoji(userReaction) + ' ' + getReactionLabel(userReaction));
        } else {
            $reactionBtn.find('.reaction-btn-text').html('<i class="bx bx-like"></i> Thích');
        }
    }
}

function getReactionEmoji(type) {
    switch (type) {
        case 'like': return '👍';
        case 'love': return '❤️';
        case 'haha': return '😆';
        case 'wow': return '😮';
        case 'sad': return '😢';
        case 'angry': return '😡';
        default: return '👍';
    }
}

function getReactionLabel(type) {
    switch (type) {
        case 'like': return 'Thích';
        case 'love': return 'Yêu thích';
        case 'haha': return 'Haha';
        case 'wow': return 'Wow';
        case 'sad': return 'Buồn';
        case 'angry': return 'Phẫn nộ';
        default: return 'Thích';
    }
}

function formatReactionCount(count) {
    if (count >= 1000000) {
        var val = (count / 1000000).toFixed(1);
        return val.endsWith('.0') ? val.slice(0, -2) + 'M' : val + 'M';
    }
    if (count >= 1000) {
        var val = (count / 1000).toFixed(1);
        return val.endsWith('.0') ? val.slice(0, -2) + 'K' : val + 'K';
    }
    return count;
}

// Lightbox functions
function openLightbox(src) {
    $('#lightbox-img').attr('src', src);
    $('#image-lightbox').fadeIn(250).css('display', 'flex');
}

function closeLightbox() {
    $('#image-lightbox').fadeOut(200);
}
function scrollAndHighlight($el) {
    setTimeout(function () {

        $el[0].scrollIntoView({
            behavior: 'smooth',
            block: 'center'
        });

        $el.addClass('highlight-comment');

        setTimeout(function () {
            $el.removeClass('highlight-comment');
        }, 5000);

    }, 200);
}
// function scrollAndHighlight($el) {
//     // Đợi 200ms để DOM layout ổn định hoàn toàn trước khi đo vị trí
//     setTimeout(function () {
//         var windowHeight = $(window).height();
//         var elHeight = $el.outerHeight();
//         var offsetTop = $el.offset().top;

//         // Tính toán cuộn để đưa phần tử vào giữa màn hình
//         var targetScroll = offsetTop - (windowHeight / 2) + (elHeight / 2);
//         if (targetScroll < 0) targetScroll = 0;

//         $('html, body').animate({
//             scrollTop: targetScroll
//         }, 600);

//         $el.addClass('highlight-comment');
//         setTimeout(function () {
//             $el.removeClass('highlight-comment');
//         }, 5000);
//     }, 200);
// }

function processCommentTargetChain() {
    if (!window.commentTargetChain || window.commentTargetChain.length === 0) return;

    var commentTabBtn = document.querySelector('.lesson-tab[data-target="tab-comment"]');
    if (commentTabBtn && !commentTabBtn.classList.contains('active')) {
        commentTabBtn.click();
    }

    setTimeout(function () {
        for (var i = 0; i < window.commentTargetChain.length; i++) {
            var currentId = window.commentTargetChain[i];
            var $currentEl = $('#comment-' + currentId);

            if ($currentEl.length === 0) {
                // Phần tử hiện tại chưa render vào DOM, dừng lại đợi AJAX load
                break;
            }

            var isFinalTarget = (i === window.commentTargetChain.length - 1);
            if (isFinalTarget) {
                // Đã tìm thấy phần tử đích cuối cùng, cuộn và highlight
                scrollAndHighlight($currentEl);
                window.commentTargetChain = null; // Hoàn thành
                break;
            } else {
                // Nút cha ở giữa chuỗi, kiểm tra xem nút con tiếp theo đã render chưa
                var nextId = window.commentTargetChain[i + 1];
                var $nextEl = $('#comment-' + nextId);
                if ($nextEl.length > 0) {
                    // Đã có con tiếp theo trong DOM, duyệt tiếp
                    continue;
                } else {
                    // Chưa có con tiếp theo, kích hoạt click mở rộng replies của cha
                    var $container = $('#replies-' + currentId);
                    if ($container.length > 0 && !$container.hasClass('open')) {
                        var $repliesBtn = $currentEl.find('.btn-show-replies');
                        if ($repliesBtn.length > 0) {
                            $repliesBtn.click();
                        }
                    }
                    break;
                }
            }
        }
    }, 150);
}

function scrollToAndHighlightComment() {
    var hash = window.location.hash;
    if (hash && hash.startsWith('#comment-')) {
        var parts = hash.replace('#comment-', '').split('-');
        window.commentTargetChain = parts;
        processCommentTargetChain();
    }
}










