# 🔥 AgenticAPARDemo

### Simulator Latihan APAR 3D yang Dibangun 100% Secara Agentic 🤖

![Unity](https://img.shields.io/badge/Unity-6000.6.0f1-black?logo=unity)
![URP](https://img.shields.io/badge/Render_Pipeline-URP_17.6-blue)
![Platform](https://img.shields.io/badge/Platform-Windows_64--bit-0078D6?logo=windows)
![Input](https://img.shields.io/badge/Input-Input_System-green)
![C#](https://img.shields.io/badge/Code-C%23-239120?logo=csharp)

> **Bukan sekadar game.** Proyek ini adalah bukti nyata bahwa AI agent bisa
> **menulis kode, mengendalikan Unity, menjalankan Play Mode, membaca error,
> memperbaiki bug, dan memverifikasi hasilnya** dalam satu loop tertutup.
> Manusia memberi misi, agent yang mengerjakan. ☕

---

## 🎮 Misi Kamu

Kamu terjebak di ruang latihan saat **3 titik api** menyala! 🔥🔥🔥

| Langkah | Aksi |
| ------- | ---- |
| 1️⃣ | Ambil **APAR merah** yang tergeletak di ruangan |
| 2️⃣ | Bidik ke arah api |
| 3️⃣ | Semprot sampai **ketiga api padam** |
| 4️⃣ | Pintu darurat terbuka otomatis 🟢 |
| 5️⃣ | Lari ke pintu sebelum **timer 60 detik** habis ⏱️ |

Skor: **+100 per api** ➕ bonus sisa waktu. Gagal kalau waktu habis duluan!

---

## 🕹️ Kontrol

| Tombol | Fungsi |
| ------ | ------ |
| `W A S D` / Panah | 🏃 Gerak |
| Mouse | 👀 Lihat sekitar |
| `E` | 🧯 Ambil APAR |
| Tahan `Space` | 💨 Semprot APAR |

---

## 🧱 Cara Kerja Sistem

Setiap sistem adalah komponen kecil yang fokus, tanpa God Object:

| Komponen | Tugas |
| -------- | ----- |
| `PlayerController` | Gerak FPS + mouse look + gravitasi |
| `PlayerInteractor` | Ambil APAR pakai `E` + teks panduan |
| `FireExtinguisher` | Semprot kerucut (jarak + sudut), tempel di kamera |
| `FireSource` | Intensitas api 1 ke 0, partikel dan cahaya mengecil |
| `MissionManager` | Timer, skor, status COMPLETE / FAILED |
| `ExitDoor` | Terkunci merah 🔴, terbuka hijau 🟢 |
| `MissionUI` | HUD TIME / FIRES / SCORE + pesan misi |

Scene dibangun **murni lewat kode** (`Assets/Editor/DemoAparBuilder.cs`,
menu `DemoAPAR/Build Scene`). Tanpa edit file `.unity` manual. 🛡️

---

## 🚀 Cara Menjalankan

**Kebutuhan:** Unity **6000.6.0f1**, Windows 64-bit.

1. Clone repo ini
2. Buka dengan Unity Hub
3. Buka scene `Assets/Scenes/DemoAPAR.unity`
4. Tekan ▶️ **Play**
5. Padamkan semua api dan selamatkan dirimu!

> Mau bangun ulang scene dari nol? Hapus isi scene, lalu klik menu
> **DemoAPAR > Build Scene**. Semua objek dibuat ulang otomatis. ✨

---

## 🤖 Dibangun dengan Agentic Loop

```
PLAN → CODE → CONTROL UNITY → RUN → OBSERVE → FIX → VERIFY → COMMIT
```

| Tahap | Bukti |
| ----- | ----- |
| 💻 Code | 7 skrip C# + 1 builder ditulis agent |
| 🎛️ Control | Scene dibangun via Unity CLI resmi + `com.unity.pipeline` |
| 🏃 Run | Play Mode dikendalikan dari CLI |
| 👀 Observe | State dibaca langsung (`phase`, `score`, `intensity`) |
| 🐛 Fix | Bug nyata tertangkap: arah semprot `forward` vs `up`, diperbaiki dan diuji ulang |
| ✅ Verify | Jalur COMPLETE dan FAILED dua-duanya lolos uji |
| 📦 Commit | 5 commit kecil, semua terverifikasi dan terpush |

**Hasil verifikasi terakhir:**

- ✅ Compile bersih, Console 0 error 0 warning
- ✅ `Running → 3 api padam → score 300 → pintu unlock → COMPLETE`
- ✅ Timer habis → `FAILED`, status terkunci
- ✅ Semprot kerucut selektif: hanya api yang dibidik yang padam

---

## 🗂️ Struktur Proyek

```
AgenticAPARDemo/
├── AGENTS.md                  Aturan kerja untuk AI agent
├── Assets/
│   ├── Scripts/               7 komponen gameplay
│   ├── Editor/                Builder scene otomatis
│   └── Scenes/DemoAPAR.unity  Scene utama (terdaftar di Build)
├── Packages/                  Input System, URP, Pipeline
└── ProjectSettings/           Input System only, Windows 64-bit
```

---

## 🛠️ Tech Stack

Unity 6000.6 • URP 17.6 • Input System • CharacterController •
Particle System • uGUI • Unity CLI + `com.unity.pipeline` •
OpenCode + Muse Spark • Git

---

## ⚠️ Keterbatasan

- Visual sengaja sederhana (primitif + partikel). Fokusnya kemampuan agent, bukan grafis.
- Belum ada suara, asap, atau kapasitas tabung APAR. Lihat `demo-apar.md` untuk ide ekstensi.

---

## 👤 Kredit

Dibuat oleh **Fathah Noor Prawita** ([@fathahnoor](https://github.com/fathahnoor))
bersama AI coding agent, sebagai demo *closed-loop agentic Unity development*
untuk riset dan pembelajaran. 🎓
