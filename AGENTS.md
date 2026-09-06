# AgenticAPARDemo - Petunjuk Kerja Agent

Simulator latihan darurat kebakaran / APAR 3D. Dibangun dengan pola
agentic loop: PLAN, CODE, CONTROL UNITY, RUN, OBSERVE, FIX, VERIFY, COMMIT.

## Proyek

- Engine: Unity 6000.6.0f1, URP 17.6.0 (PC_RPAsset)
- Bahasa: C#
- Target: Windows 64-bit
- Scene aktif: Assets/Scenes/DemoAPAR.unity
- Input: Unity Input System SAJA (activeInputHandler = New).
  Jangan pakai API legacy (Input.GetKey, Input.GetAxis).
  Polling yang benar: UnityEngine.InputSystem.Keyboard.current,
  Mouse.current. Selalu cek null.
- Kontrol: WASD gerak, mouse lihat, E ambil APAR, tahan Space semprot.
- Paket penting: com.unity.inputsystem, com.unity.pipeline (0.6.0-exp.1),
  com.unity.render-pipelines.universal.

## Aturan Coding

- Ikuti arsitektur yang ada: komponen kecil dan fokus
  (PlayerController, PlayerInteractor, FireExtinguisher, FireSource,
  MissionManager, ExitDoor, MissionUI).
- Jangan refactor sistem yang tidak terkait.
- Jangan tambah package tanpa diminta.
- Hindari pencarian mahal di Update (cache referensi di Awake).
- Referensi eksplisit lewat Inspector/builder, bukan magic string.

## Aturan Aset Unity

- Jangan pernah edit manual file .meta.
- Jangan edit manual YAML .unity atau .prefab sebagai teks.
- Perubahan scene, prefab, komponen, dan Inspector lewat
  Unity CLI / Pipeline atau script Editor (Assets/Editor/).
- Jangan sentuh: Library/, Temp/, Logs/, obj/.

## Verifikasi (wajib tiap perubahan C#)

1. Tunggu Unity compile (recompile + recompile_status).
2. Baca Console (console / get_console_logs). Nol error baru.
3. Verifikasi hierarchy bila scene berubah.
4. Uji Play Mode bila gameplay berubah (editor_play, eval, editor_stop).
5. Commit kecil per milestone, push ke origin.

## Git

- Branch: main. Remote: origin (fathahnoor/AgenticAPARDemo).
- Identity: fathahnoor / fathah.noor@yahoo.com.
- Commit kecil per milestone yang sudah terverifikasi.
