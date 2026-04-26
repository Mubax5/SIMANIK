using System;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public partial class FormRegisterPasien : Form
    {
        private readonly PasienService _pasienService;

        public FormRegisterPasien()
        {
            _pasienService = new PasienService();
            InitializeComponent();
            ApplyTheme();
            ConfigureTanggalLahir();
            LoadJenisKelaminItems();
        }

        private void ApplyTheme()
        {
            UiTheme.ApplyBase(this);
            UiTheme.StyleHeader(lblTitle, "Registrasi Pasien Baru");
            UiTheme.StylePanel(layout);
            UiTheme.StyleLabel(lblUsername);
            UiTheme.StyleLabel(lblPassword);
            UiTheme.StyleLabel(lblKonfirmasiPassword);
            UiTheme.StyleLabel(lblNamaLengkap);
            UiTheme.StyleLabel(lblTanggalLahir);
            UiTheme.StyleLabel(lblJenisKelamin);
            UiTheme.StyleLabel(lblNoTelepon);
            UiTheme.StyleLabel(lblAlamat);
            UiTheme.StyleTextBox(txtUsername);
            UiTheme.StyleTextBox(txtPassword);
            UiTheme.StyleTextBox(txtKonfirmasiPassword);
            UiTheme.StyleTextBox(txtNamaLengkap);
            UiTheme.StyleTextBox(txtNoTelepon);
            UiTheme.StyleTextBox(txtAlamat);
            UiTheme.StyleDatePicker(dtpTanggalLahir);
            UiTheme.StyleComboBox(cmbJenisKelamin);
            UiTheme.StylePrimaryButton(btnRegister);
            UiTheme.StyleSecondaryButton(btnBatal);
        }

        private void ConfigureTanggalLahir()
        {
            dtpTanggalLahir.MaxDate = DateTime.Today;
            dtpTanggalLahir.Value = DateTime.Today.AddYears(-20);
        }

        private void LoadJenisKelaminItems()
        {
            cmbJenisKelamin.Items.Add(new JenisKelaminItem("Laki-laki", JenisKelamin.LakiLaki));
            cmbJenisKelamin.Items.Add(new JenisKelaminItem("Perempuan", JenisKelamin.Perempuan));
            cmbJenisKelamin.SelectedIndex = 0;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                RegisterPasienRequest request = new RegisterPasienRequest
                {
                    Username = txtUsername.Text,
                    Password = txtPassword.Text,
                    KonfirmasiPassword = txtKonfirmasiPassword.Text,
                    NamaLengkap = txtNamaLengkap.Text,
                    TanggalLahir = dtpTanggalLahir.Value,
                    JenisKelamin = ((JenisKelaminItem)cmbJenisKelamin.SelectedItem).Value,
                    NoTelepon = txtNoTelepon.Text,
                    Alamat = txtAlamat.Text
                };

                ServiceResult result = _pasienService.RegisterPasien(request);

                if (!result.Success)
                {
                    MessageBox.Show(this, result.Message, "Registrasi gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(this, result.Message, "Registrasi berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Registrasi gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class JenisKelaminItem
        {
            public string Text { get; private set; }
            public JenisKelamin Value { get; private set; }

            public JenisKelaminItem(string text, JenisKelamin value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
