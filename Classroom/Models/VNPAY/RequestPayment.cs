using System.IO;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Classroom.Models.VNPAY
{
    /// <summary>
    /// RequestPayment
    /// </summary>
    /// <author>huynhdev24</author>
    public class RequestPayment
    {
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        public string? SoTien { get; set; }
        public string? NgonNgu { get; set; }
        public string? NganHang { get; set; }
        public string? LoaiHangHoa { get; set; }
        public string? ThoiHanThanhToan { get; set; }
    }
}
