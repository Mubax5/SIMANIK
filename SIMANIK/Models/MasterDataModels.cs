using System;

namespace SIMANIK.Models
{
    public class LookupItem
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class UserListItem
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DoctorItem
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string Status { get { return IsActive ? "Aktif" : "Nonaktif"; } }
    }

    public class DoctorScheduleItem
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Quota { get; set; }
        public bool IsActive { get; set; }
        public string Status { get { return IsActive ? "Aktif" : "Nonaktif"; } }
    }

    public class DiseaseItem
    {
        public int DiseaseId { get; set; }
        public string DiseaseCode { get; set; }
        public string DiseaseName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Status { get { return IsActive ? "Aktif" : "Nonaktif"; } }
    }

    public class MedicineItem
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string MedicineType { get; set; }
        public int Stock { get; set; }
        public string Unit { get; set; }
        public string DefaultInstruction { get; set; }
        public bool IsActive { get; set; }
        public string Status { get { return IsActive ? "Aktif" : "Nonaktif"; } }
        public string StockStatus { get { return Stock <= 10 ? "Stok rendah" : "Aman"; } }
    }
}
