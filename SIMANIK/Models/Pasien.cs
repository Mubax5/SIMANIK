using System;

namespace SIMANIK.Models
{
    public class Pasien
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NoRekamMedis { get; set; }
        public string Nik { get; set; }
        public string NamaLengkap { get; set; }
        public JenisKelamin JenisKelamin { get; set; }
        public DateTime? TanggalLahir { get; set; }
        public string NoTelepon { get; set; }
        public string Email { get; set; }
        public string Alamat { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
