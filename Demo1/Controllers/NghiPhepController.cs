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


namespace Demo1.Controllers
{
    public class NghiPhepController : Controller
    {
        private QLNVCTYEntities6 db = new QLNVCTYEntities6();
        // GET: NghiPhep
        public ActionResult Index()
        {
            // Lấy danh sách nghỉ phép từ cơ sở dữ liệu
            var danhSachNghiPhep = db.QUANLYNGHIPHEPs.ToList();

            // Truyền danh sách nghỉ phép sang View để hiển thị
            return View(danhSachNghiPhep);
        }
        public ActionResult ChiTiet(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm nghỉ phép của nhân viên có MSNV là id
            var nghiPhep = db.QUANLYNGHIPHEPs.Find(id);

            if (nghiPhep == null)
            {
                return HttpNotFound();
            }

            return View(nghiPhep);
        }
        public ActionResult DSNghiChuaDuyet(int? page)
        {
            // Số lượng mục trên mỗi trang
            int pageSize = 10;
            // Trang hiện tại, mặc định là 1 nếu không có tham số truyền vào
            int pageNumber = (page ?? 1);

            // Lấy danh sách nghỉ phép chưa được duyệt từ cơ sở dữ liệu
            var danhSachNghiChuaDuyet = db.QUANLYNGHIPHEPs.Where(np => np.TRANGTHAI == false).ToList();

            // Truyền danh sách nghỉ phép chưa được duyệt sang View để hiển thị
            return View(danhSachNghiChuaDuyet.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DuyetNghiPhep(int id)
        {
            // Tìm nghỉ phép của nhân viên có ID là id
            var nghiPhep = db.QUANLYNGHIPHEPs.Find(id);

            if (nghiPhep == null)
            {
                return HttpNotFound();
            }

            // Cập nhật trạng thái của đơn nghỉ phép sang đã duyệt (true)
            nghiPhep.TRANGTHAI = true;

            // Cập nhật vào cơ sở dữ liệu
            db.Entry(nghiPhep).State = EntityState.Modified;
            db.SaveChanges();

            // Chuyển hướng người dùng đến trang danh sách nghỉ phép chưa duyệt
            return RedirectToAction("DSNghiChuaDuyet");
        }
    }
}