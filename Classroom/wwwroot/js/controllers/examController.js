var dataExam;
var homeConfig = {
    examScheduleID: $('#ExamScheduleID').val()
}
var timerMinutes = $('#Timer').val();
var timer2 = timerMinutes + ":00";
var interval = setInterval(function () {
    var timer = timer2.split(':');
    //by parsing integer, I avoid all extra string processing
    var minutes = parseInt(timer[0], 10);
    var seconds = parseInt(timer[1], 10);
    --seconds;
    minutes = (seconds < 0) ? --minutes : minutes;
    if (minutes < 0) clearInterval(interval);
    seconds = (seconds < 0) ? 59 : seconds;
    seconds = (seconds < 10) ? '0' + seconds : seconds;
    $('.countdown').html(minutes + ':' + seconds);
    timer2 = minutes + ':' + seconds;
    const toastLiveExample1 = document.getElementById('liveToast1')
    const toastLiveExample5 = document.getElementById('liveToast5')
    if (minutes == 5 && seconds == 0) {
        $('#cardBaiThi').addClass('border-warning');
        const toast = new bootstrap.Toast(toastLiveExample5)
        toast.show()
    }
    if (minutes == 1 && seconds == 00) {
        $('#cardBaiThi').removeClass('border-warning');
        $('#cardBaiThi').addClass('border-danger');
        const toast = new bootstrap.Toast(toastLiveExample1)
        toast.show()
    }
    if (minutes == 0 && seconds == 0) {
        alert('hết giờ');
        $('.countdown').html('hết giờ');
        questionController.nopBai();
    }
}, 20);

var questionController = {
    init: function () {
        questionController.loadData();
        questionController.registerEvent();
    },
    registerEvent: function () {
        $('#btnNop').off('click').on('click', function () {
            bootbox.confirm({
                backdrop: true,
                closeButton: false,
                message: 'Bạn chắc chắn muốn nộp bài?',
                callback: function (result) {
                    if (result == true) {
                        questionController.nopBai();
                    }
                }
            });
        });
        $('input[type=radio]').change(function () {
            $('.btn' + this.name).removeClass("btn-outline-primary");
            $('.btn' + this.name).addClass("btn-primary");
        });
    },
    nopBai: function () {
        var result = document.getElementsByTagName('input');
        var mark = 0;
        var markMax = 0;
        for (i = 0; i < result.length; i++) {
            if (result[i].type == "radio") {
                if (result[i].checked) {
                    var kq;
                    const choose = result[i].name;
                    $.each(dataExam, function (i, item) {
                        if (item.questionID == choose) {
                            kq = item;
                        }
                    });
                    if (kq.optionCorrect == result[i].id) {
                        mark += kq.point;
                    }
                }
            }
        }

        $.each(dataExam, function (i, item) {
            markMax += item.point;
        });
        bootbox.dialog({
            title: 'Chúc mừng bạn đã hoàn thành bài làm của mình',
            message: 'Số điểm của bạn: ' + mark + '/' + markMax,
            backdrop: true,
            closeButton: false,
        });

        setTimeout(function () {
            $.ajax({
                url: '/Question/SaveResult',
                data: {
                    mark: mark
                },
                type: 'POST',
                dataType: 'json',
                success: function (response) {
                    if (response.status) {
                        window.location = '/ky-thi?id=' + response.data;
                    }
                }
            })
        }, 5000);
    },
    loadData: function () {
        homeConfig.valueSelected = $('#selectList').val();
        $.ajax({
            url: '/Question/LoadExam',
            data: {
                id: homeConfig.examScheduleID,
            },
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    var data = response.data;
                    var html = '';
                    var htmlQ = '';
                    var template = $('#data-template').html();
                    var templateQ = $('#data-template-listQ').html();
                    dataExam = data;
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
                            STT: i + 1
                        });
                        htmlQ += Mustache.render(templateQ, {
                            QuestionID: item.questionID,
                            STT: i + 1
                        });
                    });
                    $('#tbExamData').html(html);
                    $('#frmListQ').html(htmlQ);
                    questionController.registerEvent();
                }
            }
        })
    }
}
questionController.init();