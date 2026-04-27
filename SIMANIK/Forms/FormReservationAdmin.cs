using System;
using System.Windows.Forms;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Services;

namespace SIMANIK.Forms
{
    public class FormReservationAdmin : OperationalFormBase
    {
        private readonly ReservationService _service = new ReservationService();
        private DataGridView dgvPendingReservations;
        private TextBox txtRejectionReason;
        private Button btnConfirm;
        private Button btnReject;
        private int? selectedReservationId;

        public FormReservationAdmin() : base("Verifikasi Reservasi")
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

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                WrapContents = true,
                BackColor = UiTheme.Panel,
                Padding = new Padding(12)
            };

            txtRejectionReason = CreateTextBox(420);
            btnConfirm = CreateButton("Konfirmasi", true);
            btnReject = CreateButton("Tolak", false);
            Button btnRefresh = CreateButton("Refresh", false);

            btnConfirm.Click += delegate { ConfirmReservation(); };
            btnReject.Click += delegate { RejectReservation(); };
            btnRefresh.Click += delegate { LoadData(); };

            actionPanel.Controls.Add(CreateField("Alasan penolakan", txtRejectionReason, 430));
            actionPanel.Controls.Add(btnConfirm);
            actionPanel.Controls.Add(btnReject);
            actionPanel.Controls.Add(btnRefresh);

            Panel gridSection = CreateSection("Reservasi Menunggu Verifikasi");
            dgvPendingReservations = CreateGrid();
            dgvPendingReservations.SelectionChanged += DgvPendingReservations_SelectionChanged;
            gridSection.Controls.Add(dgvPendingReservations);
            gridSection.Controls.SetChildIndex(dgvPendingReservations, 0);

            layout.Controls.Add(actionPanel, 0, 0);
            layout.Controls.Add(gridSection, 0, 1);
            RootLayout.Controls.Add(layout, 0, 1);
        }

        private void LoadData()
        {
            dgvPendingReservations.SelectionChanged -= DgvPendingReservations_SelectionChanged;
            dgvPendingReservations.DataSource = _service.GetPendingReservations();
            UiTheme.ResizeDataGridViewColumns(dgvPendingReservations);
            dgvPendingReservations.ClearSelection();
            selectedReservationId = null;
            dgvPendingReservations.SelectionChanged += DgvPendingReservations_SelectionChanged;
            UpdateButtons();
        }

        private void ConfirmReservation()
        {
            if (!selectedReservationId.HasValue)
            {
                MessageBox.Show(this, "Pilih reservasi terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ServiceResult result = _service.ConfirmReservation(selectedReservationId.Value);
            ShowResult(result);

            if (result.Success)
            {
                txtRejectionReason.Clear();
                LoadData();
            }
        }

        private void RejectReservation()
        {
            if (!selectedReservationId.HasValue)
            {
                MessageBox.Show(this, "Pilih reservasi terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ServiceResult result = _service.RejectReservation(selectedReservationId.Value, txtRejectionReason.Text);
            ShowResult(result);

            if (result.Success)
            {
                txtRejectionReason.Clear();
                LoadData();
            }
        }

        private void DgvPendingReservations_SelectionChanged(object sender, EventArgs e)
        {
            ReservationListItem item = dgvPendingReservations.CurrentRow == null ? null : dgvPendingReservations.CurrentRow.DataBoundItem as ReservationListItem;
            selectedReservationId = item == null ? (int?)null : item.ReservationId;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnConfirm.Enabled = selectedReservationId.HasValue;
            btnReject.Enabled = selectedReservationId.HasValue;
        }
    }
}
