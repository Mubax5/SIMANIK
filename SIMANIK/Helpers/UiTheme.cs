using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SIMANIK.Helpers
{
    public static class UiTheme
    {
        public static readonly Color Primary = ColorTranslator.FromHtml("#002657");
        public static readonly Color Secondary = ColorTranslator.FromHtml("#003EAA");
        public static readonly Color Accent = ColorTranslator.FromHtml("#D6F1FA");
        public static readonly Color Background = ColorTranslator.FromHtml("#F4FBFE");
        public static readonly Color Panel = ColorTranslator.FromHtml("#EAF7FC");
        public static readonly Color TextPrimary = ColorTranslator.FromHtml("#072A38");
        public static readonly Color TextSecondary = ColorTranslator.FromHtml("#4C7382");
        public static readonly Color Border = ColorTranslator.FromHtml("#B7E5F3");

        public static void ApplyBase(Form form)
        {
            form.BackColor = Background;
            form.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        public static void StyleHeader(Label title, string text)
        {
            title.Text = text;
            title.BackColor = Primary;
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            title.Padding = new Padding(24, 0, 24, 0);
        }

        public static void StylePageTitle(Label title)
        {
            title.ForeColor = Primary;
            title.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
        }

        public static void StylePanel(Control panel)
        {
            panel.BackColor = Panel;
        }

        public static void StyleLabel(Label label)
        {
            label.ForeColor = TextSecondary;
            label.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        }

        public static void StyleInfoLabel(Label label)
        {
            label.ForeColor = TextPrimary;
            label.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
        }

        public static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Color.White;
            textBox.ForeColor = TextPrimary;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
        }

        public static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Color.White;
            comboBox.ForeColor = TextPrimary;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
        }

        public static void StyleDatePicker(DateTimePicker dateTimePicker)
        {
            dateTimePicker.CalendarMonthBackground = Color.White;
            dateTimePicker.CalendarForeColor = TextPrimary;
            dateTimePicker.CalendarTitleBackColor = Primary;
            dateTimePicker.CalendarTitleForeColor = Color.White;
            dateTimePicker.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
        }

        public static void StylePrimaryButton(Button button)
        {
            StyleButtonBase(button);
            button.BackColor = Secondary;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderColor = Secondary;
            button.FlatAppearance.MouseOverBackColor = Primary;
            button.FlatAppearance.MouseDownBackColor = Primary;
        }

        public static void StyleSecondaryButton(Button button)
        {
            StyleButtonBase(button);
            button.BackColor = Accent;
            button.ForeColor = Primary;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = Panel;
            button.FlatAppearance.MouseDownBackColor = Border;
        }

        public static void StyleMenuButton(Button button)
        {
            StyleButtonBase(button);
            button.BackColor = Color.White;
            button.ForeColor = Primary;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = Accent;
            button.FlatAppearance.MouseDownBackColor = Panel;
            button.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            button.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static void StyleLogoutButton(Button button)
        {
            StyleButtonBase(button);
            button.BackColor = Primary;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderColor = Primary;
            button.FlatAppearance.MouseOverBackColor = Secondary;
            button.FlatAppearance.MouseDownBackColor = Secondary;
        }

        public static void StyleDataGridView(DataGridView grid)
        {
            if (grid == null)
            {
                return;
            }

            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Border;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Accent;
            grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Background;

            ApplyDataGridViewScrollableStyle(grid);
        }

        public static void ApplyDataGridViewScrollableStyle(DataGridView grid)
        {
            if (grid == null)
            {
                return;
            }

            grid.AutoGenerateColumns = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.ScrollBars = ScrollBars.Both;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            grid.AllowUserToResizeColumns = true;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;

            grid.DataBindingComplete -= Grid_DataBindingComplete;
            grid.DataBindingComplete += Grid_DataBindingComplete;
            ResizeDataGridViewColumns(grid);
        }

        public static void ResizeDataGridViewColumns(DataGridView grid)
        {
            if (grid == null || grid.Columns.Count == 0)
            {
                return;
            }

            grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            grid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }

        private static void Grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ResizeDataGridViewColumns(sender as DataGridView);
        }

        public static void StyleChart(Chart chart)
        {
            chart.BackColor = Color.White;
            chart.BorderlineColor = Border;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.Palette = ChartColorPalette.BrightPastel;

            foreach (ChartArea area in chart.ChartAreas)
            {
                area.BackColor = Color.White;
                area.AxisX.LabelStyle.ForeColor = TextSecondary;
                area.AxisY.LabelStyle.ForeColor = TextSecondary;
                area.AxisX.LineColor = Border;
                area.AxisY.LineColor = Border;
                area.AxisX.MajorGrid.LineColor = Border;
                area.AxisY.MajorGrid.LineColor = Border;
            }

            foreach (Title title in chart.Titles)
            {
                title.ForeColor = Primary;
                title.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            }
        }

        public static Label CreateSectionTitle(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = Primary,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        public static Panel CreateSummaryCard(string title, string value, string subtitle)
        {
            Panel card = new Panel
            {
                BackColor = Color.White,
                Width = 180,
                Height = 94,
                Margin = new Padding(0, 0, 12, 12),
                Padding = new Padding(12)
            };

            Label lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI Semibold", 8.8F, FontStyle.Bold)
            };

            Label lblValue = new Label
            {
                Text = value,
                Dock = DockStyle.Top,
                Height = 34,
                ForeColor = Primary,
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold)
            };

            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Dock = DockStyle.Fill,
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular)
            };

            card.Controls.Add(lblSubtitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblTitle);

            return card;
        }

        private static void StyleButtonBase(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
            button.FlatAppearance.BorderSize = 1;
        }
    }
}
