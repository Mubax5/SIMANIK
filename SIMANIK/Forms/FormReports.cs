using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormReports : OperationalFormBase
    {
        private readonly ReportService _service = new ReportService();

        private ComboBox cmbReportType;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnShowReport;
        private Button btnReset;
        private DataGridView dgvReport;
        private Label lblSummary;
        private Chart chartReport;

        public FormReports() : base("Laporan Klinik")
        {
            BuildUi();
            Load += delegate
            {
                if (EnsureRole(UserRole.Admin))
                {
                    LoadReport();
                }
            };
        }

        private void BuildUi()
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12)
            };

            cmbReportType = CreateComboBox(240);
            cmbReportType.Items.AddRange(new object[]
            {
                "Reservasi Per Hari",
                "Reservasi Per Dokter",
                "Kunjungan Selesai Per Hari",
                "Jumlah Pasien Per Dokter",
                "Penyakit Paling Sering",
                "Obat Paling Sering",
                "Obat Stok Rendah",
                "Pemeriksaan Per Periode"
            });
            cmbReportType.SelectedIndex = 0;

            dtpStartDate = CreateDatePicker(DateTime.Today.AddMonths(-1));
            dtpEndDate = CreateDatePicker(DateTime.Today);
            btnShowReport = CreateButton("Tampilkan", true);
            btnReset = CreateButton("Reset", false);

            btnShowReport.Click += delegate { LoadReport(); };
            btnReset.Click += delegate { ResetFilter(); };

            filterPanel.Controls.Add(CreateField("Jenis laporan", cmbReportType, 250));
            filterPanel.Controls.Add(CreateField("Tanggal mulai", dtpStartDate, 145));
            filterPanel.Controls.Add(CreateField("Tanggal akhir", dtpEndDate, 145));
            filterPanel.Controls.Add(btnShowReport);
            filterPanel.Controls.Add(btnReset);

            lblSummary = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12, 0, 12, 0)
            };
            UiTheme.StyleInfoLabel(lblSummary);

            TableLayoutPanel reportLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            reportLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 300F));
            reportLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel chartSection = CreateSection("Grafik Laporan");
            chartReport = CreateReportChart();
            chartSection.Controls.Add(chartReport);
            chartSection.Controls.SetChildIndex(chartReport, 0);

            Panel gridSection = CreateSection("Tabel Laporan");
            dgvReport = CreateGrid();
            gridSection.Controls.Add(dgvReport);
            gridSection.Controls.SetChildIndex(dgvReport, 0);

            reportLayout.Controls.Add(chartSection, 0, 0);
            reportLayout.Controls.Add(gridSection, 0, 1);

            layout.Controls.Add(filterPanel, 0, 0);
            layout.Controls.Add(lblSummary, 0, 1);
            layout.Controls.Add(reportLayout, 0, 2);
            RootLayout.Controls.Add(layout, 0, 1);
        }

        private DateTimePicker CreateDatePicker(DateTime value)
        {
            DateTimePicker picker = new DateTimePicker
            {
                Width = 135,
                Format = DateTimePickerFormat.Short,
                Value = value
            };

            UiTheme.StyleDatePicker(picker);
            return picker;
        }

        private void LoadReport()
        {
            ReportData report = _service.GetReport(
                Convert.ToString(cmbReportType.SelectedItem),
                dtpStartDate.Value.Date,
                dtpEndDate.Value.Date);

            dgvReport.DataSource = null;
            dgvReport.DataSource = report.Items;
            UiTheme.ResizeDataGridViewColumns(dgvReport);
            dgvReport.ClearSelection();

            lblSummary.Text = string.IsNullOrWhiteSpace(report.Summary) ? "Belum ada data" : report.Summary;
            BindChart(report.ReportType, report.ChartPoints);
        }

        private void ResetFilter()
        {
            cmbReportType.SelectedIndex = 0;
            dtpStartDate.Value = DateTime.Today.AddMonths(-1);
            dtpEndDate.Value = DateTime.Today;
            LoadReport();
        }

        private Chart CreateReportChart()
        {
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            ChartArea area = new ChartArea("Main");
            area.AxisX.Title = "Kategori";
            area.AxisY.Title = "Jumlah";
            area.AxisX.Interval = 1;
            chart.ChartAreas.Add(area);

            chart.Legends.Add(new Legend("Legend")
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                Title = "Keterangan"
            });

            chart.Titles.Add("Laporan Klinik");
            UiTheme.StyleChart(chart);
            return chart;
        }

        private void BindChart(string title, List<ChartDataPoint> points)
        {
            chartReport.Series.Clear();
            chartReport.Titles.Clear();
            chartReport.Titles.Add(string.IsNullOrWhiteSpace(title) ? "Laporan Klinik" : title);

            Series series = new Series("Jumlah")
            {
                ChartType = SeriesChartType.Column,
                ChartArea = "Main",
                Legend = "Legend",
                IsValueShownAsLabel = true,
                Color = UiTheme.Secondary
            };

            bool hasData = false;
            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].Value > 0)
                    {
                        hasData = true;
                        break;
                    }
                }
            }

            if (!hasData)
            {
                DataPoint point = new DataPoint();
                point.SetValueXY("Belum ada data", 0);
                point.Label = "Belum ada data";
                point.LegendText = "Belum ada data";
                series.Points.Add(point);
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    DataPoint point = new DataPoint();
                    point.SetValueXY(points[i].Label, points[i].Value);
                    point.Label = "#VAL";
                    point.LegendText = points[i].Label;
                    series.Points.Add(point);
                }
            }

            chartReport.Series.Add(series);
            UiTheme.StyleChart(chartReport);
        }
    }
}
