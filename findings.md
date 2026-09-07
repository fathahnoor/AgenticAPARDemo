# Temuan

- 7 September 2026: worktree awal bersih, main, origin menunjuk fathahnoor/AgenticAPARDemo.
- Unity CLI terhubung ke Editor 6000.6.0f1 pada port 7800. Scene aktif Assets/Scenes/DemoAPAR.unity.
- Gambar awal memperlihatkan HUD kiri atas terpotong, ruangan berupa bidang polos, tong berupa silinder tanpa detail, dan nyala hampir tidak terlihat.
- Builder tidak menetapkan material partikel. Cone partikel menggunakan sumbu lokal Z; kode lama mengasumsikan sumbu Y untuk semprotan.
- Timer langsung berjalan, belum ada onboarding/pause/retry, dan semprotan berbasis sudut belum memeriksa penghalang.

Keputusan: pertahankan komponen gameplay dan layout dasar, buat aset/material sendiri melalui Editor API, tanpa package baru. Tampilan ruang latihan industri dengan APAR merah sebagai objek utama, penanda evakuasi hijau, dan nyala hangat.

## Verifikasi tambahan

- CLI parameters require --name value, not name=value.
- capture_game_view save_path is relative to Assets. source=screen may return a stale frame while Game View is unfocused. ux-hud.png matched ux-briefing.png and is not evidence of the HUD.
- Latest compiler accepted the modified components. FindFirstObjectByType and FindObjectsSortMode are deprecated in this Editor and currently emit warnings. Resolve only as needed in touched code; do not broaden the refactor.
- Proposed particle-axis correction remains to be confirmed visually when materials and models are rebuilt. Treat the earlier axis note as an implementation hypothesis, not a completed visual verification.
