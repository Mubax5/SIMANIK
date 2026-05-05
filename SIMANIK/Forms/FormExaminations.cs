using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormExaminations : OperationalFormBase
    {
        private readonly ExaminationService _service = new ExaminationService();
        private readonly List<PrescriptionInput> _prescriptionItems = new List<PrescriptionInput>();

        private DataGridView dgvDoctorQueue;
        private Button btnStartExamination;
        private Label lblPatientName;
        private Label lblPatientNumber;
        private Label lblPatientAge;
        private Label lblPatientGender;
        private Label lblPatientPhone;
        private Label lblInitialComplaint;
        private Label lblMedicalRecord;
        private DataGridView dgvPatientHistory;
        private TextBox txtCurrentComplaint;
        private ComboBox cmbDiseases;
        private TextBox txtDiagnosisNotes;
        private TextBox txtTreatmentNotes;
        private ComboBox cmbMedicines;
        private NumericUpDown numMedicineQuantity;
        private TextBox txtInstructionNote;
        private Button btnAddMedicine;
        private Button btnRemoveMedicine;
        private DataGridView dgvPrescriptionItems;
        private Button btnSaveExamination;
        private Button btnRefresh;

        private int? selectedVisitId;
        private PatientExaminationDetail currentDetail;
        private bool loadingQueue;

        public FormExaminations() : base("Pemeriksaan Dokter")
        {
            BuildUi();
            Load += delegate
            {
                if (EnsureRole(UserRole.Dokter))
                {
                    LoadLookups();
                    LoadQueue(null);
                    ClearPatientDetail();
                    UpdateButtons();
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

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            layout.Controls.Add(BuildQueueSection(), 0, 0);
            layout.Controls.Add(BuildPatientSection(), 0, 1);
            layout.Controls.Add(BuildInputSection(), 0, 2);

            RootLayout.Controls.Add(layout, 0, 1);
        }

        private Control BuildQueueSection()
        {
            Panel section = CreateSection("Antrian Dokter Hari Ini");
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Panel,
                Padding = new Padding(0)
            };

            btnStartExamination = CreateButton("Mulai Periksa", true);
            btnRefresh = CreateButton("Refresh", false);
            btnStartExamination.Click += delegate { StartExamination(); };
            btnRefresh.Click += delegate { LoadLookups(); LoadQueue(selectedVisitId); };

            actions.Controls.Add(btnStartExamination);
            actions.Controls.Add(btnRefresh);

            dgvDoctorQueue = CreateGrid();
            dgvDoctorQueue.SelectionChanged += DgvDoctorQueue_SelectionChanged;

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(dgvDoctorQueue, 0, 1);
            section.Controls.Add(layout);
            section.Controls.SetChildIndex(layout, 0);
            return section;
        }

        private Control BuildPatientSection()
        {
            Panel section = CreateSection("Detail Pasien, Medical Record, dan Riwayat");
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));

            FlowLayoutPanel detailPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = UiTheme.Panel
            };

            lblPatientName = CreateInfoLabel();
            lblPatientNumber = CreateInfoLabel();
            lblPatientAge = CreateInfoLabel();
            lblPatientGender = CreateInfoLabel();
            lblPatientPhone = CreateInfoLabel();
            lblInitialComplaint = CreateInfoLabel();
            lblMedicalRecord = CreateInfoLabel();
            lblInitialComplaint.Height = 44;
            lblMedicalRecord.Height = 58;

            detailPanel.Controls.Add(lblPatientName);
            detailPanel.Controls.Add(lblPatientNumber);
            detailPanel.Controls.Add(lblPatientAge);
            detailPanel.Controls.Add(lblPatientGender);
            detailPanel.Controls.Add(lblPatientPhone);
            detailPanel.Controls.Add(lblInitialComplaint);
            detailPanel.Controls.Add(lblMedicalRecord);

            dgvPatientHistory = CreateGrid();
            dgvPatientHistory.DataSource = new List<PatientExaminationHistoryItem>();

            layout.Controls.Add(detailPanel, 0, 0);
            layout.Controls.Add(dgvPatientHistory, 1, 0);
            section.Controls.Add(layout);
            section.Controls.SetChildIndex(layout, 0);
            return section;
        }

        private Control BuildInputSection()
        {
            Panel section = CreateSection("Input Pemeriksaan dan Resep");
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            layout.Controls.Add(BuildExaminationInputPanel(), 0, 0);
            layout.Controls.Add(BuildPrescriptionPanel(), 1, 0);

            section.Controls.Add(layout);
            section.Controls.SetChildIndex(layout, 0);
            return section;
        }

        private Control BuildExaminationInputPanel()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = UiTheme.Panel
            };

            txtCurrentComplaint = CreateMultilineTextBox(520, 58);
            cmbDiseases = CreateComboBox(310);
            txtDiagnosisNotes = CreateMultilineTextBox(520, 58);
            txtTreatmentNotes = CreateMultilineTextBox(520, 58);
            btnSaveExamination = CreateButton("Simpan Pemeriksaan", true);
            btnSaveExamination.Width = 170;
            btnSaveExamination.Click += delegate { SaveExamination(); };

            panel.Controls.Add(CreateTallField("Keluhan saat ini", txtCurrentComplaint, 540, 86));
            panel.Controls.Add(CreateField("Diagnosa utama", cmbDiseases, 320));
            panel.Controls.Add(CreateTallField("Catatan diagnosa", txtDiagnosisNotes, 540, 86));
            panel.Controls.Add(CreateTallField("Catatan tindakan", txtTreatmentNotes, 540, 86));
            panel.Controls.Add(btnSaveExamination);

            return panel;
        }

        private Control BuildPrescriptionPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.Panel
            };

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 102F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel inputPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = UiTheme.Panel
            };

            cmbMedicines = CreateComboBox(230);
            cmbMedicines.SelectedIndexChanged += delegate { ApplySelectedMedicineInstruction(); };
            numMedicineQuantity = new NumericUpDown
            {
                Width = 90,
                Minimum = 1,
                Maximum = 100000,
                Value = 1
            };
            txtInstructionNote = CreateTextBox(220);
            btnAddMedicine = CreateButton("Tambah Obat", true);
            btnRemoveMedicine = CreateButton("Hapus Obat", false);
            btnAddMedicine.Click += delegate { AddMedicine(); };
            btnRemoveMedicine.Click += delegate { RemoveMedicine(); };

            inputPanel.Controls.Add(CreateField("Obat aktif", cmbMedicines, 240));
            inputPanel.Controls.Add(CreateField("Jumlah", numMedicineQuantity, 100));
            inputPanel.Controls.Add(CreateField("Aturan pakai", txtInstructionNote, 230));
            inputPanel.Controls.Add(btnAddMedicine);
            inputPanel.Controls.Add(btnRemoveMedicine);

            dgvPrescriptionItems = CreateGrid();
            dgvPrescriptionItems.SelectionChanged += delegate { UpdateButtons(); };

            panel.Controls.Add(inputPanel, 0, 0);
            panel.Controls.Add(dgvPrescriptionItems, 0, 1);
            return panel;
        }

        private void LoadQueue(int? visitIdToSelect)
        {
            loadingQueue = true;
            dgvDoctorQueue.SelectionChanged -= DgvDoctorQueue_SelectionChanged;
            dgvDoctorQueue.DataSource = _service.GetDoctorQueue(SessionHelper.CurrentUser.Id);
            UiTheme.ResizeDataGridViewColumns(dgvDoctorQueue);
            dgvDoctorQueue.ClearSelection();
            selectedVisitId = null;
            currentDetail = null;

            if (visitIdToSelect.HasValue)
            {
                SelectQueueRow(visitIdToSelect.Value);
            }

            dgvDoctorQueue.SelectionChanged += DgvDoctorQueue_SelectionChanged;
            loadingQueue = false;

            if (selectedVisitId.HasValue)
            {
                LoadSelectedVisitDetail();
            }
            else
            {
                ClearPatientDetail();
            }

            UpdateButtons();
        }

        private void SelectQueueRow(int visitId)
        {
            foreach (DataGridViewRow row in dgvDoctorQueue.Rows)
            {
                DoctorQueueExaminationItem item = row.DataBoundItem as DoctorQueueExaminationItem;
                if (item != null && item.VisitId == visitId)
                {
                    row.Selected = true;
                    dgvDoctorQueue.CurrentCell = row.Cells[0];
                    selectedVisitId = visitId;
                    return;
                }
            }
        }

        private void LoadLookups()
        {
            BindDiseases(_service.GetActiveDiseases());
            BindMedicines(_service.GetActiveMedicines());
        }

        private void LoadSelectedVisitDetail()
        {
            if (!selectedVisitId.HasValue)
            {
                ClearPatientDetail();
                return;
            }

            ExaminationDetailResult result = _service.LoadExaminationDetail(SessionHelper.CurrentUser.Id, selectedVisitId.Value);
            if (!result.Success)
            {
                ShowResult(result);
                ClearPatientDetail();
                return;
            }

            currentDetail = result.Detail.PatientDetail;
            BindDiseases(result.Detail.Diseases);
            BindMedicines(result.Detail.Medicines);
            FillPatientDetail(result.Detail);
            BindPatientHistory(result.Detail.PatientHistory);
            ClearExaminationInputs();
            UpdateButtons();
        }

        private void BindDiseases(List<DiseaseItem> diseases)
        {
            cmbDiseases.DisplayMember = "DiseaseName";
            cmbDiseases.ValueMember = "DiseaseId";
            cmbDiseases.DataSource = diseases;
            if (diseases.Count > 0)
            {
                cmbDiseases.SelectedIndex = 0;
            }
        }

        private void BindMedicines(List<MedicineItem> medicines)
        {
            cmbMedicines.DisplayMember = "MedicineName";
            cmbMedicines.ValueMember = "MedicineId";
            cmbMedicines.DataSource = medicines;
            if (medicines.Count > 0)
            {
                cmbMedicines.SelectedIndex = 0;
            }
        }

        private void BindPatientHistory(List<PatientExaminationHistoryItem> history)
        {
            dgvPatientHistory.DataSource = null;
            dgvPatientHistory.DataSource = history;
            UiTheme.ResizeDataGridViewColumns(dgvPatientHistory);
            dgvPatientHistory.ClearSelection();
        }

        private void BindPrescriptionItems()
        {
            dgvPrescriptionItems.DataSource = null;
            dgvPrescriptionItems.DataSource = new List<PrescriptionInput>(_prescriptionItems);
            UiTheme.ResizeDataGridViewColumns(dgvPrescriptionItems);
            dgvPrescriptionItems.ClearSelection();
            UpdateButtons();
        }

        private void FillPatientDetail(ExaminationDetailBundle detail)
        {
            PatientExaminationDetail patient = detail.PatientDetail;
            lblPatientName.Text = "Nama: " + patient.PatientName;
            lblPatientNumber.Text = "Nomor pasien: " + patient.PatientNumber;
            lblPatientAge.Text = "Umur: " + patient.Age + " tahun";
            lblPatientGender.Text = "Jenis kelamin: " + patient.Gender;
            lblPatientPhone.Text = "Telepon: " + patient.PhoneNumber;
            lblInitialComplaint.Text = "Keluhan awal: " + patient.InitialComplaint;
            lblMedicalRecord.Text = "Medical record: " + FormatMedicalRecord(detail.MedicalRecord);
        }

        private void ClearPatientDetail()
        {
            currentDetail = null;
            lblPatientName.Text = "Nama: -";
            lblPatientNumber.Text = "Nomor pasien: -";
            lblPatientAge.Text = "Umur: -";
            lblPatientGender.Text = "Jenis kelamin: -";
            lblPatientPhone.Text = "Telepon: -";
            lblInitialComplaint.Text = "Keluhan awal: -";
            lblMedicalRecord.Text = "Medical record: -";
            BindPatientHistory(new List<PatientExaminationHistoryItem>());
            ClearExaminationInputs();
            _prescriptionItems.Clear();
            BindPrescriptionItems();
        }

        private void ClearExaminationInputs()
        {
            txtCurrentComplaint.Clear();
            txtDiagnosisNotes.Clear();
            txtTreatmentNotes.Clear();
            _prescriptionItems.Clear();
            BindPrescriptionItems();
        }

        private void StartExamination()
        {
            if (!selectedVisitId.HasValue)
            {
                MessageBox.Show(this, "Pilih antrian terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ServiceResult result = _service.StartExamination(SessionHelper.CurrentUser.Id, selectedVisitId.Value);
            ShowResult(result);

            if (result.Success)
            {
                LoadQueue(selectedVisitId);
            }
        }

        private void AddMedicine()
        {
            int medicineId = cmbMedicines.SelectedValue == null ? 0 : Convert.ToInt32(cmbMedicines.SelectedValue);
            int quantity = Convert.ToInt32(numMedicineQuantity.Value);
            ServiceResult result = _service.AddPrescriptionItemToTemporaryList(_prescriptionItems, medicineId, quantity, txtInstructionNote.Text);
            ShowResult(result);

            if (result.Success)
            {
                numMedicineQuantity.Value = 1;
                txtInstructionNote.Clear();
                ApplySelectedMedicineInstruction();
                BindPrescriptionItems();
            }
        }

        private void RemoveMedicine()
        {
            PrescriptionInput item = dgvPrescriptionItems.CurrentRow == null
                ? null
                : dgvPrescriptionItems.CurrentRow.DataBoundItem as PrescriptionInput;

            if (item == null)
            {
                MessageBox.Show(this, "Pilih obat yang ingin dihapus dari resep sementara.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _prescriptionItems.RemoveAll(value => value.MedicineId == item.MedicineId);
            BindPrescriptionItems();
        }

        private void SaveExamination()
        {
            if (!selectedVisitId.HasValue)
            {
                MessageBox.Show(this, "Pilih antrian terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ExaminationInput input = new ExaminationInput
            {
                VisitId = selectedVisitId.Value,
                DiseaseId = cmbDiseases.SelectedValue == null ? 0 : Convert.ToInt32(cmbDiseases.SelectedValue),
                CurrentComplaint = txtCurrentComplaint.Text,
                DiagnosisNotes = txtDiagnosisNotes.Text,
                TreatmentNotes = txtTreatmentNotes.Text
            };

            ServiceResult result = _service.SaveExamination(SessionHelper.CurrentUser.Id, input, _prescriptionItems);
            ShowResult(result);

            if (result.Success)
            {
                selectedVisitId = null;
                currentDetail = null;
                _prescriptionItems.Clear();
                LoadQueue(null);
            }
        }

        private void DgvDoctorQueue_SelectionChanged(object sender, EventArgs e)
        {
            if (loadingQueue)
            {
                return;
            }

            DoctorQueueExaminationItem item = dgvDoctorQueue.CurrentRow == null
                ? null
                : dgvDoctorQueue.CurrentRow.DataBoundItem as DoctorQueueExaminationItem;

            selectedVisitId = item == null ? (int?)null : item.VisitId;
            LoadSelectedVisitDetail();
            UpdateButtons();
        }

        private void ApplySelectedMedicineInstruction()
        {
            MedicineItem medicine = cmbMedicines.SelectedItem as MedicineItem;
            if (medicine == null)
            {
                txtInstructionNote.Clear();
                return;
            }

            txtInstructionNote.Text = medicine.DefaultInstruction;
        }

        private void UpdateButtons()
        {
            bool hasVisit = selectedVisitId.HasValue && currentDetail != null;
            bool canStart = hasVisit && currentDetail.VisitStatus == VisitStatusText.Waiting;
            bool canSave = hasVisit && currentDetail.VisitStatus == VisitStatusText.InProgress;
            btnStartExamination.Enabled = canStart;
            btnSaveExamination.Enabled = canSave;
            btnAddMedicine.Enabled = hasVisit;
            btnRemoveMedicine.Enabled = dgvPrescriptionItems.CurrentRow != null;
        }

        private TextBox CreateMultilineTextBox(int width, int height)
        {
            TextBox textBox = CreateTextBox(width);
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Height = height;
            return textBox;
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

        private Label CreateInfoLabel()
        {
            Label label = new Label
            {
                Width = 480,
                Height = 24,
                AutoEllipsis = true,
                Margin = new Padding(0, 0, 10, 2)
            };

            UiTheme.StyleInfoLabel(label);
            return label;
        }

        private static string FormatMedicalRecord(MedicalRecordItem record)
        {
            if (record == null)
            {
                return "Belum ada data";
            }

            string lastVisit = record.LastVisitDate.HasValue ? record.LastVisitDate.Value.ToString("dd/MM/yyyy HH:mm") : "-";
            string blood = string.IsNullOrWhiteSpace(record.BloodType) ? "-" : record.BloodType;
            string allergy = string.IsNullOrWhiteSpace(record.AllergyNotes) ? "-" : record.AllergyNotes;
            string chronic = string.IsNullOrWhiteSpace(record.ChronicDiseaseNotes) ? "-" : record.ChronicDiseaseNotes;
            return "Gol darah " + blood + " | Alergi " + allergy + " | Kronis " + chronic + " | Terakhir " + lastVisit;
        }
    }
}
