# TESTING SIMANIK

Checklist ini dipakai untuk demo presentasi end-to-end.

## 1. Setup Awal

- Clone repository.
- Buka Laragon dan jalankan MySQL.
- Import database dari `Database/simanik_db.sql`.
- Copy `konfig.example.txt` menjadi `konfig.txt`.
- Sesuaikan port Laragon dan konfigurasi database di `konfig.txt`.
- Buka `SIMANIK.slnx` di Visual Studio.
- Restore NuGet package.
- Build project sampai sukses.
- Run aplikasi.

## 2. Skenario Admin

- Login sebagai Admin.
- Buka dashboard Admin dan pastikan summary, grafik, tabel, dan pencarian tampil.
- Buka menu Akun.
- Tambah user dokter baru atau cek data user dokter yang sudah ada.
- Buka menu Dokter.
- Tambah atau cek data dokter aktif.
- Buka menu Jadwal.
- Tambah jadwal dokter untuk tanggal demo.
- Buka menu Penyakit.
- Tambah atau cek data penyakit aktif.
- Buka menu Obat.
- Tambah atau cek data obat aktif dengan stok cukup.
- Buka menu Laporan.
- Tampilkan salah satu laporan dan pastikan tabel serta chart muncul.

## 3. Skenario Pasien

- Register pasien baru atau login memakai akun demo pasien.
- Buka dashboard Pasien dan pastikan profile card tampil.
- Buka menu Reservasi.
- Pilih dokter dan jadwal.
- Isi keluhan awal.
- Buat reservasi.
- Pastikan reservasi muncul dengan status `Menunggu Verifikasi`.

## 4. Skenario Admin Reservasi

- Login sebagai Admin.
- Buka menu Reservasi.
- Pilih reservasi masuk.
- Klik Konfirmasi.
- Buka menu Check-in.
- Pilih reservasi terkonfirmasi.
- Klik Check-in.
- Buka menu Antrian.
- Pastikan pasien masuk daftar antrian dengan status `Menunggu`.

## 5. Skenario Dokter

- Login sebagai dokter yang sesuai jadwal reservasi.
- Buka dashboard Dokter.
- Buka menu Antrian dan pastikan pasien tampil.
- Buka menu Pemeriksaan.
- Pilih pasien dari antrian.
- Klik Mulai Periksa.
- Pastikan status berubah menjadi `Sedang Diperiksa`.
- Isi keluhan saat ini.
- Pilih penyakit/diagnosa utama.
- Isi catatan diagnosa.
- Isi catatan tindakan.
- Pilih obat aktif.
- Isi jumlah dan aturan pakai.
- Klik Tambah Obat.
- Klik Simpan Pemeriksaan.
- Pastikan pesan sukses tampil.
- Cek menu Obat sebagai Admin dan pastikan stok obat berkurang.

## 6. Skenario Riwayat

- Login sebagai Pasien.
- Buka menu Riwayat.
- Tampilkan riwayat Pemeriksaan, Diagnosa, dan Obat.
- Pastikan hanya data pasien sendiri yang muncul.
- Login sebagai Dokter.
- Buka menu Riwayat.
- Cari pasien relevan yang pernah diperiksa.
- Pastikan data pasien tidak relevan tidak muncul.
- Login sebagai Admin.
- Buka menu Riwayat.
- Tampilkan Reservasi, Pemeriksaan, Diagnosa, dan Obat.
- Pastikan data semua pasien bisa dilihat.

## 7. Skenario Laporan

- Login sebagai Admin.
- Buka menu Laporan.
- Pilih `Reservasi Per Hari`, atur tanggal, klik Tampilkan.
- Pilih `Reservasi Per Dokter`, klik Tampilkan.
- Pilih `Penyakit Paling Sering`, klik Tampilkan.
- Pilih `Obat Paling Sering`, klik Tampilkan.
- Pilih `Obat Stok Rendah`, klik Tampilkan.
- Pilih `Pemeriksaan Per Periode`, klik Tampilkan.
- Pastikan tabel punya scrollbar.
- Pastikan chart punya title, legend, label axis, dan fallback `Belum ada data` jika kosong.

## 8. Final Check Demo

- Build sukses.
- Tidak ada SQL di Form.
- `konfig.txt` tidak ke-track Git.
- `Database/simanik_db.sql` ada.
- `README.md` ada.
- `TESTING.md` ada.
- Dashboard tidak crash saat data kosong.
- Login demo berhasil:
  - Admin: `admin`
  - Dokter: `dokter_umum`
  - Pasien: `pasien_andi`
