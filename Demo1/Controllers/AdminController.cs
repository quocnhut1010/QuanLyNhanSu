using Demo1.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Demo1.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private QLNVCTYEntities6 db = new QLNVCTYEntities6();
        private List<NHANVIEN> Laynhanvien(int count)
        {
            return db.NHANVIENs.OrderByDescending(a => a.NGAYVAOLAM).Take(count).ToList();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn) == true)
            {
                ViewBag.Loi1 = "Hãy nhập username";
            }
            if (String.IsNullOrEmpty(matkhau) == true)
            {
                ViewBag.Loi2 = "Hãy nhạp password";
            }
            else
            {
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.MAADMIN == tendn && n.MATKHAU == matkhau);
                NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.MSNV == tendn && n.CCCD == matkhau);
                if (ad != null)
                {
                    Session["TaikhoanAdmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else if (nv != null)
                {
                    Session["TaikhoanUser"] = nv;
                    return RedirectToAction("ThongTin", "User");
                }
                else { ViewBag.Thongbao = "Username hoặc Password không đúng"; }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["TaiKhoanAdmin"] = null;
            Session["TaiKhoanUser"] = null;
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult LICHSUCC(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 8;

            var chamCongList = db.CHAMCONGs.OrderBy(n => n.IDCC).ToList();

            // Sử dụng PagedList để phân trang danh sách chấm công
            var pagedList = chamCongList.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

    }
}