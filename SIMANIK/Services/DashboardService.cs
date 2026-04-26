using System;
using System.Collections.Generic;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class DashboardService
    {
        private readonly DashboardRepository _dashboardRepository;

        public DashboardService()
        {
            _dashboardRepository = new DashboardRepository();
        }

        public AdminDashboardSummary GetAdminSummary()
        {
            return _dashboardRepository.GetAdminSummary();
        }

        public List<ChartDataPoint> GetReservationsLast7Days()
        {
            return FillLast7Days(_dashboardRepository.GetReservationsLast7Days());
        }

        public List<ChartDataPoint> GetReservationStatusDistribution()
        {
            return EnsureChartData(_dashboardRepository.GetReservationStatusDistribution());
        }

        public List<ChartDataPoint> GetPatientCountByDoctor()
        {
            return EnsureChartData(_dashboardRepository.GetPatientCountByDoctor());
        }

        public List<ChartDataPoint> GetTopDiseasesThisMonth()
        {
            return EnsureChartData(_dashboardRepository.GetTopDiseasesThisMonth());
        }

        public List<RecentReservationItem> GetRecentReservations()
        {
            return _dashboardRepository.GetRecentReservations();
        }

        public List<QueueItem> GetTodayQueues()
        {
            return _dashboardRepository.GetTodayQueues();
        }

        public List<LowStockMedicineItem> GetLowStockMedicines()
        {
            return _dashboardRepository.GetLowStockMedicines();
        }

        public List<DashboardSearchResultItem> SearchAdminDashboardData(DashboardSearchCriteria criteria)
        {
            return _dashboardRepository.SearchAdminDashboardData(NormalizeCriteria(criteria, "Semua", "Semua"));
        }

        public DoctorDashboardSummary GetDoctorSummary(int userId)
        {
            return _dashboardRepository.GetDoctorSummary(userId);
        }

        public List<QueueItem> GetDoctorTodayQueue(int userId)
        {
            return _dashboardRepository.GetDoctorTodayQueue(userId);
        }

        public List<ChartDataPoint> GetDoctorQueueStatusDistribution(int userId)
        {
            return EnsureChartData(_dashboardRepository.GetDoctorQueueStatusDistribution(userId));
        }

        public List<ChartDataPoint> GetDoctorExaminationsLast7Days(int userId)
        {
            return FillLast7Days(_dashboardRepository.GetDoctorExaminationsLast7Days(userId));
        }

        public List<ChartDataPoint> GetDoctorTopDiseases(int userId)
        {
            return EnsureChartData(_dashboardRepository.GetDoctorTopDiseases(userId));
        }

        public List<RecentExaminationItem> GetDoctorRecentExaminations(int userId)
        {
            return _dashboardRepository.GetDoctorRecentExaminations(userId);
        }

        public List<DashboardSearchResultItem> SearchDoctorDashboardData(int userId, DashboardSearchCriteria criteria)
        {
            return _dashboardRepository.SearchDoctorDashboardData(userId, NormalizeCriteria(criteria, "Antrian Hari Ini", "Semua"));
        }

        public PatientDashboardSummary GetPatientSummary(int userId)
        {
            return _dashboardRepository.GetPatientSummary(userId);
        }

        public List<ChartDataPoint> GetPatientReservationStatusDistribution(int userId)
        {
            return EnsureChartData(_dashboardRepository.GetPatientReservationStatusDistribution(userId));
        }

        public List<ChartDataPoint> GetPatientVisitsPerMonth(int userId)
        {
            return EnsureChartData(_dashboardRepository.GetPatientVisitsPerMonth(userId));
        }

        public List<ChartDataPoint> GetPatientDiseaseHistory(int userId)
        {
            return EnsureChartData(_dashboardRepository.GetPatientDiseaseHistory(userId));
        }

        public List<RecentReservationItem> GetPatientRecentReservations(int userId)
        {
            return _dashboardRepository.GetPatientRecentReservations(userId);
        }

        public List<RecentExaminationItem> GetPatientRecentExaminations(int userId)
        {
            return _dashboardRepository.GetPatientRecentExaminations(userId);
        }

        public List<LowStockMedicineItem> GetPatientRecentMedicines(int userId)
        {
            return _dashboardRepository.GetPatientRecentMedicines(userId);
        }

        public List<DashboardSearchResultItem> SearchPatientDashboardData(int userId, DashboardSearchCriteria criteria)
        {
            return _dashboardRepository.SearchPatientDashboardData(userId, NormalizeCriteria(criteria, "Reservasi Saya", "Semua"));
        }

        private static DashboardSearchCriteria NormalizeCriteria(DashboardSearchCriteria criteria, string defaultType, string defaultStatus)
        {
            DashboardSearchCriteria normalized = criteria ?? new DashboardSearchCriteria();

            normalized.Keyword = string.IsNullOrWhiteSpace(normalized.Keyword) ? string.Empty : normalized.Keyword.Trim();
            normalized.SearchType = string.IsNullOrWhiteSpace(normalized.SearchType) ? defaultType : normalized.SearchType.Trim();
            normalized.Status = string.IsNullOrWhiteSpace(normalized.Status) ? defaultStatus : normalized.Status.Trim();

            if (normalized.StartDate.HasValue)
            {
                normalized.StartDate = normalized.StartDate.Value.Date;
            }

            if (normalized.EndDate.HasValue)
            {
                normalized.EndDate = normalized.EndDate.Value.Date;
            }

            if (normalized.StartDate.HasValue && normalized.EndDate.HasValue && normalized.StartDate.Value > normalized.EndDate.Value)
            {
                DateTime startDate = normalized.StartDate.Value;
                normalized.StartDate = normalized.EndDate.Value;
                normalized.EndDate = startDate;
            }

            return normalized;
        }

        private static List<ChartDataPoint> EnsureChartData(List<ChartDataPoint> points)
        {
            if (points == null || points.Count == 0)
            {
                return new List<ChartDataPoint>
                {
                    new ChartDataPoint { Label = "Belum ada data", Value = 0 }
                };
            }

            return points;
        }

        private static List<ChartDataPoint> FillLast7Days(List<ChartDataPoint> points)
        {
            Dictionary<string, double> values = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    values[points[i].Label] = points[i].Value;
                }
            }

            List<ChartDataPoint> result = new List<ChartDataPoint>();

            for (int i = 6; i >= 0; i--)
            {
                string label = DateTime.Today.AddDays(-i).ToString("dd/MM");
                double value;
                values.TryGetValue(label, out value);

                result.Add(new ChartDataPoint
                {
                    Label = label,
                    Value = value
                });
            }

            return result;
        }
    }
}
