using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class ScheduleService
    {
        private readonly ScheduleRepository _repository;

        public ScheduleService()
        {
            _repository = new ScheduleRepository();
        }

        public List<DoctorScheduleItem> Search(int doctorId, DateTime? date, string status)
        {
            return _repository.Search(doctorId, date, status);
        }

        public ServiceResult Save(DoctorScheduleItem schedule)
        {
            if (schedule == null)
            {
                return ServiceResult.Fail("Data jadwal tidak valid.");
            }

            if (schedule.DoctorId <= 0)
            {
                return ServiceResult.Fail("Dokter wajib dipilih.");
            }

            if (schedule.Quota <= 0)
            {
                return ServiceResult.Fail("Kuota harus lebih dari 0.");
            }

            if (schedule.EndTime <= schedule.StartTime)
            {
                return ServiceResult.Fail("Jam selesai harus lebih besar dari jam mulai.");
            }

            schedule.ScheduleDate = schedule.ScheduleDate.Date;

            if (schedule.ScheduleId <= 0)
            {
                _repository.Insert(schedule);
                return ServiceResult.Ok("Jadwal dokter berhasil ditambahkan.");
            }

            _repository.Update(schedule);
            return ServiceResult.Ok("Jadwal dokter berhasil diperbarui.");
        }

        public ServiceResult SetActive(int scheduleId, bool isActive)
        {
            if (scheduleId <= 0)
            {
                return ServiceResult.Fail("Pilih jadwal terlebih dahulu.");
            }

            _repository.SetActive(scheduleId, isActive);
            return ServiceResult.Ok(isActive ? "Jadwal berhasil diaktifkan." : "Jadwal berhasil dinonaktifkan.");
        }

        public ServiceResult DeleteSchedule(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                return ServiceResult.Fail("Pilih jadwal yang ingin dihapus.");
            }

            if (_repository.HasRelations(scheduleId))
            {
                _repository.Deactivate(scheduleId);
                return ServiceResult.Ok("Data tidak bisa dihapus karena sudah dipakai. Data akan dinonaktifkan.");
            }

            try
            {
                _repository.Delete(scheduleId);
                return ServiceResult.Ok("Jadwal berhasil dihapus.");
            }
            catch (MySqlException)
            {
                _repository.Deactivate(scheduleId);
                return ServiceResult.Ok("Data tidak bisa dihapus karena sudah dipakai. Data akan dinonaktifkan.");
            }
        }
    }
}
