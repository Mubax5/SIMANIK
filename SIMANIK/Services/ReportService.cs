using System;
using System.Collections.Generic;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class ReportService
    {
        private readonly ReportRepository _repository;

        public ReportService()
        {
            _repository = new ReportRepository();
        }

        public ReportData GetReport(string reportType, DateTime startDate, DateTime endDate)
        {
            ReportData data = new ReportData
            {
                ReportType = string.IsNullOrWhiteSpace(reportType) ? "Reservasi Per Hari" : reportType.Trim()
            };

            if (!SessionHelper.HasRole(UserRole.Admin))
            {
                data.Summary = "Hanya Admin yang boleh mengakses laporan.";
                return data;
            }

            NormalizeDates(ref startDate, ref endDate);
            data.Items = LoadItems(data.ReportType, startDate, endDate);
            data.ChartPoints = BuildChartPoints(data.ReportType, data.Items);
            data.Summary = BuildSummary(data.ReportType, data.Items, startDate, endDate);

            return data;
        }

        private List<ReportItem> LoadItems(string reportType, DateTime startDate, DateTime endDate)
        {
            if (IsType(reportType, "Reservasi Per Dokter"))
            {
                return _repository.GetReservationsPerDoctor(startDate, endDate);
            }

            if (IsType(reportType, "Kunjungan Selesai Per Hari"))
            {
                return _repository.GetCompletedVisitsPerDay(startDate, endDate);
            }

            if (IsType(reportType, "Jumlah Pasien Per Dokter"))
            {
                return _repository.GetPatientCountPerDoctor(startDate, endDate);
            }

            if (IsType(reportType, "Penyakit Paling Sering"))
            {
                return _repository.GetMostFrequentDiseases(startDate, endDate);
            }

            if (IsType(reportType, "Obat Paling Sering"))
            {
                return _repository.GetMostUsedMedicines(startDate, endDate);
            }

            if (IsType(reportType, "Obat Stok Rendah"))
            {
                return _repository.GetLowStockMedicines();
            }

            if (IsType(reportType, "Pemeriksaan Per Periode"))
            {
                return _repository.GetExaminationsPerPeriod(startDate, endDate);
            }

            return _repository.GetReservationsPerDay(startDate, endDate);
        }

        private static List<ChartDataPoint> BuildChartPoints(string reportType, List<ReportItem> items)
        {
            List<ChartDataPoint> points = new List<ChartDataPoint>();

            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ReportItem item = items[i];
                    string label = GetChartLabel(reportType, item);
                    double value = IsType(reportType, "Obat Stok Rendah") ? item.Stock : item.Total;

                    points.Add(new ChartDataPoint
                    {
                        Label = label,
                        Value = value
                    });
                }
            }

            if (points.Count == 0)
            {
                points.Add(new ChartDataPoint { Label = "Belum ada data", Value = 0 });
            }

            return points;
        }

        private static string BuildSummary(string reportType, List<ReportItem> items, DateTime startDate, DateTime endDate)
        {
            if (items == null || items.Count == 0)
            {
                return "Belum ada data";
            }

            int total = 0;
            for (int i = 0; i < items.Count; i++)
            {
                total += IsType(reportType, "Obat Stok Rendah") ? items[i].Stock : items[i].Total;
            }

            string period = IsType(reportType, "Obat Stok Rendah")
                ? "tanpa filter tanggal"
                : startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy");

            if (IsType(reportType, "Obat Stok Rendah"))
            {
                return "Ditemukan " + items.Count + " obat stok rendah.";
            }

            return reportType + " | Periode " + period + " | Total: " + total + " | Baris: " + items.Count;
        }

        private static string GetChartLabel(string reportType, ReportItem item)
        {
            if (IsType(reportType, "Reservasi Per Dokter") || IsType(reportType, "Jumlah Pasien Per Dokter"))
            {
                return item.DoctorName;
            }

            if (IsType(reportType, "Penyakit Paling Sering"))
            {
                return item.DiseaseName;
            }

            if (IsType(reportType, "Obat Paling Sering") || IsType(reportType, "Obat Stok Rendah"))
            {
                return item.MedicineName;
            }

            if (IsType(reportType, "Pemeriksaan Per Periode") && !string.IsNullOrWhiteSpace(item.DoctorName))
            {
                return item.Label + " - " + item.DoctorName;
            }

            return string.IsNullOrWhiteSpace(item.Label) ? "-" : item.Label;
        }

        private static void NormalizeDates(ref DateTime startDate, ref DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            if (startDate > endDate)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
        }

        private static bool IsType(string reportType, string value)
        {
            return string.Equals(reportType, value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
