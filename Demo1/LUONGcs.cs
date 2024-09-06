using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo1
{
    public class LUONGcs
    {
        public int SoNgayLam { get; set; } // Số ngày làm từ bảng CHAMCONG
        public int SoGioTangCa { get; set; } // Số giờ tăng ca từ bảng TANGCA
        public int TongTienTangCa { get; set; } // Tổng tiền tăng ca từ bảng TANGCA
        public int TongTienNhan { get; set; } // Tổng tiền nhận theo yêu cầu của bạn
        public int IDL { get; set; } // ID của LUONG
        public int LCB { get; set; } // Lương cơ bản
        public DateTime Thang { get; set; }

    }
}