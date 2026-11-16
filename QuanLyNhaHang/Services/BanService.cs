using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;
using System.Collections.Generic;

namespace QuanLyNhaHang.Services
{
    public class BanService
    {
        private readonly BanRepository repository =
            new BanRepository();

        public List<Ban> GetAll()
        {
            return repository.GetAll();
        }

        public bool ThemBan(Ban ban)
        {
            return repository.Insert(ban);
        }
        public bool SuaBan(Ban ban)
        {
            return repository.Update(ban);
        }
    }
}