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
        private FlowLayoutPanel summaryContent;
        private FlowLayoutPanel chartContent;
        private FlowLayoutPanel tableContent;
        private FlowLayoutPanel searchContent;
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
            menuPanel.Controls.Clear();
            Button refreshButton = new Button
            {
                Text = "Refresh Dashboard",
                Width = 145,
                Height = 40,
                Margin = new Padding(8, 0, 0, 0)
            };
            UiTheme.StylePrimaryButton(refreshButton);
            refreshButton.Click += delegate { RefreshDashboard(); };
            btnAkun.Width = 92;
            btnAkun.Height = 40;
            btnAkun.Margin = new Padding(8, 0, 0, 0);
            btnLogout.Width = 92;
            btnLogout.Height = 40;
            btnLogout.Margin = new Padding(8, 0, 0, 0);
            btnAkun.Click -= ShowPendingFeature;
            btnDokter.Click -= ShowPendingFeature;
            btnJadwal.Click -= ShowPendingFeature;
            btnPenyakit.Click -= ShowPendingFeature;
            btnObat.Click -= ShowPendingFeature;
            btnReservasi.Click -= ShowPendingFeature;
            btnCheckIn.Click -= ShowPendingFeature;
            btnAntrian.Click -= ShowPendingFeature;
            btnAkun.Click += delegate { OpenMasterForm(new FormUsers()); };
            btnDokter.Click += delegate { OpenMasterForm(new FormDoctors()); };
            btnJadwal.Click += delegate { OpenMasterForm(new FormDoctorSchedules()); };
            btnPenyakit.Click += delegate { OpenMasterForm(new FormDiseases()); };
            btnObat.Click += delegate { OpenMasterForm(new FormMedicines()); };
            btnReservasi.Click += delegate { OpenMasterForm(new FormReservationAdmin()); };
            btnCheckIn.Click += delegate { OpenMasterForm(new FormCheckIn()); };
            btnAntrian.Click += delegate { OpenMasterForm(new FormQueues()); };
            menuPanel.Controls.Add(btnLogout);
            menuPanel.Controls.Add(btnAkun);
            menuPanel.Controls.Add(refreshButton);

            DashboardUiHelper.DashboardLayout layout = DashboardUiHelper.PrepareDashboardContent(this, rootLayout, menuPanel, lblTitle);
            summaryContent = layout.SummaryContent;
            chartContent = layout.ChartContent;
            tableContent = layout.TableContent;
            searchContent = layout.SearchContent;
        }

        private void RefreshDashboard()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                summaryContent.Controls.Clear();
                chartContent.Controls.Clear();
                tableContent.Controls.Clear();
                searchContent.Controls.Clear();

                BuildSummarySection();
                BuildChartSections();
                BuildTableSections();
                BuildSearchSection();
                RunAdminSearch(false);
                DashboardUiHelper.AdjustResponsiveSections(summaryContent);
                DashboardUiHelper.AdjustResponsiveSections(chartContent);
                DashboardUiHelper.AdjustResponsiveSections(tableContent);
                DashboardUiHelper.AdjustResponsiveSections(searchContent);
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
            Panel section = DashboardUiHelper.CreateSection("Ringkasan Operasional", 285, DashboardUiHelper.SectionWidthMode.Full, false, out body);
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
            summaryContent.Controls.Add(section);

            BuildQuickMenuSection();
        }

        private void BuildQuickMenuSection()
        {
            Panel body;
            Panel section = DashboardUiHelper.CreateSection("Menu Operasional", 180, DashboardUiHelper.SectionWidthMode.Full, false, out body);
            FlowLayoutPanel quickMenu = DashboardUiHelper.CreateCardFlow();

            ConfigureQuickMenuButton(btnDokter);
            ConfigureQuickMenuButton(btnJadwal);
            ConfigureQuickMenuButton(btnPenyakit);
            ConfigureQuickMenuButton(btnObat);
            ConfigureQuickMenuButton(btnReservasi);
            ConfigureQuickMenuButton(btnCheckIn);
            ConfigureQuickMenuButton(btnAntrian);
            ConfigureQuickMenuButton(btnLaporan);

            quickMenu.Controls.Add(btnDokter);
            quickMenu.Controls.Add(btnJadwal);
            quickMenu.Controls.Add(btnPenyakit);
            quickMenu.Controls.Add(btnObat);
            quickMenu.Controls.Add(btnReservasi);
            quickMenu.Controls.Add(btnCheckIn);
            quickMenu.Controls.Add(btnAntrian);
            quickMenu.Controls.Add(btnLaporan);

            body.Controls.Add(quickMenu);
            summaryContent.Controls.Add(section);
        }

        private void ConfigureQuickMenuButton(Button button)
        {
            button.Width = 132;
            button.Height = 44;
            button.Margin = new Padding(0, 0, 12, 10);
            UiTheme.StyleMenuButton(button);
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
            Panel section = DashboardUiHelper.CreateSection(title, 300, DashboardUiHelper.SectionWidthMode.Half, false, out body);
            body.Controls.Add(DashboardUiHelper.CreateChart(title, points, chartType));
            chartContent.Controls.Add(section);
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
            Panel section = DashboardUiHelper.CreateSection(title, 430, DashboardUiHelper.SectionWidthMode.Full, false, out body);
            body.Controls.Add(grid);
            tableContent.Controls.Add(section);
        }

        private void BuildSearchSection()
        {
            Panel body;
            Panel section = DashboardUiHelper.CreateSection("Pencarian Cepat", 500, DashboardUiHelper.SectionWidthMode.Full, false, out body);
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
                WrapContents = true
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
            searchContent.Controls.Add(section);
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
                Width = 70,
                Height = 33,
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

        private void OpenMasterForm(Form form)
        {
            using (form)
            {
                form.ShowDialog(this);
            }

            RefreshDashboard();
        }

        private void Logout(object sender, EventArgs e)
        {
            _authService.Logout();
            Close();
        }

    }
}
