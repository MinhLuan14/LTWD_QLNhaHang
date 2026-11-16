using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace QuanLyNhaHang
{
    [Serializable]
    public sealed class RestaurantData
    {
        public List<UserAccount> Users { get; set; } = new List<UserAccount>();
        public List<RestaurantTable> Tables { get; set; } = new List<RestaurantTable>();
        public List<MenuItem> Menu { get; set; } = new List<MenuItem>();
        public List<RestaurantOrder> Orders { get; set; } = new List<RestaurantOrder>();
    }

    [Serializable]
    public sealed class UserAccount
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
    }

    [Serializable]
    public sealed class RestaurantTable
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int Guests { get; set; }
    }

    [Serializable]
    public sealed class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }

    [Serializable]
    public sealed class OrderLine
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    [Serializable]
    public sealed class RestaurantOrder
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();
        public decimal Total { get { return Lines == null ? 0 : Lines.Sum(line => line.UnitPrice * line.Quantity); } }
    }

    public static class RestaurantStore
    {
        private static readonly object Sync = new object();
        private static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuanLyNhaHang", "restaurant-data.xml");
        private static RestaurantData data;

        public static RestaurantData Data
        {
            get
            {
                lock (Sync)
                {
                    if (data == null) data = Load();
                    return data;
                }
            }
        }

        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty)));
        }

        public static void Save()
        {
            lock (Sync)
            {
                string directory = Path.GetDirectoryName(DataPath);
                Directory.CreateDirectory(directory);
                string temporary = DataPath + ".tmp";
                var serializer = new XmlSerializer(typeof(RestaurantData));
                using (var stream = File.Create(temporary)) serializer.Serialize(stream, Data);
                if (File.Exists(DataPath)) File.Replace(temporary, DataPath, null);
                else File.Move(temporary, DataPath);
            }
        }

        public static string Money(decimal amount)
        {
            return amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ";
        }

        private static RestaurantData Load()
        {
            if (File.Exists(DataPath))
            {
                try
                {
                    using (var stream = File.OpenRead(DataPath))
                        return (RestaurantData)new XmlSerializer(typeof(RestaurantData)).Deserialize(stream);
                }
                catch
                {
                    string backup = DataPath + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    try { File.Move(DataPath, backup); } catch { }
                }
            }

            var result = new RestaurantData();
            result.Users.Add(new UserAccount { Username = "admin", PasswordHash = HashPassword("123456"), FullName = "Quản trị viên" });
            for (int i = 1; i <= 12; i++)
                result.Tables.Add(new RestaurantTable { Id = i, Status = "Trống", Guests = 4 });

            string[] categories = { "Món chính", "Món chính", "Món chính", "Khai vị", "Khai vị", "Đồ uống", "Đồ uống", "Tráng miệng" };
            string[] names = { "Cơm chiên hải sản", "Bò lúc lắc", "Gà nướng mật ong", "Gỏi cuốn tôm thịt", "Khoai tây chiên", "Trà đào", "Nước suối", "Chè khúc bạch" };
            decimal[] prices = { 85000, 125000, 145000, 55000, 45000, 35000, 15000, 40000 };
            for (int i = 0; i < names.Length; i++)
                result.Menu.Add(new MenuItem { Id = i + 1, Name = names[i], Category = categories[i], Price = prices[i] });

            return result;
        }
    }
}
