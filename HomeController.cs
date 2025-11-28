using System.Linq;
using System.Web.Mvc;
using System.Data.Entity; // Cần dòng này
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Controllers
{
    public class HomeController : Controller
    {
        QuanLyBanGiayEntities db = new QuanLyBanGiayEntities();

        // 1. Cập nhật hàm Index: Thêm tham số maLoai
        public ActionResult Index(string search = "", int? maLoai = null)
        {
            // Lấy danh sách sản phẩm
            var query = db.SanPhams.Include(s => s.LoaiGiay).AsQueryable();

            // Lọc theo tên (Tìm kiếm)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.TenSP.Contains(search) || s.ThuongHieu.Contains(search));
            }

            // Lọc theo Danh mục (Loại giày)
            if (maLoai != null)
            {
                query = query.Where(s => s.MaLoai == maLoai);
            }

            return View(query.ToList());
        }

        // 2. Thêm hàm này để hiển thị Menu trên Navbar (Gọi là Child Action)
        [ChildActionOnly]
        public ActionResult CategoryMenu()
        {
            var loais = db.LoaiGiays.ToList();
            return PartialView("_CategoryMenu", loais);
        }
    }
}