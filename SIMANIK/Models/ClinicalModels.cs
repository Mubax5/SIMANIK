using System;
using System.Collections.Generic;

namespace SIMANIK.Models
{
    public class DoctorQueueExaminationItem
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

    public class PatientExaminationDetail
    {
        public int VisitId { get; set; }
        public int ReservationId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int DoctorUserId { get; set; }
        public string VisitStatus { get; set; }
        public string ReservationStatus { get; set; }
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string InitialComplaint { get; set; }
        public DateTime CheckInTime { get; set; }
    }

    public class PatientExaminationHistoryItem
    {
        public DateTime ExaminationDate { get; set; }
        public string DoctorName { get; set; }
        public string DiseaseName { get; set; }
        public string CurrentComplaint { get; set; }
        public string DiagnosisNotes { get; set; }
        public string TreatmentNotes { get; set; }
        public string Medicines { get; set; }
    }

    public class MedicalRecordItem
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public string BloodType { get; set; }
        public string AllergyNotes { get; set; }
        public string ChronicDiseaseNotes { get; set; }
    }

    public class MedicalRecordViewItem : MedicalRecordItem
    {
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ExaminationDetailBundle
    {
        public PatientExaminationDetail PatientDetail { get; set; }
        public MedicalRecordItem MedicalRecord { get; set; }
        public List<PatientExaminationHistoryItem> PatientHistory { get; set; }
        public List<DiseaseItem> Diseases { get; set; }
        public List<MedicineItem> Medicines { get; set; }

        public ExaminationDetailBundle()
        {
            PatientHistory = new List<PatientExaminationHistoryItem>();
            Diseases = new List<DiseaseItem>();
            Medicines = new List<MedicineItem>();
        }
    }

    public class ExaminationInput
    {
        public int VisitId { get; set; }
        public int DiseaseId { get; set; }
        public string CurrentComplaint { get; set; }
        public string DiagnosisNotes { get; set; }
        public string TreatmentNotes { get; set; }
    }

    public class PrescriptionInput
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string MedicineType { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public int Stock { get; set; }
        public string InstructionNote { get; set; }
    }

    public class PrescriptionDetailItem
    {
        public int PrescriptionDetailId { get; set; }
        public int ExaminationId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string MedicineType { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string InstructionNote { get; set; }
    }

    public class HistoryItem
    {
        public string Category { get; set; }
        public DateTime? Date { get; set; }
        public string PatientName { get; set; }
        public string PatientNumber { get; set; }
        public string DoctorName { get; set; }
        public string DiseaseName { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public string InstructionNote { get; set; }
        public string Complaint { get; set; }
        public string DiagnosisNotes { get; set; }
        public string TreatmentNotes { get; set; }
        public string Status { get; set; }
        public string MedicalRecordSummary { get; set; }
        public string ExtraInfo { get; set; }
    }
}
