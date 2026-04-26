using System;
using System.ComponentModel;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public partial class FormDashboardDokter : Form
    {
        private readonly AuthService _authService;

        public FormDashboardDokter()
        {
            _authService = new AuthService();
            InitializeComponent();
            ApplyTheme();
            if (!IsInDesignMode())
            {
                LoadSessionInfo();
            }
        }

        private void ApplyTheme()
        {
            UiTheme.ApplyBase(this);
            UiTheme.StylePanel(rootLayout);
            UiTheme.StylePageTitle(lblTitle);
            UiTheme.StylePanel(infoLayout);
            UiTheme.StyleInfoLabel(lblNama);
            UiTheme.StyleInfoLabel(lblRole);
            UiTheme.StylePanel(menuPanel);
            UiTheme.StyleMenuButton(btnAntrian);
            UiTheme.StyleMenuButton(btnPemeriksaan);
            UiTheme.StyleMenuButton(btnRiwayat);
            UiTheme.StyleLogoutButton(btnLogout);
        }

        private static bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void LoadSessionInfo()
        {
            User user = SessionHelper.CurrentUser;

            if (user == null || user.Role != UserRole.Dokter)
            {
                SessionHelper.Clear();
                Close();
                return;
            }

            lblNama.Text = "Nama: " + user.NamaLengkap;
            lblRole.Text = "Role: " + user.Role;
        }

        private void ShowPendingFeature(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Fitur akan dibuat di tahap berikutnya", ((Button)sender).Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Logout(object sender, EventArgs e)
        {
            _authService.Logout();
            Close();
        }
    }
}
