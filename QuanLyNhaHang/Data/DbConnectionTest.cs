using System;
using System.Data.SqlClient;

namespace QuanLyNhaHang.Data
{
    public static class DbConnectionTest
    {
        public static string TestConnection()
        {
            try
            {
                DbConnection db = new DbConnection();

                using (SqlConnection connection = db.GetConnection())
                {
                    connection.Open();
                    return "Kết nối database thành công!";
                }
            }
            catch (Exception ex)
            {
                return "Kết nối thất bại: " + ex.Message;
            }
        }
    }
}