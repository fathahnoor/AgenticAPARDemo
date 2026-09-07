# 🔥 AgenticAPARDemo

### Simulator latihan APAR 3D yang dibuat 100% secara agentic 🤖

🎮 **Bisa langsung dicoba di browser:** [Mainkan AgenticAPARDemo di GitHub Pages](https://fathahnoor.github.io/AgenticAPARDemo/)

![Unity](https://img.shields.io/badge/Unity-6000.6.0f1-black?logo=unity)
![URP](https://img.shields.io/badge/Render_Pipeline-URP_17.6-blue)
![Platform](https://img.shields.io/badge/Platform-Windows_64--bit-0078D6?logo=windows)
![Input](https://img.shields.io/badge/Input-Input_System-green)
![C#](https://img.shields.io/badge/Code-C%23-239120?logo=csharp)

> Jadi gini. Repo ini bukan sekadar game. Ini bukti kalau AI agent bisa
> **nulis kode, ngendaliin Unity, masuk Play Mode, baca error, nge-fix bug,
> dan verifikasi hasilnya sendiri**. Manusia tinggal kasih misi,
> agent yang lembur. ☕

---

## 🎮 The Mission

Ceritanya kamu kejebak di ruang latihan pas **3 titik api** nyala! 🔥🔥🔥

Gampangnya gini:

1. Ambil **APAR merah** yang tergeletak di ruangan 🧯
2. Bidik ke apinya
3. Semprot sampai **ketiganya padam**
4. Pintu darurat kebuka sendiri 🟢
5. Kabur lewat pintu sebelum **timer 60 detik** habis ⏱️

Tiap api padam dapat **+100**, plus bonus sisa waktu. Kalau kehabisan waktu ya tamat, coba lagi. 😅

---

## 🕹️ Controls

| Tombol | Buat apa |
| ------ | -------- |
| `W A S D` / Panah | 🏃 Jalan |
| Mouse | 👀 Nengok kanan kiri |
| `E` | 🧯 Ngambil APAR |
| Tahan `Space` | 💨 Nyemprot |

---

## 🧱 How It Works

Isinya komponen kecil-kecil, masing-masing satu kerjaan. Nggak ada God Object.

| Komponen | Kerjaannya |
| -------- | --------- |
| `PlayerController` | Jalan FPS + nengok + gravitasi |
| `PlayerInteractor` | Ambil APAR pakai `E` + teks panduan |
| `FireExtinguisher` | Nyemprot bentuk kerucut, nempel di kamera |
| `FireSource` | Api 1 ke 0, partikel dan cahayanya ikut mengecil |
| `MissionManager` | Ngatur timer, skor, COMPLETE / FAILED |
| `ExitDoor` | Dikunci merah 🔴, kebuka hijau 🟢 |
| `MissionUI` | HUD TIME / FIRES / SCORE + pesan misi |

Scene-nya dibangun **murni dari kode** (`Assets/Editor/DemoAparBuilder.cs`,
menu `DemoAPAR/Build Scene`). Nggak ada edit file `.unity` manual. Aman. 🛡️

---

## 🚀 How to Run

**Butuh:** Unity **6000.6.0f1**, Windows 64-bit.

1. Clone repo ini
2. Buka pakai Unity Hub
3. Buka scene `Assets/Scenes/DemoAPAR.unity`
4. Pencet ▶️ **Play**
5. Padamin semua api dan selamatkan dirimu!

> Pengen bangun ulang scene dari nol? Hapus isi scene, terus klik menu
> **DemoAPAR > Build Scene**. Semua objek kebikin otomatis. ✨

---

## 🤖 Built with Agentic Loop

```
PLAN → CODE → CONTROL UNITY → RUN → OBSERVE → FIX → VERIFY → COMMIT
```

| Tahap | Buktinya |
| ----- | -------- |
| 💻 Code | 7 skrip C# + 1 builder, semua ditulis agent |
| 🎛️ Control | Scene dibangun via Unity CLI resmi + `com.unity.pipeline` |
| 🏃 Run | Play Mode dikendalikan dari CLI |
| 👀 Observe | State dibaca langsung (`phase`, `score`, `intensity`) |
| 🐛 Fix | Bug beneran ketangkep: arah semprot `forward` vs `up`. Difix, dites ulang, lolos |
| ✅ Verify | Jalur COMPLETE sama FAILED dua-duanya dites |
| 📦 Commit | 6 commit kecil, semuanya verified baru dipush |

**Hasil tes terakhir:**

- ✅ Compile bersih, Console 0 error 0 warning
- ✅ `Running → 3 api padam → score 300 → pintu unlock → COMPLETE`
- ✅ Timer habis → `FAILED`, status kekunci
- ✅ Semprotan kerucut selektif: cuma api yang dibidik yang padam

---

## 🗂️ Project Structure

```
AgenticAPARDemo/
├── AGENTS.md                  Aturan main buat AI agent
├── Assets/
│   ├── Scripts/               7 komponen gameplay
│   ├── Editor/                Builder scene otomatis
│   └── Scenes/DemoAPAR.unity  Scene utama (udah masuk Build)
├── Packages/                  Input System, URP, Pipeline
└── ProjectSettings/           Input System only, Windows 64-bit
```

---

## 🛠️ Tech Stack

Unity 6000.6 • URP 17.6 • Input System • CharacterController •
Particle System • uGUI • Unity CLI + `com.unity.pipeline` •
OpenCode + Muse Spark • Git

---

## ⚠️ Known Limitations

- Visualnya sengaja sederhana (primitif + partikel). Fokusnya kemampuan agent, bukan grafis.
- Belum ada suara, asap, atau isi tabung APAR yang bisa habis. Ide lanjutannya ada di `demo-apar.md`.

---

## 👤 Credits

Dibuat oleh **Fathah Noor Prawita** ([@fathahnoor](https://github.com/fathahnoor))
bareng AI coding agent, buat demo *closed-loop agentic Unity development*.
Buat riset dan pembelajaran. 🎓

Versi awal menggunakan OpenCode dan Muse Spark 1.3 Free. Pembaruan visual, UX, dan WebGL ini dikerjakan menggunakan Codex dan GPT-6 Astra.
