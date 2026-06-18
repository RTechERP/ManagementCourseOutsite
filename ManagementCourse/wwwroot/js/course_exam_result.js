var examquestions = [];
var courseExamResultID = 0;
var indexQuestion = 0;
var isSubmitSuccess = false;
var isDetailMode = false;
var imageAjaxRequest = null;
$(document).ready(function () {
    GetExamResult(courseExamId);
})

// Phím mũi tên ← → điều hướng câu hỏi khi modal đang mở
$(document).on('keydown', function (e) {
    if (!$('#modal_exam_test').hasClass('show')) return; // modal chưa mở
    var tag = (document.activeElement.tagName || '').toLowerCase();
    if (tag === 'input' || tag === 'textarea') return;   // đang focus vào ô nhập

    if (e.key === 'ArrowLeft')  { e.preventDefault(); onPrevios(); }
    if (e.key === 'ArrowRight') { e.preventDefault(); onNext(); }
});

//Get danh sách kết quả
function GetExamResult(courseExamID) {
    $.ajax({
        url: "/CourseExamResult/GetExamResult",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamID: courseExamID },
        success: function (response) {

            // ===== TÍNH THỐNG KÊ =====
            var total  = response.length;
            var passed = response.filter(function (x) { return x.Status === 1; }).length;
            var avgPct = total > 0
                ? Math.round(response.reduce(function (s, x) { return s + x.PercentageCorrect; }, 0) / total)
                : 0;
            var best = total > 0
                ? Math.round(Math.max.apply(null, response.map(function (x) { return x.PercentageCorrect; })))
                : 0;

            $('#stat_total').text(total);
            $('#stat_passed').text(passed);
            $('#stat_avg').text(avgPct + '%');
            $('#stat_best').text(best + '%');

            // ===== RENDER BẢNG =====
            if (total === 0) {
                $('#tbody_exam_result').html(
                    '<tr><td colspan="7" class="text-center text-muted py-4">Chưa có lần thi nào.</td></tr>'
                );
                return;
            }

            var html = '';
            $.each(response, function (i, item) {
                var statusBadge = item.Status === 1
                    ? '<span class="status-badge passed">&#9679; Đạt</span>'
                    : '<span class="status-badge failed">&#9679; Chưa đạt</span>';

                var finishTime = moment(item.UpdatedDate).format('DD/MM/YYYY HH:mm');
                var pct  = Math.round(item.PercentageCorrect);
                var goal = Math.round(item.Goal);

                html += `<tr>
                    <td>${item.NameCourse}</td>
                    <td>${item.NameExam}</td>
                    <td>${item.TotalCorrect} / ${item.TotalQuestion}</td>
                    <td>${finishTime}</td>
                    <td class="score-cell">
                        <span class="score-current">${pct}%</span>
                        <span class="score-goal"> / ${goal}%</span>
                    </td>
                    <td>${statusBadge}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-secondary"
                                onclick="onDetail(${item.ID})">Chi tiết</button>
                    </td>
                </tr>`;
            });

            $('#tbody_exam_result').html(html);
        },
        error: function (error) {
            $('#tbody_exam_result').html(
                '<tr><td colspan="7" class="text-center text-danger py-4">Lỗi tải dữ liệu!</td></tr>'
            );
            alert(error.responseText);
        }
    });
}

// Chế độ xem chi tiết — không có timer, không có nộp bài
function onDetail(examResultId) {
    isDetailMode = true;
    isSubmitSuccess = true;         // khóa chỉnh sửa đáp án
    courseExamResultID = examResultId;
    $('#modal_exam_test').addClass('detail-mode');
    $('#modal_exam_test').modal('show');
    GetExamQuestion(courseId, examResultId);
    // Gọi GetQuestionAnswerRight sau khi câu hỏi đã load xong (via callback trong GetExamQuestion)
}



function GetQuestionAnswerRight() {
    $.ajax({
        url: "/CourseExamResult/GetQuestionAnswerRight",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamResultId: courseExamResultID },
        success: function (response) {
            $('span.question-number').each(function () {
                var id = $(this).attr('id');
                var check = response.find(p => p.CourseQuestionId == id);
                if (!check) {
                    $(this).addClass('bg-danger');
                    $(this).addClass('text-white');
                }
            });
            clearInterval(time);
            $('.form-check-input').prop('disabled', true);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}


//Sự kiện khi click làm bài thi
function onStart(status, examResultId) {
    // Reset về chế độ làm bài
    isDetailMode = false;
    isSubmitSuccess = false;
    $('#modal_exam_test').removeClass('detail-mode');

    var obj = {
        CourseExamId: parseInt(courseExamId)
    }

    if (status == 1) {
        $.ajax({
            url: "/CourseExamResult/CreateExamResult",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(obj),
            success: function (response) {
                if (parseInt(response) > 0) {
                    courseExamResultID = parseInt(response);
                    $('#modal_exam_test').modal('show');
                    GetExamQuestion(courseId, parseInt(response));
                    TimeCountDown();
                } else {
                    alert("Xảy ra lỗi khi tạo bài thi!");
                }
            },
            error: function (error) {
                alert(error);
            }
        });
    } else {
        courseExamResultID = examResultId;
        $('#modal_exam_test').modal('show');
        GetExamQuestion(courseId, examResultId);
    }
}

//Get danh sách câu hỏi
function GetExamQuestion(courseId, courseExamResultID) {
    $.ajax({
        url: "/CourseExamResult/GetExamQuestion",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: {
            courseId: courseId,
            courseExamResultID: courseExamResultID
        },
        success: function (response) {
            // Detail mode: giữ thứ tự gốc, không xào trộn
            if (isDetailMode) {
                examquestions = response;
            } else {
                examquestions = response.sort(() => Math.random() - 0.5);
                $.each(examquestions, function (i, item) {
                    item.ExamAnswers.sort(() => Math.random() - 0.5);
                });
            }
            addNumberQuestion(examquestions);
            indexQuestion = 0;
            var item = examquestions[0];
            $(`span[id="${item.ID}"]`).addClass('current-question');
            addContentQuestion(item);

            // Detail mode: tô màu câu đúng/sai sau khi danh sách câu hỏi đã render
            if (isDetailMode) {
                GetQuestionAnswerRight();
            }
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

//Sự kiện khi click next
function onNext() {
    if (isDetailMode) {
        $('.current-question').removeClass('current-question');
        if (examquestions.length > 0 && indexQuestion < examquestions.length - 1) {
            indexQuestion++;
        }
        var item = examquestions[indexQuestion];
        addContentQuestion(item);
        $(`span[id="${item.ID}"]`).addClass('current-question');
    } else {
        AddResultDetail(false, true, false, false);
    }
}

//sự kiện khi click prev
function onPrevios() {
    if (isDetailMode) {
        $('.current-question').removeClass('current-question');
        if (examquestions.length > 0 && indexQuestion > 0) {
            indexQuestion--;
        }
        var item = examquestions[indexQuestion];
        addContentQuestion(item);
        $(`span[id="${item.ID}"]`).addClass('current-question');
    } else {
        AddResultDetail(false, false, false, true);
    }
}

//Sự kiện khi click chọn câu hỏi — tự lưu đáp án hiện tại rồi chuyển
function onChosenQuestion(event, id) {
    var targetIdx = examquestions.findIndex(x => x.ID == id);
    if (targetIdx === indexQuestion) return; // đang ở câu này rồi

    if (isDetailMode) {
        indexQuestion = targetIdx;
        $('.current-question').removeClass('current-question');
        var item = examquestions[indexQuestion];
        addContentQuestion(item);
        $(`span[id="${item.ID}"]`).addClass('current-question');
        return;
    }

    // Thu thập đáp án đang được chọn
    var answers = [];
    $('.form-check-input').each(function () {
        if ($(this).is(':checked')) {
            answers.push({
                AnswerText: $(`label[for="${$(this).attr('id')}"]`).html(),
                CourseQuestionId: parseInt($(this).attr('name')),
                CourseAnswerId: parseInt($(this).attr('id')),
                CourseExamResultId: courseExamResultID
            });
        }
    });

    // Di chuyển UI đến câu targetIdx
    function goTo(idx) {
        indexQuestion = idx;
        $('.current-question').removeClass('current-question');
        var item = examquestions[indexQuestion];
        addContentQuestion(item);
        $(`span[id="${item.ID}"]`).addClass('current-question');
    }

    // Không có đáp án nào → chuyển ngay, không gọi API
    if (answers.length === 0) {
        goTo(targetIdx);
        return;
    }

    // Có đáp án → lưu rồi chuyển
    $.ajax({
        url: "/CourseExamResult/CreateExamResultDetail",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(answers),
        success: function (response) {
            // Cập nhật local data
            var item = examquestions.find(x => x.ID == response.Item1);
            var index = examquestions.findIndex(x => x.ID == response.Item1);
            if (index >= 0) {
                $.each(examquestions[index].ExamAnswers, function (key, data) {
                    data.CourseAnswerChosenID = 0;
                });
            }
            if (item != null) {
                item.QuestionChosenID = response.Item1;
                $.each(response.Item2, function (key, data) {
                    var answer = item.ExamAnswers.find(x => x.ID == data);
                    answer.CourseAnswerChosenID = data;
                });
                addNumberQuestion(examquestions); // cập nhật tick ✓ trên danh sách số câu
            }
            $('.form-check-input').prop('checked', false);
            goTo(targetIdx);
        },
        error: function (error) { alert(error); }
    });
}

//Get đáp án
function AddResultDetail(isSubmit, isNext, isSave, isPrev) {
    var answers = [];
    $('.form-check-input').each(function () {

        var checked = $(this).is(':checked');
        if (checked) {
            var obj = {
                AnswerText: $(`label[for="${$(this).attr('id')}"]`).html(),
                CourseQuestionId: parseInt($(this).attr('name')),
                CourseAnswerId: parseInt($(this).attr('id')),
                CourseExamResultId: courseExamResultID
            }
            answers.push(obj);

        }
    })
    $.ajax({
        url: "/CourseExamResult/CreateExamResultDetail",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(answers),
        success: function (response) {

            var item = examquestions.find(x => x.ID == response.Item1);

            var index = examquestions.findIndex(x => x.ID == response.Item1);
            if (index >= 0) {
                $.each(examquestions[index].ExamAnswers, function (key, data) {
                    data.CourseAnswerChosenID = 0;
                })
            }

            if (item != null) {
                item.QuestionChosenID = response.Item1;
                $.each(response.Item2, function (key, data) {
                    var answer = item.ExamAnswers.find(x => x.ID == data);
                    answer.CourseAnswerChosenID = data;
                })
                addNumberQuestion(examquestions);
            }

            $('.form-check-input').prop('checked', false);

            if (isSave) {
                $('.current-question').removeClass('current-question');
                var item = examquestions[indexQuestion];
                addContentQuestion(item);
                $(`span[id="${item.ID}"]`).addClass('current-question');
            }

            if (isNext) {
                $('.current-question').removeClass('current-question');
                if (examquestions.length > 0 && indexQuestion < examquestions.length - 1) {
                    indexQuestion++;
                }
                var item = examquestions[indexQuestion];
                addContentQuestion(item);
                $(`span[id="${item.ID}"]`).addClass('current-question');
            }

            if (isPrev) {
                $('.current-question').removeClass('current-question');
                if (examquestions.length > 0 && indexQuestion > 0) {
                    indexQuestion--;
                }
                var item = examquestions[indexQuestion];
                addContentQuestion(item);
                $(`span[id="${item.ID}"]`).addClass('current-question');
            }

            if (isSubmit) {
                isSubmitSuccess = true;
                GetQuestionAnswerRight();
                GetExamResult(courseExamId);
            }
        },
        error: function (error) {
            alert(error);
        }
    });
}

//Show câu hỏi lên giao diện
function addContentQuestion(item) {
    $("#current_question").html(indexQuestion + 1)

    //var inputname = item.ID;
    //var imageContainer = item.Image == '' ? '' : `<div class="d-flex justify-content-start">
    //                                                <img src="http://113.190.234.64:8083/api/Upload/Images/Courses/${item.Image}" style="width:100%;height:100%" />
    //                                            </div>`;

    //var htmlContent = `<input type="hidden" id="course_exam_question_id_${item.ID}" value="${item.ID}" />
    //                    <div class="card-body p-0 mb-3">
    //                        <div class="w-100">
    //                            <p class="text-dark fs-5 fw-bold">
    //                                ${item.QuestionText}
    //                            </p>
    //                        </div>
    //                        ${imageContainer}
    //                    </div>`;
    //var htmlAnswer = '';
    //$.each(item.ExamAnswers, function (i, item) {
    //    var checked = item.ID == item.CourseAnswerChosenID ? 'checked' : '';
    //    var disabled = isSubmitSuccess ? 'disabled' : '';
    //    var borderClass = i == 0 ? '' : 'border-top';

    //    htmlAnswer += `<div class="form-check form-check-exam-answer py-3 m-0 ${borderClass}">
    //                        <input class="form-check-input rounded-0" type="checkbox" name="${inputname}" id="${item.ID}" ${checked} ${disabled}>
    //                        <label class="form-check-label text-dark" for="${item.ID}">
    //                            ${item.AnswerText}
    //                        </label>
    //                    </div>`;

    //})

    //var html = htmlContent;
    //$('#exam_question_content').html(html);
    //$('#exam_content_answer').html(htmlAnswer);




    var inputname = item.ID;
    //var htmlImage = item.Image == '' ? '' : ` <img src="http://113.190.234.64:8083/api/Upload/Images/Courses/${item.Image}" style="width:100%;height:100%" />`;


    //var htmlImage = item.Image == '' ? '' : ` <img src="http://113.190.234.64:8083/api/Upload/Images/Courses/${item.Image}" style="width:85%;" />`;
    var colQuestion = item.Image == '' ? 'col-12' : `col-sm-6 col-12`;
    var htmlContent = `<div class="card-body p-0">
                            <input type="hidden" id="course_exam_question_id" value="0" />
                            <div class="card-body p-0">
                                <div class="w-100">
                                    <p class="text-dark fs-5 fw-bold" style="text-align:justify;">
                                        ${item.QuestionText}
                                    </p>
                                </div>
                            </div>
                        </div>`;
    var htmlAnswer = '';
    $.each(item.ExamAnswers, function (i, item) {
        var checked = item.ID == item.CourseAnswerChosenID ? 'checked' : '';
        var disabled = isSubmitSuccess ? 'disabled' : '';
        var borderClass = i == 0 ? '' : 'border-top';

        htmlAnswer += `<div class="form-check form-check-exam-answer py-3 m-0 ${borderClass}">
                            <input class="form-check-input rounded-0" type="checkbox" name="${inputname}" id="${item.ID}" ${checked} ${disabled}>
                            <label class="form-check-label text-dark" for="${item.ID}">
                                ${item.AnswerText}
                            </label>
                        </div>`;

    })
    $('#container_question_content').removeClass();
    $('#container_question_content').addClass(`${colQuestion}`);
    var html = htmlContent;
    $('#exam_question_content').html(html);
    $('#exam_content_answer').html(htmlAnswer);

    // Hủy AJAX request tải ảnh trước đó nếu vẫn đang chạy để tránh race condition
    if (imageAjaxRequest) {
        imageAjaxRequest.abort();
        imageAjaxRequest = null;
    }

    // Xóa sạch nội dung container hình ảnh cũ ngay lập tức (đồng bộ)
    $('#image_question_container').html('');

    // Chỉ gọi AJAX tải ảnh nếu câu hỏi thực sự có ảnh đính kèm
    if (item.Image && item.Image !== '') {
        imageAjaxRequest = $.ajax({
            url: '/CourseExamResult/getUrlImageByKey',
            type: 'GET',
            dataType: 'text',
            data: {
                key: "CourseExamExerciseImages",
                imageName: item.Image
            },
            success: function (res) {
                var stringUrl = res;
                var htmlImage = '';
                if (stringUrl !== '') {
                    htmlImage = `<a href="${stringUrl}" target="_blank" title="Click để xem ảnh gốc trong tab mới">
                        <img src="${stringUrl}" class="exam-question-image" alt="Hình minh họa câu hỏi" />
                    </a>`;
                }
                $('#image_question_container').html(htmlImage);
            },
            error: function (xhr, status, error) {
                // Không log lỗi nếu request bị chủ động hủy bởi hàm abort()
                if (status !== 'abort') {
                    console.error('Lỗi khi tải ảnh:', error);
                }
            }
        });
    }


}

//Add number question
function addNumberQuestion(data) {
    // Chế độ làm bài: đã nộp bài rồi thì không cần render lại
    // Chế độ xem chi tiết: luôn render để hiển thị danh sách câu hỏi
    if (isSubmitSuccess && !isDetailMode) {
        return;
    }
    var html = '';
    $.each(data, function (key, item) {
        var classSuccess = item.QuestionChosenID == item.ID ? 'question-success' : '';
        html += `<span class="question-number m-1 border border-dark ${classSuccess}" id="${item.ID}"
                            onclick="return onChosenQuestion(event,${item.ID});">${key + 1}</span>`;
    })

    if (data.length > 0) {
        $("#total_question_exam").html(data.length);
    }

    $('#exam_question_navigation').html(html);

    $(".question-number").css({
        display: "inline-block",
        width: "30px",
        height: "30px",
        textAlign: "center",
        lineHeight: "30px",
        border: "1px solid #000",
        cursor: "pointer"
    });
}

//Sự kiện khi click nộp bài
function onSubmit() {

    Swal.fire({
        title: 'Bạn có chắc muốn nộp bài không?',
        showCancelButton: true,
        confirmButtonText: 'OK',
        overlay: true,
    }).then((result) => {
        if (result.isConfirmed) {
            AddResultDetail(true, false, false);
        }
    })

}

//Sự kiện khi click đóng
function onClose() {
    if (isDetailMode || isSubmitSuccess) {
        $('#modal_exam_test').modal('hide');
        $('#modal_exam_test').removeClass('detail-mode');
        isDetailMode = false;
        isSubmitSuccess = false;
    } else {
        onSubmit();
    }
}

var time = null;
//Sự kiện đếm ngược thời gian
function TimeCountDown() {
    testTime = parseInt(testTime);
    var remainingSeconds = testTime * 60;
    var step = 1000;
    time = setInterval(function () {
        if (testTime > 0) {
            remainingSeconds--;
        }
        document.getElementById("test_time").innerHTML = formatTime(remainingSeconds);
        if (remainingSeconds <= 0) {
            clearInterval(time);
            // Hết giờ → tự động nộp bài, không hiện dialog xác nhận
            AddResultDetail(true, false, false);
        }
    }, step);
}

//Sự kiện khi load lại trang
window.onbeforeunload = (event) => {
    //$('#myModal').is(':visible');
    //console.log('modal is ', $('#modal_exam_test').is(':visible'));
    event.preventDefault();
    AddResultDetail(true, false, false);
};

//Format thời gian để hiển thị lên giao diện
function formatTime(time) {
    var minutes = Math.floor(time / 60);
    var seconds = Math.floor(time % 60);

    //console.log(time, minutes, seconds);

    var formattedTime = minutes.toString().padStart(2, '0') + ':' + seconds.toString().padStart(2, '0');

    return formattedTime;
}