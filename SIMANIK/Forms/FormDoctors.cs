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
        private int _selectedDoctorId;
        private TextBox txtSearch;
        private ComboBox cmbStatusFilter;
        private ComboBox cmbUser;
        private TextBox txtFullName;
        private TextBox txtSpecialization;
        private TextBox txtPhone;
        private CheckBox chkActive;

        public FormDoctors() : base("Master Dokter")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) { LoadUserOptions(0); LoadData(); } };
        }

        private void BuildUi()
        {
            txtSearch = CreateTextBox(260);
            cmbStatusFilter = CreateComboBox(150);
            cmbStatusFilter.Items.AddRange(new object[] { "Semua", "Aktif", "Nonaktif" });
            cmbStatusFilter.SelectedIndex = 0;

            Button btnSearch = CreateButton("Cari", true);
            Button btnReset = CreateButton("Reset", false);
            btnSearch.Click += delegate { LoadData(); };
            btnReset.Click += delegate { txtSearch.Clear(); cmbStatusFilter.SelectedIndex = 0; LoadData(); };

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

            Button btnAdd = CreateButton("Tambah", true);
            Button btnUpdate = CreateButton("Ubah", true);
            Button btnDeactivate = CreateButton("Nonaktifkan", false);
            Button btnRefresh = CreateButton("Refresh", false);
            btnAdd.Click += delegate { Save(0); };
            btnUpdate.Click += delegate { Save(_selectedDoctorId); };
            btnDeactivate.Click += delegate { SetActive(false); };
            btnRefresh.Click += delegate { ClearEditor(); LoadUserOptions(0); LoadData(); };

            ButtonPanel.Controls.Add(btnRefresh);
            ButtonPanel.Controls.Add(btnDeactivate);
            ButtonPanel.Controls.Add(btnUpdate);
            ButtonPanel.Controls.Add(btnAdd);

            Grid.SelectionChanged += Grid_SelectionChanged;
        }

        private void LoadData()
        {
            Grid.DataSource = _service.Search(txtSearch.Text, Convert.ToString(cmbStatusFilter.SelectedItem));
        }

        private void LoadUserOptions(int selectedUserId)
        {
            List<LookupItem> users = _service.GetDoctorUserOptions(selectedUserId);
            cmbUser.DataSource = users;
            cmbUser.DisplayMember = "Text";
            cmbUser.ValueMember = "Id";

            if (selectedUserId > 0)
            {
                cmbUser.SelectedValue = selectedUserId;
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
                ClearEditor();
                LoadUserOptions(0);
                LoadData();
            }
        }

        private void SetActive(bool isActive)
        {
            ServiceResult result = _service.SetActive(_selectedDoctorId, isActive);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            DoctorItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as DoctorItem;
            if (item == null)
            {
                return;
            }

            _selectedDoctorId = item.DoctorId;
            LoadUserOptions(item.UserId);
            txtFullName.Text = item.FullName;
            txtSpecialization.Text = item.Specialization;
            txtPhone.Text = item.PhoneNumber;
            chkActive.Checked = item.IsActive;
        }

        private void ClearEditor()
        {
            _selectedDoctorId = 0;
            txtFullName.Clear();
            txtSpecialization.Clear();
            txtPhone.Clear();
            chkActive.Checked = true;
        }
    }
}
