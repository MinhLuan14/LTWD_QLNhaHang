namespace QuanLyNhaHang
{
    partial class ucMonAn
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.btnThemMon = new System.Windows.Forms.Button();
            this.lblDonGia = new System.Windows.Forms.Label();
            this.lblTenMon = new System.Windows.Forms.Label();
            this.picHinhMon = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHinhMon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.panel1.Controls.Add(this.lblTrangThai);
            this.panel1.Controls.Add(this.btnThemMon);
            this.panel1.Controls.Add(this.lblDonGia);
            this.panel1.Controls.Add(this.lblTenMon);
            this.panel1.Controls.Add(this.picHinhMon);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(190, 200);
            this.panel1.TabIndex = 0;
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblTrangThai.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrangThai.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTrangThai.Location = new System.Drawing.Point(114, 5);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(73, 17);
            this.lblTrangThai.TabIndex = 4;
            this.lblTrangThai.Text = "Trạng thái";
            this.lblTrangThai.Click += new System.EventHandler(this.lblTrangThai_Click);
            // 
            // btnThemMon
            // 
            this.btnThemMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.btnThemMon.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnThemMon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThemMon.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnThemMon.Location = new System.Drawing.Point(5, 160);
            this.btnThemMon.Name = "btnThemMon";
            this.btnThemMon.Size = new System.Drawing.Size(180, 35);
            this.btnThemMon.TabIndex = 3;
            this.btnThemMon.Text = "Thêm Món";
            this.btnThemMon.UseVisualStyleBackColor = false;
            this.btnThemMon.Click += new System.EventHandler(this.btnThemMon_Click);
            // 
            // lblDonGia
            // 
            this.lblDonGia.AutoSize = true;
            this.lblDonGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDonGia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.lblDonGia.Location = new System.Drawing.Point(8, 138);
            this.lblDonGia.Name = "lblDonGia";
            this.lblDonGia.Size = new System.Drawing.Size(79, 20);
            this.lblDonGia.TabIndex = 2;
            this.lblDonGia.Text = "Giá món";
            // 
            // lblTenMon
            // 
            this.lblTenMon.AutoSize = true;
            this.lblTenMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.lblTenMon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenMon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(74)))), ((int)(((byte)(115)))));
            this.lblTenMon.Location = new System.Drawing.Point(8, 118);
            this.lblTenMon.Name = "lblTenMon";
            this.lblTenMon.Size = new System.Drawing.Size(81, 20);
            this.lblTenMon.TabIndex = 1;
            this.lblTenMon.Text = "Tên Món";
            // 
            // picHinhMon
            // 
            this.picHinhMon.Dock = System.Windows.Forms.DockStyle.Top;
            this.picHinhMon.Location = new System.Drawing.Point(5, 5);
            this.picHinhMon.Name = "picHinhMon";
            this.picHinhMon.Size = new System.Drawing.Size(180, 110);
            this.picHinhMon.TabIndex = 0;
            this.picHinhMon.TabStop = false;
            // 
            // ucMonAn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(190, 200);
            this.MinimumSize = new System.Drawing.Size(190, 200);
            this.Name = "ucMonAn";
            this.Size = new System.Drawing.Size(190, 200);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHinhMon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picHinhMon;
        private System.Windows.Forms.Button btnThemMon;
        private System.Windows.Forms.Label lblDonGia;
        private System.Windows.Forms.Label lblTenMon;
        private System.Windows.Forms.Label lblTrangThai;
    }
}
