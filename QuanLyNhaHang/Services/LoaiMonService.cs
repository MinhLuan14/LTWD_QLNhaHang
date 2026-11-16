
using System;
using System.Collections.Generic;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class LoaiMonService
    {
        private readonly LoaiMonRepository repository =
            new LoaiMonRepository();

        // Lấy tất cả danh mục
        public List<LoaiMon> GetAll()
        {
            return repository.GetAll();
        }

        // Thêm danh mục
        public bool Them(LoaiMon loaiMon)
        {
            if (loaiMon == null)
                throw new Exception("Thông tin danh mục không hợp lệ!");

            if (string.IsNullOrWhiteSpace(loaiMon.TenLoaiMon))
                throw new Exception("Vui lòng nhập tên danh mục!");

            loaiMon.TenLoaiMon = loaiMon.TenLoaiMon.Trim();

            if (loaiMon.TenLoaiMon.Length > 100)
                throw new Exception("Tên danh mục không được vượt quá 100 ký tự!");

            return repository.Them(loaiMon);
        }

        // Sửa danh mục
        public bool Sua(LoaiMon loaiMon)
        {
            if (loaiMon == null || loaiMon.MaLoaiMon <= 0)
                throw new Exception("Vui lòng chọn danh mục cần sửa!");

            if (string.IsNullOrWhiteSpace(loaiMon.TenLoaiMon))
                throw new Exception("Vui lòng nhập tên danh mục!");

            loaiMon.TenLoaiMon = loaiMon.TenLoaiMon.Trim();

            if (loaiMon.TenLoaiMon.Length > 100)
                throw new Exception("Tên danh mục không được vượt quá 100 ký tự!");

            return repository.Sua(loaiMon);
        }

        // Xóa danh mục
        public bool Xoa(int maLoaiMon)
        {
            if (maLoaiMon <= 0)
                throw new Exception("Mã danh mục không hợp lệ!");

            return repository.Xoa(maLoaiMon);
        }
    }
}
