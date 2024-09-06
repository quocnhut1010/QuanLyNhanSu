using Demo1.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Net;
using System.Data.Entity;
using static System.Net.Mime.MediaTypeNames;
using Antlr.Runtime.Misc;

namespace Demo1.Controllers
{
    public class TangCaController : Controller
    {
        // GET: TangCa
        QLNVCTYEntities6 db = new QLNVCTYEntities6();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LSTangCa(int? page)
        {
            //if (Session["TaikhoanAdmin"] == null)
            //    return RedirectToAction("Login", "Admin");
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            // Lấy danh sách các tăng ca chưa duyệt từ cơ sở dữ liệu
            return View(db.TANGCAs.ToList().OrderBy(n => n.IDTC).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TANGCA tc = db.TANGCAs.Find(id);
            if (tc == null)
            {
                return HttpNotFound();
            }
            return View(tc);
        }
        public ActionResult LSTangCaChuaDuyet(int? page)
        {
            // Kiểm tra session nếu cần thiết
            //if (Session["TaikhoanAdmin"] == null)
            //    return RedirectToAction("Login", "Admin");

            int pageNumber = (page ?? 1);
            int pageSize = 6;

            // Lấy danh sách các tăng ca chưa duyệt từ cơ sở dữ liệu
            var tangCaChuaDuyet = db.TANGCAs.Where(tc => tc.TRANGTHAI == false).OrderBy(tc => tc.IDTC);

            // Sử dụng ToPagedList để phân trang
            return View(tangCaChuaDuyet.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Duyet(int id)
        {
            // Kiểm tra xem người dùng đang đăng nhập là admin hay nhân viên
            if (Session["TaikhoanAdmin"] != null)
            {
                // Tìm tăng ca dựa trên ID
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                // Nếu không tìm thấy tăng ca, trả về NotFound
                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                // Đặt trạng thái của tăng ca thành true (đã duyệt)
                tangCa.TRANGTHAI = true;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                // Gửi thông báo thành công về view
                ViewBag.SuccessMessage = "Duyệt tăng ca thành công.";

                // Redirect về danh sách tăng ca chưa duyệt
                return RedirectToAction("LSTangCaChuaDuyet");
            }
            else if (Session["TaikhoanUser"] != null)
            {
                // Tìm tăng ca dựa trên ID
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                // Nếu không tìm thấy tăng ca, trả về NotFound
                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                // Đặt trạng thái của tăng ca thành true (đã duyệt)
                tangCa.TRANGTHAI = true;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                // Nếu người dùng là nhân viên, chuyển hướng đến trang lichsutangca
                return RedirectToAction("LichSuTangCa", "User");
            }
            else
            {
                // Nếu không có session nào tồn tại, chuyển hướng về trang đăng nhập
                return RedirectToAction("Login");
            }
        }

        // không duyệt đơn
        public ActionResult KhongDuyet(int id)
        {
            // Kiểm tra xem người dùng đang đăng nhập là admin hay nhân viên
            if (Session["TaikhoanAdmin"] != null)
            {
                // Tìm tăng ca dựa trên ID
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                // Nếu không tìm thấy tăng ca, trả về NotFound
                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                // Đặt trạng thái của tăng ca thành true (đã duyệt)
                tangCa.TRANGTHAI = false;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                // Gửi thông báo thành công về view
                ViewBag.SuccessMessage = "Duyệt tăng ca thành công.";

                // Redirect về danh sách tăng ca chưa duyệt
                return RedirectToAction("LSTangCaChuaDuyet");
            }
            else if (Session["TaikhoanUser"] != null)
            {
                // Tìm tăng ca dựa trên ID
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                // Nếu không tìm thấy tăng ca, trả về NotFound
                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                // Đặt trạng thái của tăng ca thành true (đã duyệt)
                tangCa.TRANGTHAI = false;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                // Nếu người dùng là nhân viên, chuyển hướng đến trang lichsutangca
                return RedirectToAction("LichSuTangCa", "User");
            }
            else
            {
                // Nếu không có session nào tồn tại, chuyển hướng về trang đăng nhập
                return RedirectToAction("Login");
            }
        }
    }
}