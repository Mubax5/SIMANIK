using System;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormDiseases : MasterDataFormBase
    {
        private readonly DiseaseService _service = new DiseaseService();
        private int _selectedDiseaseId;
        private TextBox txtSearch;
        private ComboBox cmbStatusFilter;
        private TextBox txtCode;
        private TextBox txtName;
        private TextBox txtDescription;
        private CheckBox chkActive;

        public FormDiseases() : base("Master Penyakit")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) LoadData(); };
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

            Button btnAdd = CreateButton("Tambah", true);
            Button btnUpdate = CreateButton("Ubah", true);
            Button btnDeactivate = CreateButton("Nonaktifkan", false);
            Button btnRefresh = CreateButton("Refresh", false);
            btnAdd.Click += delegate { Save(0); };
            btnUpdate.Click += delegate { Save(_selectedDiseaseId); };
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
            Grid.DataSource = _service.Search(txtSearch.Text, Convert.ToString(cmbStatusFilter.SelectedItem));
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
                ClearEditor();
                LoadData();
            }
        }

        private void SetActive(bool isActive)
        {
            ServiceResult result = _service.SetActive(_selectedDiseaseId, isActive);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            DiseaseItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as DiseaseItem;
            if (item == null)
            {
                return;
            }

            _selectedDiseaseId = item.DiseaseId;
            txtCode.Text = item.DiseaseCode;
            txtName.Text = item.DiseaseName;
            txtDescription.Text = item.Description;
            chkActive.Checked = item.IsActive;
        }

        private void ClearEditor()
        {
            _selectedDiseaseId = 0;
            txtCode.Clear();
            txtName.Clear();
            txtDescription.Clear();
            chkActive.Checked = true;
        }
    }
}
