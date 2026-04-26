namespace SIMANIK.Forms
{
    partial class FormDashboardAdmin
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel infoLayout;
        private System.Windows.Forms.Label lblNama;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.FlowLayoutPanel menuPanel;
        private System.Windows.Forms.Button btnAkun;
        private System.Windows.Forms.Button btnDokter;
        private System.Windows.Forms.Button btnJadwal;
        private System.Windows.Forms.Button btnPenyakit;
        private System.Windows.Forms.Button btnObat;
        private System.Windows.Forms.Button btnReservasi;
        private System.Windows.Forms.Button btnCheckIn;
        private System.Windows.Forms.Button btnAntrian;
        private System.Windows.Forms.Button btnLaporan;
        private System.Windows.Forms.Button btnLogout;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.infoLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblNama = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.menuPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAkun = new System.Windows.Forms.Button();
            this.btnDokter = new System.Windows.Forms.Button();
            this.btnJadwal = new System.Windows.Forms.Button();
            this.btnPenyakit = new System.Windows.Forms.Button();
            this.btnObat = new System.Windows.Forms.Button();
            this.btnReservasi = new System.Windows.Forms.Button();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.btnAntrian = new System.Windows.Forms.Button();
            this.btnLaporan = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.infoLayout.SuspendLayout();
            this.menuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.infoLayout, 0, 1);
            this.rootLayout.Controls.Add(this.menuPanel, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(28);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(760, 420);
            this.rootLayout.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(31, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(698, 56);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard Admin";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // infoLayout
            // 
            this.infoLayout.ColumnCount = 1;
            this.infoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.infoLayout.Controls.Add(this.lblNama, 0, 0);
            this.infoLayout.Controls.Add(this.lblRole, 0, 1);
            this.infoLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoLayout.Location = new System.Drawing.Point(31, 87);
            this.infoLayout.Name = "infoLayout";
            this.infoLayout.RowCount = 2;
            this.infoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.infoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.infoLayout.Size = new System.Drawing.Size(698, 66);
            this.infoLayout.TabIndex = 1;
            // 
            // lblNama
            // 
            this.lblNama.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNama.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNama.Location = new System.Drawing.Point(3, 0);
            this.lblNama.Name = "lblNama";
            this.lblNama.Size = new System.Drawing.Size(692, 33);
            this.lblNama.TabIndex = 0;
            this.lblNama.Text = "Nama:";
            this.lblNama.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRole
            // 
            this.lblRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRole.Location = new System.Drawing.Point(3, 33);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(692, 33);
            this.lblRole.TabIndex = 1;
            this.lblRole.Text = "Role:";
            this.lblRole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuPanel
            // 
            this.menuPanel.AutoScroll = true;
            this.menuPanel.Controls.Add(this.btnAkun);
            this.menuPanel.Controls.Add(this.btnDokter);
            this.menuPanel.Controls.Add(this.btnJadwal);
            this.menuPanel.Controls.Add(this.btnPenyakit);
            this.menuPanel.Controls.Add(this.btnObat);
            this.menuPanel.Controls.Add(this.btnReservasi);
            this.menuPanel.Controls.Add(this.btnCheckIn);
            this.menuPanel.Controls.Add(this.btnAntrian);
            this.menuPanel.Controls.Add(this.btnLaporan);
            this.menuPanel.Controls.Add(this.btnLogout);
            this.menuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuPanel.Location = new System.Drawing.Point(31, 159);
            this.menuPanel.Name = "menuPanel";
            this.menuPanel.Size = new System.Drawing.Size(698, 230);
            this.menuPanel.TabIndex = 2;
            this.menuPanel.WrapContents = true;
            // 
            // btnAkun
            // 
            this.btnAkun.Location = new System.Drawing.Point(0, 0);
            this.btnAkun.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnAkun.Name = "btnAkun";
            this.btnAkun.Size = new System.Drawing.Size(126, 44);
            this.btnAkun.TabIndex = 0;
            this.btnAkun.Text = "Akun";
            this.btnAkun.UseVisualStyleBackColor = true;
            this.btnAkun.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnDokter
            // 
            this.btnDokter.Location = new System.Drawing.Point(138, 0);
            this.btnDokter.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnDokter.Name = "btnDokter";
            this.btnDokter.Size = new System.Drawing.Size(126, 44);
            this.btnDokter.TabIndex = 1;
            this.btnDokter.Text = "Dokter";
            this.btnDokter.UseVisualStyleBackColor = true;
            this.btnDokter.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnJadwal
            // 
            this.btnJadwal.Location = new System.Drawing.Point(276, 0);
            this.btnJadwal.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnJadwal.Name = "btnJadwal";
            this.btnJadwal.Size = new System.Drawing.Size(126, 44);
            this.btnJadwal.TabIndex = 2;
            this.btnJadwal.Text = "Jadwal";
            this.btnJadwal.UseVisualStyleBackColor = true;
            this.btnJadwal.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnPenyakit
            // 
            this.btnPenyakit.Location = new System.Drawing.Point(414, 0);
            this.btnPenyakit.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnPenyakit.Name = "btnPenyakit";
            this.btnPenyakit.Size = new System.Drawing.Size(126, 44);
            this.btnPenyakit.TabIndex = 3;
            this.btnPenyakit.Text = "Penyakit";
            this.btnPenyakit.UseVisualStyleBackColor = true;
            this.btnPenyakit.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnObat
            // 
            this.btnObat.Location = new System.Drawing.Point(552, 0);
            this.btnObat.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnObat.Name = "btnObat";
            this.btnObat.Size = new System.Drawing.Size(126, 44);
            this.btnObat.TabIndex = 4;
            this.btnObat.Text = "Obat";
            this.btnObat.UseVisualStyleBackColor = true;
            this.btnObat.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnReservasi
            // 
            this.btnReservasi.Location = new System.Drawing.Point(0, 56);
            this.btnReservasi.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnReservasi.Name = "btnReservasi";
            this.btnReservasi.Size = new System.Drawing.Size(126, 44);
            this.btnReservasi.TabIndex = 5;
            this.btnReservasi.Text = "Reservasi";
            this.btnReservasi.UseVisualStyleBackColor = true;
            this.btnReservasi.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.Location = new System.Drawing.Point(138, 56);
            this.btnCheckIn.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(126, 44);
            this.btnCheckIn.TabIndex = 6;
            this.btnCheckIn.Text = "Check-in";
            this.btnCheckIn.UseVisualStyleBackColor = true;
            this.btnCheckIn.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnAntrian
            // 
            this.btnAntrian.Location = new System.Drawing.Point(276, 56);
            this.btnAntrian.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnAntrian.Name = "btnAntrian";
            this.btnAntrian.Size = new System.Drawing.Size(126, 44);
            this.btnAntrian.TabIndex = 7;
            this.btnAntrian.Text = "Antrian";
            this.btnAntrian.UseVisualStyleBackColor = true;
            this.btnAntrian.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnLaporan
            // 
            this.btnLaporan.Location = new System.Drawing.Point(414, 56);
            this.btnLaporan.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnLaporan.Name = "btnLaporan";
            this.btnLaporan.Size = new System.Drawing.Size(126, 44);
            this.btnLaporan.TabIndex = 8;
            this.btnLaporan.Text = "Laporan";
            this.btnLaporan.UseVisualStyleBackColor = true;
            this.btnLaporan.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(552, 56);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(126, 44);
            this.btnLogout.TabIndex = 9;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.Logout);
            // 
            // FormDashboardAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 420);
            this.Controls.Add(this.rootLayout);
            this.MinimumSize = new System.Drawing.Size(776, 459);
            this.Name = "FormDashboardAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SIMANIK - Dashboard Admin";
            this.rootLayout.ResumeLayout(false);
            this.infoLayout.ResumeLayout(false);
            this.menuPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
