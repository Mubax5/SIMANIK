using System;

namespace SIMANIK.Models
{
    public class AdminDashboardSummary
    {
        public int TotalPatients { get; set; }
        public int TotalActiveDoctors { get; set; }
        public int ReservationsToday { get; set; }
        public int PendingReservations { get; set; }
        public int CheckedInPatientsToday { get; set; }
        public int CompletedExaminationsToday { get; set; }
        public int LowStockMedicines { get; set; }
        public string TopDiseaseThisMonth { get; set; }
    }

    public class DoctorDashboardSummary
    {
        public int TodaySchedules { get; set; }
        public int WaitingPatients { get; set; }
        public int InProgressPatients { get; set; }
        public int CompletedExaminationsToday { get; set; }
    }

    public class PatientDashboardSummary
    {
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public int Age { get; set; }
        public string NextReservation { get; set; }
        public string LastReservationStatus { get; set; }
    }

    public class ChartDataPoint
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }

    public class RecentReservationItem
    {
        public DateTime? Date { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Complaint { get; set; }
        public string Status { get; set; }
        public string ExtraInfo { get; set; }
    }

    public class QueueItem
    {
        public DateTime? Date { get; set; }
        public string QueueNumber { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Complaint { get; set; }
        public string Status { get; set; }
    }

    public class LowStockMedicineItem
    {
        public string MedicineName { get; set; }
        public string MedicineType { get; set; }
        public int Stock { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
    }

    public class RecentExaminationItem
    {
        public DateTime? Date { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DiseaseName { get; set; }
        public string Complaint { get; set; }
        public string Treatment { get; set; }
    }

    public class DashboardSearchCriteria
    {
        public string Keyword { get; set; }
        public string SearchType { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class DashboardSearchResultItem
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public string ExtraInfo { get; set; }
    }
}
