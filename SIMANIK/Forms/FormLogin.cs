using System;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public partial class FormLogin : Form
    {
        private readonly AuthService _authService;

        public FormLogin()
        {
            _authService = new AuthService();
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            UiTheme.ApplyBase(this);
            UiTheme.StyleHeader(lblTitle, "SIMANIK  |  Klinik & Rekam Medis");
            UiTheme.StylePanel(layout);
            UiTheme.StyleLabel(lblUsername);
            UiTheme.StyleLabel(lblPassword);
            UiTheme.StyleTextBox(txtUsername);
            UiTheme.StyleTextBox(txtPassword);
            UiTheme.StylePrimaryButton(btnLogin);
            UiTheme.StyleSecondaryButton(btnRegister);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                LoginResult result = _authService.Login(txtUsername.Text, txtPassword.Text);

                if (!result.Success)
                {
                    MessageBox.Show(this, result.Message, "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OpenDashboard(result.User.Role);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (FormRegisterPasien form = new FormRegisterPasien())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
        }

        private void OpenDashboard(UserRole role)
        {
            Form dashboard;

            switch (role)
            {
                case UserRole.Admin:
                    dashboard = new FormDashboardAdmin();
                    break;
                case UserRole.Dokter:
                    dashboard = new FormDashboardDokter();
                    break;
                case UserRole.Pasien:
                    dashboard = new FormDashboardPasien();
                    break;
                default:
                    MessageBox.Show(this, "Role user tidak dikenali.", "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SessionHelper.Clear();
                    return;
            }

            dashboard.FormClosed += Dashboard_FormClosed;
            Hide();
            dashboard.Show();
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SessionHelper.IsLoggedIn)
            {
                Application.Exit();
                return;
            }

            txtPassword.Clear();
            Show();
            txtUsername.Focus();
        }
    }
}
