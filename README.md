# AgenticAPARDemo

Latihan APAR dalam ruang 3D: ambil tabung merah, padamkan tiga titik api, lalu keluar sebelum waktu habis.

**[Mainkan langsung di browser](https://fathahnoor.github.io/AgenticAPARDemo/)**

Gunakan laptop atau komputer dengan keyboard dan mouse. Pemuatan pertama mengunduh ruang latihan; setelah terbuka, pilih **Mulai latihan**. Timer 60 detik baru berjalan saat Anda mulai.

## Kontrol

| Tombol | Aksi |
| --- | --- |
| WASD atau panah | Bergerak |
| Mouse | Melihat dan membidik |
| E | Mengambil APAR ketika dekat |
| Tahan Space | Menyemprot |
| Esc | Jeda dan melepas mouse |
| Enter | Mulai atau melanjutkan |
| R | Mengulang dari layar jeda atau hasil |

Bidik pangkal api sampai penanda berubah hijau. Semprotan memeriksa jarak, arah, dan penghalang. Setiap api yang padam memberi 100 poin. Setelah ketiganya padam, ikuti jalur hijau menuju pintu keluar untuk mendapat bonus waktu.

## Ruang latihan

Tabung APAR memakai badan melengkung, katup, tuas, pin, manometer, selang, dan nozzle. Tong memiliki bibir logam, rusuk, arang, dan baki penampung. Ruangan dilengkapi pipa sprinkler, saluran ventilasi, kabinet, jendela, lampu, serta pintu darurat dengan panic bar.

Material bertekstur, pantulan logam, cahaya api, asap, dan pengaturan warna URP memberi bentuk dan kedalaman pada objek. Semua mesh dan material dibuat melalui Editor builder dalam proyek ini.

HUD menampilkan langkah misi, waktu, skor, status APAR, dan intensitas api yang dibidik. Latihan otomatis dijeda saat tab kehilangan fokus atau mouse dilepas oleh browser.

## Membuka di Unity

Gunakan **Unity 6000.6.0f1** dengan modul **Web Build Support**, URP 17.6, dan Input System. Clone dengan Git LFS agar aset sumber ikut tersedia, lalu buka `Assets/Scenes/DemoAPAR.unity` dan masuk Play Mode.

Menu `DemoAPAR > Build Scene` membangun ulang scene melalui Editor API. Tidak perlu menghapus scene terlebih dahulu. Perubahan scene dan aset tidak dilakukan dengan mengedit YAML secara manual.

## Build dan GitHub Pages

Unity Editor harus terbuka dengan package Pipeline aktif. Jalankan dari root repo:

```powershell
unity command recompile --json
unity command recompile_status --json
unity command menu --path 'DemoAPAR/Configure WebGL' --json
unity command switch_build_target --target WebGL --confirm true --json
unity command switch_build_target_status --json
unity command menu --path 'DemoAPAR/Build WebGL for GitHub Pages' --json
```

Tunggu switch selesai sebelum memulai build. Hasil build berada di `docs/`, dan laporan ada di `Documentation/webgl-build.txt`. GitHub Pages memakai branch `main`, folder `/docs`. Build memakai Gzip dengan JavaScript decompression fallback agar dapat dilayani oleh Pages. File dalam `docs/` disimpan sebagai berkas asli, bukan pointer Git LFS.

Petunjuk verifikasi dan deployment: [webgl-hosting-notes.md](Documentation/webgl-hosting-notes.md).

## Arsitektur dan verifikasi

Komponen gameplay tetap kecil: `PlayerController`, `PlayerInteractor`, `FireExtinguisher`, `FireSource`, `MissionManager`, `ExitDoor`, dan `MissionUI`. Builder scene, aset, UI, serta build WebGL ada di `Assets/Editor/`.

`DemoAparVerification` menjalankan 19 pemeriksaan melalui frame Play Mode dan Input System, mencakup briefing, gerakan, pickup, bidikan, penghalang, semprotan selektif, skor, pintu, hasil, timeout, dan restart. Hasil terakhir tersimpan di [ux-verification.txt](Documentation/ux-verification.txt). Untuk menjalankannya lewat Unity CLI:

```powershell
unity command editor_play --json
unity command eval --code 'DemoAparVerification.Run(); return DemoAparVerification.Status;' --json
```

Tunggu hasil laporan, lalu keluar dari Play Mode. Bukti visual ada di `Documentation/VisualQA/`. Untuk melanjutkan pengembangan lintas agent, mulai dari [HANDOFF.md](HANDOFF.md).

## Batas demo

Kontrol sentuh, suara, dan kapasitas isi tabung belum dibuat. Latihan ini menyederhanakan pengoperasian APAR untuk demo interaktif dan pembelajaran kontrol.

Dibuat oleh **Fathah Noor Prawita** bersama AI coding agent melalui siklus plan, code, control Unity, run, observe, fix, verify, dan commit. Versi awal menggunakan OpenCode dan Muse Spark. Pembaruan visual, UX, dan WebGL ini dikerjakan menggunakan GPT-6 Astra.
