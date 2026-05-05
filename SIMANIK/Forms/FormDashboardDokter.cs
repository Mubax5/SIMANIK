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
    public partial class FormDashboardDokter : Form
    {
        private readonly AuthService _authService;
        private readonly DashboardService _dashboardService;
        private FlowLayoutPanel summaryContent;
        private FlowLayoutPanel chartContent;
        private FlowLayoutPanel tableContent;
        private FlowLayoutPanel searchContent;
        private Button btnAkun;
        private TextBox txtSearchDoctor;
        private ComboBox cmbSearchDoctorType;
        private ComboBox cmbDoctorQueueStatus;
        private DateTimePicker dtpDoctorStartDate;
        private DateTimePicker dtpDoctorEndDate;
        private Button btnSearchDoctor;
        private Button btnResetSearchDoctor;
        private DataGridView dgvDoctorSearchResults;

        public FormDashboardDokter()
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
            UiTheme.StyleMenuButton(btnAntrian);
            UiTheme.StyleMenuButton(btnPemeriksaan);
            UiTheme.StyleMenuButton(btnRiwayat);
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
            btnAkun = new Button
            {
                Text = "Akun",
                Width = 92,
                Height = 40,
                Margin = new Padding(8, 0, 0, 0)
            };
            UiTheme.StyleMenuButton(btnAkun);
            btnAkun.Click += ShowPendingFeature;
            btnLogout.Width = 92;
            btnLogout.Height = 40;
            btnLogout.Margin = new Padding(8, 0, 0, 0);
            btnAntrian.Click -= ShowPendingFeature;
            btnPemeriksaan.Click -= ShowPendingFeature;
            btnRiwayat.Click -= ShowPendingFeature;
            btnAntrian.Click += delegate { OpenOperationalForm(new FormQueues()); };
            btnPemeriksaan.Click += delegate { OpenOperationalForm(new FormExaminations()); };
            btnRiwayat.Click += delegate { OpenOperationalForm(new FormHistory()); };
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
                RunDoctorSearch(false);
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
            DoctorDashboardSummary summary = _dashboardService.GetDoctorSummary(SessionHelper.CurrentUser.Id);
            Panel body;
            Panel section = DashboardUiHelper.CreateSection("Ringkasan Dokter", 150, DashboardUiHelper.SectionWidthMode.Full, false, out body);
            FlowLayoutPanel cards = DashboardUiHelper.CreateCardFlow();

            cards.Controls.Add(UiTheme.CreateSummaryCard("Jadwal hari ini", summary.TodaySchedules.ToString(), "Praktik aktif"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Pasien menunggu", summary.WaitingPatients.ToString(), "Antrian saat ini"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Sedang diperiksa", summary.InProgressPatients.ToString(), "Dalam pemeriksaan"));
            cards.Controls.Add(UiTheme.CreateSummaryCard("Selesai hari ini", summary.CompletedExaminationsToday.ToString(), "Pemeriksaan selesai"));

            body.Controls.Add(cards);
            summaryContent.Controls.Add(section);

            BuildQuickMenuSection();
        }

        private void BuildQuickMenuSection()
        {
            Panel body;
            Panel section = DashboardUiHelper.CreateSection("Menu Dokter", 120, DashboardUiHelper.SectionWidthMode.Full, false, out body);
            FlowLayoutPanel quickMenu = DashboardUiHelper.CreateCardFlow();

            ConfigureQuickMenuButton(btnAntrian);
            ConfigureQuickMenuButton(btnPemeriksaan);
            ConfigureQuickMenuButton(btnRiwayat);

            quickMenu.Controls.Add(btnAntrian);
            quickMenu.Controls.Add(btnPemeriksaan);
            quickMenu.Controls.Add(btnRiwayat);

            body.Controls.Add(quickMenu);
            summaryContent.Controls.Add(section);
        }

        private void ConfigureQuickMenuButton(Button button)
        {
            button.Width = 150;
            button.Height = 44;
            button.Margin = new Padding(0, 0, 12, 10);
            UiTheme.StyleMenuButton(button);
        }

        private void BuildChartSections()
        {
            int userId = SessionHelper.CurrentUser.Id;
            AddChartSection("Status Antrian Hari Ini", _dashboardService.GetDoctorQueueStatusDistribution(userId), SeriesChartType.Pie);
            AddChartSection("Pemeriksaan 7 Hari Terakhir", _dashboardService.GetDoctorExaminationsLast7Days(userId), SeriesChartType.Column);
            AddChartSection("Penyakit Sering Ditangani", _dashboardService.GetDoctorTopDiseases(userId), SeriesChartType.Bar);
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
            int userId = SessionHelper.CurrentUser.Id;
            AddGridSection("Antrian Pasien Hari Ini", DashboardUiHelper.CreateGrid(_dashboardService.GetDoctorTodayQueue(userId)));
            AddGridSection("Jadwal Dokter Hari Ini", DashboardUiHelper.CreateGrid(_dashboardService.SearchDoctorDashboardData(userId, new DashboardSearchCriteria
            {
                SearchType = "Jadwal Praktik",
                Status = "Semua",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            })));
            AddGridSection("Riwayat Pemeriksaan Terakhir", DashboardUiHelper.CreateGrid(_dashboardService.GetDoctorRecentExaminations(userId)));
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
            Panel section = DashboardUiHelper.CreateSection("Cari Pasien / Antrian", 500, DashboardUiHelper.SectionWidthMode.Full, false, out body);
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

            txtSearchDoctor = CreateSearchTextBox();
            cmbSearchDoctorType = CreateComboBox(new[] { "Antrian Hari Ini", "Riwayat Pemeriksaan", "Jadwal Praktik", "Diagnosa/Penyakit" }, 185);
            cmbDoctorQueueStatus = CreateComboBox(new[] { "Semua", "Menunggu", "Sedang Diperiksa", "Selesai" }, 160);
            dtpDoctorStartDate = CreateDatePicker(DateTime.Today.AddDays(-30));
            dtpDoctorEndDate = CreateDatePicker(DateTime.Today);
            btnSearchDoctor = CreateButton("Cari", true);
            btnResetSearchDoctor = CreateButton("Reset", false);

            btnSearchDoctor.Click += BtnSearchDoctor_Click;
            btnResetSearchDoctor.Click += BtnResetSearchDoctor_Click;

            filters.Controls.Add(CreateFilterBlock("Keyword", txtSearchDoctor, 210));
            filters.Controls.Add(CreateFilterBlock("Tipe", cmbSearchDoctorType, 190));
            filters.Controls.Add(CreateFilterBlock("Status", cmbDoctorQueueStatus, 165));
            filters.Controls.Add(CreateFilterBlock("Mulai", dtpDoctorStartDate, 130));
            filters.Controls.Add(CreateFilterBlock("Sampai", dtpDoctorEndDate, 130));
            filters.Controls.Add(btnSearchDoctor);
            filters.Controls.Add(btnResetSearchDoctor);

            dgvDoctorSearchResults = DashboardUiHelper.CreateGrid(new List<DashboardSearchResultItem>());

            layout.Controls.Add(filters, 0, 0);
            layout.Controls.Add(dgvDoctorSearchResults, 0, 1);
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

        private void RunDoctorSearch(bool showEmptyMessage)
        {
            DashboardSearchCriteria criteria = new DashboardSearchCriteria
            {
                Keyword = txtSearchDoctor.Text,
                SearchType = Convert.ToString(cmbSearchDoctorType.SelectedItem),
                Status = Convert.ToString(cmbDoctorQueueStatus.SelectedItem),
                StartDate = dtpDoctorStartDate.Value.Date,
                EndDate = dtpDoctorEndDate.Value.Date
            };

            List<DashboardSearchResultItem> results = _dashboardService.SearchDoctorDashboardData(SessionHelper.CurrentUser.Id, criteria);
            DashboardUiHelper.BindGrid(dgvDoctorSearchResults, results);

            if (showEmptyMessage && results.Count == 0)
            {
                MessageBox.Show(this, "Belum ada data", "Pencarian", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSearchDoctor_Click(object sender, EventArgs e)
        {
            RunDoctorSearch(true);
        }

        private void BtnResetSearchDoctor_Click(object sender, EventArgs e)
        {
            txtSearchDoctor.Clear();
            cmbSearchDoctorType.SelectedIndex = 0;
            cmbDoctorQueueStatus.SelectedIndex = 0;
            dtpDoctorStartDate.Value = DateTime.Today.AddDays(-30);
            dtpDoctorEndDate.Value = DateTime.Today;
            RunDoctorSearch(false);
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

        private void OpenOperationalForm(Form form)
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
