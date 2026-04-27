using System;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormCheckIn : OperationalFormBase
    {
        private readonly ReservationService _reservationService = new ReservationService();
        private readonly VisitService _visitService = new VisitService();
        private DataGridView dgvConfirmedReservations;
        private Button btnCheckIn;
        private int? selectedReservationId;

        public FormCheckIn() : base("Check-in Pasien")
        {
            BuildUi();
            Load += delegate
            {
                if (EnsureRole(UserRole.Admin))
                {
                    LoadData();
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
                RowCount = 2
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12)
            };

            btnCheckIn = CreateButton("Check-in", true);
            Button btnRefresh = CreateButton("Refresh", false);
            btnCheckIn.Click += delegate { CheckIn(); };
            btnRefresh.Click += delegate { LoadData(); };
            actionPanel.Controls.Add(btnCheckIn);
            actionPanel.Controls.Add(btnRefresh);

            Panel gridSection = CreateSection("Reservasi Terkonfirmasi");
            dgvConfirmedReservations = CreateGrid();
            dgvConfirmedReservations.SelectionChanged += DgvConfirmedReservations_SelectionChanged;
            gridSection.Controls.Add(dgvConfirmedReservations);
            gridSection.Controls.SetChildIndex(dgvConfirmedReservations, 0);

            layout.Controls.Add(actionPanel, 0, 0);
            layout.Controls.Add(gridSection, 0, 1);
            RootLayout.Controls.Add(layout, 0, 1);
        }

        private void LoadData()
        {
            dgvConfirmedReservations.SelectionChanged -= DgvConfirmedReservations_SelectionChanged;
            dgvConfirmedReservations.DataSource = _reservationService.GetConfirmedReservations();
            UiTheme.ResizeDataGridViewColumns(dgvConfirmedReservations);
            dgvConfirmedReservations.ClearSelection();
            selectedReservationId = null;
            dgvConfirmedReservations.SelectionChanged += DgvConfirmedReservations_SelectionChanged;
            UpdateButtons();
        }

        private void CheckIn()
        {
            if (!selectedReservationId.HasValue)
            {
                MessageBox.Show(this, "Pilih reservasi yang ingin di-check-in.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ServiceResult result = _visitService.CheckInReservation(selectedReservationId.Value);
            ShowResult(result);

            if (result.Success)
            {
                LoadData();
            }
        }

        private void DgvConfirmedReservations_SelectionChanged(object sender, EventArgs e)
        {
            ReservationListItem item = dgvConfirmedReservations.CurrentRow == null ? null : dgvConfirmedReservations.CurrentRow.DataBoundItem as ReservationListItem;
            selectedReservationId = item == null ? (int?)null : item.ReservationId;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnCheckIn.Enabled = selectedReservationId.HasValue;
        }
    }
}
