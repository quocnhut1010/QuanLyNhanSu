using Demo1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Demo1.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly QLNVCTYEntities6 db = new QLNVCTYEntities6();

        private static string NormalizeGenderValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalized = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().ToLowerInvariant();
        }

        private List<SelectListItem> BuildGenderOptions(string selectedValue = null)
        {
            var genders = new List<SelectListItem>
            {
                new SelectListItem { Text = "Nam", Value = "Nam" },
                new SelectListItem { Text = "Nữ", Value = "Nữ" },
                new SelectListItem { Text = "Không xác định", Value = "Không xác định" }
            };

            var normalizedSelected = NormalizeGenderValue(selectedValue);

            foreach (var option in genders)
            {
                if (!string.IsNullOrEmpty(normalizedSelected) &&
                    NormalizeGenderValue(option.Value).Equals(normalizedSelected, StringComparison.Ordinal))
                {
                    option.Selected = true;
                }
            }

            return genders;
        }

        private void PrepareGenderViewBag(string selected = null)
        {
            ViewBag.GioiTinhList = BuildGenderOptions(selected);
        }

        public ActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var data = db.NHANVIENs
                .OrderBy(n => n.MSNV)
                .ToPagedList(pageNumber, pageSize);

            return View(data);
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var nhanVien = db.NHANVIENs.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            return View(nhanVien);
        }

        public ActionResult Create()
        {
            PrepareGenderViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NHANVIEN nhanvien, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(file.InputStream))
                    {
                        var imageData = reader.ReadBytes(file.ContentLength);
                        nhanvien.HINHANH = Convert.ToBase64String(imageData);
                    }
                }

                db.NHANVIENs.Add(nhanvien);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            PrepareGenderViewBag(nhanvien.GIOITINH);
            return View(nhanvien);
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var nhanVien = db.NHANVIENs.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            PrepareGenderViewBag(nhanVien.GIOITINH);
            return View(nhanVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NHANVIEN nhanvien, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                PrepareGenderViewBag(nhanvien.GIOITINH);
                return View(nhanvien);
            }

            var existing = db.NHANVIENs.Find(nhanvien.MSNV);
            if (existing == null)
            {
                return HttpNotFound();
            }

            existing.HOTEN = nhanvien.HOTEN;
            existing.GIOITINH = nhanvien.GIOITINH;
            existing.NGAYSINH = nhanvien.NGAYSINH;
            existing.QUEQUAN = nhanvien.QUEQUAN;
            existing.TAMTRU = nhanvien.TAMTRU;
            existing.SDT = nhanvien.SDT;
            existing.EMAIL = nhanvien.EMAIL;
            existing.DANTOC = nhanvien.DANTOC;
            existing.HOCVAN = nhanvien.HOCVAN;
            existing.NGAYVAOLAM = nhanvien.NGAYVAOLAM;
            existing.CCCD = nhanvien.CCCD;
            existing.MSHD = nhanvien.MSHD;

            if (file != null && file.ContentLength > 0)
            {
                using (var reader = new BinaryReader(file.InputStream))
                {
                    var imageData = reader.ReadBytes(file.ContentLength);
                    existing.HINHANH = Convert.ToBase64String(imageData);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var nhanVien = db.NHANVIENs.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            return View(nhanVien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var nhanVien = db.NHANVIENs
                .Include(n => n.CHAMCONGs)
                .Include(n => n.TANGCAs)
                .Include(n => n.QUANLYNGHIPHEPs)
                .Include(n => n.LUONGs)
                .Include(n => n.HOPDONGLAODONGs)
                .Include(n => n.HOCVANKINHNGHIEMs)
                .Include(n => n.CHUCVUs)
                .SingleOrDefault(n => n.MSNV == id);

            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            if (nhanVien.CHAMCONGs.Any())
            {
                db.CHAMCONGs.RemoveRange(nhanVien.CHAMCONGs.ToList());
            }

            if (nhanVien.TANGCAs.Any())
            {
                db.TANGCAs.RemoveRange(nhanVien.TANGCAs.ToList());
            }

            if (nhanVien.QUANLYNGHIPHEPs.Any())
            {
                db.QUANLYNGHIPHEPs.RemoveRange(nhanVien.QUANLYNGHIPHEPs.ToList());
            }

            if (nhanVien.LUONGs.Any())
            {
                db.LUONGs.RemoveRange(nhanVien.LUONGs.ToList());
            }

            if (nhanVien.HOPDONGLAODONGs.Any())
            {
                db.HOPDONGLAODONGs.RemoveRange(nhanVien.HOPDONGLAODONGs.ToList());
            }

            if (nhanVien.HOCVANKINHNGHIEMs.Any())
            {
                db.HOCVANKINHNGHIEMs.RemoveRange(nhanVien.HOCVANKINHNGHIEMs.ToList());
            }

            if (nhanVien.CHUCVUs.Any())
            {
                db.CHUCVUs.RemoveRange(nhanVien.CHUCVUs.ToList());
            }

            db.NHANVIENs.Remove(nhanVien);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult TinhtongNV()
        {
            int tongSoLuongNV = db.NHANVIENs.Count();
            ViewBag.TongSoLuongNV = tongSoLuongNV;

            return View();
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
