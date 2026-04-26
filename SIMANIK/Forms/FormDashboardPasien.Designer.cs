namespace SIMANIK.Forms
{
    partial class FormDashboardPasien
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel infoLayout;
        private System.Windows.Forms.Label lblNama;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.FlowLayoutPanel menuPanel;
        private System.Windows.Forms.Button btnProfil;
        private System.Windows.Forms.Button btnReservasi;
        private System.Windows.Forms.Button btnRiwayat;
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
            this.btnProfil = new System.Windows.Forms.Button();
            this.btnReservasi = new System.Windows.Forms.Button();
            this.btnRiwayat = new System.Windows.Forms.Button();
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
            this.rootLayout.Size = new System.Drawing.Size(620, 360);
            this.rootLayout.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(31, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(558, 56);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard Pasien";
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
            this.infoLayout.Size = new System.Drawing.Size(558, 66);
            this.infoLayout.TabIndex = 1;
            // 
            // lblNama
            // 
            this.lblNama.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNama.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNama.Location = new System.Drawing.Point(3, 0);
            this.lblNama.Name = "lblNama";
            this.lblNama.Size = new System.Drawing.Size(552, 33);
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
            this.lblRole.Size = new System.Drawing.Size(552, 33);
            this.lblRole.TabIndex = 1;
            this.lblRole.Text = "Role:";
            this.lblRole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuPanel
            // 
            this.menuPanel.AutoScroll = false;
            this.menuPanel.Controls.Add(this.btnProfil);
            this.menuPanel.Controls.Add(this.btnReservasi);
            this.menuPanel.Controls.Add(this.btnRiwayat);
            this.menuPanel.Controls.Add(this.btnLogout);
            this.menuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuPanel.Location = new System.Drawing.Point(31, 159);
            this.menuPanel.Name = "menuPanel";
            this.menuPanel.Size = new System.Drawing.Size(558, 170);
            this.menuPanel.TabIndex = 2;
            this.menuPanel.WrapContents = true;
            // 
            // btnProfil
            // 
            this.btnProfil.Location = new System.Drawing.Point(0, 0);
            this.btnProfil.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnProfil.Name = "btnProfil";
            this.btnProfil.Size = new System.Drawing.Size(126, 44);
            this.btnProfil.TabIndex = 0;
            this.btnProfil.Text = "Profil";
            this.btnProfil.UseVisualStyleBackColor = true;
            this.btnProfil.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnReservasi
            // 
            this.btnReservasi.Location = new System.Drawing.Point(138, 0);
            this.btnReservasi.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnReservasi.Name = "btnReservasi";
            this.btnReservasi.Size = new System.Drawing.Size(126, 44);
            this.btnReservasi.TabIndex = 1;
            this.btnReservasi.Text = "Reservasi";
            this.btnReservasi.UseVisualStyleBackColor = true;
            this.btnReservasi.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnRiwayat
            // 
            this.btnRiwayat.Location = new System.Drawing.Point(276, 0);
            this.btnRiwayat.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnRiwayat.Name = "btnRiwayat";
            this.btnRiwayat.Size = new System.Drawing.Size(126, 44);
            this.btnRiwayat.TabIndex = 2;
            this.btnRiwayat.Text = "Riwayat";
            this.btnRiwayat.UseVisualStyleBackColor = true;
            this.btnRiwayat.Click += new System.EventHandler(this.ShowPendingFeature);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(414, 0);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(126, 44);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.Logout);
            // 
            // FormDashboardPasien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 360);
            this.Controls.Add(this.rootLayout);
            this.MinimumSize = new System.Drawing.Size(636, 399);
            this.Name = "FormDashboardPasien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SIMANIK - Dashboard Pasien";
            this.rootLayout.ResumeLayout(false);
            this.infoLayout.ResumeLayout(false);
            this.menuPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
