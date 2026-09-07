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

Tanggal 7 September 2026, Comet melalui CDP 9222.

- GitHub Pages API melaporkan `status: built`, source `main:/docs`, HTTPS aktif, dan commit build `8ebf281`.
- Halaman publik memuat loader, data, framework, dan WebAssembly dari URL relatif repository.
- Unity membuat konteks WebGL 2.0. Briefing, scene 3D, HUD, api, APAR, dan layar jeda terlihat pada `Documentation/VisualQA/browser-live-ready.png`, `browser-live-playing.png`, dan `browser-live-paused.png`.
- Sesi live menerima klik mulai, input keyboard untuk lanjut/jeda, dan input gameplay yang menghasilkan HUD `2 / 3 API PADAM`, `200 POIN`, serta status `APAR / MENYEMPROT` pada `browser-live-resumed.png`.
- Console live hanya berisi log startup Unity, saran header `Content-Encoding: gzip`, dan shader URP opsional yang ter-strip. Saat automation Comet melepaskan pointer lock, runtime mencatat `WrongDocumentError`; ini berasal dari batas pointer lock pada sesi automation dan tidak menghentikan pemuatan atau gameplay.

URL publik: https://fathahnoor.github.io/AgenticAPARDemo/
