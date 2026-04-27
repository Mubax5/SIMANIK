using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormDoctorSchedules : MasterDataFormBase
    {
        private readonly ScheduleService _scheduleService = new ScheduleService();
        private readonly DoctorService _doctorService = new DoctorService();
        private int _selectedScheduleId;
        private ComboBox cmbFilterDoctor;
        private CheckBox chkFilterDate;
        private DateTimePicker dtpFilterDate;
        private ComboBox cmbStatusFilter;
        private ComboBox cmbDoctor;
        private DateTimePicker dtpScheduleDate;
        private DateTimePicker dtpStartTime;
        private DateTimePicker dtpEndTime;
        private NumericUpDown numQuota;
        private CheckBox chkActive;

        public FormDoctorSchedules() : base("Master Jadwal Dokter")
        {
            BuildUi();
            Load += delegate { if (EnsureAdmin()) { LoadDoctorOptions(); LoadData(); } };
        }

        private void BuildUi()
        {
            cmbFilterDoctor = CreateComboBox(220);
            chkFilterDate = CreateCheckBox("Filter tanggal");
            dtpFilterDate = CreateDatePicker(130);
            cmbStatusFilter = CreateComboBox(150);
            cmbStatusFilter.Items.AddRange(new object[] { "Semua", "Aktif", "Nonaktif" });
            cmbStatusFilter.SelectedIndex = 0;

            Button btnSearch = CreateButton("Cari", true);
            Button btnReset = CreateButton("Reset", false);
            btnSearch.Click += delegate { LoadData(); };
            btnReset.Click += delegate { cmbFilterDoctor.SelectedIndex = 0; chkFilterDate.Checked = false; dtpFilterDate.Value = DateTime.Today; cmbStatusFilter.SelectedIndex = 0; LoadData(); };

            FilterPanel.Controls.Add(CreateField("Dokter", cmbFilterDoctor, 230));
            FilterPanel.Controls.Add(CreateField("Tanggal", dtpFilterDate, 140));
            FilterPanel.Controls.Add(CreateField("Pakai tanggal", chkFilterDate, 150));
            FilterPanel.Controls.Add(CreateField("Status", cmbStatusFilter, 160));
            FilterPanel.Controls.Add(btnSearch);
            FilterPanel.Controls.Add(btnReset);

            cmbDoctor = CreateComboBox(240);
            dtpScheduleDate = CreateDatePicker(140);
            dtpStartTime = CreateTimePicker(120);
            dtpEndTime = CreateTimePicker(120);
            numQuota = new NumericUpDown
            {
                Width = 100,
                Minimum = 1,
                Maximum = 1000,
                Value = 10
            };
            chkActive = CreateCheckBox("Aktif");

            EditorPanel.Controls.Add(CreateField("Dokter", cmbDoctor, 250));
            EditorPanel.Controls.Add(CreateField("Tanggal", dtpScheduleDate, 150));
            EditorPanel.Controls.Add(CreateField("Jam mulai", dtpStartTime, 130));
            EditorPanel.Controls.Add(CreateField("Jam selesai", dtpEndTime, 130));
            EditorPanel.Controls.Add(CreateField("Kuota", numQuota, 110));
            EditorPanel.Controls.Add(CreateField("Status", chkActive, 150));

            Button btnAdd = CreateButton("Tambah", true);
            Button btnUpdate = CreateButton("Ubah", true);
            Button btnDeactivate = CreateButton("Nonaktifkan", false);
            Button btnRefresh = CreateButton("Refresh", false);
            btnAdd.Click += delegate { Save(0); };
            btnUpdate.Click += delegate { Save(_selectedScheduleId); };
            btnDeactivate.Click += delegate { SetActive(false); };
            btnRefresh.Click += delegate { ClearEditor(); LoadDoctorOptions(); LoadData(); };

            ButtonPanel.Controls.Add(btnRefresh);
            ButtonPanel.Controls.Add(btnDeactivate);
            ButtonPanel.Controls.Add(btnUpdate);
            ButtonPanel.Controls.Add(btnAdd);

            Grid.SelectionChanged += Grid_SelectionChanged;
        }

        private void LoadDoctorOptions()
        {
            List<LookupItem> doctors = _doctorService.GetActiveDoctorOptions();
            List<LookupItem> filterDoctors = new List<LookupItem>();
            filterDoctors.Add(new LookupItem { Id = 0, Text = "Semua" });
            filterDoctors.AddRange(doctors);

            cmbFilterDoctor.DataSource = filterDoctors;
            cmbFilterDoctor.DisplayMember = "Text";
            cmbFilterDoctor.ValueMember = "Id";

            cmbDoctor.DataSource = doctors;
            cmbDoctor.DisplayMember = "Text";
            cmbDoctor.ValueMember = "Id";
        }

        private void LoadData()
        {
            int doctorId = cmbFilterDoctor.SelectedValue == null ? 0 : Convert.ToInt32(cmbFilterDoctor.SelectedValue);
            DateTime? date = chkFilterDate.Checked ? (DateTime?)dtpFilterDate.Value.Date : null;
            Grid.DataSource = _scheduleService.Search(doctorId, date, Convert.ToString(cmbStatusFilter.SelectedItem));
        }

        private void Save(int scheduleId)
        {
            DoctorScheduleItem schedule = new DoctorScheduleItem
            {
                ScheduleId = scheduleId,
                DoctorId = cmbDoctor.SelectedValue == null ? 0 : Convert.ToInt32(cmbDoctor.SelectedValue),
                ScheduleDate = dtpScheduleDate.Value.Date,
                StartTime = dtpStartTime.Value.TimeOfDay,
                EndTime = dtpEndTime.Value.TimeOfDay,
                Quota = Convert.ToInt32(numQuota.Value),
                IsActive = chkActive.Checked
            };

            ServiceResult result = _scheduleService.Save(schedule);
            ShowResult(result);

            if (result.Success)
            {
                ClearEditor();
                LoadData();
            }
        }

        private void SetActive(bool isActive)
        {
            ServiceResult result = _scheduleService.SetActive(_selectedScheduleId, isActive);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            DoctorScheduleItem item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem as DoctorScheduleItem;
            if (item == null)
            {
                return;
            }

            _selectedScheduleId = item.ScheduleId;
            cmbDoctor.SelectedValue = item.DoctorId;
            dtpScheduleDate.Value = item.ScheduleDate;
            dtpStartTime.Value = DateTime.Today.Add(item.StartTime);
            dtpEndTime.Value = DateTime.Today.Add(item.EndTime);
            numQuota.Value = Math.Max(numQuota.Minimum, Math.Min(numQuota.Maximum, item.Quota));
            chkActive.Checked = item.IsActive;
        }

        private void ClearEditor()
        {
            _selectedScheduleId = 0;
            if (cmbDoctor.Items.Count > 0)
            {
                cmbDoctor.SelectedIndex = 0;
            }
            dtpScheduleDate.Value = DateTime.Today;
            dtpStartTime.Value = DateTime.Today.AddHours(8);
            dtpEndTime.Value = DateTime.Today.AddHours(10);
            numQuota.Value = 10;
            chkActive.Checked = true;
        }
    }
}
