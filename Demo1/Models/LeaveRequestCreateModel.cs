using System;
using System.ComponentModel.DataAnnotations;

namespace Demo1.Models
{
    public class LeaveRequestCreateModel
    {
        [Required(ErrorMessage = "Vui long chon loai nghi.")]
        [Display(Name = "Loai nghi")]
        public string LoaiNghi { get; set; }

        [Required(ErrorMessage = "Vui long chon ngay bat dau.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngay bat dau")]
        public DateTime? NgayBatDau { get; set; }

        [Required(ErrorMessage = "Vui long chon ngay ket thuc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngay ket thuc")]
        public DateTime? NgayKetThuc { get; set; }
    }
}
