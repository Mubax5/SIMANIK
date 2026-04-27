using System;
using System.Drawing;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class MasterDataFormBase : Form
    {
        protected readonly FlowLayoutPanel FilterPanel;
        protected readonly FlowLayoutPanel EditorPanel;
        protected readonly FlowLayoutPanel ButtonPanel;
        protected readonly DataGridView Grid;

        protected MasterDataFormBase(string title)
        {
            Text = "SIMANIK - " + title;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(980, 650);
            ClientSize = new Size(1120, 720);
            UiTheme.ApplyBase(this);

            TableLayoutPanel root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(18),
                BackColor = UiTheme.Background
            };

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));

            Label titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            UiTheme.StylePageTitle(titleLabel);

            FilterPanel = CreateFlowPanel();
            EditorPanel = CreateFlowPanel();
            ButtonPanel = CreateFlowPanel();
            ButtonPanel.FlowDirection = FlowDirection.RightToLeft;

            Grid = new DataGridView
            {
                Dock = DockStyle.Fill
            };
            UiTheme.StyleDataGridView(Grid);

            Panel gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(1)
            };
            gridPanel.Controls.Add(Grid);

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(FilterPanel, 0, 1);
            root.Controls.Add(gridPanel, 0, 2);
            root.Controls.Add(EditorPanel, 0, 3);
            root.Controls.Add(ButtonPanel, 0, 4);

            Controls.Add(root);
        }

        protected bool EnsureAdmin()
        {
            if (!SessionHelper.HasRole(UserRole.Admin))
            {
                MessageBox.Show(this, "Hanya Admin yang boleh mengakses form ini.", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return false;
            }

            return true;
        }

        protected Panel CreateField(string labelText, Control input, int width)
        {
            Panel panel = new Panel
            {
                Width = width,
                Height = 62,
                Margin = new Padding(0, 0, 10, 8)
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

        protected TextBox CreateTextBox(int width)
        {
            TextBox textBox = new TextBox { Width = width };
            UiTheme.StyleTextBox(textBox);
            return textBox;
        }

        protected ComboBox CreateComboBox(int width)
        {
            ComboBox comboBox = new ComboBox
            {
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            UiTheme.StyleComboBox(comboBox);
            return comboBox;
        }

        protected DateTimePicker CreateDatePicker(int width)
        {
            DateTimePicker picker = new DateTimePicker
            {
                Width = width,
                Format = DateTimePickerFormat.Short
            };
            UiTheme.StyleDatePicker(picker);
            return picker;
        }

        protected DateTimePicker CreateTimePicker(int width)
        {
            DateTimePicker picker = new DateTimePicker
            {
                Width = width,
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true
            };
            UiTheme.StyleDatePicker(picker);
            return picker;
        }

        protected Button CreateButton(string text, bool primary)
        {
            Button button = new Button
            {
                Text = text,
                Width = 118,
                Height = 38,
                Margin = new Padding(8, 8, 0, 0)
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

        protected CheckBox CreateCheckBox(string text)
        {
            return new CheckBox
            {
                Text = text,
                Width = 140,
                Height = 32,
                ForeColor = UiTheme.TextPrimary,
                Checked = true
            };
        }

        protected void ShowResult(ServiceResult result)
        {
            MessageBox.Show(
                this,
                result.Message,
                result.Success ? "Berhasil" : "Validasi",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private static FlowLayoutPanel CreateFlowPanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = UiTheme.Panel,
                Padding = new Padding(10)
            };
        }
    }
}
