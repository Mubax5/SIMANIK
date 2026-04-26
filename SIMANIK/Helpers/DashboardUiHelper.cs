using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SIMANIK.Models;

namespace SIMANIK.Helpers
{
    public static class DashboardUiHelper
    {
        public static FlowLayoutPanel PrepareDashboardContent(Form form, TableLayoutPanel rootLayout, FlowLayoutPanel menuPanel)
        {
            form.ClientSize = new Size(1180, 780);
            form.MinimumSize = new Size(980, 650);

            rootLayout.SuspendLayout();
            rootLayout.Controls.Remove(menuPanel);
            rootLayout.RowStyles.Clear();
            rootLayout.RowCount = 4;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(menuPanel, 0, 2);
            rootLayout.ResumeLayout(false);

            menuPanel.Height = 60;
            menuPanel.WrapContents = false;
            menuPanel.AutoScroll = true;

            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = UiTheme.Background
            };

            FlowLayoutPanel contentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = true,
                Padding = new Padding(0, 6, 0, 24),
                BackColor = UiTheme.Background
            };

            scrollPanel.Controls.Add(contentPanel);
            rootLayout.Controls.Add(scrollPanel, 0, 3);

            return contentPanel;
        }

        public static Panel CreateSection(string title, int width, int height, out Panel body)
        {
            Panel section = new Panel
            {
                Width = width,
                Height = height,
                BackColor = UiTheme.Panel,
                Margin = new Padding(0, 0, 16, 16),
                Padding = new Padding(12)
            };

            Label titleLabel = UiTheme.CreateSectionTitle(title);

            body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Panel,
                Padding = new Padding(0, 8, 0, 0)
            };

            section.Controls.Add(body);
            section.Controls.Add(titleLabel);

            return section;
        }

        public static FlowLayoutPanel CreateCardFlow()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = UiTheme.Panel
            };
        }

        public static DataGridView CreateGrid(IEnumerable dataSource)
        {
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = dataSource
            };

            UiTheme.StyleDataGridView(grid);
            return grid;
        }

        public static void BindGrid(DataGridView grid, IEnumerable dataSource)
        {
            grid.DataSource = null;
            grid.DataSource = dataSource;
            UiTheme.StyleDataGridView(grid);
        }

        public static Chart CreateChart(string title, List<ChartDataPoint> points, SeriesChartType chartType)
        {
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };

            ChartArea area = new ChartArea("Default");
            chart.ChartAreas.Add(area);
            chart.Titles.Add(title);

            Series series = new Series("Data")
            {
                ChartArea = "Default",
                ChartType = chartType,
                IsValueShownAsLabel = true,
                Color = UiTheme.Secondary
            };

            if (points == null || points.Count == 0)
            {
                DataPoint point = new DataPoint();
                point.SetValueXY("Belum ada data", chartType == SeriesChartType.Pie ? 1 : 0);
                point.Label = "Belum ada data";
                series.Points.Add(point);
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    double value = points[i].Value;

                    if (chartType == SeriesChartType.Pie && value <= 0 && points.Count == 1)
                    {
                        value = 1;
                    }

                    DataPoint point = new DataPoint();
                    point.SetValueXY(points[i].Label, value);

                    if (points[i].Value <= 0 && points.Count == 1)
                    {
                        point.Label = points[i].Label;
                    }

                    series.Points.Add(point);
                }
            }

            chart.Series.Add(series);
            UiTheme.StyleChart(chart);

            return chart;
        }

        public static Label CreateEmptyLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Bottom,
                Height = 24,
                ForeColor = UiTheme.TextSecondary,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }
    }
}
