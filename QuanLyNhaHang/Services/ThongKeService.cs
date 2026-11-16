using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Services
{
    internal class ThongKeService
    {
        private readonly ThongKeRepository repository = new ThongKeRepository();

        public ThongKeBan GetThongKeBan()
        {
            return repository.GetThongKeBan();

        }
    }
}
