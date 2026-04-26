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
    public partial class FormDashboardAdmin : Form
    {
        private readonly AuthService _authService;
        private readonly DashboardService _dashboardService;
        private FlowLayoutPanel dashboardContent;
        private TextBox txtSearchAdmin;
        private ComboBox cmbSearchAdminType;
        private ComboBox cmbSearchAdminStatus;
        private DateTimePicker dtpAdminStartDate;
        private DateTimePicker dtpAdminEndDate;
        private Button btnSearchAdmin;
        private Button btnResetSearchAdmin;
        private DataGridView dgvAdminSearchResults;

        public FormDashboardAdmin()
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
            UiTheme.StyleMenuButton(btnAkun);
            UiTheme.StyleMenuButton(btnDokter);
            UiTheme.StyleMenuButton(btnJadwal);
            UiTheme.StyleMenuButton(btnPenyakit);
            UiTheme.StyleMenuButton(btnObat);
            UiTheme.StyleMenuButton(btnReservasi);
            UiTheme.StyleMenuButton(btnCheckIn);
            UiTheme.StyleMenuButton(btnAntrian);
            UiTheme.StyleMenuButton(btnLaporan);
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
                RunAdminSearch(false);
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
            AdminDashboardSummary summary = _dashboardService.GetAdminSummary();
            Panel body;
            Panel section = DashboardUiHelper.CreateSection("Ringkasan Operasional", 1050, 245, out body);
            FlowLayoutPanel cards = DashboardUiHelper.CreateCardFlow();

            cards.Controls.Add(UiTheme.CreateSummaryCard("Total pasien", summary.TotalPatients.ToString(), "Data pasien terdaftar"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Dokter aktif", summary.TotalActiveDoctors.ToString(), "Dokter tersedia"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Reservasi hari ini", summary.ReservationsToday.ToString(), "Berdasarkan jadwal"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Menunggu verifikasi", summary.PendingReservations.ToString(), "Reservasi perlu dicek"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Check-in hari ini", summary.CheckedInPatientsToday.ToString(), "Pasien sudah hadir"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Pemeriksaan selesai", summary.CompletedExaminationsToday.ToString(), "Hari ini"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Stok rendah", summary.LowStockMedicines.ToString(), "Obat perlu perhatian"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Penyakit terbanyak", summary.TopDiseaseThisMonth, "Bulan ini"));

            body.Controls.Add(cards);
            dashboardContent.Controls.Add(section);
        }

        private void BuildChartSections()
        {
            AddChartSection("Reservasi 7 Hari Terakhir", _dashboardService.GetReservationsLast7Days(), SeriesChartType.Column);
            AddChartSection("Komposisi Status Reservasi", _dashboardService.GetReservationStatusDistribution(), SeriesChartType.Pie);
            AddChartSection("Jumlah Pasien per Dokter", _dashboardService.GetPatientCountByDoctor(), SeriesChartType.Bar);
            AddChartSection("Penyakit Paling Sering Bulan Ini", _dashboardService.GetTopDiseasesThisMonth(), SeriesChartType.Column);
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
            AddGridSection("Reservasi Terbaru", DashboardUiHelper.CreateGrid(_dashboardService.GetRecentReservations()));
            AddGridSection("Antrian Hari Ini", DashboardUiHelper.CreateGrid(_dashboardService.GetTodayQueues()));
            AddGridSection("Obat Stok Rendah", DashboardUiHelper.CreateGrid(_dashboardService.GetLowStockMedicines()));
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
            Panel section = DashboardUiHelper.CreateSection("Pencarian Cepat", 1050, 430, out body);
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

            txtSearchAdmin = CreateSearchTextBox();
            cmbSearchAdminType = CreateComboBox(new[] { "Semua", "Pasien", "Dokter", "Reservasi", "Antrian", "Obat", "Penyakit" });
            cmbSearchAdminStatus = CreateComboBox(new[] { "Semua", "Menunggu Verifikasi", "Dikonfirmasi", "Ditolak", "Dibatalkan Pasien", "Check-in", "Selesai", "Stok Rendah", "Aktif", "Nonaktif" });
            dtpAdminStartDate = CreateDatePicker(DateTime.Today.AddDays(-30));
            dtpAdminEndDate = CreateDatePicker(DateTime.Today);
            btnSearchAdmin = CreateButton("Cari", true);
            btnResetSearchAdmin = CreateButton("Reset", false);

            btnSearchAdmin.Click += BtnSearchAdmin_Click;
            btnResetSearchAdmin.Click += BtnResetSearchAdmin_Click;

            filters.Controls.Add(CreateFilterBlock("Keyword", txtSearchAdmin, 210));
            filters.Controls.Add(CreateFilterBlock("Tipe", cmbSearchAdminType, 150));
            filters.Controls.Add(CreateFilterBlock("Status", cmbSearchAdminStatus, 180));
            filters.Controls.Add(CreateFilterBlock("Mulai", dtpAdminStartDate, 130));
            filters.Controls.Add(CreateFilterBlock("Sampai", dtpAdminEndDate, 130));
            filters.Controls.Add(btnSearchAdmin);
            filters.Controls.Add(btnResetSearchAdmin);

            dgvAdminSearchResults = DashboardUiHelper.CreateGrid(new List<DashboardSearchResultItem>());

            layout.Controls.Add(filters, 0, 0);
            layout.Controls.Add(dgvAdminSearchResults, 0, 1);
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

        private ComboBox CreateComboBox(string[] items)
        {
            ComboBox comboBox = new ComboBox
            {
                Width = 145,
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

        private void RunAdminSearch(bool showEmptyMessage)
        {
            DashboardSearchCriteria criteria = new DashboardSearchCriteria
            {
                Keyword = txtSearchAdmin.Text,
                SearchType = Convert.ToString(cmbSearchAdminType.SelectedItem),
                Status = Convert.ToString(cmbSearchAdminStatus.SelectedItem),
                StartDate = dtpAdminStartDate.Value.Date,
                EndDate = dtpAdminEndDate.Value.Date
            };

            List<DashboardSearchResultItem> results = _dashboardService.SearchAdminDashboardData(criteria);
            DashboardUiHelper.BindGrid(dgvAdminSearchResults, results);

            if (showEmptyMessage && results.Count == 0)
            {
                MessageBox.Show(this, "Belum ada data", "Pencarian", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSearchAdmin_Click(object sender, EventArgs e)
        {
            RunAdminSearch(true);
        }

        private void BtnResetSearchAdmin_Click(object sender, EventArgs e)
        {
            txtSearchAdmin.Clear();
            cmbSearchAdminType.SelectedIndex = 0;
            cmbSearchAdminStatus.SelectedIndex = 0;
            dtpAdminStartDate.Value = DateTime.Today.AddDays(-30);
            dtpAdminEndDate.Value = DateTime.Today;
            RunAdminSearch(false);
        }

        private void LoadSessionInfo()
        {
            User user = SessionHelper.CurrentUser;

            if (user == null || user.Role != UserRole.Admin)
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
