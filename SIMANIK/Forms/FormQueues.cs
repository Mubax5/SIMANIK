using System;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormQueues : OperationalFormBase
    {
        private readonly VisitService _service = new VisitService();
        private DataGridView dgvQueues;
        private ComboBox cmbVisitStatus;
        private TextBox txtSearch;

        public FormQueues() : base("Antrian Hari Ini")
        {
            BuildUi();
            Load += delegate
            {
                if (EnsureAdminOrDoctor())
                {
                    LoadData();
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

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 86F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                WrapContents = true,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12)
            };

            cmbVisitStatus = CreateComboBox(180);
            cmbVisitStatus.Items.AddRange(new object[] { "Semua", "Menunggu", "Sedang Diperiksa", "Selesai" });
            cmbVisitStatus.SelectedIndex = 0;
            txtSearch = CreateTextBox(260);

            Button btnSearch = CreateButton("Cari", true);
            Button btnReset = CreateButton("Reset", false);
            Button btnRefresh = CreateButton("Refresh", false);

            btnSearch.Click += delegate { LoadData(); };
            btnReset.Click += delegate { txtSearch.Clear(); cmbVisitStatus.SelectedIndex = 0; LoadData(); };
            btnRefresh.Click += delegate { LoadData(); };

            filterPanel.Controls.Add(CreateField("Status", cmbVisitStatus, 190));
            filterPanel.Controls.Add(CreateField("Nama pasien / no. antrian", txtSearch, 270));
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);
            filterPanel.Controls.Add(btnRefresh);

            Panel gridSection = CreateSection("Daftar Antrian");
            dgvQueues = CreateGrid();
            gridSection.Controls.Add(dgvQueues);
            gridSection.Controls.SetChildIndex(dgvQueues, 0);

            layout.Controls.Add(filterPanel, 0, 0);
            layout.Controls.Add(gridSection, 0, 1);
            RootLayout.Controls.Add(layout, 0, 1);
        }

        private void LoadData()
        {
            dgvQueues.DataSource = _service.GetTodayQueuesForCurrentUser(Convert.ToString(cmbVisitStatus.SelectedItem), txtSearch.Text);
            UiTheme.ResizeDataGridViewColumns(dgvQueues);
            dgvQueues.ClearSelection();

            if (dgvQueues.Rows.Count == 0)
            {
                Text = "SIMANIK - Antrian Hari Ini";
            }
        }
    }
}
