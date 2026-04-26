using System;

namespace SIMANIK.Models
{
    public class Dokter
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NamaLengkap { get; set; }
        public string NoSip { get; set; }
        public string Spesialisasi { get; set; }
        public string NoTelepon { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
