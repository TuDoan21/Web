using QuanLyBanGiay.Models;
using QuanLyBanGiay.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyBanGiay.Controllers
{
    public class CartController : Controller
    {
        QuanLyBanGiayEntities db = new QuanLyBanGiayEntities();

        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<CartItemViewModel> ?? new List<CartItemViewModel>();
            return View(cart);
        }

        // 2. Hàm thêm vào giỏ (Quan trọng)
        public ActionResult AddToCart(int id)
        {
            var sp = db.SanPhams.Find(id);
            if (sp == null) return HttpNotFound(); // Lỗi nếu không tìm thấy ID giày

            // Lấy giỏ hàng từ Session, nếu chưa có thì tạo mới
            var cart = Session["Cart"] as List<CartItemViewModel> ?? new List<CartItemViewModel>();

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var item = cart.FirstOrDefault(c => c.MaSP == id);

            if (item != null)
            {
                item.SoLuong++; // Có rồi thì tăng số lượng
            }
            else
            {
                // Chưa có thì thêm mới
                cart.Add(new CartItemViewModel
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    Gia = sp.Gia,
                    AnhUrl = sp.AnhUrl,
                    SoLuong = 1
                });
            }

            // Lưu ngược lại vào Session
            Session["Cart"] = cart;

            // Chuyển hướng về trang xem giỏ hàng
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateCart(int id, int soluong)
        {
            // 1. Lấy giỏ hàng từ Session
            var cart = Session["Cart"] as List<CartItemViewModel>;

            // 2. Kiểm tra nếu giỏ hàng rỗng thì thoát
            if (cart == null) return RedirectToAction("Index");

            // 3. Tìm sản phẩm trong giỏ theo ID
            var item = cart.FirstOrDefault(c => c.MaSP == id);

            if (item != null)
            {
                // 4. Cập nhật số lượng
                item.SoLuong = soluong;

                // 5. Nếu số lượng <= 0 thì xóa sản phẩm khỏi giỏ (Logic cho nút Xóa)
                if (item.SoLuong <= 0)
                {
                    cart.Remove(item);
                }
            }

            // 6. Lưu lại Session (quan trọng)
            Session["Cart"] = cart;

            // 7. Load lại trang
            return RedirectToAction("Index");
        }
        // 1. Trang xác nhận thanh toán (GET)
        [HttpGet]
        public ActionResult Checkout()
        {
            // Kiểm tra đăng nhập
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng
            var cart = Session["Cart"] as List<CartItemViewModel>;
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index"); // Giỏ trống thì về trang giỏ hàng
            }

            // Lấy thông tin user để điền sẵn vào form
            var user = Session["User"] as KhachHang;
            ViewBag.User = user;

            return View(cart); // Truyền giỏ hàng sang View để hiển thị lại lần cuối
        }

        // 2. Xử lý lưu đơn hàng (POST)
        [HttpPost]
        public ActionResult Checkout(string NguoiNhan, string SDT, string DiaChi)
        {
            var user = Session["User"] as KhachHang;
            var cart = Session["Cart"] as List<CartItemViewModel>;

            if (user == null || cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            // A. Tạo hóa đơn (Order)
            HoaDon hd = new HoaDon();
            hd.MaKH = user.MaKH;
            hd.NgayLap = DateTime.Now;
            hd.TrangThai = "Chờ xác nhận";
            hd.PhiShip = 20000;

            // Tính tổng tiền = Tổng tiền hàng + Ship
            decimal tongTienHang = cart.Sum(item => item.ThanhTien);
            hd.TongTien = tongTienHang + (decimal)hd.PhiShip;

            // Lưu thông tin giao hàng (có thể khác thông tin đăng ký)
            hd.DiaChi = DiaChi; // Lưu địa chỉ giao hàng người dùng nhập
            hd.SDT = SDT;       // Lưu SĐT giao hàng

            db.HoaDons.Add(hd);
            db.SaveChanges(); // Lưu để lấy được hd.MaHD

            // B. Tạo chi tiết hóa đơn (Order Details)
            foreach (var item in cart)
            {
                ChiTietHoaDon ct = new ChiTietHoaDon();
                ct.MaHD = hd.MaHD;
                ct.MaSP = item.MaSP;
                ct.SoLuong = item.SoLuong;
                ct.DonGia = item.Gia;

                db.ChiTietHoaDons.Add(ct);
            }
            db.SaveChanges();

            // C. Xóa giỏ hàng và chuyển hướng
            Session["Cart"] = null;
            return RedirectToAction("OrderSuccess");
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }
        public ActionResult MyOrders()
        {
            // 1. Kiểm tra đăng nhập
            var user = Session["User"] as QuanLyBanGiay.Models.KhachHang;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Lấy danh sách hóa đơn của khách hàng đó (Sắp xếp mới nhất lên đầu)
            var orders = db.HoaDons
                           .Where(h => h.MaKH == user.MaKH)
                           .OrderByDescending(h => h.NgayLap)
                           .ToList();

            return View(orders);
        }
    }
}