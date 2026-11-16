namespace QuanLyNhaHang.Forms
{
    partial class ucBan
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
            this.lblSoChoNgoi = new System.Windows.Forms.Label();
            this.lblTenBan = new System.Windows.Forms.Label();
            this.picBan = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBan)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(245)))), ((int)(((byte)(254)))));
            this.panel1.Controls.Add(this.lblTrangThai);
            this.panel1.Controls.Add(this.lblSoChoNgoi);
            this.panel1.Controls.Add(this.lblTenBan);
            this.panel1.Controls.Add(this.picBan);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(175, 124);
            this.panel1.TabIndex = 0;
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrangThai.Location = new System.Drawing.Point(17, 99);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(150, 20);
            this.lblTrangThai.TabIndex = 3;
            this.lblTrangThai.Text = "Trống";
            this.lblTrangThai.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSoChoNgoi
            // 
            this.lblSoChoNgoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoChoNgoi.Location = new System.Drawing.Point(97, 38);
            this.lblSoChoNgoi.Name = "lblSoChoNgoi";
            this.lblSoChoNgoi.Size = new System.Drawing.Size(70, 20);
            this.lblSoChoNgoi.TabIndex = 2;
            this.lblSoChoNgoi.Text = "4 Chỗ";
            this.lblSoChoNgoi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTenBan
            // 
            this.lblTenBan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenBan.Location = new System.Drawing.Point(96, 13);
            this.lblTenBan.Name = "lblTenBan";
            this.lblTenBan.Size = new System.Drawing.Size(81, 25);
            this.lblTenBan.TabIndex = 1;
            this.lblTenBan.Text = "Bàn 01";
            // 
            // picBan
            // 
            this.picBan.Location = new System.Drawing.Point(21, 13);
            this.picBan.Name = "picBan";
            this.picBan.Size = new System.Drawing.Size(64, 64);
            this.picBan.TabIndex = 0;
            this.picBan.TabStop = false;
            // 
            // ucBan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ucBan";
            this.Size = new System.Drawing.Size(175, 124);
            this.Click += new System.EventHandler(this.ucBan_Click);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBan)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblSoChoNgoi;
        private System.Windows.Forms.Label lblTenBan;
        private System.Windows.Forms.PictureBox picBan;
        private System.Windows.Forms.Label lblTrangThai;
    }
}
