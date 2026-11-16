
using System;
using System.Collections.Generic;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class DoanhThuService
    {
        private readonly DoanhThuRepository repository =
            new DoanhThuRepository();

        public Dictionary<int, decimal> LayDoanhThuTheoGio(
            DateTime ngay)
        {
            return repository.LayDoanhThuTheoGio(ngay);
        }
    }
}
