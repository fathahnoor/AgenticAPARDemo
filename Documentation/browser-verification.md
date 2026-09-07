# Verifikasi browser

## Lokal

Tanggal 7 September 2026, Comet melalui CDP 9222.

- Server: `python -m http.server 8765 --bind 127.0.0.1 --directory docs`
- Halaman memuat `docs/index.html`, stylesheet, loader, data, framework, dan WebAssembly.
- Unity membuat konteks WebGL 2.0 pada renderer WebKit WebGL.
- Briefing awal, tombol mulai, HUD, api, APAR, lampu, dan layar jeda terlihat pada screenshot `Documentation/VisualQA/browser-local-shell.png`, `browser-local-running.png`, dan `browser-local-paused.png`.
- `node --check docs/app.js` lulus. Tidak ada macro template tersisa pada output `docs/`.
- Console hanya melaporkan saran header `Content-Encoding: gzip` dan shader URP opsional yang ter-strip. Server statis memakai decompression fallback.

## Publik

URL yang dituju: https://fathahnoor.github.io/AgenticAPARDemo/

Status deployment dan uji kontrol publik dicatat setelah GitHub Pages selesai membangun source `main:/docs`. Jangan menyatakan playable publik sebelum halaman, loader, dan input browser diperiksa pada URL ini.
