namespace SIMANIK.Forms
{
    partial class FormRegisterPasien
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblKonfirmasiPassword;
        private System.Windows.Forms.Label lblNamaLengkap;
        private System.Windows.Forms.Label lblTanggalLahir;
        private System.Windows.Forms.Label lblJenisKelamin;
        private System.Windows.Forms.Label lblNoTelepon;
        private System.Windows.Forms.Label lblAlamat;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtKonfirmasiPassword;
        private System.Windows.Forms.TextBox txtNamaLengkap;
        private System.Windows.Forms.DateTimePicker dtpTanggalLahir;
        private System.Windows.Forms.ComboBox cmbJenisKelamin;
        private System.Windows.Forms.TextBox txtNoTelepon;
        private System.Windows.Forms.TextBox txtAlamat;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnBatal;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblKonfirmasiPassword = new System.Windows.Forms.Label();
            this.lblNamaLengkap = new System.Windows.Forms.Label();
            this.lblTanggalLahir = new System.Windows.Forms.Label();
            this.lblJenisKelamin = new System.Windows.Forms.Label();
            this.lblNoTelepon = new System.Windows.Forms.Label();
            this.lblAlamat = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtKonfirmasiPassword = new System.Windows.Forms.TextBox();
            this.txtNamaLengkap = new System.Windows.Forms.TextBox();
            this.dtpTanggalLahir = new System.Windows.Forms.DateTimePicker();
            this.cmbJenisKelamin = new System.Windows.Forms.ComboBox();
            this.txtNoTelepon = new System.Windows.Forms.TextBox();
            this.txtAlamat = new System.Windows.Forms.TextBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnBatal = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(520, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Register Pasien";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // layout
            // 
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66F));
            this.layout.Controls.Add(this.lblUsername, 0, 0);
            this.layout.Controls.Add(this.txtUsername, 1, 0);
            this.layout.Controls.Add(this.lblPassword, 0, 1);
            this.layout.Controls.Add(this.txtPassword, 1, 1);
            this.layout.Controls.Add(this.lblKonfirmasiPassword, 0, 2);
            this.layout.Controls.Add(this.txtKonfirmasiPassword, 1, 2);
            this.layout.Controls.Add(this.lblNamaLengkap, 0, 3);
            this.layout.Controls.Add(this.txtNamaLengkap, 1, 3);
            this.layout.Controls.Add(this.lblTanggalLahir, 0, 4);
            this.layout.Controls.Add(this.dtpTanggalLahir, 1, 4);
            this.layout.Controls.Add(this.lblJenisKelamin, 0, 5);
            this.layout.Controls.Add(this.cmbJenisKelamin, 1, 5);
            this.layout.Controls.Add(this.lblNoTelepon, 0, 6);
            this.layout.Controls.Add(this.txtNoTelepon, 1, 6);
            this.layout.Controls.Add(this.lblAlamat, 0, 7);
            this.layout.Controls.Add(this.txtAlamat, 1, 7);
            this.layout.Controls.Add(this.buttonPanel, 1, 8);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 48);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(28, 12, 28, 20);
            this.layout.RowCount = 9;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Size = new System.Drawing.Size(520, 452);
            this.layout.TabIndex = 1;
            // 
            // lblUsername
            // 
            this.lblUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUsername.Location = new System.Drawing.Point(31, 12);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(151, 42);
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPassword
            // 
            this.lblPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPassword.Location = new System.Drawing.Point(31, 54);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(151, 42);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKonfirmasiPassword
            // 
            this.lblKonfirmasiPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblKonfirmasiPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblKonfirmasiPassword.Location = new System.Drawing.Point(31, 96);
            this.lblKonfirmasiPassword.Name = "lblKonfirmasiPassword";
            this.lblKonfirmasiPassword.Size = new System.Drawing.Size(151, 42);
            this.lblKonfirmasiPassword.TabIndex = 4;
            this.lblKonfirmasiPassword.Text = "Konfirmasi";
            this.lblKonfirmasiPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNamaLengkap
            // 
            this.lblNamaLengkap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNamaLengkap.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNamaLengkap.Location = new System.Drawing.Point(31, 138);
            this.lblNamaLengkap.Name = "lblNamaLengkap";
            this.lblNamaLengkap.Size = new System.Drawing.Size(151, 42);
            this.lblNamaLengkap.TabIndex = 6;
            this.lblNamaLengkap.Text = "Nama lengkap";
            this.lblNamaLengkap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTanggalLahir
            // 
            this.lblTanggalLahir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTanggalLahir.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTanggalLahir.Location = new System.Drawing.Point(31, 180);
            this.lblTanggalLahir.Name = "lblTanggalLahir";
            this.lblTanggalLahir.Size = new System.Drawing.Size(151, 42);
            this.lblTanggalLahir.TabIndex = 8;
            this.lblTanggalLahir.Text = "Tanggal lahir";
            this.lblTanggalLahir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblJenisKelamin
            // 
            this.lblJenisKelamin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblJenisKelamin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblJenisKelamin.Location = new System.Drawing.Point(31, 222);
            this.lblJenisKelamin.Name = "lblJenisKelamin";
            this.lblJenisKelamin.Size = new System.Drawing.Size(151, 42);
            this.lblJenisKelamin.TabIndex = 10;
            this.lblJenisKelamin.Text = "Jenis kelamin";
            this.lblJenisKelamin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNoTelepon
            // 
            this.lblNoTelepon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoTelepon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNoTelepon.Location = new System.Drawing.Point(31, 264);
            this.lblNoTelepon.Name = "lblNoTelepon";
            this.lblNoTelepon.Size = new System.Drawing.Size(151, 42);
            this.lblNoTelepon.TabIndex = 12;
            this.lblNoTelepon.Text = "No. telepon";
            this.lblNoTelepon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAlamat
            // 
            this.lblAlamat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAlamat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAlamat.Location = new System.Drawing.Point(31, 306);
            this.lblAlamat.Name = "lblAlamat";
            this.lblAlamat.Size = new System.Drawing.Size(151, 90);
            this.lblAlamat.TabIndex = 14;
            this.lblAlamat.Text = "Alamat";
            this.lblAlamat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.Location = new System.Drawing.Point(188, 20);
            this.txtUsername.MaxLength = 50;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(301, 25);
            this.txtUsername.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.Location = new System.Drawing.Point(188, 62);
            this.txtPassword.MaxLength = 255;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(301, 25);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtKonfirmasiPassword
            // 
            this.txtKonfirmasiPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKonfirmasiPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtKonfirmasiPassword.Location = new System.Drawing.Point(188, 104);
            this.txtKonfirmasiPassword.MaxLength = 255;
            this.txtKonfirmasiPassword.Name = "txtKonfirmasiPassword";
            this.txtKonfirmasiPassword.Size = new System.Drawing.Size(301, 25);
            this.txtKonfirmasiPassword.TabIndex = 5;
            this.txtKonfirmasiPassword.UseSystemPasswordChar = true;
            // 
            // txtNamaLengkap
            // 
            this.txtNamaLengkap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNamaLengkap.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNamaLengkap.Location = new System.Drawing.Point(188, 146);
            this.txtNamaLengkap.MaxLength = 100;
            this.txtNamaLengkap.Name = "txtNamaLengkap";
            this.txtNamaLengkap.Size = new System.Drawing.Size(301, 25);
            this.txtNamaLengkap.TabIndex = 7;
            // 
            // dtpTanggalLahir
            // 
            this.dtpTanggalLahir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpTanggalLahir.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpTanggalLahir.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTanggalLahir.Location = new System.Drawing.Point(188, 188);
            this.dtpTanggalLahir.Name = "dtpTanggalLahir";
            this.dtpTanggalLahir.Size = new System.Drawing.Size(301, 25);
            this.dtpTanggalLahir.TabIndex = 9;
            // 
            // cmbJenisKelamin
            // 
            this.cmbJenisKelamin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbJenisKelamin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJenisKelamin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbJenisKelamin.FormattingEnabled = true;
            this.cmbJenisKelamin.Location = new System.Drawing.Point(188, 231);
            this.cmbJenisKelamin.Name = "cmbJenisKelamin";
            this.cmbJenisKelamin.Size = new System.Drawing.Size(301, 25);
            this.cmbJenisKelamin.TabIndex = 11;
            // 
            // txtNoTelepon
            // 
            this.txtNoTelepon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNoTelepon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNoTelepon.Location = new System.Drawing.Point(188, 272);
            this.txtNoTelepon.MaxLength = 20;
            this.txtNoTelepon.Name = "txtNoTelepon";
            this.txtNoTelepon.Size = new System.Drawing.Size(301, 25);
            this.txtNoTelepon.TabIndex = 13;
            // 
            // txtAlamat
            // 
            this.txtAlamat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAlamat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAlamat.Location = new System.Drawing.Point(188, 309);
            this.txtAlamat.MaxLength = 255;
            this.txtAlamat.Multiline = true;
            this.txtAlamat.Name = "txtAlamat";
            this.txtAlamat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAlamat.Size = new System.Drawing.Size(301, 84);
            this.txtAlamat.TabIndex = 15;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(this.btnRegister);
            this.buttonPanel.Controls.Add(this.btnBatal);
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.buttonPanel.Location = new System.Drawing.Point(286, 408);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(203, 32);
            this.buttonPanel.TabIndex = 16;
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(3, 0);
            this.btnRegister.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(100, 32);
            this.btnRegister.TabIndex = 0;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // btnBatal
            // 
            this.btnBatal.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBatal.Location = new System.Drawing.Point(109, 0);
            this.btnBatal.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnBatal.Name = "btnBatal";
            this.btnBatal.Size = new System.Drawing.Size(100, 32);
            this.btnBatal.TabIndex = 1;
            this.btnBatal.Text = "Batal";
            this.btnBatal.UseVisualStyleBackColor = true;
            // 
            // FormRegisterPasien
            // 
            this.AcceptButton = this.btnRegister;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnBatal;
            this.ClientSize = new System.Drawing.Size(520, 500);
            this.Controls.Add(this.layout);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(536, 539);
            this.Name = "FormRegisterPasien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SIMANIK - Register Pasien";
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
