using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormHistory : OperationalFormBase
    {
        private readonly HistoryService _service = new HistoryService();

        private ComboBox cmbHistoryType;
        private TextBox txtSearch;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnSearch;
        private Button btnReset;
        private DataGridView dgvHistory;

        public FormHistory() : base("Riwayat")
        {
            BuildUi();
            Load += delegate
            {
                if (EnsureLoggedIn())
                {
                    ConfigureForRole();
                    LoadData(false);
                }
            };
        }

        private void BuildUi()
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12)
            };

            cmbHistoryType = CreateComboBox(170);
            cmbHistoryType.Items.AddRange(new object[] { "Reservasi", "Pemeriksaan", "Obat", "Diagnosa" });
            cmbHistoryType.SelectedIndex = 0;

            txtSearch = CreateTextBox(280);
            dtpStartDate = CreateDatePicker(DateTime.Today.AddMonths(-6));
            dtpEndDate = CreateDatePicker(DateTime.Today);
            btnSearch = CreateButton("Cari", true);
            btnReset = CreateButton("Reset", false);

            btnSearch.Click += delegate { LoadData(true); };
            btnReset.Click += delegate { ResetFilters(); };

            filterPanel.Controls.Add(CreateField("Jenis riwayat", cmbHistoryType, 180));
            filterPanel.Controls.Add(CreateField("Cari pasien/dokter/diagnosa/obat", txtSearch, 290));
            filterPanel.Controls.Add(CreateField("Tanggal mulai", dtpStartDate, 140));
            filterPanel.Controls.Add(CreateField("Tanggal akhir", dtpEndDate, 140));
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);

            Panel section = CreateSection("Data Riwayat");
            dgvHistory = CreateGrid();
            section.Controls.Add(dgvHistory);
            section.Controls.SetChildIndex(dgvHistory, 0);

            layout.Controls.Add(filterPanel, 0, 0);
            layout.Controls.Add(section, 0, 1);
            RootLayout.Controls.Add(layout, 0, 1);
        }

        private DateTimePicker CreateDatePicker(DateTime value)
        {
            DateTimePicker picker = new DateTimePicker
            {
                Width = 130,
                Format = DateTimePickerFormat.Short,
                Value = value
            };

            UiTheme.StyleDatePicker(picker);
            return picker;
        }

        private bool EnsureLoggedIn()
        {
            if (SessionHelper.CurrentUser == null)
            {
                MessageBox.Show(this, "Silakan login terlebih dahulu.", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return false;
            }

            return true;
        }

        private void ConfigureForRole()
        {
            if (SessionHelper.HasRole(UserRole.Pasien))
            {
                Text = "SIMANIK - Riwayat Saya";
                TitleLabel.Text = "Riwayat Saya";
            }
            else if (SessionHelper.HasRole(UserRole.Dokter))
            {
                Text = "SIMANIK - Riwayat Pasien Dokter";
                TitleLabel.Text = "Riwayat Pasien Dokter";
            }
            else
            {
                Text = "SIMANIK - Riwayat Semua Pemeriksaan";
                TitleLabel.Text = "Riwayat Semua Pemeriksaan";
            }
        }

        private void LoadData(bool showEmptyMessage)
        {
            string type = Convert.ToString(cmbHistoryType.SelectedItem);
            List<HistoryItem> items = _service.GetHistoryForCurrentUser(type, txtSearch.Text, dtpStartDate.Value.Date, dtpEndDate.Value.Date);

            dgvHistory.DataSource = null;
            dgvHistory.DataSource = items;
            UiTheme.ResizeDataGridViewColumns(dgvHistory);
            dgvHistory.ClearSelection();

            if (showEmptyMessage && items.Count == 0)
            {
                MessageBox.Show(this, "Belum ada data riwayat yang sesuai.", "Riwayat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetFilters()
        {
            cmbHistoryType.SelectedIndex = 0;
            txtSearch.Clear();
            dtpStartDate.Value = DateTime.Today.AddMonths(-6);
            dtpEndDate.Value = DateTime.Today;
            LoadData(false);
        }
    }
}
