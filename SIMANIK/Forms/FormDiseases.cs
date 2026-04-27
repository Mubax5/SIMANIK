using System;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormDiseases : MasterDataFormBase
    {
        private readonly DiseaseService _service = new DiseaseService();
        private int? selectedId = null;
        private bool isEditMode = false;
        private TextBox txtSearch;
        private ComboBox cmbStatusFilter;
        private TextBox txtCode;
        private TextBox txtName;
        private TextBox txtDescription;
        private CheckBox chkActive;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormDiseases() : base("Master Penyakit")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) { LoadData(); ClearForm(); } };
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

            FilterPanel.Controls.Add(CreateField("Kode/Nama", txtSearch, 270));
            FilterPanel.Controls.Add(CreateField("Status", cmbStatusFilter, 160));
            FilterPanel.Controls.Add(btnSearch);
            FilterPanel.Controls.Add(btnReset);

            txtCode = CreateTextBox(160);
            txtName = CreateTextBox(260);
            txtDescription = CreateTextBox(360);
            chkActive = CreateCheckBox("Aktif");

            EditorPanel.Controls.Add(CreateField("Kode penyakit", txtCode, 170));
            EditorPanel.Controls.Add(CreateField("Nama penyakit", txtName, 270));
            EditorPanel.Controls.Add(CreateField("Deskripsi", txtDescription, 370));
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
            Grid.DataSource = _service.Search(txtSearch.Text, Convert.ToString(cmbStatusFilter.SelectedItem));
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

            ServiceResult result = _service.DeleteDisease(selectedId.Value);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Save(int diseaseId)
        {
            DiseaseItem disease = new DiseaseItem
            {
                DiseaseId = diseaseId,
                DiseaseCode = txtCode.Text,
                DiseaseName = txtName.Text,
                Description = txtDescription.Text,
                IsActive = chkActive.Checked
            };

            ServiceResult result = _service.Save(disease);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            DiseaseItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as DiseaseItem;
            if (item == null)
            {
                return;
            }

            selectedId = item.DiseaseId;
            isEditMode = true;
            txtCode.Text = item.DiseaseCode;
            txtName.Text = item.DiseaseName;
            txtDescription.Text = item.Description;
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
            txtCode.Clear();
            txtName.Clear();
            txtDescription.Clear();
            chkActive.Checked = true;
            UpdateActionButtons();
            txtCode.Focus();
        }

        private void UpdateActionButtons()
        {
            btnAdd.Enabled = !isEditMode && !selectedId.HasValue;
            btnUpdate.Enabled = selectedId.HasValue;
            btnDelete.Enabled = selectedId.HasValue;
        }
    }
}
