using Demo1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo1.Controllers
{
    public class NhanVienController : Controller
    {
        // GET: NhanVien
        QLNVCTYEntities6 db = new QLNVCTYEntities6();
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            // Lấy danh sách nhân viên
            return View(db.NHANVIENs.ToList().OrderBy(n => n.MSNV).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            // Tạo danh sách cho việc chọn giới tính
            ViewBag.GioiTinhList = new List<SelectListItem>
{
    new SelectListItem { Text = "Nam", Value = "Nam" },
    new SelectListItem { Text = "Nữ", Value = "Nữ" },
    new SelectListItem { Text = "Không xác định", Value = "Không xác định" }
};

            return View();
        }

        // POST: Tạo mới nhân viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NHANVIEN nhanvien, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem file đã được upload chưa
                if (file != null && file.ContentLength > 0)
                {
                    // Lưu hình ảnh vào database
                    using (var reader = new BinaryReader(file.InputStream))
                    {
                        byte[] imageData = reader.ReadBytes(file.ContentLength);
                        nhanvien.HINHANH = Convert.ToBase64String(imageData);
                    }
                }

                // Lưu thông tin nhân viên vào database
                db.NHANVIENs.Add(nhanvien);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, quay lại trang tạo mới và hiển thị lỗi
            ViewBag.GioiTinhList = new List<SelectListItem>
{
    new SelectListItem { Text = "Nam", Value = "Nam" },
    new SelectListItem { Text = "Nữ", Value = "Nữ" },
    new SelectListItem { Text = "Không xác định", Value = "Không xác định" }
};

            return View(nhanvien);
        }
        public ActionResult Delete(string id)
        {
            var nhanVien = db.NHANVIENs.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            return View(nhanVien); // Trả về view ConfirmDelete với thông tin nhân viên để xác nhận xóa
        }

        public ActionResult TinhtongNV()
        {
            int tongSoLuongNV = db.NHANVIENs.Count();
            ViewBag.TongSoLuongNV = tongSoLuongNV;

            return View();
        }
    }
}