using System.Drawing;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class OperationalFormBase : Form
    {
        protected readonly TableLayoutPanel RootLayout;
        protected readonly Label TitleLabel;

        protected OperationalFormBase(string title)
        {
            Text = "SIMANIK - " + title;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(1000, 650);
            ClientSize = new Size(1160, 720);
            UiTheme.ApplyBase(this);

            RootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(18),
                BackColor = UiTheme.Background
            };

            RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            RootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            TitleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            UiTheme.StylePageTitle(TitleLabel);

            RootLayout.Controls.Add(TitleLabel, 0, 0);
            Controls.Add(RootLayout);
        }

        protected bool EnsureRole(UserRole role)
        {
            if (!SessionHelper.HasRole(role))
            {
                MessageBox.Show(this, "Anda tidak memiliki akses ke halaman ini.", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return false;
            }

            return true;
        }

        protected bool EnsureAdminOrDoctor()
        {
            if (!SessionHelper.HasRole(UserRole.Admin) && !SessionHelper.HasRole(UserRole.Dokter))
            {
                MessageBox.Show(this, "Hanya Admin dan Dokter yang boleh melihat antrian.", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return false;
            }

            return true;
        }

        protected DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill
            };
            UiTheme.StyleDataGridView(grid);
            return grid;
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

        protected Button CreateButton(string text, bool primary)
        {
            Button button = new Button
            {
                Text = text,
                Width = 130,
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

        protected Panel CreateSection(string title)
        {
            Panel section = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12),
                Margin = new Padding(0, 0, 0, 12)
            };

            Label label = UiTheme.CreateSectionTitle(title);
            section.Controls.Add(label);
            return section;
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
    }
}
