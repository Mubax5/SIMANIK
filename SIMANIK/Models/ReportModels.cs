using System;
using System.Collections.Generic;

namespace SIMANIK.Models
{
    public class ReportItem
    {
        public DateTime? Date { get; set; }
        public string Label { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string DiseaseName { get; set; }
        public string MedicineName { get; set; }
        public int Total { get; set; }
        public int Stock { get; set; }
        public string Unit { get; set; }
        public string ExtraInfo { get; set; }
    }

    public class ReportData
    {
        public string ReportType { get; set; }
        public string Summary { get; set; }
        public List<ReportItem> Items { get; set; }
        public List<ChartDataPoint> ChartPoints { get; set; }

        public ReportData()
        {
            Items = new List<ReportItem>();
            ChartPoints = new List<ChartDataPoint>();
        }
    }
}
