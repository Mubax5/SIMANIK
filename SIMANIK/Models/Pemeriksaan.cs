using System;

namespace SIMANIK.Models
{
    public class Pemeriksaan
    {
        public int Id { get; set; }
        public int ReservasiId { get; set; }
        public int PasienId { get; set; }
        public int DokterId { get; set; }
        public DateTime TanggalPemeriksaan { get; set; }
        public string Anamnesis { get; set; }
        public string Diagnosis { get; set; }
        public string Tindakan { get; set; }
        public string Resep { get; set; }
        public string Catatan { get; set; }
        public StatusPemeriksaan Status { get; set; }
    }
}
