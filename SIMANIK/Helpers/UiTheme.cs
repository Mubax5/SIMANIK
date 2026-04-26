using System.Drawing;
using System.Windows.Forms;

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
