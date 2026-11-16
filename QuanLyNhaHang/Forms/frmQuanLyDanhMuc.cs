
using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;
using System.Collections.Generic;
using System.Linq;
using Krypton.Toolkit;
using System.Drawing;
namespace QuanLyNhaHang.Forms
{
    public partial class frmQuanLyDanhMuc : Form
    {
        private readonly LoaiMonService service =
            new LoaiMonService();

        private int maLoaiMonDangChon = 0;
        private List<LoaiMon> danhSachDanhMuc =
    new List<LoaiMon>();
        public frmQuanLyDanhMuc()
        {
            InitializeComponent();
        }

        // Load Form
        private void frmQuanLyDanhMuc_Load(object sender, EventArgs e)
        {
            CauHinhDataGridView();
            LoadDanhMuc();
            LamMoi();
        }

        // Cấu hình DataGridView
        private void CauHinhDataGridView()
        {
            dgvDanhMuc.AutoGenerateColumns = false;

            dgvDanhMuc.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvDanhMuc.MultiSelect = false;
            dgvDanhMuc.ReadOnly = true;
            dgvDanhMuc.AllowUserToAddRows = false;

            dgvDanhMuc.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // Ánh xạ dữ liệu Model vào các cột đã thiết kế
            dgvDanhMuc.Columns["MaDanhMuc"].DataPropertyName = "MaLoaiMon";
            dgvDanhMuc.Columns["TenDanhMuc"].DataPropertyName = "TenLoaiMon";
            dgvDanhMuc.Columns["NgayThem"].DataPropertyName = "NgayThem";

            dgvDanhMuc.Columns["NgayThem"].DefaultCellStyle.Format = "dd/MM/yyyy";
            // Giao diện bảng danh mục
            dgvDanhMuc.BackgroundColor = Color.White;
            dgvDanhMuc.BorderStyle = BorderStyle.None;
            dgvDanhMuc.GridColor = Color.FromArgb(220, 230, 240);

            dgvDanhMuc.EnableHeadersVisualStyles = false;

            dgvDanhMuc.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(0, 87, 153);

            dgvDanhMuc.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvDanhMuc.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvDanhMuc.ColumnHeadersHeight = 38;

            dgvDanhMuc.DefaultCellStyle.Font =
                new Font("Segoe UI", 10F);

            dgvDanhMuc.DefaultCellStyle.ForeColor =
                Color.FromArgb(45, 55, 65);

            dgvDanhMuc.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(214, 235, 250);

            dgvDanhMuc.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(0, 65, 110);

            dgvDanhMuc.RowTemplate.Height = 35;
            dgvDanhMuc.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 249, 252);
        }

        // Hiển thị danh sách danh mục
        private void LoadDanhMuc()
        {
            try
            {
                danhSachDanhMuc = service.GetAll();

                HienThiDanhSach(danhSachDanhMuc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh mục: " + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Hiển thị danh sách lên DataGridView
        private void HienThiDanhSach(List<LoaiMon> danhSach)
        {
            dgvDanhMuc.DataSource = null;
            dgvDanhMuc.DataSource = danhSach;

            dgvDanhMuc.ClearSelection();
            maLoaiMonDangChon = 0;
        }

        // Thêm danh mục
        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            try
            {
                LoaiMon loaiMon = new LoaiMon
                {
                    TenLoaiMon = txtTenLoaiMon.Text.Trim()
                };

                if (service.Them(loaiMon))
                {
                    MessageBox.Show("Thêm danh mục thành công!",
                        "Thông báo", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadDanhMuc();
                    LamMoi();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Tên danh mục đã tồn tại!");
                }
                else
                {
                    MessageBox.Show("Lỗi thêm danh mục: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Chọn danh mục trên DataGridView
       

        // Sửa danh mục
        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                LoaiMon loaiMon = new LoaiMon
                {
                    MaLoaiMon = maLoaiMonDangChon,
                    TenLoaiMon = txtTenLoaiMon.Text.Trim()
                };

                if (service.Sua(loaiMon))
                {
                    MessageBox.Show("Cập nhật danh mục thành công!",
                        "Thông báo", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadDanhMuc();
                    LamMoi();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy danh mục cần sửa!");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Tên danh mục đã tồn tại!");
                else
                    MessageBox.Show("Lỗi sửa danh mục: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Xóa danh mục
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (maLoaiMonDangChon <= 0)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa danh mục này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                if (service.Xoa(maLoaiMonDangChon))
                {
                    MessageBox.Show("Xóa danh mục thành công!",
                        "Thông báo", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadDanhMuc();
                    LamMoi();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy danh mục cần xóa!");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    MessageBox.Show(
                        "Không thể xóa danh mục vì đang có món ăn thuộc danh mục này!",
                        "Thông báo", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Lỗi xóa danh mục: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Làm mới
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void LamMoi()
        {
            maLoaiMonDangChon = 0;

            txtTenLoaiMon.Clear();

            dgvDanhMuc.ClearSelection();

            txtTenLoaiMon.Focus();
        }

        private void dgvDanhMuc_CellClick(object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDanhMuc.Rows[e.RowIndex];

                maLoaiMonDangChon = Convert.ToInt32(
                    row.Cells["MaDanhMuc"].Value);

                txtTenLoaiMon.Text =
                    row.Cells["TenDanhMuc"].Value.ToString();
            }
        }

        // Tìm kiếm danh mục
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            TimKiemDanhMuc();
        }

        // Xử lý tìm kiếm
        private void TimKiemDanhMuc()
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                HienThiDanhSach(danhSachDanhMuc);
                return;
            }

            List<LoaiMon> ketQua = danhSachDanhMuc
                .Where(x =>
                    x.TenLoaiMon != null &&
                    x.TenLoaiMon.IndexOf(
                        tuKhoa,
                        StringComparison.CurrentCultureIgnoreCase
                    ) >= 0
                )
                .ToList();

            HienThiDanhSach(ketQua);

            if (ketQua.Count == 0)
            {
                MessageBox.Show(
                    "Không tìm thấy danh mục phù hợp!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
