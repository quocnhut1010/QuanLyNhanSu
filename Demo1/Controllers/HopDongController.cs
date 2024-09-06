using Demo1.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Demo1.Controllers
{
    public class HopDongController : Controller
    {
        // GET: HopDong
        QLNVCTYEntities6 db = new QLNVCTYEntities6();
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            // Lấy danh sách các tăng ca chưa duyệt từ cơ sở dữ liệu
            return View(db.HOPDONGLAODONGs.ToList().OrderBy(n => n.MAHOPDONG).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HOPDONGLAODONG hopdong = db.HOPDONGLAODONGs.Find(id);
            if (hopdong == null)
            {
                return HttpNotFound();
            }

            ViewBag.MSNV = hopdong.MSNV; // Truyền giá trị MSNV vào ViewBag
            return View(hopdong);
        }

        // POST: HOPDONGLAODONGs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAHOPDONG,MSNV,LOAIHOPDONG,NGAYKY,NGAYHETHAN,LUONGCOBAN")] HOPDONGLAODONG hopdong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hopdong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hopdong);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HOPDONGLAODONG hopdong)
        {
            if (ModelState.IsValid)
            {
                var newHopDong = new HOPDONGLAODONG
                {
                    MSNV = hopdong.MSNV,
                    LOAIHOPDONG = hopdong.LOAIHOPDONG,
                    NGAYKY = hopdong.NGAYKY,
                    NGAYHETHAN = hopdong.NGAYHETHAN,
                    LUONGCOBAN = hopdong.LUONGCOBAN
                };

                db.HOPDONGLAODONGs.Add(hopdong);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(hopdong);
        }

    }
}