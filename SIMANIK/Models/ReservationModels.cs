using System;

namespace SIMANIK.Models
{
    public static class ReservationStatusText
    {
        public const string Pending = "Menunggu Verifikasi";
        public const string Confirmed = "Dikonfirmasi";
        public const string Rejected = "Ditolak";
        public const string CancelledByPatient = "Dibatalkan Pasien";
        public const string CheckedIn = "Check-in";
        public const string Completed = "Selesai";
    }

    public static class VisitStatusText
    {
        public const string Waiting = "Menunggu";
        public const string InProgress = "Sedang Diperiksa";
        public const string Completed = "Selesai";
    }

    public class PatientLookupItem
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string PatientNumber { get; set; }
        public string FullName { get; set; }
    }

    public class ReservationScheduleItem
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Quota { get; set; }
        public int UsedQuota { get; set; }
        public int RemainingQuota { get { return Math.Max(0, Quota - UsedQuota); } }
        public string ScheduleText
        {
            get
            {
                return ScheduleDate.ToString("dd/MM/yyyy") + " " + StartTime.ToString(@"hh\:mm") + "-" + EndTime.ToString(@"hh\:mm") + " | Sisa " + RemainingQuota;
            }
        }
    }

    public class ReservationListItem
    {
        public int ReservationId { get; set; }
        public int PatientId { get; set; }
        public int ScheduleId { get; set; }
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Complaint { get; set; }
        public string ReservationStatus { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ScheduleText
        {
            get
            {
                return ScheduleDate.ToString("dd/MM/yyyy") + " " + StartTime.ToString(@"hh\:mm") + "-" + EndTime.ToString(@"hh\:mm");
            }
        }
    }

    public class QueueListItem
    {
        public int VisitId { get; set; }
        public int ReservationId { get; set; }
        public int QueueNumber { get; set; }
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public string DoctorName { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Complaint { get; set; }
        public string VisitStatus { get; set; }
        public DateTime CheckInTime { get; set; }
        public string ScheduleText
        {
            get
            {
                return ScheduleDate.ToString("dd/MM/yyyy") + " " + StartTime.ToString(@"hh\:mm") + "-" + EndTime.ToString(@"hh\:mm");
            }
        }
    }
}
