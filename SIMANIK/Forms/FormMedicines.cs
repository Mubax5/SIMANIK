using System;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormMedicines : MasterDataFormBase
    {
        private readonly MedicineService _service = new MedicineService();
        private int _selectedMedicineId;
        private TextBox txtSearch;
        private ComboBox cmbStatusFilter;
        private TextBox txtName;
        private TextBox txtType;
        private NumericUpDown numStock;
        private TextBox txtUnit;
        private TextBox txtInstruction;
        private CheckBox chkActive;

        public FormMedicines() : base("Master Obat")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) LoadData(); };
        }

        private void BuildUi()
        {
            txtSearch = CreateTextBox(260);
            cmbStatusFilter = CreateComboBox(150);
            cmbStatusFilter.Items.AddRange(new object[] { "Semua", "Aktif", "Nonaktif", "Stok Rendah" });
            cmbStatusFilter.SelectedIndex = 0;

            Button btnSearch = CreateButton("Cari", true);
            Button btnReset = CreateButton("Reset", false);
            btnSearch.Click += delegate { LoadData(); };
            btnReset.Click += delegate { txtSearch.Clear(); cmbStatusFilter.SelectedIndex = 0; LoadData(); };

            FilterPanel.Controls.Add(CreateField("Nama/Jenis", txtSearch, 270));
            FilterPanel.Controls.Add(CreateField("Status", cmbStatusFilter, 160));
            FilterPanel.Controls.Add(btnSearch);
            FilterPanel.Controls.Add(btnReset);

            txtName = CreateTextBox(230);
            txtType = CreateTextBox(170);
            numStock = new NumericUpDown
            {
                Width = 110,
                Minimum = 0,
                Maximum = 100000,
                Value = 0
            };
            txtUnit = CreateTextBox(120);
            txtInstruction = CreateTextBox(330);
            chkActive = CreateCheckBox("Aktif");

            EditorPanel.Controls.Add(CreateField("Nama obat", txtName, 240));
            EditorPanel.Controls.Add(CreateField("Jenis", txtType, 180));
            EditorPanel.Controls.Add(CreateField("Stok", numStock, 120));
            EditorPanel.Controls.Add(CreateField("Satuan", txtUnit, 130));
            EditorPanel.Controls.Add(CreateField("Instruksi default", txtInstruction, 340));
            EditorPanel.Controls.Add(CreateField("Status", chkActive, 150));

            Button btnAdd = CreateButton("Tambah", true);
            Button btnUpdate = CreateButton("Ubah", true);
            Button btnDeactivate = CreateButton("Nonaktifkan", false);
            Button btnRefresh = CreateButton("Refresh", false);
            btnAdd.Click += delegate { Save(0); };
            btnUpdate.Click += delegate { Save(_selectedMedicineId); };
            btnDeactivate.Click += delegate { SetActive(false); };
            btnRefresh.Click += delegate { ClearEditor(); LoadData(); };

            ButtonPanel.Controls.Add(btnRefresh);
            ButtonPanel.Controls.Add(btnDeactivate);
            ButtonPanel.Controls.Add(btnUpdate);
            ButtonPanel.Controls.Add(btnAdd);

            Grid.SelectionChanged += Grid_SelectionChanged;
            Grid.CellFormatting += Grid_CellFormatting;
        }

        private void LoadData()
        {
            Grid.DataSource = _service.Search(txtSearch.Text, Convert.ToString(cmbStatusFilter.SelectedItem));
        }

        private void Save(int medicineId)
        {
            MedicineItem medicine = new MedicineItem
            {
                MedicineId = medicineId,
                MedicineName = txtName.Text,
                MedicineType = txtType.Text,
                Stock = Convert.ToInt32(numStock.Value),
                Unit = txtUnit.Text,
                DefaultInstruction = txtInstruction.Text,
                IsActive = chkActive.Checked
            };

            ServiceResult result = _service.Save(medicine);
            ShowResult(result);

            if (result.Success)
            {
                ClearEditor();
                LoadData();
            }
        }

        private void SetActive(bool isActive)
        {
            ServiceResult result = _service.SetActive(_selectedMedicineId, isActive);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            MedicineItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as MedicineItem;
            if (item == null)
            {
                return;
            }

            _selectedMedicineId = item.MedicineId;
            txtName.Text = item.MedicineName;
            txtType.Text = item.MedicineType;
            numStock.Value = Math.Max(numStock.Minimum, Math.Min(numStock.Maximum, item.Stock));
            txtUnit.Text = item.Unit;
            txtInstruction.Text = item.DefaultInstruction;
            chkActive.Checked = item.IsActive;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (Grid.Columns[e.ColumnIndex].Name == "StockStatus" && Convert.ToString(e.Value) == "Stok rendah")
            {
                e.CellStyle.ForeColor = System.Drawing.Color.Firebrick;
                e.CellStyle.Font = new System.Drawing.Font(Grid.Font, System.Drawing.FontStyle.Bold);
            }
        }

        private void ClearEditor()
        {
            _selectedMedicineId = 0;
            txtName.Clear();
            txtType.Clear();
            numStock.Value = 0;
            txtUnit.Clear();
            txtInstruction.Clear();
            chkActive.Checked = true;
        }
    }
}
