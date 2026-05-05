using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class ExaminationDetailResult : ServiceResult
    {
        public ExaminationDetailBundle Detail { get; private set; }

        private ExaminationDetailResult(bool success, string message, ExaminationDetailBundle detail)
            : base(success, message)
        {
            Detail = detail;
        }

        public static ExaminationDetailResult Ok(ExaminationDetailBundle detail)
        {
            return new ExaminationDetailResult(true, "Detail pemeriksaan berhasil dimuat.", detail);
        }

        public new static ExaminationDetailResult Fail(string message)
        {
            return new ExaminationDetailResult(false, message, null);
        }
    }

    public class ExaminationService
    {
        private readonly ExaminationRepository _examinationRepository;
        private readonly PrescriptionRepository _prescriptionRepository;
        private readonly MedicalRecordRepository _medicalRecordRepository;

        public ExaminationService()
        {
            _examinationRepository = new ExaminationRepository();
            _prescriptionRepository = new PrescriptionRepository();
            _medicalRecordRepository = new MedicalRecordRepository();
        }

        public List<DoctorQueueExaminationItem> GetDoctorQueue(int userId)
        {
            if (!SessionHelper.HasRole(UserRole.Dokter) || SessionHelper.CurrentUser == null || SessionHelper.CurrentUser.Id != userId)
            {
                return new List<DoctorQueueExaminationItem>();
            }

            return _examinationRepository.GetDoctorQueueForExamination(userId);
        }

        public List<DiseaseItem> GetActiveDiseases()
        {
            if (!SessionHelper.HasRole(UserRole.Dokter))
            {
                return new List<DiseaseItem>();
            }

            return _examinationRepository.GetActiveDiseases();
        }

        public List<MedicineItem> GetActiveMedicines()
        {
            if (!SessionHelper.HasRole(UserRole.Dokter))
            {
                return new List<MedicineItem>();
            }

            return _examinationRepository.GetActiveMedicines();
        }

        public ServiceResult StartExamination(int userId, int visitId)
        {
            if (!SessionHelper.HasRole(UserRole.Dokter) || SessionHelper.CurrentUser == null || SessionHelper.CurrentUser.Id != userId)
            {
                return ServiceResult.Fail("Hanya Dokter yang boleh memulai pemeriksaan.");
            }

            if (visitId <= 0)
            {
                return ServiceResult.Fail("Pilih antrian yang ingin diperiksa.");
            }

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    PatientExaminationDetail detail = _examinationRepository.GetPatientDetailByVisitId(visitId, connection, transaction);
                    if (detail == null)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Data kunjungan tidak ditemukan.");
                    }

                    if (detail.DoctorUserId != userId)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Pasien ini tidak masuk antrian dokter login.");
                    }

                    if (detail.VisitStatus == VisitStatusText.Completed)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Kunjungan ini sudah selesai.");
                    }

                    if (_examinationRepository.VisitHasExamination(visitId, connection, transaction))
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Kunjungan ini sudah memiliki pemeriksaan.");
                    }

                    if (detail.VisitStatus == VisitStatusText.InProgress)
                    {
                        transaction.Commit();
                        return ServiceResult.Ok("Pemeriksaan sudah dalam status Sedang Diperiksa.");
                    }

                    if (detail.VisitStatus != VisitStatusText.Waiting)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Status kunjungan tidak bisa diperiksa.");
                    }

                    int affected = _examinationRepository.StartExamination(visitId, connection, transaction);
                    if (affected <= 0)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Status kunjungan gagal diubah menjadi Sedang Diperiksa.");
                    }

                    transaction.Commit();
                    return ServiceResult.Ok("Pemeriksaan dimulai. Status kunjungan menjadi Sedang Diperiksa.");
                }
                catch (Exception ex)
                {
                    RollbackSilently(transaction);
                    return ServiceResult.Fail("Pemeriksaan gagal dimulai: " + ex.Message);
                }
            }
        }

        public ExaminationDetailResult LoadExaminationDetail(int userId, int visitId)
        {
            if (!SessionHelper.HasRole(UserRole.Dokter) || SessionHelper.CurrentUser == null || SessionHelper.CurrentUser.Id != userId)
            {
                return ExaminationDetailResult.Fail("Hanya Dokter yang boleh melihat detail pemeriksaan.");
            }

            if (visitId <= 0)
            {
                return ExaminationDetailResult.Fail("Pilih antrian terlebih dahulu.");
            }

            PatientExaminationDetail detail = _examinationRepository.GetPatientDetailByVisitId(visitId);
            if (detail == null)
            {
                return ExaminationDetailResult.Fail("Data kunjungan tidak ditemukan.");
            }

            if (detail.DoctorUserId != userId)
            {
                return ExaminationDetailResult.Fail("Pasien ini tidak relevan dengan dokter login.");
            }

            _medicalRecordRepository.CreateIfNotExists(detail.PatientId);

            ExaminationDetailBundle bundle = new ExaminationDetailBundle
            {
                PatientDetail = detail,
                MedicalRecord = _medicalRecordRepository.GetByPatientId(detail.PatientId),
                PatientHistory = _examinationRepository.GetPatientHistoryByVisitId(visitId),
                Diseases = _examinationRepository.GetActiveDiseases(),
                Medicines = _examinationRepository.GetActiveMedicines()
            };

            return ExaminationDetailResult.Ok(bundle);
        }

        public ServiceResult AddPrescriptionItemToTemporaryList(
            List<PrescriptionInput> prescriptions,
            int medicineId,
            int quantity,
            string instructionNote)
        {
            if (!SessionHelper.HasRole(UserRole.Dokter))
            {
                return ServiceResult.Fail("Hanya Dokter yang boleh menambahkan resep.");
            }

            if (prescriptions == null)
            {
                return ServiceResult.Fail("Daftar resep tidak valid.");
            }

            if (medicineId <= 0)
            {
                return ServiceResult.Fail("Pilih obat terlebih dahulu.");
            }

            if (quantity <= 0)
            {
                return ServiceResult.Fail("Jumlah obat harus lebih dari 0.");
            }

            MedicineItem medicine = _examinationRepository.GetMedicineById(medicineId);
            if (medicine == null)
            {
                return ServiceResult.Fail("Obat tidak ditemukan.");
            }

            if (!medicine.IsActive)
            {
                return ServiceResult.Fail("Obat nonaktif tidak boleh diresepkan.");
            }

            PrescriptionInput existing = prescriptions.FirstOrDefault(item => item.MedicineId == medicineId);
            int totalQuantity = quantity + (existing == null ? 0 : existing.Quantity);

            if (totalQuantity > medicine.Stock)
            {
                return ServiceResult.Fail("Stok " + medicine.MedicineName + " tidak cukup. Stok tersedia: " + medicine.Stock + ".");
            }

            string cleanInstruction = string.IsNullOrWhiteSpace(instructionNote)
                ? medicine.DefaultInstruction
                : instructionNote.Trim();

            if (existing != null)
            {
                existing.Quantity = totalQuantity;
                existing.Stock = medicine.Stock;
                if (!string.IsNullOrWhiteSpace(cleanInstruction))
                {
                    existing.InstructionNote = cleanInstruction;
                }

                return ServiceResult.Ok("Obat sudah ada di resep, jumlah digabung.");
            }

            prescriptions.Add(new PrescriptionInput
            {
                MedicineId = medicine.MedicineId,
                MedicineName = medicine.MedicineName,
                MedicineType = medicine.MedicineType,
                Quantity = quantity,
                Unit = medicine.Unit,
                Stock = medicine.Stock,
                InstructionNote = cleanInstruction
            });

            return ServiceResult.Ok("Obat berhasil ditambahkan ke daftar resep sementara.");
        }

        public ServiceResult SaveExamination(int userId, ExaminationInput input, List<PrescriptionInput> prescriptions)
        {
            if (!SessionHelper.HasRole(UserRole.Dokter) || SessionHelper.CurrentUser == null || SessionHelper.CurrentUser.Id != userId)
            {
                return ServiceResult.Fail("Hanya Dokter yang boleh menyimpan pemeriksaan.");
            }

            string validationMessage = ValidateExaminationInput(input);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return ServiceResult.Fail(validationMessage);
            }

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    PatientExaminationDetail detail = _examinationRepository.GetPatientDetailByVisitId(input.VisitId, connection, transaction);
                    if (detail == null)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Data kunjungan tidak ditemukan.");
                    }

                    if (detail.DoctorUserId != userId)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Dokter hanya boleh menyimpan pemeriksaan pasien yang relevan.");
                    }

                    if (detail.VisitStatus != VisitStatusText.Waiting && detail.VisitStatus != VisitStatusText.InProgress)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Kunjungan hanya bisa disimpan jika status Menunggu atau Sedang Diperiksa.");
                    }

                    if (_examinationRepository.VisitHasExamination(input.VisitId, connection, transaction))
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Satu kunjungan hanya boleh memiliki satu pemeriksaan.");
                    }

                    DiseaseItem disease = _examinationRepository.GetDiseaseById(input.DiseaseId, connection, transaction);
                    if (disease == null || !disease.IsActive)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Penyakit/diagnosa utama tidak aktif atau tidak ditemukan.");
                    }

                    List<PrescriptionInput> mergedPrescriptions = NormalizePrescriptions(prescriptions);
                    foreach (PrescriptionInput prescription in mergedPrescriptions)
                    {
                        MedicineItem medicine = _examinationRepository.GetMedicineById(prescription.MedicineId, connection, transaction, true);
                        if (medicine == null)
                        {
                            transaction.Rollback();
                            return ServiceResult.Fail("Obat tidak ditemukan.");
                        }

                        if (!medicine.IsActive)
                        {
                            transaction.Rollback();
                            return ServiceResult.Fail("Obat " + medicine.MedicineName + " sudah nonaktif.");
                        }

                        if (prescription.Quantity > medicine.Stock)
                        {
                            transaction.Rollback();
                            return ServiceResult.Fail("Stok " + medicine.MedicineName + " tidak cukup. Stok tersedia: " + medicine.Stock + ".");
                        }

                        prescription.MedicineName = medicine.MedicineName;
                        prescription.MedicineType = medicine.MedicineType;
                        prescription.Unit = medicine.Unit;
                        prescription.Stock = medicine.Stock;
                        if (string.IsNullOrWhiteSpace(prescription.InstructionNote))
                        {
                            prescription.InstructionNote = medicine.DefaultInstruction;
                        }
                    }

                    int examinationId = _examinationRepository.CreateExamination(
                        input.VisitId,
                        detail.DoctorId,
                        input.DiseaseId,
                        input.CurrentComplaint.Trim(),
                        input.DiagnosisNotes,
                        input.TreatmentNotes.Trim(),
                        connection,
                        transaction);

                    foreach (PrescriptionInput prescription in mergedPrescriptions)
                    {
                        _prescriptionRepository.CreatePrescriptionDetail(
                            examinationId,
                            prescription.MedicineId,
                            prescription.Quantity,
                            prescription.InstructionNote,
                            connection,
                            transaction);

                        int reduced = _prescriptionRepository.ReduceMedicineStock(
                            prescription.MedicineId,
                            prescription.Quantity,
                            connection,
                            transaction);

                        if (reduced <= 0)
                        {
                            transaction.Rollback();
                            return ServiceResult.Fail("Stok " + prescription.MedicineName + " gagal dikurangi. Pemeriksaan dibatalkan.");
                        }
                    }

                    _examinationRepository.UpdateVisitStatus(input.VisitId, VisitStatusText.Completed, connection, transaction);
                    _examinationRepository.UpdateReservationStatusByVisitId(input.VisitId, ReservationStatusText.Completed, connection, transaction);
                    _medicalRecordRepository.CreateIfNotExists(detail.PatientId, connection, transaction);
                    _medicalRecordRepository.UpdateLastVisitDate(detail.PatientId, DateTime.Now, connection, transaction);

                    transaction.Commit();
                    return ServiceResult.Ok("Pemeriksaan berhasil disimpan. Status kunjungan dan reservasi menjadi Selesai.");
                }
                catch (Exception ex)
                {
                    RollbackSilently(transaction);
                    return ServiceResult.Fail("Pemeriksaan gagal disimpan: " + ex.Message);
                }
            }
        }

        private static string ValidateExaminationInput(ExaminationInput input)
        {
            if (input == null)
            {
                return "Data pemeriksaan tidak valid.";
            }

            if (input.VisitId <= 0)
            {
                return "Pilih antrian terlebih dahulu.";
            }

            if (string.IsNullOrWhiteSpace(input.CurrentComplaint))
            {
                return "Keluhan saat ini wajib diisi.";
            }

            if (input.DiseaseId <= 0)
            {
                return "Penyakit/diagnosa utama wajib dipilih.";
            }

            if (string.IsNullOrWhiteSpace(input.TreatmentNotes))
            {
                return "Catatan tindakan wajib diisi. Isi '-' jika tidak ada tindakan khusus.";
            }

            return null;
        }

        private static List<PrescriptionInput> NormalizePrescriptions(List<PrescriptionInput> prescriptions)
        {
            List<PrescriptionInput> result = new List<PrescriptionInput>();
            if (prescriptions == null)
            {
                return result;
            }

            foreach (PrescriptionInput item in prescriptions)
            {
                if (item == null || item.MedicineId <= 0)
                {
                    throw new InvalidOperationException("Data obat pada resep tidak valid.");
                }

                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException("Jumlah obat harus lebih dari 0.");
                }

                PrescriptionInput existing = result.FirstOrDefault(value => value.MedicineId == item.MedicineId);
                if (existing == null)
                {
                    result.Add(new PrescriptionInput
                    {
                        MedicineId = item.MedicineId,
                        MedicineName = item.MedicineName,
                        MedicineType = item.MedicineType,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Stock = item.Stock,
                        InstructionNote = string.IsNullOrWhiteSpace(item.InstructionNote) ? string.Empty : item.InstructionNote.Trim()
                    });
                }
                else
                {
                    existing.Quantity += item.Quantity;
                    if (!string.IsNullOrWhiteSpace(item.InstructionNote))
                    {
                        existing.InstructionNote = item.InstructionNote.Trim();
                    }
                }
            }

            return result;
        }

        private static void RollbackSilently(MySqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
            }
        }
    }
}
