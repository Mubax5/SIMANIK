using System;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormUsers : MasterDataFormBase
    {
        private readonly UserService _service = new UserService();
        private int _selectedUserId;
        private TextBox txtSearch;
        private ComboBox cmbRoleFilter;
        private ComboBox cmbStatusFilter;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private ComboBox cmbRole;
        private CheckBox chkActive;

        public FormUsers() : base("Master User")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) LoadData(); };
        }

        private void BuildUi()
        {
            txtSearch = CreateTextBox(220);
            cmbRoleFilter = CreateComboBox(140);
            cmbRoleFilter.Items.AddRange(new object[] { "Semua", "Admin", "Dokter", "Pasien" });
            cmbRoleFilter.SelectedIndex = 0;
            cmbStatusFilter = CreateComboBox(140);
            cmbStatusFilter.Items.AddRange(new object[] { "Semua", "Aktif", "Nonaktif" });
            cmbStatusFilter.SelectedIndex = 0;

            Button btnSearch = CreateButton("Cari", true);
            Button btnReset = CreateButton("Reset", false);
            btnSearch.Click += delegate { LoadData(); };
            btnReset.Click += delegate { txtSearch.Clear(); cmbRoleFilter.SelectedIndex = 0; cmbStatusFilter.SelectedIndex = 0; LoadData(); };

            FilterPanel.Controls.Add(CreateField("Keyword", txtSearch, 230));
            FilterPanel.Controls.Add(CreateField("Role", cmbRoleFilter, 150));
            FilterPanel.Controls.Add(CreateField("Status", cmbStatusFilter, 150));
            FilterPanel.Controls.Add(btnSearch);
            FilterPanel.Controls.Add(btnReset);

            txtUsername = CreateTextBox(220);
            txtPassword = CreateTextBox(220);
            txtPassword.UseSystemPasswordChar = true;
            cmbRole = CreateComboBox(150);
            cmbRole.Items.AddRange(new object[] { "Admin", "Dokter", "Pasien" });
            cmbRole.SelectedIndex = 1;
            chkActive = CreateCheckBox("Aktif");

            EditorPanel.Controls.Add(CreateField("Username", txtUsername, 230));
            EditorPanel.Controls.Add(CreateField("Password", txtPassword, 230));
            EditorPanel.Controls.Add(CreateField("Role", cmbRole, 160));
            EditorPanel.Controls.Add(CreateField("Status", chkActive, 150));

            Button btnAdd = CreateButton("Tambah", true);
            Button btnUpdate = CreateButton("Ubah", true);
            Button btnDeactivate = CreateButton("Nonaktifkan", false);
            Button btnRefresh = CreateButton("Refresh", false);
            btnAdd.Click += delegate { Save(0); };
            btnUpdate.Click += delegate { Save(_selectedUserId); };
            btnDeactivate.Click += delegate { SetActive(false); };
            btnRefresh.Click += delegate { ClearEditor(); LoadData(); };

            ButtonPanel.Controls.Add(btnRefresh);
            ButtonPanel.Controls.Add(btnDeactivate);
            ButtonPanel.Controls.Add(btnUpdate);
            ButtonPanel.Controls.Add(btnAdd);

            Grid.SelectionChanged += Grid_SelectionChanged;
        }

        private void LoadData()
        {
            Grid.DataSource = _service.Search(txtSearch.Text, Convert.ToString(cmbRoleFilter.SelectedItem), Convert.ToString(cmbStatusFilter.SelectedItem));
        }

        private void Save(int userId)
        {
            ServiceResult result = _service.Save(userId, txtUsername.Text, txtPassword.Text, Convert.ToString(cmbRole.SelectedItem), chkActive.Checked);
            ShowResult(result);

            if (result.Success)
            {
                ClearEditor();
                LoadData();
            }
        }

        private void SetActive(bool isActive)
        {
            ServiceResult result = _service.SetActive(_selectedUserId, isActive);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            UserListItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as UserListItem;
            if (item == null)
            {
                return;
            }

            _selectedUserId = item.UserId;
            txtUsername.Text = item.Username;
            txtPassword.Clear();
            cmbRole.SelectedItem = item.Role;
            chkActive.Checked = item.Status == "Aktif";
        }

        private void ClearEditor()
        {
            _selectedUserId = 0;
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = 1;
            chkActive.Checked = true;
        }
    }
}
