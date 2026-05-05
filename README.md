# SIMANIK

SIMANIK adalah aplikasi desktop manajemen klinik untuk alur reservasi pasien, verifikasi admin, check-in, antrian, pemeriksaan dokter, resep obat, riwayat, medical record, dashboard, dan laporan operasional.

## Teknologi

- C# Windows Forms .NET Framework 4.7.2
- MySQL
- Laragon
- MySql.Data
- Visual Studio

## Role

- Admin: mengelola akun, dokter, jadwal, penyakit, obat, reservasi, check-in, antrian, medical record, riwayat, dan laporan.
- Dokter: melihat antrian, melakukan pemeriksaan, memilih diagnosa, memberi obat, dan melihat riwayat pasien relevan.
- Pasien: register, reservasi, melihat status, medical record, dan riwayat sendiri.

## Setup

1. Clone repository.
2. Buka Laragon dan pastikan MySQL berjalan.
3. Import database dari `Database/simanik_db.sql`.
4. Copy `konfig.example.txt` menjadi `konfig.txt`.
5. Sesuaikan isi `konfig.txt`:
   - `DB_HOST`
   - `DB_PORT`
   - `DB_NAME`
   - `DB_USER`
   - `DB_PASSWORD`
6. Buka `SIMANIK.slnx` di Visual Studio.
7. Restore NuGet package.
8. Build project.
9. Run aplikasi.

Catatan: `konfig.txt` berisi konfigurasi lokal dan tidak boleh masuk Git. `konfig.example.txt` aman untuk repository.

## Akun Demo

Gunakan akun seed database untuk demo lokal:

| Role | Username | Password |
| --- | --- | --- |
| Admin | `admin` | `admin123` |
| Dokter | `dokter_umum` | `dokter123` |
| Pasien | `pasien_andi` | `pasien123` |

Jangan gunakan password pribadi untuk akun demo.

## Alur Demo

1. Pasien register atau login memakai akun demo.
2. Pasien membuat reservasi.
3. Admin login, verifikasi reservasi, lalu check-in pasien.
4. Dokter login, buka antrian, mulai pemeriksaan, pilih diagnosa, tambah obat, lalu simpan pemeriksaan.
5. Pasien login kembali dan melihat riwayat pemeriksaan serta obat.
6. Admin membuka riwayat dan laporan operasional.

## Fitur Utama

- Login multi-role dan session.
- Dashboard Admin, Dokter, dan Pasien.
- Master data user, dokter, jadwal, penyakit, dan obat.
- Reservasi pasien, verifikasi admin, check-in, dan antrian.
- Pemeriksaan dokter, diagnosa, resep obat, validasi stok, dan pengurangan stok otomatis.
- Medical record ringkas.
- Riwayat reservasi, pemeriksaan, diagnosa, dan obat.
- Laporan admin:
  - Reservasi Per Hari
  - Reservasi Per Dokter
  - Kunjungan Selesai Per Hari
  - Jumlah Pasien Per Dokter
  - Penyakit Paling Sering
  - Obat Paling Sering
  - Obat Stok Rendah
  - Pemeriksaan Per Periode

## Struktur Penting

```text
Database/simanik_db.sql
SIMANIK/Forms
SIMANIK/Helpers
SIMANIK/Models
SIMANIK/Repositories
SIMANIK/Services
konfig.example.txt
```

## Tag Rollback

- `tahap-1-pondasi`
- `tahap-2-auth-dashboard`
- `tahap-3.1-fix-crud-master-data`
- `tahap-4-reservasi-checkin-antrian`
- `tahap-5-pemeriksaan-resep-riwayat`
- `tahap-6-final-demo`
