var dataQuestion;
var dataUpdate = [];
var homeConfig = {
    pageSize: 10,
    pageIndex: 1,
    valueSelected: 0
}
var statusAdd = 0;

var questionController = {
    init: function () {
        questionController.loadData();
        questionController.registerEvent();
    },
    registerEvent: function () {
        $('#frmSaveData').validate({
            rules: {
                txtQSAdd: {
                    required: true
                },
                txtOp1Add: {
                    required: true,
                },
                txtOp2Add: {
                    required: true,
                }
            },
            messages: {
                txtQSAdd: {
                    required: "Nội dung câu hỏi không được bổ trống"
                },
                txtOp1Add: {
                    required: "Đáp án 1 không được bỏ trống"
                },
                txtOp2Add: {
                    required: "Đáp án 2 không được bỏ trống"
                }
            }
        });
        $.each(dataQuestion, function (i, item) {
            $('.slOpC' + item.questionID).val(item.optionCorrect).change();
        });
        $('.btnSave').off('click').on('click', function () {
            $.each(dataQuestion, function (i, item) {
                var data = {
                    QuestionID: item.questionID,
                    QuestionString: $('.txtQS' + item.questionID).val(),
                    Point: $('.txtPoint' + item.questionID).val(),
                    Option1: $('.txtOp1' + item.questionID).val(),
                    Option2: $('.txtOp2' + item.questionID).val(),
                    Option3: $('.txtOp3' + item.questionID).val(),
                    Option4: $('.txtOp4' + item.questionID).val(),
                    OptionCorrect: $('.slOpC' + item.questionID).val()
                };
                dataUpdate.push(data)
            });
            questionController.updateAll(dataUpdate);
        });
        $('#clickAdd').off('click').on('click', function(){
            if(statusAdd==0){
                $('#frmAdd').removeClass("d-none");
                statusAdd = 1;
            }
            else{
                $('#frmAdd').addClass("d-none");
                statusAdd = 0;
            }
            
        })
        $('.btnSaveOne').off('click').on('click', function () {
            var id = $(this).data('id');
            var qS = $('.txtQS' + id).val();
            var point = $('.txtPoint' + id).val();
            var op1 = $('.txtOp1' + id).val();
            var op2 = $('.txtOp2' + id).val();
            var op3 = $('.txtOp3' + id).val();
            var op4 = $('.txtOp4' + id).val();
            var opC = $('.slOpC' + id).val();
            questionController.updatePoint(id, qS, point, op1, op2, op3, op4, opC);
        });
        $('.btnDelete').off('click').on('click', function () {
            var id = $(this).data('id');
            bootbox.dialog({
                message: 'Bạn có chắn chắn muốn xoá câu hỏi này?',
                backdrop: true,
                closeButton: false,
                onEscape: true,
                backdrop: true,
                buttons: {
                    fee: {
                        label: 'Xoá',
                        className: 'btn-danger',
                        callback: function (result) {
                            questionController.deleteQuestion(id);
                        }
                    }
                }

            });

        });
        $('#btnAddNew').off('click').on('click', function () {
            if ($('#frmSaveData').valid()) {
                questionController.saveData();
                questionController.resetForm();
            }
        });
        $('#selectList').off('change').on('change',function () {
            var optionSelected = $(this).find("option:selected");
            homeConfig.valueSelected = optionSelected.val();
            $('#pagination').empty();
            $('#pagination').removeData("twbs-pagination");
            $('#pagination').unbind("page");
            questionController.loadData(true);
        });
    },
    updatePoint: function (id, qS, point, op1, op2, op3, op4, opC) {
        var data = {
            QuestionID: id,
            QuestionString: qS,
            Point: point,
            Option1: op1,
            Option2: op2,
            Option3: op3,
            Option4: op4,
            OptionCorrect: opC
        };
        $.ajax({
            url: '/Question/UpdatePoint',
            type: 'POST',
            dataType: 'json',
            data: { model: JSON.stringify(data) },
            success: function (response) {
                if (response.status) {
                    bootbox.dialog({
                        message: 'Đã cập nhật thành công câu hỏi: ' + qS,
                        backdrop: true,
                        closeButton: false,
                    });
                }
                else {
                    bootbox.dialog({
                        message: response.message,
                        backdrop: true,
                        closeButton: false,
                    });
                }
            }
        })
    },
    deleteQuestion: function (id) {
        $.ajax({
            url: '/Question/DeleteQuestion',
            data: {
                id: id
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status == true) {
                    bootbox.dialog({
                        message: 'Xoá câu hỏi thành công',
                        backdrop: true,
                        closeButton: false,
                    });
                    $('#pagination').empty();
                    $('#pagination').removeData("twbs-pagination");
                    $('#pagination').unbind("page");
                    questionController.loadData(true);
                }
                else {
                    bootbox.alert(response.message);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    },
    updateAll: function (dataUpdate) {
        $.ajax({
            url: '/Question/UpdateAll',
            type: 'POST',
            dataType: 'json',
            data: { model: JSON.stringify(dataUpdate) },
            success: function (response) {
                if (response.status) {
                    bootbox.dialog({
                        message: 'Đã cập nhật thành công',
                        backdrop: true,
                        closeButton: false,
                    });
                }
                else {
                    bootbox.dialog({
                        message: response.message,
                        backdrop: true,
                        closeButton: false,
                    });
                }
            }
        })
    },
    loadData: function () {
        homeConfig.valueSelected = $('#selectList').val();
        $.ajax({
            url: '/Question/LoadData',
            data: {
                page: homeConfig.pageIndex,
                pageSize: homeConfig.pageSize,
                exId: homeConfig.valueSelected
            },
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    var data = response.data;
                    dataQuestion = data;
                    var html = '';
                    var template = $('#data-template').html();
                    $.each(data, function (i, item) {
                        html += Mustache.render(template, {
                            QuestionID: item.questionID,
                            ExamScheduleID: item.examScheduleID,
                            QuestionString: item.questionString,
                            Point: item.point,
                            Option1: item.option1,
                            Option2: item.option2,
                            Option3: item.option3,
                            Option4: item.option4,
                            OptionCorrect: item.optionCorrect,
                            STT: homeConfig.pageSize * homeConfig.pageIndex + i - homeConfig.pageSize + 1
                        });
                    });
                    $('#tbData').html(html);
                    questionController.paging(response.total, function () {
                        questionController.loadData();
                    });
                    questionController.registerEvent();
                }
            }
        })
    },
    resetForm: function () {
        $('#txtOp1Add').val('');
        $('#txtOp2Add').val('');
        $('#txtOp3Add').val('');
        $('#txtOp4Add').val('');
        $('#txtQSAdd').val('');
        $('#txtPointAdd').val(0);
        $('#slOpCAdd').val(1).change();
    },
    saveData: function () {
        var exId = $('#selectList').val();
        var QuestionsCreateRequest = {
            ExamScheduleID: exId,
            QuestionString: $('#txtQSAdd').val(),
            Point: parseFloat($('#txtPointAdd').val()),
            Option1: $('#txtOp1Add').val(),
            Option2: $('#txtOp2Add').val(),
            Option3: $('#txtOp3Add').val(),
            Option4: $('#txtOp4Add').val(),
            OptionCorrect: parseInt($('#slOpCAdd').val())
        }
        $.ajax({
            url: '/Question/SaveData',
            data: {
                strQuestionsCreateRequest: JSON.stringify(QuestionsCreateRequest)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status == true) {
                    bootbox.dialog({
                        message: 'Đã thêm câu hỏi thành công',
                        backdrop: true,
                        closeButton: false,
                    });
                    $('#pagination').empty();
                    $('#pagination').removeData("twbs-pagination");
                    $('#pagination').unbind("page");
                    questionController.loadData(true);
                }
                else {
                    bootbox.alert(response.message);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    },
    paging: function (totalRow, callback) {
        var totalPage = Math.ceil(totalRow / homeConfig.pageSize);
        $('#pagination').twbsPagination({
            totalPages: totalPage,
            first: "Đầu",
            next: "Tiếp",
            last: "Cuối",
            prev: "Trước",
            visiblePages: 10,
            onPageClick: function (event, page) {
                homeConfig.pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    }
}
questionController.init();