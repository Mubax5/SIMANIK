using System;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormMedicines : MasterDataFormBase
    {
        private readonly MedicineService _service = new MedicineService();
        private int? selectedId = null;
        private bool isEditMode = false;
        private TextBox txtSearch;
        private ComboBox cmbStatusFilter;
        private TextBox txtName;
        private TextBox txtType;
        private NumericUpDown numStock;
        private TextBox txtUnit;
        private TextBox txtInstruction;
        private CheckBox chkActive;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormMedicines() : base("Master Obat")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) { LoadData(); ClearForm(); } };
        }

        private void BuildUi()
        {
            txtSearch = CreateTextBox(260);
            cmbStatusFilter = CreateComboBox(150);
            cmbStatusFilter.Items.AddRange(new object[] { "Semua", "Aktif", "Nonaktif", "Stok Rendah" });
            cmbStatusFilter.SelectedIndex = 0;

            Button btnSearch = CreateButton("Cari", true);
            Button btnReset = CreateButton("Reset", false);
            btnSearch.Click += delegate { LoadData(); ClearForm(); };
            btnReset.Click += delegate { txtSearch.Clear(); cmbStatusFilter.SelectedIndex = 0; LoadData(); ClearForm(); };

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
            Grid.CellFormatting += Grid_CellFormatting;
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

            ServiceResult result = _service.DeleteMedicine(selectedId.Value);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
                ClearForm();
            }
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
                LoadData();
                ClearForm();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            MedicineItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as MedicineItem;
            if (item == null)
            {
                return;
            }

            selectedId = item.MedicineId;
            isEditMode = true;
            txtName.Text = item.MedicineName;
            txtType.Text = item.MedicineType;
            numStock.Value = Math.Max(numStock.Minimum, Math.Min(numStock.Maximum, item.Stock));
            txtUnit.Text = item.Unit;
            txtInstruction.Text = item.DefaultInstruction;
            chkActive.Checked = item.IsActive;
            UpdateActionButtons();
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (Grid.Columns[e.ColumnIndex].Name == "StockStatus" && Convert.ToString(e.Value) == "Stok rendah")
            {
                e.CellStyle.ForeColor = System.Drawing.Color.Firebrick;
                e.CellStyle.Font = new System.Drawing.Font(Grid.Font, System.Drawing.FontStyle.Bold);
            }
        }

        private void ClearForm()
        {
            Grid.SelectionChanged -= Grid_SelectionChanged;
            ClearGridSelection();
            Grid.SelectionChanged += Grid_SelectionChanged;
            selectedId = null;
            isEditMode = false;
            txtName.Clear();
            txtType.Clear();
            numStock.Value = 0;
            txtUnit.Clear();
            txtInstruction.Clear();
            chkActive.Checked = true;
            UpdateActionButtons();
            txtName.Focus();
        }

        private void UpdateActionButtons()
        {
            btnAdd.Enabled = !isEditMode && !selectedId.HasValue;
            btnUpdate.Enabled = selectedId.HasValue;
            btnDelete.Enabled = selectedId.HasValue;
        }
    }
}
