# Verifikasi audio

Tanggal 7 September 2026, Unity 6000.6.0f1 dan Comet melalui CDP 9222.

## Paket audio

- `bgm_training_loop.wav`: musik ambient 16 detik, loop, volume 0,12.
- `fire_crackle_loop.wav`: ambience api 8 detik, loop, volume 0,09.
- `apar_spray_loop.wav`: hiss semprotan 1,4 detik, loop, volume 0,28.
- `apar_pickup.wav`, `mission_start.wav`, `fire_extinguished.wav`, `exit_unlock.wav`, `mission_complete.wav`, dan `mission_failed.wav`: bunyi respons misi.
- Clip dibuat sebagai WAV 44,1 kHz, lalu diatur melalui `AudioImporter` menjadi CompressedInMemory, Vorbis, kualitas 0,7, dan preload aktif.

## Unity Play Mode

- Recompile melalui Unity CLI: selesai, `failed=false`, tanpa error baru.
- Builder `DemoAPAR/Build Scene` membuat objek `MissionAudio` dengan empat `AudioSource` dan seluruh clip terhubung.
- Evaluasi CLI mengonfirmasi semua clip terpasang dan volume BGM/ambience tetap kecil: `0.12`, `0.09`, `0.28`.
- Trigger fase siap ke berjalan memanggil start BGM dan ambience. Pause menghentikan sementara keduanya, sedangkan hasil misi menghentikan loop dan memainkan bunyi hasil.

## WebGL lokal

- Build Unity CLI: `Succeeded`, 0 error, 11 warning shader opsional, 52.483.745 byte.
- `node --check docs/app.js` lulus.
- Comet memuat halaman lokal, klik mulai mengubah status menjadi `Latihan berlangsung`, konteks audio browser melaporkan `Audio context resumed`, dan `errors` kosong.
- Screenshot: `Documentation/VisualQA/browser-audio-local.png`, `browser-audio-running.png`, dan `browser-audio-spray.png`.
