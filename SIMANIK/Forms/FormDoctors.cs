using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormDoctors : MasterDataFormBase
    {
        private readonly DoctorService _service = new DoctorService();
        private int? selectedId = null;
        private bool isEditMode = false;
        private TextBox txtSearch;
        private ComboBox cmbStatusFilter;
        private ComboBox cmbUser;
        private TextBox txtFullName;
        private TextBox txtSpecialization;
        private TextBox txtPhone;
        private CheckBox chkActive;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormDoctors() : base("Master Dokter")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) { LoadUserOptions(0); LoadData(); ClearForm(); } };
        }

        private void BuildUi()
        {
            txtSearch = CreateTextBox(260);
            cmbStatusFilter = CreateComboBox(150);
            cmbStatusFilter.Items.AddRange(new object[] { "Semua", "Aktif", "Nonaktif" });
            cmbStatusFilter.SelectedIndex = 0;

            Button btnSearch = CreateButton("Cari", true);
            Button btnReset = CreateButton("Reset", false);
            btnSearch.Click += delegate { LoadData(); ClearForm(); };
            btnReset.Click += delegate { txtSearch.Clear(); cmbStatusFilter.SelectedIndex = 0; LoadData(); ClearForm(); };

            FilterPanel.Controls.Add(CreateField("Nama/Spesialisasi", txtSearch, 270));
            FilterPanel.Controls.Add(CreateField("Status", cmbStatusFilter, 160));
            FilterPanel.Controls.Add(btnSearch);
            FilterPanel.Controls.Add(btnReset);

            cmbUser = CreateComboBox(220);
            txtFullName = CreateTextBox(240);
            txtSpecialization = CreateTextBox(220);
            txtPhone = CreateTextBox(180);
            chkActive = CreateCheckBox("Aktif");

            EditorPanel.Controls.Add(CreateField("Akun dokter", cmbUser, 230));
            EditorPanel.Controls.Add(CreateField("Nama dokter", txtFullName, 250));
            EditorPanel.Controls.Add(CreateField("Spesialisasi", txtSpecialization, 230));
            EditorPanel.Controls.Add(CreateField("No. telepon", txtPhone, 190));
            EditorPanel.Controls.Add(CreateField("Status", chkActive, 150));

            Button btnNew = CreateButton("Baru/Reset", false);
            btnAdd = CreateButton("Tambah", true);
            btnUpdate = CreateButton("Ubah", true);
            btnDelete = CreateButton("Hapus", false);
            Button btnRefresh = CreateButton("Refresh", false);
            btnNew.Click += delegate { LoadUserOptions(0); ClearForm(); };
            btnAdd.Click += delegate { AddData(); };
            btnUpdate.Click += delegate { UpdateData(); };
            btnDelete.Click += delegate { DeleteData(); };
            btnRefresh.Click += delegate { LoadUserOptions(0); LoadData(); ClearForm(); };

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
            Grid.DataSource = _service.Search(txtSearch.Text, Convert.ToString(cmbStatusFilter.SelectedItem));
            SIMANIK.Helpers.UiTheme.ResizeDataGridViewColumns(Grid);
            ClearGridSelection();
            Grid.SelectionChanged += Grid_SelectionChanged;
        }

        private void LoadUserOptions(int selectedUserId)
        {
            List<LookupItem> users = selectedUserId > 0
                ? _service.GetDoctorUserOptions(selectedUserId)
                : _service.GetAvailableDoctorUsers();

            cmbUser.DisplayMember = "Text";
            cmbUser.ValueMember = "Id";
            cmbUser.DataSource = users;

            if (selectedUserId > 0)
            {
                cmbUser.SelectedValue = selectedUserId;
            }
            else if (cmbUser.Items.Count > 0)
            {
                cmbUser.SelectedIndex = 0;
            }
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

            ServiceResult result = _service.DeleteDoctor(selectedId.Value);
            ShowResult(result);

            if (result.Success)
            {
                LoadUserOptions(0);
                LoadData();
                ClearForm();
            }
        }

        private void Save(int doctorId)
        {
            DoctorItem doctor = new DoctorItem
            {
                DoctorId = doctorId,
                UserId = cmbUser.SelectedValue == null ? 0 : Convert.ToInt32(cmbUser.SelectedValue),
                FullName = txtFullName.Text,
                Specialization = txtSpecialization.Text,
                PhoneNumber = txtPhone.Text,
                IsActive = chkActive.Checked
            };

            ServiceResult result = _service.Save(doctor);
            ShowResult(result);

            if (result.Success)
            {
                LoadUserOptions(0);
                LoadData();
                ClearForm();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            DoctorItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as DoctorItem;
            if (item == null)
            {
                return;
            }

            selectedId = item.DoctorId;
            isEditMode = true;
            LoadUserOptions(item.UserId);
            txtFullName.Text = item.FullName;
            txtSpecialization.Text = item.Specialization;
            txtPhone.Text = item.PhoneNumber;
            chkActive.Checked = item.IsActive;
            UpdateActionButtons();
        }

        private void ClearForm()
        {
            Grid.SelectionChanged -= Grid_SelectionChanged;
            ClearGridSelection();
            Grid.SelectionChanged += Grid_SelectionChanged;
            selectedId = null;
            isEditMode = false;
            if (cmbUser.Items.Count > 0)
            {
                cmbUser.SelectedIndex = 0;
            }
            txtFullName.Clear();
            txtSpecialization.Clear();
            txtPhone.Clear();
            chkActive.Checked = true;
            UpdateActionButtons();
            cmbUser.Focus();
        }

        private void UpdateActionButtons()
        {
            btnAdd.Enabled = !isEditMode && !selectedId.HasValue;
            btnUpdate.Enabled = selectedId.HasValue;
            btnDelete.Enabled = selectedId.HasValue;
        }
    }
}
