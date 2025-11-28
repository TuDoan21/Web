using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanGiay.Models;
using System.Data.Entity;

namespace QuanLyBanGiay.Controllers
{
    public class AdminController : Controller
    {
        QuanLyBanGiayEntities db = new QuanLyBanGiayEntities();

        // Middleware kiểm tra quyền Admin
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = Session["User"] as KhachHang;
            if (user == null || user.IsAdmin != true)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
            base.OnActionExecuting(filterContext);
        }

        // UC10: Dashboard & Báo cáo
        public ActionResult Dashboard()
        {
            // Data cho Chart: Doanh thu theo ngày
            return View();
        }

        public ActionResult GetRevenueData()
        {
            var data = db.HoaDons
                .Where(h => h.TrangThai == "Hoàn thành")
                .GroupBy(h => DbFunctions.TruncateTime(h.NgayLap))
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.TongTien) })
                .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // UC9: Quản lý đơn hàng
        public ActionResult ManageOrders()
        {
            var orders = db.HoaDons.Include(h => h.KhachHang).OrderByDescending(h => h.NgayLap).ToList();
            return View(orders);
        }

        public ActionResult UpdateStatus(int id)
        {
            var order = db.HoaDons.Find(id);
            if (order.TrangThai == "Chờ xác nhận") order.TrangThai = "Hoàn thành";
            else order.TrangThai = "Chờ xác nhận";
            db.SaveChanges();
            return RedirectToAction("ManageOrders");
        }

        // UC8: Quản lý sản phẩm (Ví dụ trang Index, bạn tự Add View Create/Edit bằng Scaffolding)
        public ActionResult ManageProducts()
        {
            return View(db.SanPhams.Include(s => s.LoaiGiay).ToList());
        }
    }
}