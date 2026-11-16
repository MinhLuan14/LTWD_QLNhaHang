using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Models
{
    public class NhanVien
    {
        public int MaNhanVien { get; set; }

        public string HoVaTenLot { get; set; }

        public string TenNhanVien { get; set; }

        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        public int MaTaiKhoan { get; set; }
    }
}