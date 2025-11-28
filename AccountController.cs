using QuanLyBanGiay.Models;
using QuanLyBanGiay.Helpers; // Nhớ dòng này để dùng SecurityHelper
using System;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyBanGiay.Controllers
{
    public class AccountController : Controller
    {
        private QuanLyBanGiayEntities db = new QuanLyBanGiayEntities();

        // --- ĐĂNG KÝ (Register) ---
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string Ten, string Email, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // Kiểm tra Email đã tồn tại chưa
            if (db.KhachHangs.Any(x => x.Email == Email))
            {
                ViewBag.Error = "Email này đã được đăng ký!";
                return View();
            }

            // Tạo người dùng mới
            var newUser = new KhachHang
            {
                Ten = Ten,
                Email = Email,
                // Mã hóa mật khẩu
                PasswordHash = SecurityHelper.HashPassword(Password),
                IsAdmin = false, // Mặc định là khách
                CreatedAt = DateTime.Now,
                SDT = "",     // Để trống tạm
                DiaChi = ""   // Để trống tạm
            };

            db.KhachHangs.Add(newUser);
            db.SaveChanges();

            // Đăng ký xong chuyển sang đăng nhập
            return RedirectToAction("Login");
        }

        // --- ĐĂNG NHẬP (Login) ---
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            // Mã hóa mật khẩu người dùng nhập vào để so sánh
            var inputHash = SecurityHelper.HashPassword(Password);

            // Tìm user theo Email
            var user = db.KhachHangs.FirstOrDefault(u => u.Email == Email);

            // Kiểm tra: Có user VÀ Mật khẩu băm khớp nhau (so sánh mảng byte)
            if (user != null && user.PasswordHash.SequenceEqual(inputHash))
            {
                // Lưu thông tin vào Session
                Session["User"] = user;
                Session["UserName"] = user.Ten;
                Session["IsAdmin"] = user.IsAdmin;

                // Nếu là Admin thì vào trang quản lý, ngược lại về trang chủ
                if (user.IsAdmin == true)
                {
                    // Giả sử bạn có AdminController
                    // return RedirectToAction("Dashboard", "Admin");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Sai Email hoặc Mật khẩu!";
            return View();
        }

        // --- ĐĂNG XUẤT (Logout) ---
        public ActionResult Logout()
        {
            Session.Clear(); 
            return RedirectToAction("Index", "Home");
        }
    }
}