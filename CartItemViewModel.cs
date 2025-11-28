using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyBanGiay.ViewModels
{
    public class CartItemViewModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string AnhUrl { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien
        {
            get { return SoLuong * Gia; }
        }
    }
}