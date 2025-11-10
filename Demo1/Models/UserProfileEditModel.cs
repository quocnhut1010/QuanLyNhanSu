using System.ComponentModel.DataAnnotations;

namespace Demo1.Models
{
    public class UserProfileEditModel
    {
        [Required]
        public string MSNV { get; set; }

        [Display(Name = "Ho ten")]
        public string HOTEN { get; set; }

        [Display(Name = "Que quan")]
        public string QUEQUAN { get; set; }

        [Display(Name = "Tam tru")]
        public string TAMTRU { get; set; }

        [Display(Name = "So dien thoai")]
        [Phone(ErrorMessage = "So dien thoai khong hop le")]
        public string SDT { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email khong hop le")]
        public string EMAIL { get; set; }

        [Display(Name = "Hoc van")]
        public string HOCVAN { get; set; }
    }
}
