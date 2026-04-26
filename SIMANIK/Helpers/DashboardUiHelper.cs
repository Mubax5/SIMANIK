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
        public enum SectionWidthMode
        {
            Full,
            Half,
            Third
        }

        public class DashboardLayout
        {
            public TabControl Tabs { get; set; }
            public FlowLayoutPanel SummaryContent { get; set; }
            public FlowLayoutPanel ChartContent { get; set; }
            public FlowLayoutPanel TableContent { get; set; }
            public FlowLayoutPanel SearchContent { get; set; }
        }

        private class SectionSizing
        {
            public SectionWidthMode WidthMode { get; set; }
            public int MinWidth { get; set; }
        }

        public static DashboardLayout PrepareDashboardContent(Form form, TableLayoutPanel rootLayout, FlowLayoutPanel menuPanel, Label titleLabel)
        {
            form.ClientSize = new Size(1280, 760);
            form.MinimumSize = new Size(1040, 650);

            rootLayout.SuspendLayout();
            rootLayout.Controls.Remove(titleLabel);
            rootLayout.Controls.Remove(menuPanel);
            rootLayout.RowStyles.Clear();
            rootLayout.RowCount = 3;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            TableLayoutPanel headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Background,
                ColumnCount = 2,
                RowCount = 1
            };

            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            headerLayout.Controls.Add(titleLabel, 0, 0);
            headerLayout.Controls.Add(menuPanel, 1, 0);

            rootLayout.Controls.Add(headerLayout, 0, 0);
            rootLayout.ResumeLayout(false);

            menuPanel.Dock = DockStyle.Fill;
            menuPanel.WrapContents = false;
            menuPanel.AutoScroll = false;
            menuPanel.FlowDirection = FlowDirection.RightToLeft;
            menuPanel.Padding = new Padding(0, 8, 0, 0);

            TabControl tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };

            FlowLayoutPanel summaryContent = CreateTabPage(tabs, "Ringkasan", false);
            FlowLayoutPanel chartContent = CreateTabPage(tabs, "Grafik", true);
            FlowLayoutPanel tableContent = CreateTabPage(tabs, "Tabel", true);
            FlowLayoutPanel searchContent = CreateTabPage(tabs, "Pencarian", true);

            rootLayout.Controls.Add(tabs, 0, 2);

            form.Resize += delegate
            {
                AdjustResponsiveSections(summaryContent);
                AdjustResponsiveSections(chartContent);
                AdjustResponsiveSections(tableContent);
                AdjustResponsiveSections(searchContent);
            };

            return new DashboardLayout
            {
                Tabs = tabs,
                SummaryContent = summaryContent,
                ChartContent = chartContent,
                TableContent = tableContent,
                SearchContent = searchContent
            };
        }

        private static FlowLayoutPanel CreateTabPage(TabControl tabs, string title, bool scrollable)
        {
            TabPage page = new TabPage
            {
                Text = title,
                BackColor = UiTheme.Background
            };

            FlowLayoutPanel contentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = scrollable,
                WrapContents = true,
                Padding = new Padding(8),
                BackColor = UiTheme.Background
            };

            contentPanel.Resize += delegate { AdjustResponsiveSections(contentPanel); };
            contentPanel.ControlAdded += delegate { AdjustResponsiveSections(contentPanel); };
            page.Controls.Add(contentPanel);
            tabs.TabPages.Add(page);

            return contentPanel;
        }

        public static Panel CreateSection(string title, int width, int height, out Panel body)
        {
            return CreateSection(title, height, SectionWidthMode.Full, false, out body);
        }

        public static Panel CreateSection(string title, int height, SectionWidthMode widthMode, bool bodyScrollable, out Panel body)
        {
            Panel section = new Panel
            {
                Width = GetMinSectionWidth(widthMode),
                Height = height,
                BackColor = UiTheme.Panel,
                Margin = new Padding(0, 0, 16, 16),
                Padding = new Padding(12),
                Tag = new SectionSizing
                {
                    WidthMode = widthMode,
                    MinWidth = GetMinSectionWidth(widthMode)
                }
            };

            Label titleLabel = UiTheme.CreateSectionTitle(title);

            body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Panel,
                Padding = new Padding(0, 8, 0, 0),
                AutoScroll = bodyScrollable
            };

            section.Controls.Add(body);
            section.Controls.Add(titleLabel);

            return section;
        }

        public static void AdjustResponsiveSections(FlowLayoutPanel contentPanel)
        {
            if (contentPanel == null || contentPanel.ClientSize.Width <= 0)
            {
                return;
            }

            int availableWidth = Math.Max(280, contentPanel.ClientSize.Width - contentPanel.Padding.Horizontal - 24);

            for (int i = 0; i < contentPanel.Controls.Count; i++)
            {
                Control control = contentPanel.Controls[i];
                SectionSizing sizing = control.Tag as SectionSizing;

                if (sizing == null)
                {
                    continue;
                }

                control.Width = GetResponsiveSectionWidth(availableWidth, sizing);
            }
        }

        private static int GetResponsiveSectionWidth(int availableWidth, SectionSizing sizing)
        {
            if (sizing.WidthMode == SectionWidthMode.Full || availableWidth <= sizing.MinWidth)
            {
                return availableWidth;
            }

            if (sizing.WidthMode == SectionWidthMode.Half)
            {
                if (availableWidth >= (sizing.MinWidth * 2) + 32)
                {
                    return (availableWidth - 32) / 2;
                }

                return availableWidth;
            }

            if (availableWidth >= (sizing.MinWidth * 3) + 48)
            {
                return (availableWidth - 48) / 3;
            }

            if (availableWidth >= (sizing.MinWidth * 2) + 32)
            {
                return (availableWidth - 32) / 2;
            }

            return availableWidth;
        }

        private static int GetMinSectionWidth(SectionWidthMode widthMode)
        {
            if (widthMode == SectionWidthMode.Half)
            {
                return 460;
            }

            if (widthMode == SectionWidthMode.Third)
            {
                return 340;
            }

            return 620;
        }

        public static FlowLayoutPanel CreateCardFlow()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
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
