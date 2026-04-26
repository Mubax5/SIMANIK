using System;

namespace SIMANIK.Models
{
    public class Reservasi
    {
        public int Id { get; set; }
        public int PasienId { get; set; }
        public int DokterId { get; set; }
        public string NomorAntrian { get; set; }
        public DateTime TanggalReservasi { get; set; }
        public string KeluhanAwal { get; set; }
        public StatusReservasi Status { get; set; }
        public string CatatanAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
