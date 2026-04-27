using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormReservations : OperationalFormBase
    {
        private readonly ReservationService _service = new ReservationService();
        private ComboBox cmbDoctors;
        private ComboBox cmbSchedules;
        private TextBox txtComplaint;
        private Button btnCancelReservation;
        private DataGridView dgvMyReservations;
        private int? selectedReservationId;

        public FormReservations() : base("Reservasi Pasien")
        {
            BuildUi();
            Load += delegate
            {
                if (EnsureRole(UserRole.Pasien))
                {
                    LoadDoctors();
                    LoadReservations();
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

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel formPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                WrapContents = true,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12)
            };

            cmbDoctors = CreateComboBox(260);
            cmbSchedules = CreateComboBox(340);
            txtComplaint = CreateTextBox(360);
            txtComplaint.Multiline = true;
            txtComplaint.Height = 70;

            Button btnCreateReservation = CreateButton("Buat Reservasi", true);
            btnCancelReservation = CreateButton("Batalkan", false);
            Button btnRefresh = CreateButton("Refresh", false);

            cmbDoctors.SelectedIndexChanged += delegate { LoadSchedules(); };
            btnCreateReservation.Click += delegate { CreateReservation(); };
            btnCancelReservation.Click += delegate { CancelReservation(); };
            btnRefresh.Click += delegate { LoadDoctors(); LoadReservations(); };

            formPanel.Controls.Add(CreateField("Dokter", cmbDoctors, 270));
            formPanel.Controls.Add(CreateField("Jadwal", cmbSchedules, 350));
            formPanel.Controls.Add(CreateField("Keluhan awal", txtComplaint, 370));
            formPanel.Controls.Add(btnCreateReservation);
            formPanel.Controls.Add(btnCancelReservation);
            formPanel.Controls.Add(btnRefresh);

            Panel scheduleSection = CreateSection("Jadwal Dokter Terpilih");
            DataGridView dgvSchedules = CreateGrid();
            dgvSchedules.DataSource = new List<ReservationScheduleItem>();
            UiTheme.ResizeDataGridViewColumns(dgvSchedules);
            scheduleSection.Controls.Add(dgvSchedules);
            scheduleSection.Controls.SetChildIndex(dgvSchedules, 0);
            cmbSchedules.Tag = dgvSchedules;

            Panel reservationSection = CreateSection("Reservasi Saya");
            dgvMyReservations = CreateGrid();
            dgvMyReservations.SelectionChanged += DgvMyReservations_SelectionChanged;
            reservationSection.Controls.Add(dgvMyReservations);
            reservationSection.Controls.SetChildIndex(dgvMyReservations, 0);

            layout.Controls.Add(formPanel, 0, 0);
            layout.Controls.Add(scheduleSection, 0, 1);
            layout.Controls.Add(reservationSection, 0, 2);
            RootLayout.Controls.Add(layout, 0, 1);
        }

        private void LoadDoctors()
        {
            List<LookupItem> doctors = _service.GetActiveDoctors();
            cmbDoctors.DisplayMember = "Text";
            cmbDoctors.ValueMember = "Id";
            cmbDoctors.DataSource = doctors;

            if (doctors.Count == 0)
            {
                cmbSchedules.DataSource = new List<ReservationScheduleItem>();
                BindScheduleGrid(new List<ReservationScheduleItem>());
            }
        }

        private void LoadSchedules()
        {
            int doctorId = cmbDoctors.SelectedValue == null ? 0 : Convert.ToInt32(cmbDoctors.SelectedValue);
            List<ReservationScheduleItem> schedules = _service.GetSchedulesByDoctor(doctorId);

            cmbSchedules.DisplayMember = "ScheduleText";
            cmbSchedules.ValueMember = "ScheduleId";
            cmbSchedules.DataSource = schedules;
            BindScheduleGrid(schedules);
        }

        private void BindScheduleGrid(List<ReservationScheduleItem> schedules)
        {
            DataGridView grid = cmbSchedules.Tag as DataGridView;
            if (grid != null)
            {
                grid.DataSource = null;
                grid.DataSource = schedules;
                UiTheme.ResizeDataGridViewColumns(grid);
            }
        }

        private void LoadReservations()
        {
            dgvMyReservations.SelectionChanged -= DgvMyReservations_SelectionChanged;
            dgvMyReservations.DataSource = _service.GetPatientReservations(SessionHelper.CurrentUser.Id);
            UiTheme.ResizeDataGridViewColumns(dgvMyReservations);
            dgvMyReservations.ClearSelection();
            selectedReservationId = null;
            dgvMyReservations.SelectionChanged += DgvMyReservations_SelectionChanged;
            UpdateButtons();
        }

        private void CreateReservation()
        {
            int scheduleId = cmbSchedules.SelectedValue == null ? 0 : Convert.ToInt32(cmbSchedules.SelectedValue);
            ServiceResult result = _service.CreateReservation(SessionHelper.CurrentUser.Id, scheduleId, txtComplaint.Text);
            ShowResult(result);

            if (result.Success)
            {
                txtComplaint.Clear();
                LoadSchedules();
                LoadReservations();
            }
        }

        private void CancelReservation()
        {
            if (!selectedReservationId.HasValue)
            {
                MessageBox.Show(this, "Pilih reservasi yang ingin dibatalkan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(this, "Yakin ingin membatalkan reservasi ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            ServiceResult result = _service.CancelReservation(SessionHelper.CurrentUser.Id, selectedReservationId.Value);
            ShowResult(result);

            if (result.Success)
            {
                LoadSchedules();
                LoadReservations();
            }
        }

        private void DgvMyReservations_SelectionChanged(object sender, EventArgs e)
        {
            ReservationListItem item = dgvMyReservations.CurrentRow == null ? null : dgvMyReservations.CurrentRow.DataBoundItem as ReservationListItem;
            selectedReservationId = item == null ? (int?)null : item.ReservationId;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnCancelReservation.Enabled = selectedReservationId.HasValue;
        }
    }
}
