using Demo1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo1.Controllers
{
    public class UserController : Controller
    {
        private readonly QLNVCTYEntities6 db = new QLNVCTYEntities6();

        private NHANVIEN GetCurrentUserFromSession()
        {
            return Session["TaikhoanUser"] as NHANVIEN;
        }

        private NHANVIEN LoadCurrentUser()
        {
            var sessionUser = GetCurrentUserFromSession();
            if (sessionUser == null)
            {
                return null;
            }

            return db.NHANVIENs.Find(sessionUser.MSNV);
        }

        private IEnumerable<SelectListItem> BuildLeaveTypes(string selectedValue = null)
        {
            var items = new List<SelectListItem>
            {
                new SelectListItem { Text = "Nghi phep nam", Value = "Nghi phep nam" },
                new SelectListItem { Text = "Nghi co luong", Value = "Nghi co luong" },
                new SelectListItem { Text = "Nghi khong luong", Value = "Nghi khong luong" },
                new SelectListItem { Text = "Nghi benh", Value = "Nghi benh" },
                new SelectListItem { Text = "Khac", Value = "Khac" }
            };

            if (!string.IsNullOrEmpty(selectedValue))
            {
                var option = items.FirstOrDefault(i => string.Equals(i.Value, selectedValue, StringComparison.OrdinalIgnoreCase));
                if (option != null)
                {
                    option.Selected = true;
                }
            }

            return items;
        }

        private IEnumerable<SelectListItem> BuildOvertimeTimeOptions(string selectedValue = null)
        {
            var times = new[] { "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" };
            foreach (var time in times)
            {
                yield return new SelectListItem
                {
                    Text = time,
                    Value = time,
                    Selected = string.Equals(time, selectedValue, StringComparison.OrdinalIgnoreCase)
                };
            }
        }

        public ActionResult ThongTin()
        {
            if (Session["TaikhoanAdmin"] == null && Session["TaikhoanUser"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var nhanVien = LoadCurrentUser();
            if (nhanVien == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            Session["TaikhoanUser"] = nhanVien;
            ViewBag.Message = TempData["UserProfileMessage"] as string;
            return View(nhanVien);
        }

        public ActionResult ChinhSua()
        {
            var nhanVien = LoadCurrentUser();
            if (nhanVien == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var model = new UserProfileEditModel
            {
                MSNV = nhanVien.MSNV,
                HOTEN = nhanVien.HOTEN,
                QUEQUAN = nhanVien.QUEQUAN,
                TAMTRU = nhanVien.TAMTRU,
                SDT = nhanVien.SDT,
                EMAIL = nhanVien.EMAIL,
                HOCVAN = nhanVien.HOCVAN
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSua(UserProfileEditModel model, HttpPostedFileBase file)
        {
            var nhanVien = LoadCurrentUser();
            if (nhanVien == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            nhanVien.QUEQUAN = model.QUEQUAN;
            nhanVien.TAMTRU = model.TAMTRU;
            nhanVien.SDT = model.SDT;
            nhanVien.EMAIL = model.EMAIL;
            nhanVien.HOCVAN = model.HOCVAN;

            if (file != null && file.ContentLength > 0)
            {
                using (var reader = new BinaryReader(file.InputStream))
                {
                    var data = reader.ReadBytes(file.ContentLength);
                    nhanVien.HINHANH = Convert.ToBase64String(data);
                }
            }

            db.Entry(nhanVien).State = EntityState.Modified;
            db.SaveChanges();

            Session["TaikhoanUser"] = nhanVien;
            TempData["UserProfileMessage"] = "Cap nhat thong tin thanh cong.";

            return RedirectToAction("ThongTin");
        }

        public ActionResult Index(string manv)
        {
            var us = db.NHANVIENs.Find(manv);
            return View(us);
        }

        public ActionResult LichSuTangCa(int? page)
        {
            if (Session["TaikhoanAdmin"] != null)
            {
                int pageNumber = page ?? 1;
                int pageSize = 6;
                return View(db.TANGCAs.ToList().OrderBy(n => n.IDTC).ToPagedList(pageNumber, pageSize));
            }
            else if (Session["TaikhoanUser"] != null)
            {
                NHANVIEN user = Session["TaikhoanUser"] as NHANVIEN;
                if (user != null)
                {
                    ViewBag.OvertimeMessage = TempData["OvertimeMessage"] as string;
                    int pageNumber = page ?? 1;
                    int pageSize = 6;
                    var lichSuTangCa = db.TANGCAs.Where(tc => tc.MSNV == user.MSNV).OrderBy(tc => tc.IDTC).ToPagedList(pageNumber, pageSize);
                    return View(lichSuTangCa);
                }
            }

            return RedirectToAction("Login", "Admin");
        }

        [HttpGet]
        public ActionResult TaoYeuCauTangCa()
        {
            var currentUser = LoadCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            ViewBag.GioBatDauOptions = BuildOvertimeTimeOptions();
            ViewBag.GioKetThucOptions = BuildOvertimeTimeOptions();

            var model = new TANGCA
            {
                MSNV = currentUser.MSNV,
                NGAY = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoYeuCauTangCa(TANGCA model, string gioBatDau, string gioKetThuc)
        {
            var currentUser = LoadCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            ViewBag.GioBatDauOptions = BuildOvertimeTimeOptions(gioBatDau);
            ViewBag.GioKetThucOptions = BuildOvertimeTimeOptions(gioKetThuc);

            if (!model.NGAY.HasValue)
            {
                ModelState.AddModelError("NGAY", "Vui long chon ngay tang ca.");
            }

            if (string.IsNullOrEmpty(gioBatDau) || string.IsNullOrEmpty(gioKetThuc))
            {
                ModelState.AddModelError(string.Empty, "Vui long chon gio bat dau va gio ket thuc.");
            }

            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan endTime = TimeSpan.Zero;
            if (!TimeSpan.TryParse(gioBatDau, out startTime) || !TimeSpan.TryParse(gioKetThuc, out endTime))
            {
                ModelState.AddModelError(string.Empty, "Gio tang ca khong hop le.");
            }
            else if (endTime <= startTime)
            {
                ModelState.AddModelError(string.Empty, "Gio ket thuc phai lon hon gio bat dau.");
            }

            if (!ModelState.IsValid)
            {
                model.MSNV = currentUser.MSNV;
                return View(model);
            }

            var date = model.NGAY.Value.Date;
            var hours = (int)Math.Round((endTime - startTime).TotalHours);
            if (hours <= 0)
            {
                ModelState.AddModelError(string.Empty, "So gio tang ca khong hop le.");
            }

            var monthlySum = db.TANGCAs
                .Where(tc => tc.MSNV == currentUser.MSNV
                    && tc.NGAY.HasValue
                    && tc.NGAY.Value.Year == date.Year
                    && tc.NGAY.Value.Month == date.Month
                    && tc.TRANGTHAI != false)
                .Select(tc => tc.SOGIO ?? 0)
                .DefaultIfEmpty(0)
                .Sum();

            if (monthlySum + hours > 8)
            {
                ModelState.AddModelError(string.Empty, string.Format("Tong so gio tang ca trong thang {0:MM/yyyy} khong duoc vuot qua 8 gio. Ban da dang ky {1} gio.", date, monthlySum));
            }

            if (!ModelState.IsValid)
            {
                model.MSNV = currentUser.MSNV;
                return View(model);
            }

            var nextId = (db.TANGCAs.Max(tc => (int?)tc.IDTC) ?? 0) + 1;

            var tangCa = new TANGCA
            {
                IDTC = nextId,
                MSNV = currentUser.MSNV,
                GIOBATDAU = startTime,
                GIOKETTHUC = endTime,
                SOGIO = hours,
                NGAY = date,
                TRANGTHAI = null
            };

            db.TANGCAs.Add(tangCa);
            db.SaveChanges();

            TempData["OvertimeMessage"] = "Gui yeu cau tang ca thanh cong.";
            return RedirectToAction("LichSuTangCa");
        }

        public ActionResult ChamCong(string manv)
        {
            var employees = db.NHANVIENs.Select(nv => new SelectListItem
            {
                Text = nv.MSNV,
                Value = nv.MSNV
            }).ToList();

            ViewBag.EmployeeList = employees;
            ViewData["MSNV"] = employees;

            if (string.IsNullOrEmpty(manv))
            {
                return View(new CHAMCONG());
            }

            var user = db.CHAMCONGs.FirstOrDefault(c => c.MSNV == manv);
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
                    if (chamCong.GIOCHAMCONG == null)
                    {
                        chamCong.GIOCHAMCONG = DateTime.Now.TimeOfDay;
                    }
                    if (chamCong.NGAY == null)
                    {
                        chamCong.NGAY = DateTime.Today;
                    }
                    if (string.IsNullOrEmpty(chamCong.LOAI))
                    {
                        chamCong.LOAI = "Chinh thuc";
                    }

                    db.CHAMCONGs.Add(chamCong);
                    db.SaveChanges();

                    return RedirectToAction("ChamCong");
                }
                catch (Exception)
                {
                    return RedirectToAction("Error");
                }
            }

            return View(chamCong);
        }
    }
}
