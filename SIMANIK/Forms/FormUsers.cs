using System;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormUsers : MasterDataFormBase
    {
        private readonly UserService _service = new UserService();
        private int? selectedId = null;
        private bool isEditMode = false;
        private TextBox txtSearch;
        private ComboBox cmbRoleFilter;
        private ComboBox cmbStatusFilter;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private ComboBox cmbRole;
        private CheckBox chkActive;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormUsers() : base("Master User")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) { LoadData(); ClearForm(); } };
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
            btnSearch.Click += delegate { LoadData(); ClearForm(); };
            btnReset.Click += delegate { txtSearch.Clear(); cmbRoleFilter.SelectedIndex = 0; cmbStatusFilter.SelectedIndex = 0; LoadData(); ClearForm(); };

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

            Button btnNew = CreateButton("Baru/Reset", false);
            btnAdd = CreateButton("Tambah", true);
            btnUpdate = CreateButton("Ubah", true);
            btnDelete = CreateButton("Hapus", false);
            Button btnRefresh = CreateButton("Refresh", false);
            btnNew.Click += delegate { ClearForm(); };
            btnAdd.Click += delegate { AddData(); };
            btnUpdate.Click += delegate { UpdateData(); };
            btnDelete.Click += delegate { DeleteData(); };
            btnRefresh.Click += delegate { LoadData(); ClearForm(); };

            ButtonPanel.Controls.Add(btnRefresh);
            ButtonPanel.Controls.Add(btnDelete);
            ButtonPanel.Controls.Add(btnUpdate);
            ButtonPanel.Controls.Add(btnAdd);
            ButtonPanel.Controls.Add(btnNew);

            Grid.SelectionChanged += Grid_SelectionChanged;
            UpdateActionButtons();
        }

        private void LoadData()
        {
            Grid.SelectionChanged -= Grid_SelectionChanged;
            Grid.DataSource = _service.Search(txtSearch.Text, Convert.ToString(cmbRoleFilter.SelectedItem), Convert.ToString(cmbStatusFilter.SelectedItem));
            ClearGridSelection();
            Grid.SelectionChanged += Grid_SelectionChanged;
        }

        private void AddData()
        {
            if (selectedId.HasValue || isEditMode)
            {
                MessageBox.Show(this, "Anda sedang memilih data dari tabel. Klik Baru/Reset terlebih dahulu untuk menambah data baru.", "Mode tambah", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Save(0);
        }

        private void UpdateData()
        {
            if (!selectedId.HasValue)
            {
                MessageBox.Show(this, "Pilih data yang ingin diubah.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Save(selectedId.Value);
        }

        private void DeleteData()
        {
            if (!selectedId.HasValue)
            {
                MessageBox.Show(this, "Pilih data yang ingin dihapus.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(this, "Yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            ServiceResult result = _service.DeleteUser(selectedId.Value);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Save(int userId)
        {
            ServiceResult result = _service.Save(userId, txtUsername.Text, txtPassword.Text, Convert.ToString(cmbRole.SelectedItem), chkActive.Checked);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            UserListItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as UserListItem;
            if (item == null)
            {
                return;
            }

            selectedId = item.UserId;
            isEditMode = true;
            txtUsername.Text = item.Username;
            txtPassword.Clear();
            cmbRole.SelectedItem = item.Role;
            chkActive.Checked = item.Status == "Aktif";
            UpdateActionButtons();
        }

        private void ClearForm()
        {
            Grid.SelectionChanged -= Grid_SelectionChanged;
            ClearGridSelection();
            Grid.SelectionChanged += Grid_SelectionChanged;
            selectedId = null;
            isEditMode = false;
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = 1;
            chkActive.Checked = true;
            UpdateActionButtons();
            txtUsername.Focus();
        }

        private void UpdateActionButtons()
        {
            btnAdd.Enabled = !isEditMode && !selectedId.HasValue;
            btnUpdate.Enabled = selectedId.HasValue;
            btnDelete.Enabled = selectedId.HasValue;
        }
    }
}
