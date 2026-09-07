# WebGL dan GitHub Pages

Versi browser memakai template di Assets/WebGLTemplates/APAR. Targetnya laptop atau komputer dengan keyboard dan mouse. Kontrol sentuh belum tersedia.

Build melalui Unity CLI:

```powershell
unity command menu --path 'DemoAPAR/Configure WebGL' --json
unity command switch_build_target --target WebGL --confirm true --json
unity command switch_build_target_status --json
unity command menu --path 'DemoAPAR/Build WebGL for GitHub Pages' --json
```

Tunggu switch selesai sebelum build. Status build disimpan di Documentation/webgl-build.txt, output di docs/. Folder ini dipublikasikan dari main melalui GitHub Pages. File docs/.nojekyll mencegah pemrosesan Jekyll. .gitattributes mengecualikan docs/ dari LFS agar browser menerima build asli.

Konfigurasi release menggunakan Gzip dengan decompression fallback, tanpa WebAssembly threads, serta URP Forward. Fallback diperlukan untuk hosting statis yang tidak menyediakan header Content-Encoding sesuai file Unity. Alamat Build/ dan StreamingAssets/ relatif agar bisa berjalan pada subpath repositori. Template memanggil Pause saat pointer lock hilang atau window kehilangan fokus. Timer mulai setelah pemain menekan tombol mulai.

Sumber primer yang diperiksa 7 September 2026:

- [Unity: Deploy a Web application](https://docs.unity3d.com/6000.0/Documentation/Manual/webgl-deploying.html).
- [GitHub: Configure a Pages publishing source](https://docs.github.com/en/pages/getting-started-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site).
- Template bawaan Unity 6000.6.0f1 pada Editor/Data/PlaybackEngines/WebGLSupport, dibaca untuk format macro loader dan config.

Build lokal terverifikasi 7 September 2026 pukul 11:23 WIB: status Succeeded, error 0, 8 warning shader yang tidak dipakai, ukuran laporan 51.611.310 byte. Comet pada CDP 9222 memuat scene WebGL 2.0 dan menampilkan briefing, HUD, api, APAR, lampu, serta layar jeda. GitHub Pages kemudian dibangun dengan status `built` dari source `main:/docs` dan sesi live menerima input mulai, gameplay, jeda, serta lanjut.
