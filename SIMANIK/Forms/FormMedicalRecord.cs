using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormMedicalRecord : OperationalFormBase
    {
        private readonly MedicalRecordService _service = new MedicalRecordService();

        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnReset;
        private DataGridView dgvMedicalRecords;
        private TextBox txtBloodType;
        private TextBox txtAllergyNotes;
        private TextBox txtChronicDiseaseNotes;
        private Button btnSave;
        private int? selectedPatientId;

        public FormMedicalRecord() : base("Medical Record")
        {
            BuildUi();
            Load += delegate
            {
                if (EnsureAccess())
                {
                    ConfigureByRole();
                    LoadData();
                }
            };
        }

        private void BuildUi()
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));

            FlowLayoutPanel filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12),
                WrapContents = true
            };

            txtSearch = CreateTextBox(300);
            btnSearch = CreateButton("Cari", true);
            btnReset = CreateButton("Reset", false);
            btnSearch.Click += delegate { LoadData(); };
            btnReset.Click += delegate { txtSearch.Clear(); LoadData(); };

            filterPanel.Controls.Add(CreateField("Cari pasien / no. pasien", txtSearch, 310));
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);

            Panel gridSection = CreateSection("Daftar Medical Record");
            dgvMedicalRecords = CreateGrid();
            dgvMedicalRecords.SelectionChanged += DgvMedicalRecords_SelectionChanged;
            gridSection.Controls.Add(dgvMedicalRecords);
            gridSection.Controls.SetChildIndex(dgvMedicalRecords, 0);

            FlowLayoutPanel editorPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12),
                WrapContents = true,
                AutoScroll = true
            };

            txtBloodType = CreateTextBox(120);
            txtAllergyNotes = CreateTextBox(360);
            txtAllergyNotes.Multiline = true;
            txtAllergyNotes.Height = 58;
            txtChronicDiseaseNotes = CreateTextBox(360);
            txtChronicDiseaseNotes.Multiline = true;
            txtChronicDiseaseNotes.Height = 58;
            btnSave = CreateButton("Simpan", true);
            btnSave.Click += delegate { Save(); };

            editorPanel.Controls.Add(CreateField("Golongan darah", txtBloodType, 130));
            editorPanel.Controls.Add(CreateTallField("Catatan alergi", txtAllergyNotes, 370, 86));
            editorPanel.Controls.Add(CreateTallField("Catatan penyakit kronis", txtChronicDiseaseNotes, 370, 86));
            editorPanel.Controls.Add(btnSave);

            layout.Controls.Add(filterPanel, 0, 0);
            layout.Controls.Add(gridSection, 0, 1);
            layout.Controls.Add(editorPanel, 0, 2);
            RootLayout.Controls.Add(layout, 0, 1);
        }

        private bool EnsureAccess()
        {
            if (!SessionHelper.HasRole(UserRole.Admin) && !SessionHelper.HasRole(UserRole.Pasien))
            {
                MessageBox.Show(this, "Hanya Admin dan Pasien yang boleh mengakses medical record.", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return false;
            }

            return true;
        }

        private void ConfigureByRole()
        {
            bool isAdmin = SessionHelper.HasRole(UserRole.Admin);
            txtSearch.Enabled = isAdmin;
            btnSearch.Enabled = isAdmin;
            btnReset.Enabled = isAdmin;
            txtBloodType.ReadOnly = !isAdmin;
            txtAllergyNotes.ReadOnly = !isAdmin;
            txtChronicDiseaseNotes.ReadOnly = !isAdmin;
            btnSave.Visible = isAdmin;

            if (SessionHelper.HasRole(UserRole.Pasien))
            {
                TitleLabel.Text = "Medical Record Saya";
                Text = "SIMANIK - Medical Record Saya";
            }
        }

        private void LoadData()
        {
            dgvMedicalRecords.SelectionChanged -= DgvMedicalRecords_SelectionChanged;
            List<MedicalRecordViewItem> items;

            if (SessionHelper.HasRole(UserRole.Admin))
            {
                items = _service.SearchMedicalRecordsForAdmin(txtSearch.Text);
            }
            else
            {
                items = new List<MedicalRecordViewItem>();
                MedicalRecordViewItem item = _service.GetMedicalRecordForCurrentPatient();
                if (item != null)
                {
                    items.Add(item);
                }
            }

            dgvMedicalRecords.DataSource = null;
            dgvMedicalRecords.DataSource = items;
            UiTheme.ResizeDataGridViewColumns(dgvMedicalRecords);
            dgvMedicalRecords.ClearSelection();
            selectedPatientId = null;
            ClearEditor();

            dgvMedicalRecords.SelectionChanged += DgvMedicalRecords_SelectionChanged;

            if (!SessionHelper.HasRole(UserRole.Admin) && items.Count > 0)
            {
                dgvMedicalRecords.Rows[0].Selected = true;
                dgvMedicalRecords.CurrentCell = dgvMedicalRecords.Rows[0].Cells[0];
                FillEditor(items[0]);
            }
        }

        private void Save()
        {
            if (!selectedPatientId.HasValue)
            {
                MessageBox.Show(this, "Pilih pasien terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MedicalRecordItem record = new MedicalRecordItem
            {
                PatientId = selectedPatientId.Value,
                BloodType = txtBloodType.Text,
                AllergyNotes = txtAllergyNotes.Text,
                ChronicDiseaseNotes = txtChronicDiseaseNotes.Text
            };

            ServiceResult result = _service.UpdateMedicalRecord(record);
            ShowResult(result);

            if (result.Success)
            {
                int patientId = selectedPatientId.Value;
                LoadData();
                SelectPatient(patientId);
            }
        }

        private void DgvMedicalRecords_SelectionChanged(object sender, EventArgs e)
        {
            MedicalRecordViewItem item = dgvMedicalRecords.CurrentRow == null
                ? null
                : dgvMedicalRecords.CurrentRow.DataBoundItem as MedicalRecordViewItem;

            if (item == null)
            {
                selectedPatientId = null;
                ClearEditor();
                return;
            }

            FillEditor(item);
        }

        private void FillEditor(MedicalRecordViewItem item)
        {
            selectedPatientId = item.PatientId;
            txtBloodType.Text = item.BloodType;
            txtAllergyNotes.Text = item.AllergyNotes;
            txtChronicDiseaseNotes.Text = item.ChronicDiseaseNotes;
        }

        private void ClearEditor()
        {
            txtBloodType.Clear();
            txtAllergyNotes.Clear();
            txtChronicDiseaseNotes.Clear();
        }

        private void SelectPatient(int patientId)
        {
            foreach (DataGridViewRow row in dgvMedicalRecords.Rows)
            {
                MedicalRecordViewItem item = row.DataBoundItem as MedicalRecordViewItem;
                if (item != null && item.PatientId == patientId)
                {
                    row.Selected = true;
                    dgvMedicalRecords.CurrentCell = row.Cells[0];
                    FillEditor(item);
                    return;
                }
            }
        }

        private Panel CreateTallField(string labelText, Control input, int width, int height)
        {
            Panel panel = new Panel
            {
                Width = width,
                Height = height,
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
    }
}
