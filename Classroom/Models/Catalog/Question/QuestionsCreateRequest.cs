using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom.Models.Catalog.Question;

public class QuestionsCreateRequest
{
    [Display(Name = "Mã kì thi")]
    public int ExamScheduleID { set; get; }

    [Display(Name = "Nội dung câu hỏi")]
    public string? QuestionString { set; get; }

    [Display(Name = "Điểm số")]
    public float Point { set; get; }
    [Display(Name = "Đáp án 1")]
    public string? Option1 { get; set; }
    [Display(Name = "Đáp án 2")]
    public string? Option2 { get; set; }
    [Display(Name = "Đáp án 3")]
    public string? Option3 { get; set; }
    [Display(Name = "Đáp án 4")]
    public string? Option4 { get; set; }
    [Display(Name = "Đáp án đúng")]
    public int OptionCorrect { get; set; }
}