namespace QuanLyNhaHang.Forms
{
    partial class frmChiTietDatBan
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tlpFooter = new System.Windows.Forms.TableLayoutPanel();
            this.lblTongSoChoNgoi = new System.Windows.Forms.Label();
            this.lblTongSoBan = new System.Windows.Forms.Label();
            this.btnInPhieu = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.btnHuyBan = new System.Windows.Forms.Button();
            this.btnHuyToanBo = new System.Windows.Forms.Button();
            this.pnlDanhSachBan = new System.Windows.Forms.Panel();
            this.dgvDanhSachBan = new System.Windows.Forms.DataGridView();
            this.maBan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.soBan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.soChoNgoi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trangThai = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTieuDeDanhSachBan = new System.Windows.Forms.Label();
            this.pnlThongTinDatBan = new System.Windows.Forms.Panel();
            this.tlpThongTinDatBan = new System.Windows.Forms.TableLayoutPanel();
            this.lblTieuDeDatBan = new System.Windows.Forms.Label();
            this.lblNgayDat = new System.Windows.Forms.Label();
            this.lblSoLuongKhach = new System.Windows.Forms.Label();
            this.lblGioDat = new System.Windows.Forms.Label();
            this.lblGhiChu = new System.Windows.Forms.Label();
            this.pnlThongTinKhachHang = new System.Windows.Forms.Panel();
            this.tlpThongTinKhachHang = new System.Windows.Forms.TableLayoutPanel();
            this.lblSoDienThoai = new System.Windows.Forms.Label();
            this.lblDiaChi = new System.Windows.Forms.Label();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.lblTieuDeKhachHang = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.lblMaDatBan = new System.Windows.Forms.Label();
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tlpFooter.SuspendLayout();
            this.pnlDanhSachBan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachBan)).BeginInit();
            this.pnlThongTinDatBan.SuspendLayout();
            this.tlpThongTinDatBan.SuspendLayout();
            this.pnlThongTinKhachHang.SuspendLayout();
            this.tlpThongTinKhachHang.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.pnlMain.Controls.Add(this.panel1);
            this.pnlMain.Controls.Add(this.pnlDanhSachBan);
            this.pnlMain.Controls.Add(this.pnlThongTinDatBan);
            this.pnlMain.Controls.Add(this.pnlThongTinKhachHang);
            this.pnlMain.Controls.Add(this.pnlHeader);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(5);
            this.pnlMain.Size = new System.Drawing.Size(782, 696);
            this.pnlMain.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tlpFooter);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(5, 601);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(772, 90);
            this.panel1.TabIndex = 4;
            // 
            // tlpFooter
            // 
            this.tlpFooter.BackColor = System.Drawing.Color.Transparent;
            this.tlpFooter.ColumnCount = 3;
            this.tlpFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.07001F));
            this.tlpFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.92999F));
            this.tlpFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tlpFooter.Controls.Add(this.lblTongSoChoNgoi, 0, 0);
            this.tlpFooter.Controls.Add(this.lblTongSoBan, 0, 1);
            this.tlpFooter.Controls.Add(this.btnInPhieu, 1, 1);
            this.tlpFooter.Controls.Add(this.btnDong, 2, 1);
            this.tlpFooter.Controls.Add(this.btnHuyBan, 1, 0);
            this.tlpFooter.Controls.Add(this.btnHuyToanBo, 2, 0);
            this.tlpFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tlpFooter.Location = new System.Drawing.Point(0, 1);
            this.tlpFooter.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.tlpFooter.Name = "tlpFooter";
            this.tlpFooter.Padding = new System.Windows.Forms.Padding(5);
            this.tlpFooter.RowCount = 2;
            this.tlpFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpFooter.Size = new System.Drawing.Size(772, 89);
            this.tlpFooter.TabIndex = 11;
            // 
            // lblTongSoChoNgoi
            // 
            this.lblTongSoChoNgoi.AutoSize = true;
            this.lblTongSoChoNgoi.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTongSoChoNgoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongSoChoNgoi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.lblTongSoChoNgoi.Location = new System.Drawing.Point(8, 5);
            this.lblTongSoChoNgoi.Name = "lblTongSoChoNgoi";
            this.lblTongSoChoNgoi.Size = new System.Drawing.Size(418, 20);
            this.lblTongSoChoNgoi.TabIndex = 7;
            this.lblTongSoChoNgoi.Text = "\tTổng sức chứa";
            // 
            // lblTongSoBan
            // 
            this.lblTongSoBan.AutoSize = true;
            this.lblTongSoBan.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTongSoBan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongSoBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.lblTongSoBan.Location = new System.Drawing.Point(8, 44);
            this.lblTongSoBan.Name = "lblTongSoBan";
            this.lblTongSoBan.Size = new System.Drawing.Size(418, 20);
            this.lblTongSoBan.TabIndex = 6;
            this.lblTongSoBan.Text = "Tổng số bàn";
            // 
            // btnInPhieu
            // 
            this.btnInPhieu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.btnInPhieu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInPhieu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnInPhieu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInPhieu.ForeColor = System.Drawing.Color.White;
            this.btnInPhieu.Location = new System.Drawing.Point(432, 47);
            this.btnInPhieu.Name = "btnInPhieu";
            this.btnInPhieu.Size = new System.Drawing.Size(166, 34);
            this.btnInPhieu.TabIndex = 8;
            this.btnInPhieu.Text = "In phiếu";
            this.btnInPhieu.UseVisualStyleBackColor = false;
            // 
            // btnDong
            // 
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnDong.Location = new System.Drawing.Point(604, 47);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(160, 34);
            this.btnDong.TabIndex = 9;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = true;
            // 
            // btnHuyBan
            // 
            this.btnHuyBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.btnHuyBan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnHuyBan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHuyBan.ForeColor = System.Drawing.Color.White;
            this.btnHuyBan.Location = new System.Drawing.Point(432, 8);
            this.btnHuyBan.Name = "btnHuyBan";
            this.btnHuyBan.Size = new System.Drawing.Size(166, 33);
            this.btnHuyBan.TabIndex = 10;
            this.btnHuyBan.Text = "\tHủy bàn đã chọn";
            this.btnHuyBan.UseVisualStyleBackColor = false;
            this.btnHuyBan.Click += new System.EventHandler(this.btnHuyBan_Click);
            // 
            // btnHuyToanBo
            // 
            this.btnHuyToanBo.BackColor = System.Drawing.Color.Red;
            this.btnHuyToanBo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnHuyToanBo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHuyToanBo.ForeColor = System.Drawing.Color.White;
            this.btnHuyToanBo.Location = new System.Drawing.Point(604, 8);
            this.btnHuyToanBo.Name = "btnHuyToanBo";
            this.btnHuyToanBo.Size = new System.Drawing.Size(160, 33);
            this.btnHuyToanBo.TabIndex = 11;
            this.btnHuyToanBo.Text = "Hủy toàn bộ bàn";
            this.btnHuyToanBo.UseVisualStyleBackColor = false;
            this.btnHuyToanBo.Click += new System.EventHandler(this.btnHuyToanBo_Click);
            // 
            // pnlDanhSachBan
            // 
            this.pnlDanhSachBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.pnlDanhSachBan.Controls.Add(this.dgvDanhSachBan);
            this.pnlDanhSachBan.Controls.Add(this.lblTieuDeDanhSachBan);
            this.pnlDanhSachBan.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDanhSachBan.Location = new System.Drawing.Point(5, 390);
            this.pnlDanhSachBan.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.pnlDanhSachBan.Name = "pnlDanhSachBan";
            this.pnlDanhSachBan.Padding = new System.Windows.Forms.Padding(15, 12, 15, 12);
            this.pnlDanhSachBan.Size = new System.Drawing.Size(772, 263);
            this.pnlDanhSachBan.TabIndex = 3;
            // 
            // dgvDanhSachBan
            // 
            this.dgvDanhSachBan.AllowUserToAddRows = false;
            this.dgvDanhSachBan.AllowUserToDeleteRows = false;
            this.dgvDanhSachBan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDanhSachBan.BackgroundColor = System.Drawing.Color.White;
            this.dgvDanhSachBan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDanhSachBan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDanhSachBan.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.maBan,
            this.soBan,
            this.soChoNgoi,
            this.trangThai});
            this.dgvDanhSachBan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDanhSachBan.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvDanhSachBan.Location = new System.Drawing.Point(15, 37);
            this.dgvDanhSachBan.MultiSelect = false;
            this.dgvDanhSachBan.Name = "dgvDanhSachBan";
            this.dgvDanhSachBan.ReadOnly = true;
            this.dgvDanhSachBan.RowHeadersVisible = false;
            this.dgvDanhSachBan.RowHeadersWidth = 51;
            this.dgvDanhSachBan.RowTemplate.Height = 40;
            this.dgvDanhSachBan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDanhSachBan.Size = new System.Drawing.Size(742, 214);
            this.dgvDanhSachBan.TabIndex = 10;
            // 
            // maBan
            // 
            this.maBan.DataPropertyName = "MaBan";
            this.maBan.HeaderText = "Mã bàn";
            this.maBan.MinimumWidth = 6;
            this.maBan.Name = "maBan";
            this.maBan.ReadOnly = true;
            // 
            // soBan
            // 
            this.soBan.DataPropertyName = "SoBan";
            this.soBan.HeaderText = "Tên bàn";
            this.soBan.MinimumWidth = 6;
            this.soBan.Name = "soBan";
            this.soBan.ReadOnly = true;
            // 
            // soChoNgoi
            // 
            this.soChoNgoi.DataPropertyName = "SoChoNgoi";
            this.soChoNgoi.HeaderText = "Số chỗ ngồi";
            this.soChoNgoi.MinimumWidth = 6;
            this.soChoNgoi.Name = "soChoNgoi";
            this.soChoNgoi.ReadOnly = true;
            // 
            // trangThai
            // 
            this.trangThai.DataPropertyName = "TrangThai";
            this.trangThai.HeaderText = "Trạng thái";
            this.trangThai.MinimumWidth = 6;
            this.trangThai.Name = "trangThai";
            this.trangThai.ReadOnly = true;
            // 
            // lblTieuDeDanhSachBan
            // 
            this.lblTieuDeDanhSachBan.AutoSize = true;
            this.lblTieuDeDanhSachBan.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTieuDeDanhSachBan.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDeDanhSachBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.lblTieuDeDanhSachBan.Location = new System.Drawing.Point(15, 12);
            this.lblTieuDeDanhSachBan.Name = "lblTieuDeDanhSachBan";
            this.lblTieuDeDanhSachBan.Size = new System.Drawing.Size(245, 25);
            this.lblTieuDeDanhSachBan.TabIndex = 5;
            this.lblTieuDeDanhSachBan.Text = "DANH SÁCH BÀN ĐÃ ĐẶT";
            // 
            // pnlThongTinDatBan
            // 
            this.pnlThongTinDatBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.pnlThongTinDatBan.Controls.Add(this.tlpThongTinDatBan);
            this.pnlThongTinDatBan.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlThongTinDatBan.Location = new System.Drawing.Point(5, 235);
            this.pnlThongTinDatBan.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.pnlThongTinDatBan.Name = "pnlThongTinDatBan";
            this.pnlThongTinDatBan.Padding = new System.Windows.Forms.Padding(15, 12, 15, 12);
            this.pnlThongTinDatBan.Size = new System.Drawing.Size(772, 155);
            this.pnlThongTinDatBan.TabIndex = 2;
            // 
            // tlpThongTinDatBan
            // 
            this.tlpThongTinDatBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.tlpThongTinDatBan.ColumnCount = 2;
            this.tlpThongTinDatBan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinDatBan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinDatBan.Controls.Add(this.lblTieuDeDatBan, 0, 0);
            this.tlpThongTinDatBan.Controls.Add(this.lblNgayDat, 0, 1);
            this.tlpThongTinDatBan.Controls.Add(this.lblSoLuongKhach, 0, 2);
            this.tlpThongTinDatBan.Controls.Add(this.lblGioDat, 1, 0);
            this.tlpThongTinDatBan.Controls.Add(this.lblGhiChu, 1, 1);
            this.tlpThongTinDatBan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpThongTinDatBan.Location = new System.Drawing.Point(15, 12);
            this.tlpThongTinDatBan.Name = "tlpThongTinDatBan";
            this.tlpThongTinDatBan.Padding = new System.Windows.Forms.Padding(5);
            this.tlpThongTinDatBan.RowCount = 3;
            this.tlpThongTinDatBan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinDatBan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinDatBan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tlpThongTinDatBan.Size = new System.Drawing.Size(742, 131);
            this.tlpThongTinDatBan.TabIndex = 0;
            // 
            // lblTieuDeDatBan
            // 
            this.lblTieuDeDatBan.AutoSize = true;
            this.lblTieuDeDatBan.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDeDatBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.lblTieuDeDatBan.Location = new System.Drawing.Point(8, 5);
            this.lblTieuDeDatBan.Name = "lblTieuDeDatBan";
            this.lblTieuDeDatBan.Size = new System.Drawing.Size(206, 25);
            this.lblTieuDeDatBan.TabIndex = 4;
            this.lblTieuDeDatBan.Text = "THÔNG TIN ĐẶT BÀN";
            // 
            // lblNgayDat
            // 
            this.lblNgayDat.AutoSize = true;
            this.lblNgayDat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNgayDat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblNgayDat.Location = new System.Drawing.Point(8, 52);
            this.lblNgayDat.Name = "lblNgayDat";
            this.lblNgayDat.Size = new System.Drawing.Size(80, 23);
            this.lblNgayDat.TabIndex = 1;
            this.lblNgayDat.Text = "Ngày đặt";
            // 
            // lblSoLuongKhach
            // 
            this.lblSoLuongKhach.AutoSize = true;
            this.lblSoLuongKhach.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoLuongKhach.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblSoLuongKhach.Location = new System.Drawing.Point(8, 99);
            this.lblSoLuongKhach.Name = "lblSoLuongKhach";
            this.lblSoLuongKhach.Size = new System.Drawing.Size(128, 23);
            this.lblSoLuongKhach.TabIndex = 2;
            this.lblSoLuongKhach.Text = "Số lượng khách";
            // 
            // lblGioDat
            // 
            this.lblGioDat.AutoSize = true;
            this.lblGioDat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGioDat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblGioDat.Location = new System.Drawing.Point(374, 5);
            this.lblGioDat.Name = "lblGioDat";
            this.lblGioDat.Size = new System.Drawing.Size(66, 23);
            this.lblGioDat.TabIndex = 3;
            this.lblGioDat.Text = "Giờ đặt";
            // 
            // lblGhiChu
            // 
            this.lblGhiChu.AutoSize = true;
            this.lblGhiChu.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGhiChu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblGhiChu.Location = new System.Drawing.Point(374, 52);
            this.lblGhiChu.Name = "lblGhiChu";
            this.lblGhiChu.Size = new System.Drawing.Size(69, 23);
            this.lblGhiChu.TabIndex = 5;
            this.lblGhiChu.Text = "Ghi chú";
            // 
            // pnlThongTinKhachHang
            // 
            this.pnlThongTinKhachHang.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.pnlThongTinKhachHang.Controls.Add(this.tlpThongTinKhachHang);
            this.pnlThongTinKhachHang.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlThongTinKhachHang.Location = new System.Drawing.Point(5, 90);
            this.pnlThongTinKhachHang.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.pnlThongTinKhachHang.Name = "pnlThongTinKhachHang";
            this.pnlThongTinKhachHang.Padding = new System.Windows.Forms.Padding(15, 12, 15, 12);
            this.pnlThongTinKhachHang.Size = new System.Drawing.Size(772, 145);
            this.pnlThongTinKhachHang.TabIndex = 1;
            // 
            // tlpThongTinKhachHang
            // 
            this.tlpThongTinKhachHang.ColumnCount = 2;
            this.tlpThongTinKhachHang.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinKhachHang.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinKhachHang.Controls.Add(this.lblSoDienThoai, 1, 0);
            this.tlpThongTinKhachHang.Controls.Add(this.lblDiaChi, 1, 1);
            this.tlpThongTinKhachHang.Controls.Add(this.lblHoTen, 0, 1);
            this.tlpThongTinKhachHang.Controls.Add(this.lblTieuDeKhachHang, 0, 0);
            this.tlpThongTinKhachHang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpThongTinKhachHang.Location = new System.Drawing.Point(15, 12);
            this.tlpThongTinKhachHang.Name = "tlpThongTinKhachHang";
            this.tlpThongTinKhachHang.Padding = new System.Windows.Forms.Padding(5);
            this.tlpThongTinKhachHang.RowCount = 2;
            this.tlpThongTinKhachHang.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinKhachHang.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpThongTinKhachHang.Size = new System.Drawing.Size(742, 121);
            this.tlpThongTinKhachHang.TabIndex = 0;
            // 
            // lblSoDienThoai
            // 
            this.lblSoDienThoai.AutoSize = true;
            this.lblSoDienThoai.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDienThoai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblSoDienThoai.Location = new System.Drawing.Point(374, 5);
            this.lblSoDienThoai.Name = "lblSoDienThoai";
            this.lblSoDienThoai.Size = new System.Drawing.Size(111, 23);
            this.lblSoDienThoai.TabIndex = 2;
            this.lblSoDienThoai.Text = "Số điện thoại";
            // 
            // lblDiaChi
            // 
            this.lblDiaChi.AutoSize = true;
            this.lblDiaChi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiaChi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblDiaChi.Location = new System.Drawing.Point(374, 60);
            this.lblDiaChi.Name = "lblDiaChi";
            this.lblDiaChi.Size = new System.Drawing.Size(62, 23);
            this.lblDiaChi.TabIndex = 3;
            this.lblDiaChi.Text = "Địa chỉ";
            // 
            // lblHoTen
            // 
            this.lblHoTen.AutoSize = true;
            this.lblHoTen.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoTen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblHoTen.Location = new System.Drawing.Point(8, 60);
            this.lblHoTen.Name = "lblHoTen";
            this.lblHoTen.Size = new System.Drawing.Size(156, 23);
            this.lblHoTen.TabIndex = 1;
            this.lblHoTen.Text = "Họ tên khách hàng";
            // 
            // lblTieuDeKhachHang
            // 
            this.lblTieuDeKhachHang.AutoSize = true;
            this.lblTieuDeKhachHang.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDeKhachHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.lblTieuDeKhachHang.Location = new System.Drawing.Point(8, 5);
            this.lblTieuDeKhachHang.Name = "lblTieuDeKhachHang";
            this.lblTieuDeKhachHang.Size = new System.Drawing.Size(252, 25);
            this.lblTieuDeKhachHang.TabIndex = 0;
            this.lblTieuDeKhachHang.Text = "THÔNG TIN KHÁCH HÀNG";
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(87)))), ((int)(((byte)(155)))));
            this.pnlHeader.Controls.Add(this.lblTrangThai);
            this.pnlHeader.Controls.Add(this.lblMaDatBan);
            this.pnlHeader.Controls.Add(this.lblTieuDe);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(5, 5);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(15, 12, 15, 12);
            this.pnlHeader.Size = new System.Drawing.Size(772, 85);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.BackColor = System.Drawing.Color.DarkOrange;
            this.lblTrangThai.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrangThai.ForeColor = System.Drawing.Color.White;
            this.lblTrangThai.Location = new System.Drawing.Point(850, 27);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(64, 23);
            this.lblTrangThai.TabIndex = 2;
            this.lblTrangThai.Text = "Đã đặt";
            // 
            // lblMaDatBan
            // 
            this.lblMaDatBan.AutoSize = true;
            this.lblMaDatBan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblMaDatBan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)), true);
            this.lblMaDatBan.ForeColor = System.Drawing.Color.White;
            this.lblMaDatBan.Location = new System.Drawing.Point(15, 50);
            this.lblMaDatBan.Name = "lblMaDatBan";
            this.lblMaDatBan.Size = new System.Drawing.Size(140, 23);
            this.lblMaDatBan.TabIndex = 1;
            this.lblMaDatBan.Text = "Mã phiếu: DB001";
            // 
            // lblTieuDe
            // 
            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTieuDe.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDe.ForeColor = System.Drawing.Color.White;
            this.lblTieuDe.Location = new System.Drawing.Point(15, 12);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(332, 41);
            this.lblTieuDe.TabIndex = 0;
            this.lblTieuDe.Text = "CHI TIẾT BÀN ĐÃ ĐẶT";
            // 
            // frmChiTietDatBan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(782, 696);
            this.Controls.Add(this.pnlMain);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 680);
            this.Name = "frmChiTietDatBan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chi tiết đặt bàn";
            this.Load += new System.EventHandler(this.frmChiTietDatBan_Load);
            this.pnlMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tlpFooter.ResumeLayout(false);
            this.tlpFooter.PerformLayout();
            this.pnlDanhSachBan.ResumeLayout(false);
            this.pnlDanhSachBan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachBan)).EndInit();
            this.pnlThongTinDatBan.ResumeLayout(false);
            this.tlpThongTinDatBan.ResumeLayout(false);
            this.tlpThongTinDatBan.PerformLayout();
            this.pnlThongTinKhachHang.ResumeLayout(false);
            this.tlpThongTinKhachHang.ResumeLayout(false);
            this.tlpThongTinKhachHang.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlThongTinKhachHang;
        private System.Windows.Forms.Panel pnlThongTinDatBan;
        private System.Windows.Forms.Panel pnlDanhSachBan;
        private System.Windows.Forms.Label lblTrangThai;
        private System.Windows.Forms.Label lblMaDatBan;
        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.TableLayoutPanel tlpThongTinKhachHang;
        private System.Windows.Forms.Label lblTieuDeKhachHang;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.Label lblSoDienThoai;
        private System.Windows.Forms.Label lblDiaChi;
        private System.Windows.Forms.TableLayoutPanel tlpThongTinDatBan;
        private System.Windows.Forms.Label lblNgayDat;
        private System.Windows.Forms.Label lblSoLuongKhach;
        private System.Windows.Forms.Label lblGioDat;
        private System.Windows.Forms.Label lblTieuDeDatBan;
        private System.Windows.Forms.Label lblGhiChu;
        private System.Windows.Forms.Label lblTieuDeDanhSachBan;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvDanhSachBan;
        private System.Windows.Forms.TableLayoutPanel tlpFooter;
        private System.Windows.Forms.Label lblTongSoChoNgoi;
        private System.Windows.Forms.Label lblTongSoBan;
        private System.Windows.Forms.Button btnInPhieu;
        private System.Windows.Forms.Button btnDong;
        private System.Windows.Forms.DataGridViewTextBoxColumn maBan;
        private System.Windows.Forms.DataGridViewTextBoxColumn soBan;
        private System.Windows.Forms.DataGridViewTextBoxColumn soChoNgoi;
        private System.Windows.Forms.DataGridViewTextBoxColumn trangThai;
        private System.Windows.Forms.Button btnHuyBan;
        private System.Windows.Forms.Button btnHuyToanBo;
    }
}