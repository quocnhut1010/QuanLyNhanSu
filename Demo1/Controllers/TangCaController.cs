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
using Demo1.Services;

namespace Demo1.Controllers
{
    public class TangCaController : Controller
    {
        QLNVCTYEntities6 db = new QLNVCTYEntities6();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LSTangCa(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.TANGCAs.OrderBy(n => n.IDTC).ToPagedList(pageNumber, pageSize));

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
            int pageNumber = (page ?? 1);
            int pageSize = 6;

            var tangCaChuaDuyet = db.TANGCAs.Where(tc => !tc.TRANGTHAI.HasValue).OrderBy(tc => tc.IDTC);

            return View(tangCaChuaDuyet.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Duyet(int id)
        {
            if (Session["TaikhoanAdmin"] != null)
            {
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                tangCa.TRANGTHAI = true;

                db.SaveChanges();

                var nv = db.NHANVIENs.FirstOrDefault(n => n.MSNV == tangCa.MSNV);
                var to = nv?.EMAIL;
                var subject = "Thong bao: Yeu cau tang ca duoc duyet";
                var body = string.Format(
                    "<p>Xin chao {0},</p><p>Yeu cau tang ca (Ma: {1}) vao ngay {2} tu {3} den {4} da duoc duyet.</p>",
                    nv?.HOTEN ?? tangCa.MSNV,
                    tangCa.IDTC,
                    tangCa.NGAY.HasValue ? tangCa.NGAY.Value.ToString("dd/MM/yyyy") : "",
                    tangCa.GIOBATDAU.HasValue ? DateTime.Today.Add(tangCa.GIOBATDAU.Value).ToString("HH:mm") : "",
                    tangCa.GIOKETTHUC.HasValue ? DateTime.Today.Add(tangCa.GIOKETTHUC.Value).ToString("HH:mm") : "");
                EmailService.SendSafe(to, subject, body);

                return RedirectToAction("LSTangCaChuaDuyet");
            }
            else if (Session["TaikhoanUser"] != null)
            {
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                tangCa.TRANGTHAI = true;

                db.SaveChanges();
                var nv = db.NHANVIENs.FirstOrDefault(n => n.MSNV == tangCa.MSNV);
                var to = nv?.EMAIL;
                var subject = "Thong bao: Yeu cau tang ca duoc duyet";
                var body = string.Format(
                    "<p>Xin chao {0},</p><p>Yeu cau tang ca (Ma: {1}) vao ngay {2} tu {3} den {4} da duoc duyet.</p>",
                    nv?.HOTEN ?? tangCa.MSNV,
                    tangCa.IDTC,
                    tangCa.NGAY.HasValue ? tangCa.NGAY.Value.ToString("dd/MM/yyyy") : "",
                    tangCa.GIOBATDAU.HasValue ? DateTime.Today.Add(tangCa.GIOBATDAU.Value).ToString("HH:mm") : "",
                    tangCa.GIOKETTHUC.HasValue ? DateTime.Today.Add(tangCa.GIOKETTHUC.Value).ToString("HH:mm") : "");
                EmailService.SendSafe(to, subject, body);
                return RedirectToAction("LichSuTangCa", "User");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult KhongDuyet(int id)
        {
            if (Session["TaikhoanAdmin"] != null)
            {
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                tangCa.TRANGTHAI = false;

                db.SaveChanges();

                ViewBag.SuccessMessage = "Duyet tang ca that bai.";

                var nv = db.NHANVIENs.FirstOrDefault(n => n.MSNV == tangCa.MSNV);
                var to = nv?.EMAIL;
                var subject = "Thong bao: Yeu cau tang ca bi tu choi";
                var body = string.Format(
                    "<p>Xin chao {0},</p><p>Yeu cau tang ca (Ma: {1}) vao ngay {2} tu {3} den {4} da bi tu choi.</p>",
                    nv?.HOTEN ?? tangCa.MSNV,
                    tangCa.IDTC,
                    tangCa.NGAY.HasValue ? tangCa.NGAY.Value.ToString("dd/MM/yyyy") : "",
                    tangCa.GIOBATDAU.HasValue ? DateTime.Today.Add(tangCa.GIOBATDAU.Value).ToString("HH:mm") : "",
                    tangCa.GIOKETTHUC.HasValue ? DateTime.Today.Add(tangCa.GIOKETTHUC.Value).ToString("HH:mm") : "");
                EmailService.SendSafe(to, subject, body);

                return RedirectToAction("LSTangCaChuaDuyet");
            }
            else if (Session["TaikhoanUser"] != null)
            {
                var tangCa = db.TANGCAs.FirstOrDefault(tc => tc.IDTC == id);

                if (tangCa == null)
                {
                    return HttpNotFound();
                }

                tangCa.TRANGTHAI = false;

                db.SaveChanges();
                var nv = db.NHANVIENs.FirstOrDefault(n => n.MSNV == tangCa.MSNV);
                var to = nv?.EMAIL;
                var subject = "Thong bao: Yeu cau tang ca bi tu choi";
                var body = string.Format(
                    "<p>Xin chao {0},</p><p>Yeu cau tang ca (Ma: {1}) vao ngay {2} tu {3} den {4} da bi tu choi.</p>",
                    nv?.HOTEN ?? tangCa.MSNV,
                    tangCa.IDTC,
                    tangCa.NGAY.HasValue ? tangCa.NGAY.Value.ToString("dd/MM/yyyy") : "",
                    tangCa.GIOBATDAU.HasValue ? DateTime.Today.Add(tangCa.GIOBATDAU.Value).ToString("HH:mm") : "",
                    tangCa.GIOKETTHUC.HasValue ? DateTime.Today.Add(tangCa.GIOKETTHUC.Value).ToString("HH:mm") : "");
                EmailService.SendSafe(to, subject, body);
                return RedirectToAction("LichSuTangCa", "User");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
