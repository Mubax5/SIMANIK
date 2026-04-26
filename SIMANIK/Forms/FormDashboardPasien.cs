using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public partial class FormDashboardPasien : Form
    {
        private readonly AuthService _authService;
        private readonly DashboardService _dashboardService;
        private FlowLayoutPanel dashboardContent;
        private TextBox txtSearchPatient;
        private ComboBox cmbSearchPatientType;
        private ComboBox cmbPatientReservationStatus;
        private DateTimePicker dtpPatientStartDate;
        private DateTimePicker dtpPatientEndDate;
        private Button btnSearchPatient;
        private Button btnResetSearchPatient;
        private DataGridView dgvPatientSearchResults;

        public FormDashboardPasien()
        {
            _authService = new AuthService();
            _dashboardService = new DashboardService();
            InitializeComponent();
            ApplyTheme();

            if (!IsInDesignMode())
            {
                LoadSessionInfo();
                InitializeDashboard();
                RefreshDashboard();
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
            UiTheme.StyleMenuButton(btnProfil);
            UiTheme.StyleMenuButton(btnReservasi);
            UiTheme.StyleMenuButton(btnRiwayat);
            UiTheme.StyleLogoutButton(btnLogout);
        }

        private static bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void InitializeDashboard()
        {
            dashboardContent = DashboardUiHelper.PrepareDashboardContent(this, rootLayout, menuPanel);
            Button refreshButton = new Button
            {
                Text = "Refresh Dashboard",
                Width = 150,
                Height = 44,
                Margin = new Padding(0, 0, 12, 12)
            };
            UiTheme.StylePrimaryButton(refreshButton);
            refreshButton.Click += delegate { RefreshDashboard(); };
            menuPanel.Controls.Add(refreshButton);
        }

        private void RefreshDashboard()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                dashboardContent.Controls.Clear();

                BuildSummarySection();
                BuildChartSections();
                BuildTableSections();
                BuildSearchSection();
                RunPatientSearch(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Dashboard gagal dimuat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BuildSummarySection()
        {
            PatientDashboardSummary summary = _dashboardService.GetPatientSummary(SessionHelper.CurrentUser.Id);
            Panel body;
            Panel section = DashboardUiHelper.CreateSection("Profil dan Ringkasan Saya", 1050, 170, out body);
            FlowLayoutPanel cards = DashboardUiHelper.CreateCardFlow();

            cards.Controls.Add(UiTheme.CreateSummaryCard("Nama pasien", summary.PatientName, "Data akun pasien"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Nomor pasien", summary.PatientNumber, "Nomor rekam medis"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Umur", summary.Age.ToString(), "Tahun"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Reservasi terdekat", summary.NextReservation, "Jadwal berikutnya"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Status terakhir", summary.LastReservationStatus, "Reservasi terbaru"));

            body.Controls.Add(cards);
            dashboardContent.Controls.Add(section);
        }

        private void BuildChartSections()
        {
            int userId = SessionHelper.CurrentUser.Id;
            AddChartSection("Status Reservasi Saya", _dashboardService.GetPatientReservationStatusDistribution(userId), SeriesChartType.Pie);
            AddChartSection("Kunjungan per Bulan", _dashboardService.GetPatientVisitsPerMonth(userId), SeriesChartType.Column);
            AddChartSection("Diagnosa yang Pernah Tercatat", _dashboardService.GetPatientDiseaseHistory(userId), SeriesChartType.Bar);
        }

        private void AddChartSection(string title, List<ChartDataPoint> points, SeriesChartType chartType)
        {
            Panel body;
            Panel section = DashboardUiHelper.CreateSection(title, 515, 300, out body);
            body.Controls.Add(DashboardUiHelper.CreateChart(title, points, chartType));
            dashboardContent.Controls.Add(section);
        }

        private void BuildTableSections()
        {
            int userId = SessionHelper.CurrentUser.Id;
            AddGridSection("Reservasi Terakhir Saya", DashboardUiHelper.CreateGrid(_dashboardService.GetPatientRecentReservations(userId)));
            AddGridSection("Riwayat Pemeriksaan Terakhir", DashboardUiHelper.CreateGrid(_dashboardService.GetPatientRecentExaminations(userId)));
            AddGridSection("Obat Terakhir Diberikan", DashboardUiHelper.CreateGrid(_dashboardService.GetPatientRecentMedicines(userId)));
        }

        private void AddGridSection(string title, DataGridView grid)
        {
            Panel body;
            Panel section = DashboardUiHelper.CreateSection(title, 515, 285, out body);
            body.Controls.Add(grid);
            dashboardContent.Controls.Add(section);
        }

        private void BuildSearchSection()
        {
            Panel body;
            Panel section = DashboardUiHelper.CreateSection("Cari Riwayat Saya", 1050, 430, out body);
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false
            };

            txtSearchPatient = CreateSearchTextBox();
            cmbSearchPatientType = CreateComboBox(new[] { "Reservasi Saya", "Riwayat Pemeriksaan", "Obat Saya", "Diagnosa Saya", "Jadwal Dokter" }, 185);
            cmbPatientReservationStatus = CreateComboBox(new[] { "Semua", "Menunggu Verifikasi", "Dikonfirmasi", "Ditolak", "Dibatalkan Pasien", "Check-in", "Selesai" }, 180);
            dtpPatientStartDate = CreateDatePicker(DateTime.Today.AddMonths(-6));
            dtpPatientEndDate = CreateDatePicker(DateTime.Today.AddMonths(1));
            btnSearchPatient = CreateButton("Cari", true);
            btnResetSearchPatient = CreateButton("Reset", false);

            btnSearchPatient.Click += BtnSearchPatient_Click;
            btnResetSearchPatient.Click += BtnResetSearchPatient_Click;

            filters.Controls.Add(CreateFilterBlock("Keyword", txtSearchPatient, 210));
            filters.Controls.Add(CreateFilterBlock("Tipe", cmbSearchPatientType, 190));
            filters.Controls.Add(CreateFilterBlock("Status", cmbPatientReservationStatus, 185));
            filters.Controls.Add(CreateFilterBlock("Mulai", dtpPatientStartDate, 130));
            filters.Controls.Add(CreateFilterBlock("Sampai", dtpPatientEndDate, 130));
            filters.Controls.Add(btnSearchPatient);
            filters.Controls.Add(btnResetSearchPatient);

            dgvPatientSearchResults = DashboardUiHelper.CreateGrid(new List<DashboardSearchResultItem>());

            layout.Controls.Add(filters, 0, 0);
            layout.Controls.Add(dgvPatientSearchResults, 0, 1);
            body.Controls.Add(layout);
            dashboardContent.Controls.Add(section);
        }

        private TextBox CreateSearchTextBox()
        {
            TextBox textBox = new TextBox
            {
                Width = 205
            };

            UiTheme.StyleTextBox(textBox);
            return textBox;
        }

        private ComboBox CreateComboBox(string[] items, int width)
        {
            ComboBox comboBox = new ComboBox
            {
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            comboBox.Items.AddRange(items);
            comboBox.SelectedIndex = 0;
            UiTheme.StyleComboBox(comboBox);

            return comboBox;
        }

        private DateTimePicker CreateDatePicker(DateTime value)
        {
            DateTimePicker picker = new DateTimePicker
            {
                Width = 125,
                Format = DateTimePickerFormat.Short,
                Value = value
            };

            UiTheme.StyleDatePicker(picker);
            return picker;
        }

        private Button CreateButton(string text, bool primary)
        {
            Button button = new Button
            {
                Text = text,
                Width = 86,
                Height = 48,
                Margin = new Padding(8, 18, 0, 0)
            };

            if (primary)
            {
                UiTheme.StylePrimaryButton(button);
            }
            else
            {
                UiTheme.StyleSecondaryButton(button);
            }

            return button;
        }

        private Control CreateFilterBlock(string labelText, Control input, int width)
        {
            Panel panel = new Panel
            {
                Width = width,
                Height = 62,
                Margin = new Padding(0, 0, 10, 0)
            };

            Label label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 22
            };

            UiTheme.StyleLabel(label);
            input.Dock = DockStyle.Top;
            panel.Controls.Add(input);
            panel.Controls.Add(label);

            return panel;
        }

        private void RunPatientSearch(bool showEmptyMessage)
        {
            DashboardSearchCriteria criteria = new DashboardSearchCriteria
            {
                Keyword = txtSearchPatient.Text,
                SearchType = Convert.ToString(cmbSearchPatientType.SelectedItem),
                Status = Convert.ToString(cmbPatientReservationStatus.SelectedItem),
                StartDate = dtpPatientStartDate.Value.Date,
                EndDate = dtpPatientEndDate.Value.Date
            };

            List<DashboardSearchResultItem> results = _dashboardService.SearchPatientDashboardData(SessionHelper.CurrentUser.Id, criteria);
            DashboardUiHelper.BindGrid(dgvPatientSearchResults, results);

            if (showEmptyMessage && results.Count == 0)
            {
                MessageBox.Show(this, "Belum ada data", "Pencarian", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSearchPatient_Click(object sender, EventArgs e)
        {
            RunPatientSearch(true);
        }

        private void BtnResetSearchPatient_Click(object sender, EventArgs e)
        {
            txtSearchPatient.Clear();
            cmbSearchPatientType.SelectedIndex = 0;
            cmbPatientReservationStatus.SelectedIndex = 0;
            dtpPatientStartDate.Value = DateTime.Today.AddMonths(-6);
            dtpPatientEndDate.Value = DateTime.Today.AddMonths(1);
            RunPatientSearch(false);
        }

        private void LoadSessionInfo()
        {
            User user = SessionHelper.CurrentUser;

            if (user == null || user.Role != UserRole.Pasien)
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
