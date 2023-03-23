using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom.Models.Catalog.StudentExams
{
    public class StudentExamsUpdateRequest
    {
        public int StudentExamID { set; get; }

        public int ExamScheduleID { set; get; }

        public float Mark { set; get; }

        public string? Note { set; get; }

        public DateTime DateTimeStudentExam { set; get; }
    }
}