var examquestions = [];
var indexQuestion = 0;
var isSubmitSuccess = false;
var resultExamId = 0;
var listIdQuestion = [];

$(document).ready(function () {
    GetHistoryPractice();
    GetQuestion();
})

function GetHistoryPractice() {
    $.ajax({
        url: "/CourseExamPractice/GetHistoryResultPractice",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamId: courseExamId },
        success: function (response) {
            var html = '';
            if (response && response.length > 0) {
                $.each(response, function (i, item) {
                    var dateStart = moment(item.createdDate).isValid() ? moment(item.createdDate).format("DD/MM/YYYY HH:mm") : '';
                    var statusClass = '';
                    if (item.statusText === "Đạt") {
                        statusClass = 'status-passed';
                    } else if (item.statusText === "Không đạt") {
                        statusClass = 'status-failed';
                    } else {
                        statusClass = 'status-pending';
                    }
                    var resultId = item.id !== undefined ? item.id : (item.ID !== undefined ? item.ID : item.Id);
                    html += `<tr tabindex='0' onclick="GetResultPractice(this, ${resultId})">
                                <td class="text-center">${dateStart}</td>
                                <td class="text-center">${item.goal}</td>
                                <td class="text-center fw-bold">${item.practicePoints}</td>
                                <td class="text-center">
                                    <span class="status-badge ${statusClass}">${item.statusText}</span>
                                </td>
                            </tr>`;
                });
            } else {
                html = `<tr>
                            <td colspan="4" class="text-center text-muted py-4">Chưa có lịch sử làm bài tập</td>
                        </tr>`;
            }
            $('#tbody_history_result').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function GetResultPractice(rowElement, id) {
    // Toggle class selected-row cho dòng lịch sử
    $('#tbody_history_result tr').removeClass('selected-row');
    $(rowElement).addClass('selected-row');

    $.ajax({
        url: "/CourseExamPractice/GetResultPractice",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseResultId: id },
        success: function (response) {
            console.log(response)
            var html = '';
            if (response && response.length > 0) {
                $.each(response, function (i, item) {
                    html += `<tr>
                                <td class="text-center fw-bold">${item.stt}</td>
                                <td class="text-justify">${item.questionText}</td>
                                <td class="text-center fw-bold text-primary">${item.point}</td>
                                <td>${item.note || ''}</td>
                            </tr>`;
                });
                // Hiện bảng kết quả, ẩn empty state
                $('#detail_empty_state').hide();
                $('#detail_table_container').show();
            } else {
                html = `<tr>
                            <td colspan="4" class="text-center text-muted py-4">Không có dữ liệu kết quả chi tiết</td>
                        </tr>`;
                $('#detail_empty_state').hide();
                $('#detail_table_container').show();
            }
            $('#tbody_practice_result').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function GetQuestion() {
    $.ajax({
        url: "/CourseExamPractice/GetQuesstion",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamId: courseExamId },
        success: function (response) {
            var html = '';
            listIdQuestion = [];
            $.each(response, function (i, item) {
                listIdQuestion.push(item.id);
                html += `
                <div class="col">
                    <div class="question-practice-card card shadow-sm h-100">
                        <div class="card-body p-3">
                            <span class="badge bg-primary mb-2">Câu hỏi ${item.stt}</span>
                            <p class="text-dark fw-semibold mb-3 text-justify text-line-clamp-3" title="${item.questionText}" style="white-space: pre-line;">${item.questionText}</p>
                        </div>
                        <div class="question-card-footer text-end">
                            <a href="/CourseExamResult/QuestionDetails?questionId=${item.id}&courseId=${courseId}" 
                               target="_blank" class="btn btn-outline-primary btn-sm px-3 rounded-pill fw-bold">
                                <i class='bx bx-play-circle me-1'></i> Thực hành ngay
                            </a>
                        </div>
                    </div>
                </div>`;
            });
            $('#practice_question_grid').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function DoExam() {
    $("#time-exam").html('');
    $("#modal_exam_practice_test").modal('show');
}

function onSubmit() {
    Swal.fire({
        title: 'Bạn có chắc muốn nộp bài không?',
        showCancelButton: true,
        confirmButtonText: 'OK',
        overlay: true,
    }).then((result) => {
        if (result.isConfirmed) {
            $("#modal_exam_practice_test").modal('hide');
            onSave();
            GetHistoryPractice();
        }
    })
}

function onSave() {
    let obj = {
        Id: 0,
        CourseExamId: parseInt(courseExamId),
        Note: ""
    };
    $.ajax({
        url: "/CourseExamPractice/CreatePracticeResult",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(obj),
        success: function (result) {
            var dateCreate = moment(result.createdDate).isValid() ? moment(result.createdDate).format("DD/MM/YYYY HH:mm") : '';
            if (dateCreate !== '') {
                $("#time-exam").html(`<i class="bx bx-calendar me-1"></i> Ngày bắt đầu: ${dateCreate}`);
            } else {
                $("#time-exam").html('');
            }
            resultExamId = result.id;
            let objExamResult = {
                Id: parseInt(resultExamId),
                CourseExamId: parseInt(courseExamId),
                Status: 1,
                Note: ""
            };
            $.ajax({
                url: "/CourseExamPractice/ConfirmPractice",
                type: "POST",
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(objExamResult),
                success: function (response) {
                    if (response == null) {
                        alert("Không thể lưu bài tập!");
                    }
                    else {
                        let arrData = [];
                        listIdQuestion.forEach(e => {
                            let obj = {
                                Id: 0,
                                CourseExamResultId: parseInt(resultExamId),
                                CourseQuestionId: parseInt(e),
                                Point: 0,
                                Note: ""
                            };

                            arrData.push(obj);
                        })

                        console.log(arrData);
                        $.ajax({
                            url: "/CourseExamPractice/CreateListExamValuate",
                            type: "POST",
                            dataType: 'json',
                            contentType: 'application/json',
                            data: JSON.stringify(arrData),
                            success: function (result) {
                                // Tải lại lịch sử sau khi hoàn thành tạo dữ liệu chi tiết
                                GetHistoryPractice();
                            },
                            error: function (error) {
                                alert(error.responseText);
                            }
                        });
                    }
                },
                error: function (error) {
                    alert(error.responseText);
                }
            });
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}
