using Demo1.Models;
using Microsoft.SqlServer.Server;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo1.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        QLNVCTYEntities6 db = new QLNVCTYEntities6();
        private List<NHANVIEN> Laynhanvien(int count)
        {
            return db.NHANVIENs.OrderByDescending(a => a.NGAYVAOLAM).Take(count).ToList();
        }
        public ActionResult ThongTin()
        {
            // Kiểm tra xem người dùng đã đăng nhập là admin hay user
            if (Session["TaikhoanAdmin"] != null || Session["TaikhoanUser"] != null)
            {
                NHANVIEN user = Session["TaikhoanUser"] as NHANVIEN;
                if (user != null)
                {
                    // Truy vấn thông tin của nhân viên dựa trên mã nhân viên đăng nhập
                    var userInfo = db.NHANVIENs.Find(user.MSNV);
                    if (userInfo != null)
                    {
                        return View(userInfo);
                    }
                    else
                    {
                        // Nếu không tìm thấy thông tin nhân viên, có thể xử lý ở đây
                        return RedirectToAction("Error");
                    }
                }
            }

            // Nếu không phải admin hoặc user, hoặc session không tồn tại, chuyển hướng về trang đăng nhập
            return RedirectToAction("Login","Admin");
        }
        public ActionResult Index(string manv)
        {
            var us = db.NHANVIENs.Find(manv);
            return View(us);
        }
        public ActionResult LichSuTangCa(int? page)
        {
            // Kiểm tra xem người dùng đã đăng nhập là admin hay user
            if (Session["TaikhoanAdmin"] != null)
            {
                // Đối với admin, không cần hạn chế hiển thị theo mã nhân viên, vẫn hiển thị tất cả lịch sử tăng ca
                int pageNumber = (page ?? 1);
                int pageSize = 6;
                return View(db.TANGCAs.ToList().OrderBy(n => n.IDTC).ToPagedList(pageNumber, pageSize));
            }
            else if (Session["TaikhoanUser"] != null)
            {
                // Đối với user, chỉ hiển thị lịch sử tăng ca của chính họ
                NHANVIEN user = Session["TaikhoanUser"] as NHANVIEN;
                if (user != null)
                {
                    int pageNumber = (page ?? 1);
                    int pageSize = 6;
                    // Lấy danh sách các tăng ca chỉ cho mã nhân viên đã đăng nhập
                    var lichSuTangCa = db.TANGCAs.Where(tc => tc.MSNV == user.MSNV).OrderBy(tc => tc.IDTC).ToPagedList(pageNumber, pageSize);
                    return View(lichSuTangCa);
                }
            }

            // Nếu không phải admin hoặc user, hoặc session không tồn tại, chuyển hướng về trang đăng nhập
            return RedirectToAction("Login");
        }
        public ActionResult ChamCong(string manv)
        {
            // Lấy danh sách các nhân viên để hiển thị trong dropdownlist
            var employees = db.NHANVIENs.Select(nv => new SelectListItem
            {
                Text = nv.MSNV,
                Value = nv.MSNV
            }).ToList();

            ViewBag.EmployeeList = employees;

            // Đặt danh sách này vào ViewData để sử dụng trong view
            ViewData["MSNV"] = employees;

            // Kiểm tra xem manv có giá trị hay không
            if (string.IsNullOrEmpty(manv))
            {
                // Nếu manv không có giá trị, trả về view với một model trống
                return View(new CHAMCONG());
            }

            // Lấy thông tin người dùng từ database
            var user = db.CHAMCONGs.FirstOrDefault(c => c.MSNV == manv);

            // Trả về view với dữ liệu người dùng được truy vấn từ database
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChamCong(CHAMCONG chamCong)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Thiết lập thời gian chấm công và ngày hiện tại nếu không được cung cấp từ form
                    if (chamCong.GIOCHAMCONG == null)
                    {
                        chamCong.GIOCHAMCONG = DateTime.Now.TimeOfDay;
                    }
                    if (chamCong.NGAY == null)
                    {
                        chamCong.NGAY = DateTime.Today;
                    }

                    // Thiết lập giá trị mặc định cho CHAMCONG và LOAI nếu không được cung cấp từ form
                    //if (chamCong.CHAMCONG1 == 0)
                    //{
                    //    chamCong.CHAMCONG1 = 8;
                    //}
                    if (string.IsNullOrEmpty(chamCong.LOAI))
                    {
                        chamCong.LOAI = "Chính thức";
                    }

                    // Lưu thông tin vào cơ sở dữ liệu
                    db.CHAMCONGs.Add(chamCong);
                    db.SaveChanges();

                    return RedirectToAction("ChamCong"); // Chuyển hướng đến trang Index sau khi đã chấm công thành công
                }
                catch (Exception ex)
                {
                    // Bắt và xử lý ngoại lệ
                    // Truy cập thông tin trong inner exception để hiểu nguyên nhân cụ thể của lỗi
                    var innerException = ex.InnerException;
                    // Log hoặc xử lý ngoại lệ ở đây
                    return RedirectToAction("Error"); // Chuyển hướng đến trang lỗi
                }
            }

            // Nếu dữ liệu không hợp lệ, trả về lại view với model để hiển thị lại các lỗi
            return View(chamCong);
        }

    }
}