using Demo1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Configuration;
using System.Web.Mvc;
using Demo1.Services;

namespace Demo1.Controllers
{
    public class NghiPhepController : Controller
    {
        private readonly QLNVCTYEntities6 db = new QLNVCTYEntities6();

        private bool IsAdmin => Session["TaikhoanAdmin"] != null;

        private NHANVIEN LoadCurrentUser()
        {
            var sessionUser = Session["TaikhoanUser"] as NHANVIEN;
            if (sessionUser == null)
            {
                return null;
            }

            return db.NHANVIENs.Find(sessionUser.MSNV);
        }

        private static int CountOverlappingDays(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            if (endA < startB || endB < startA)
            {
                return 0;
            }

            var start = startA > startB ? startA : startB;
            var end = endA < endB ? endA : endB;
            return (end - start).Days + 1; // inclusive
        }

        private static int GetConfigInt(string key, int defaultValue)
        {
            var raw = ConfigurationManager.AppSettings[key];
            if (int.TryParse(raw, out var value) && value >= 0)
            {
                return value;
            }
            return defaultValue;
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

        public ActionResult Index()
        {
            if (IsAdmin)
            {
                var danhSach = db.QUANLYNGHIPHEPs
                    .Include(np => np.NHANVIEN)
                    .OrderByDescending(np => np.NGAYBATDAU ?? np.NGAYKETTHUC)
                    .ToList();

                return View(danhSach);
            }

            if (Session["TaikhoanUser"] != null)
            {
                return RedirectToAction("DanhSachCuaToi");
            }

            return RedirectToAction("Login", "Admin");
        }

        public ActionResult ChiTiet(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var nghiPhep = db.QUANLYNGHIPHEPs
                .Include(np => np.NHANVIEN)
                .FirstOrDefault(np => np.MANP == id);

            if (nghiPhep == null)
            {
                return HttpNotFound();
            }

            if (IsAdmin)
            {
                return View(nghiPhep);
            }

            var currentUser = LoadCurrentUser();
            if (currentUser != null && string.Equals(currentUser.MSNV, nghiPhep.MSNV, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsUserView = true;
                return View(nghiPhep);
            }

            return RedirectToAction("Login", "Admin");
        }

        public ActionResult DSNghiChuaDuyet(int? page)
        {
            if (!IsAdmin)
            {
                return RedirectToAction("Login", "Admin");
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var danhSach = db.QUANLYNGHIPHEPs
                .Include(np => np.NHANVIEN)
                .Where(np => !np.TRANGTHAI.HasValue)
                .OrderByDescending(np => np.NGAYBATDAU ?? np.NGAYKETTHUC)
                .ToPagedList(pageNumber, pageSize);

            return View(danhSach);
        }

        public ActionResult DuyetNghiPhep(int id)
        {
            if (!IsAdmin)
            {
                return RedirectToAction("Login", "Admin");
            }

            var nghiPhep = db.QUANLYNGHIPHEPs.Include(np => np.NHANVIEN).FirstOrDefault(np => np.MANP == id);
            if (nghiPhep == null)
            {
                return HttpNotFound();
            }

            nghiPhep.TRANGTHAI = true;
            db.Entry(nghiPhep).State = EntityState.Modified;
            db.SaveChanges();

            var to = nghiPhep.NHANVIEN?.EMAIL;
            var subject = "Thong bao: Don nghi phep da duoc duyet";
            var body = string.Format(
                "<p>Xin chao {0},</p><p>Don nghi phep (Ma: {1}) cua ban tu {2} den {3} da duoc duyet.</p>",
                nghiPhep.NHANVIEN?.HOTEN ?? nghiPhep.MSNV,
                nghiPhep.MANP,
                nghiPhep.NGAYBATDAU.HasValue ? nghiPhep.NGAYBATDAU.Value.ToString("dd/MM/yyyy") : "",
                nghiPhep.NGAYKETTHUC.HasValue ? nghiPhep.NGAYKETTHUC.Value.ToString("dd/MM/yyyy") : "");
            EmailService.SendSafe(to, subject, body);

            return RedirectToAction("Index");
        }

        public ActionResult TuChoiNghiPhep(int id)
        {
            if (!IsAdmin)
            {
                return RedirectToAction("Login", "Admin");
            }

            var nghiPhep = db.QUANLYNGHIPHEPs.Include(np => np.NHANVIEN).FirstOrDefault(np => np.MANP == id);
            if (nghiPhep == null)
            {
                return HttpNotFound();
            }

            nghiPhep.TRANGTHAI = false;
            db.Entry(nghiPhep).State = EntityState.Modified;
            db.SaveChanges();

            var to = nghiPhep.NHANVIEN?.EMAIL;
            var subject = "Thong bao: Don nghi phep bi tu choi";
            var body = string.Format(
                "<p>Xin chao {0},</p><p>Don nghi phep (Ma: {1}) cua ban tu {2} den {3} da bi tu choi.</p>",
                nghiPhep.NHANVIEN?.HOTEN ?? nghiPhep.MSNV,
                nghiPhep.MANP,
                nghiPhep.NGAYBATDAU.HasValue ? nghiPhep.NGAYBATDAU.Value.ToString("dd/MM/yyyy") : "",
                nghiPhep.NGAYKETTHUC.HasValue ? nghiPhep.NGAYKETTHUC.Value.ToString("dd/MM/yyyy") : "");
            EmailService.SendSafe(to, subject, body);

            return RedirectToAction("Index");
        }

        public ActionResult DanhSachCuaToi()
        {
            var currentUser = LoadCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var danhSach = db.QUANLYNGHIPHEPs
                .Include(np => np.NHANVIEN)
                .Where(np => np.MSNV == currentUser.MSNV)
                .OrderByDescending(np => np.NGAYBATDAU ?? np.NGAYKETTHUC)
                .ToList();

            ViewBag.Message = TempData["LeaveMessage"] as string;
            return View("DanhSachCuaToi", danhSach);
        }

        [HttpGet]
        public ActionResult TaoMoi()
        {
            var currentUser = LoadCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            ViewBag.LeaveTypes = BuildLeaveTypes();
            var model = new LeaveRequestCreateModel
            {
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoMoi(LeaveRequestCreateModel model)
        {
            var currentUser = LoadCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.LeaveTypes = BuildLeaveTypes(model.LoaiNghi);
                return View(model);
            }

            if (model.NgayKetThuc < model.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngay ket thuc phai lon hon hoac bang ngay bat dau.");
                ViewBag.LeaveTypes = BuildLeaveTypes(model.LoaiNghi);
                return View(model);
            }

            var soNgay = (model.NgayKetThuc - model.NgayBatDau).Value.Days + 1;
            if (soNgay <= 0)
            {
                ModelState.AddModelError("NgayKetThuc", "So ngay nghi khong hop le.");
                ViewBag.LeaveTypes = BuildLeaveTypes(model.LoaiNghi);
                return View(model);
            }

            // Han muc nghi phep (configurable qua appSettings)
            int monthlyLimit = GetConfigInt("LeaveMonthlyLimit", 3);
            int yearlyLimit = GetConfigInt("LeaveYearlyLimit", 12);

            var reqStart = model.NgayBatDau.Value.Date;
            var reqEnd = model.NgayKetThuc.Value.Date;

            // Lay cac don nghi phep lien quan (da duyet hoac dang cho)
            var relatedLeaves = db.QUANLYNGHIPHEPs
                .Where(np => np.MSNV == currentUser.MSNV
                             && np.NGAYBATDAU.HasValue
                             && np.NGAYKETTHUC.HasValue
                             && np.TRANGTHAI != false)
                .ToList();

            // Kiem tra theo tung thang trong khoang yeu cau
            for (var cursor = new DateTime(reqStart.Year, reqStart.Month, 1);
                 cursor <= new DateTime(reqEnd.Year, reqEnd.Month, 1);
                 cursor = cursor.AddMonths(1))
            {
                var monthStart = new DateTime(cursor.Year, cursor.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var requestedInMonth = CountOverlappingDays(reqStart, reqEnd, monthStart, monthEnd);
                if (requestedInMonth == 0) continue;

                var existingInMonth = relatedLeaves.Sum(l =>
                {
                    var s = l.NGAYBATDAU.Value.Date;
                    var e = l.NGAYKETTHUC.Value.Date;
                    return CountOverlappingDays(s, e, monthStart, monthEnd);
                });

                if (existingInMonth + requestedInMonth > monthlyLimit)
                {
                    var remaining = Math.Max(0, monthlyLimit - existingInMonth);
                    ModelState.AddModelError(string.Empty,
                        string.Format("Vuot qua han muc ngay nghi trong thang {0:MM/yyyy}. Con lai {1} ngay.", monthStart, remaining));
                }
            }

            // Kiem tra theo tung nam trong khoang yeu cau
            for (int year = reqStart.Year; year <= reqEnd.Year; year++)
            {
                var yearStart = new DateTime(year, 1, 1);
                var yearEnd = new DateTime(year, 12, 31);

                var requestedInYear = CountOverlappingDays(reqStart, reqEnd, yearStart, yearEnd);
                if (requestedInYear == 0) continue;

                var existingInYear = relatedLeaves.Sum(l =>
                {
                    var s = l.NGAYBATDAU.Value.Date;
                    var e = l.NGAYKETTHUC.Value.Date;
                    return CountOverlappingDays(s, e, yearStart, yearEnd);
                });

                if (existingInYear + requestedInYear > yearlyLimit)
                {
                    var remaining = Math.Max(0, yearlyLimit - existingInYear);
                    ModelState.AddModelError(string.Empty,
                        string.Format("Vuot qua han muc ngay nghi trong nam {0}. Con lai {1} ngay.", year, remaining));
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.LeaveTypes = BuildLeaveTypes(model.LoaiNghi);
                return View(model);
            }

            var nextId = (db.QUANLYNGHIPHEPs.Max(np => (int?)np.MANP) ?? 0) + 1;

            var nghiPhep = new QUANLYNGHIPHEP
            {
                MANP = nextId,
                MSNV = currentUser.MSNV,
                LOAINGHI = model.LoaiNghi,
                NGAYBATDAU = model.NgayBatDau,
                NGAYKETTHUC = model.NgayKetThuc,
                SONGAYNGHI = soNgay,
                TRANGTHAI = null
            };
            db.QUANLYNGHIPHEPs.Add(nghiPhep);
            db.SaveChanges();

            TempData["LeaveMessage"] = "Gui yeu cau nghi phep thanh cong.";

            return RedirectToAction("DanhSachCuaToi");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
