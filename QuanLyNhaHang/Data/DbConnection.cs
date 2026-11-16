
using System;
using System.Data.SqlClient;
using System.Configuration;

namespace QuanLyNhaHang.Data
{
    public class DbConnection
    {
        private readonly string connectionString;

        public DbConnection()
        {
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[
                    "QuanLyNhaHangConnection"
                ];

            if (settings == null ||
                string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Không tìm thấy chuỗi kết nối QuanLyNhaHangConnection trong App.config!"
                );
            }

            connectionString = settings.ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
