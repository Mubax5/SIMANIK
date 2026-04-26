namespace SIMANIK.Models
{
    public enum UserRole
    {
        Admin = 1,
        Dokter = 2,
        Pasien = 3
    }

    public enum JenisKelamin
    {
        TidakDiketahui = 0,
        LakiLaki = 1,
        Perempuan = 2
    }

    public enum StatusReservasi
    {
        MenungguVerifikasi = 1,
        Terverifikasi = 2,
        CheckIn = 3,
        Selesai = 4,
        Dibatalkan = 5
    }

    public enum StatusPemeriksaan
    {
        Menunggu = 1,
        DalamPemeriksaan = 2,
        Selesai = 3
    }
}
