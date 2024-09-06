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
using Microsoft.Build.Utilities;
using System.Web.UI;
using System.Web.Helpers;
using System.Drawing;

namespace Demo1.Controllers
{
    public class LuongController : Controller
    {
        private QLNVCTYEntities6 db = new QLNVCTYEntities6();

        // GET: Luong
        // GET: Luong
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;

            // Tạo danh sách để lưu trữ các đối tượng LUONG
            var luongsByMonth = new Dictionary<DateTime, List<LUONG>>();

            // Lấy danh sách nhân viên có chấm công từ bảng CHAMCONG
            var nhanviensWithChamCong = db.CHAMCONGs.Select(cc => cc.MSNV).Distinct().ToList();

            foreach (var msnv in nhanviensWithChamCong)
            {
                var luongByEmployeeAndMonth = new Dictionary<string, LUONG>();

                // Lấy danh sách chấm công cho nhân viên hiện tại
                var chamCongNhanVien = db.CHAMCONGs.Where(cc => cc.MSNV == msnv).ToList();

                foreach (var chamCong in chamCongNhanVien)
                {
                    // Check if NGAY is not null before proceeding
                    if (chamCong.NGAY.HasValue)
                    {
                        var thangChamCong = new DateTime(chamCong.NGAY.Value.Year, chamCong.NGAY.Value.Month, 1);
                        var key = $"{msnv}_{thangChamCong}";

                        if (!luongByEmployeeAndMonth.ContainsKey(key))
                        {
                            var luong = new LUONG();
                            luong.MSNV = msnv;
                            luong.THANG = thangChamCong;

                            // Lấy thông tin nhân viên từ bảng NHANVIEN
                            var nhanvien = db.NHANVIENs.FirstOrDefault(nv => nv.MSNV == msnv);
                            luong.NHANVIEN = nhanvien;

                            // Set các giá trị mặc định cho LCB, BHYT, BHXH, BHTN
                            luong.BHYT = 200000;
                            luong.BHXH = 150000;
                            luong.BHTN = 150000;
                            luong.TONGPHUCAP = 500000;

                            // Tính số ngày làm trong tháng
                            var soNgayLamTrongThang = chamCongNhanVien.Count(cc => cc.NGAY.Value.Year == thangChamCong.Year && cc.NGAY.Value.Month == thangChamCong.Month);
                            luong.SONGAYLAM = soNgayLamTrongThang;

                            // Tính toán số giờ tăng ca từ bảng TANGCA (nếu có)
                            var soGioTangCaTrongThang = db.TANGCAs
                                .Where(tc => tc.MSNV == msnv && tc.NGAY.Value.Year == thangChamCong.Year && tc.NGAY.Value.Month == thangChamCong.Month)
                                .Sum(tc => tc.SOGIO);

                            if (soGioTangCaTrongThang > 0)
                            {
                                luong.SOGIOTANGCA = soGioTangCaTrongThang;
                                luong.TONGTIENTANGCA = soGioTangCaTrongThang * 50000;
                            }
                            else
                            {
                                luong.SOGIOTANGCA = 0;
                                luong.TONGTIENTANGCA = 0;
                            }

                            // Lấy thông tin hợp đồng lao động của nhân viên
                            var hopDong = db.HOPDONGLAODONGs.FirstOrDefault(hd => hd.MSNV == msnv &&
                                                                                   hd.NGAYKY <= thangChamCong &&
                                                                                   (hd.NGAYHETHAN == null || hd.NGAYHETHAN >= thangChamCong));

                            if (hopDong != null)
                            {
                                // Cập nhật lương cơ bản từ hợp đồng vào đối tượng luong
                                luong.LCB = hopDong.LUONGCOBAN;
                            }
                            else
                            {
                                // Xử lý trường hợp không tìm thấy hợp đồng lao động nếu cần
                            }

                            // Tính tổng tiền nhận
                            // Tính tổng tiền nhận
                            luong.TONGTIENNHAN = luong.LCB + luong.SONGAYLAM * 300000 + luong.BHYT + luong.BHXH + luong.BHTN + luong.TONGPHUCAP + luong.TONGTIENTANGCA;
                            // Thêm vào luongByEmployeeAndMonth
                            luongByEmployeeAndMonth[key] = luong;
                        }
                        else
                        {
                            // Đã tính toán lương cho nhân viên và tháng này trước đó, chỉ cập nhật số ngày làm
                            var luong = luongByEmployeeAndMonth[key];
                            // Cập nhật số ngày làm
                            luong.SONGAYLAM += 1; // Hoặc tính toán số ngày làm phù hợp
                        }
                    }
                    else
                    {
                        // Handle the case where NGAY is null if needed
                    }
                }

                // Thêm vào luongsByMonth
                foreach (var entry in luongByEmployeeAndMonth)
                {
                    var keyParts = entry.Key.Split('_');
                    var msnvKey = keyParts[0];
                    var thangChamCongKey = DateTime.Parse(keyParts[1]);

                    if (!luongsByMonth.ContainsKey(thangChamCongKey))
                    {
                        luongsByMonth[thangChamCongKey] = new List<LUONG>();
                    }

                    luongsByMonth[thangChamCongKey].Add(entry.Value);
                }
            }

            // Xóa dữ liệu cũ trong bảng LUONG trước khi thêm mới
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE LUONG");

            // Lưu danh sách đối tượng LUONG vào cơ sở dữ liệu
            foreach (var kvp in luongsByMonth)
            {
                db.LUONGs.AddRange(kvp.Value);
            }
            db.SaveChanges(); // Lưu thay đổi sau khi thêm mới

            // Trả về view với dữ liệu đã được tính toán
            return View(luongsByMonth.SelectMany(kvp => kvp.Value).OrderBy(n => n.IDL).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult XemChiTiet(int? id)
        {
            // Lấy đối tượng LUONG dựa trên ID
            var luong = db.LUONGs.FirstOrDefault(l => l.IDL == id);

            if (luong == null)
            {
                // Nếu không tìm thấy, có thể xử lý lỗi 404 hoặc chuyển hướng đến trang khác
                return HttpNotFound(); // Sử dụng HttpNotFound() nếu muốn trả về lỗi 404
            }

            // Trả về view với đối tượng LUONG để hiển thị chi tiết
            return View(luong);
        }

        public ActionResult TKLuongNhanVien()
        {
            // Nhóm các bản ghi theo MSNV và tính tổng lương cho mỗi nhóm
            var luongTheoNhanVien = db.LUONGs
                .GroupBy(l => l.MSNV)
                .Select(g => new { MSNV = g.Key, TongLuong = g.Sum(l => l.TONGTIENNHAN) })
                .ToList();

            // Tạo danh sách chứa tên nhân viên và tổng tiền nhận của họ
            List<string> tenNhanVien = new List<string>();
            List<int> tongTienNhan = new List<int>();

            // Lặp qua từng nhóm để lấy dữ liệu cho biểu đồ
            foreach (var item in luongTheoNhanVien)
            {
                // Lấy tên nhân viên từ MSNV (có thể thay thế bằng các truy vấn khác để lấy tên nhân viên từ bảng NHANVIEN)
                var ten = db.NHANVIENs.FirstOrDefault(nv => nv.MSNV == item.MSNV)?.MSNV;
                if (ten != null)
                {
                    tenNhanVien.Add(ten);
                    tongTienNhan.Add(item.TongLuong ?? 0);
                }
            }

            // Biểu đồ cột
            var chart = new Chart(width: 600, height: 400)
                .AddTitle("Tổng Tiền Nhận Của Mỗi Nhân Viên")
                .AddSeries(
                    name: "Tổng Tiền Nhận",
                    xValue: tenNhanVien.ToArray(),
                    yValues: tongTienNhan.ToArray(),
                    chartType: "Column"
                );

            // Chuyển đổi biểu đồ thành hình ảnh để hiển thị trong view
            var chartBytes = chart.GetBytes();

            // Convert hình ảnh biểu đồ thành định dạng Base64 để nhúng vào HTML
            string chartBase64 = Convert.ToBase64String(chartBytes);
            string chartDataUrl = $"data:image/jpeg;base64,{chartBase64}";

            // Trả về view có chứa dữ liệu biểu đồ
            return View((object)chartDataUrl);
        }



        public ActionResult TongLuongCanTra()
        {
            var tongLuongTheoThang = db.LUONGs
                .Where(l => l.THANG != null)
                .GroupBy(l => new { l.THANG.Value.Year, l.THANG.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TongLuong = g.Sum(l => l.LCB + l.TONGPHUCAP + l.TONGTIENNHAN)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            // Tạo danh sách để lưu trữ tổng lương cần trả mỗi tháng
            List<int> tongLuong = new List<int>();
            // Tạo danh sách để lưu trữ tháng tương ứng
            List<string> thang = new List<string>();

            // Duyệt qua từng nhóm và lưu dữ liệu vào các danh sách
            foreach (var item in tongLuongTheoThang)
            {
                tongLuong.Add(item.TongLuong ?? 0); // Use null-coalescing operator to provide a default value if TongLuong is null
                thang.Add(new DateTime(item.Year, item.Month, 1).ToString("MM/yyyy"));
            }


            // Biểu đồ dạng đường tăng trưởng
            var chart = new Chart(width: 600, height: 400)
                .AddTitle("Biểu đồ tổng lương cần trả mỗi tháng")
                .AddSeries(
                    name: "Tổng lương",
                    chartType: "Line",
                    xValue: thang.ToArray(), // Biểu diễn tháng trên trục x
                    yValues: tongLuong.ToArray() // Biểu diễn tổng lương trên trục y
                 );

            // Chuyển đổi biểu đồ thành hình ảnh để hiển thị trong view
            var chartBytes = chart.GetBytes();

            // Convert hình ảnh biểu đồ thành định dạng Base64 để nhúng vào HTML
            string chartBase64 = Convert.ToBase64String(chartBytes);
            string chartDataUrl = $"data:image/jpeg;base64,{chartBase64}";

            // Trả về view có chứa dữ liệu biểu đồ
            return View("TongLuongCanTra", model: chartDataUrl);
        }
        public ActionResult TongLuongThangHienTaiView()
        {
            DateTime thangHienTai = DateTime.Now;

            // Check if thangHienTai is nullable and has a value
            if (thangHienTai != null)
            {
                var luongsThangHienTai = db.LUONGs.Where(l => l.THANG.Value.Month == thangHienTai.Month && l.THANG.Value.Year == thangHienTai.Year).ToList();

                // Use GetValueOrDefault() method to handle nullable integer
                int tongLuongThangHienTai = luongsThangHienTai.Sum(l => l.TONGTIENNHAN.GetValueOrDefault());

                ViewBag.TongLuongThangHienTai = tongLuongThangHienTai;

                return View();
            }
            else
            {
                // Handle the case when thangHienTai is null
                // You can return an error message or handle it based on your application logic
                return Content("Error: thangHienTai is null.");
            }
        }

    }
}