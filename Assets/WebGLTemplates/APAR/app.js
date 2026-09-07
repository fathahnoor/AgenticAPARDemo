(() => {
  "use strict";

  const canvas = document.getElementById("unity-canvas");
  const loadingPanel = document.getElementById("loading-panel");
  const loadingText = document.getElementById("loading-text");
  const progressBar = document.getElementById("loading-progress");
  const progressText = document.getElementById("loading-percent");
  const status = document.getElementById("player-status");
  const fullscreenButton = document.getElementById("fullscreen-button");
  const notice = document.getElementById("player-notice");
  let unityInstance = null;
  let failed = false;
  let wasLocked = false;
  let noticeTimer;

  function showNotice(message) {
    clearTimeout(noticeTimer);
    notice.textContent = message;
    notice.hidden = false;
    noticeTimer = setTimeout(() => { notice.hidden = true; }, 6000);
  }

  function showError(detail, title, message) {
    if (failed) return;
    failed = true;
    clearTimeout(slowLoadTimer);
    loadingPanel.hidden = true;
    notice.hidden = true;
    fullscreenButton.disabled = true;
    document.getElementById("error-title").textContent = title || "Ruang latihan belum terbuka";
    document.getElementById("error-message").textContent = message || "Periksa koneksi internet, lalu coba muat ulang. Jika masih gagal, coba tutup tab lain atau perbarui browser.";
    document.getElementById("error-detail").textContent = String(detail);
    document.getElementById("error-panel").hidden = false;
    status.textContent = "Permainan belum dapat dijalankan";
    console.error("APAR:", detail);
  }

  const slowLoadTimer = setTimeout(() => {
    document.getElementById("loading-hint").textContent = "Masih menyiapkan permainan. Biarkan tab ini terbuka, terutama pada koneksi yang lambat.";
  }, 20000);

  document.getElementById("retry-button").addEventListener("click", () => window.location.reload());
  document.getElementById("touch-note").hidden = !window.matchMedia("(pointer: coarse)").matches;

  // Unity owns the mission state and cursor lock. The page only manages canvas focus.
  canvas.addEventListener("pointerdown", () => canvas.focus({ preventScroll: true }));
  canvas.addEventListener("contextmenu", (event) => event.preventDefault());
  canvas.addEventListener("keydown", (event) => {
    if (["Space", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"].includes(event.code)) event.preventDefault();
  });
  canvas.addEventListener("webglcontextlost", (event) => {
    event.preventDefault();
    showError("WebGL context lost", "Tampilan permainan terhenti", "Muat ulang untuk membuka latihan kembali. Menutup tab lain bisa membantu mengurangi beban perangkat.");
  });
  document.addEventListener("pointerlockchange", () => {
    const locked = document.pointerLockElement === canvas;
    if (unityInstance && !failed) {
      if (wasLocked && !locked) unityInstance.SendMessage("MissionManager", "Pause");
      status.textContent = locked ? "Latihan berlangsung" : "Klik area permainan untuk menggunakan kontrol";
    }
    wasLocked = locked;
  });
  window.addEventListener("blur", () => { if (unityInstance && !failed) unityInstance.SendMessage("MissionManager", "Pause"); });
  document.addEventListener("pointerlockerror", () => showNotice("Klik area permainan, lalu pilih Mulai atau Lanjutkan untuk mengaktifkan mouse."));
  document.addEventListener("fullscreenerror", () => showNotice("Layar penuh belum tersedia. Anda tetap bisa bermain di halaman ini."));
  fullscreenButton.addEventListener("click", () => {
    if (!unityInstance || failed) return;
    canvas.focus({ preventScroll: true });
    try { unityInstance.SetFullscreen(1); }
    catch (error) { showNotice("Layar penuh belum tersedia. Anda tetap bisa bermain di halaman ini."); console.warn(error); }
  });

  if (typeof WebAssembly !== "object") {
    showError("WebAssembly unavailable", "Browser perlu diperbarui", "Gunakan browser terbaru di laptop atau komputer untuk menjalankan latihan ini.");
    return;
  }

  const buildUrl = "Build";
  const config = {
    arguments: [],
#if PROGRESSIVE_ASSET_LOADING
    primaryDataUrls: {{{ JSON.stringify(PRIMARY_DATA_FILES) }}},
    secondaryDataUrls: {{{ JSON.stringify(SECONDARY_DATA_FILES) }}},
#else
    dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
#endif
    frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
#if USE_WASM
    codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}",
#endif
#if SYMBOLS_FILENAME
    symbolsUrl: buildUrl + "/{{{ SYMBOLS_FILENAME }}}",
#endif
    streamingAssetsUrl: "StreamingAssets",
    fullscreenElementID: "unity-fullscreen-container",
    companyName: {{{ JSON.stringify(COMPANY_NAME) }}},
    productName: {{{ JSON.stringify(PRODUCT_NAME) }}},
    productVersion: {{{ JSON.stringify(PRODUCT_VERSION) }}},
    devicePixelRatio: Math.min(window.devicePixelRatio || 1, 1.5),
    showBanner: (message, type) => {
      if (type === "error") showError(message);
      else console.warn("Unity:", message);
    }
  };

  const loader = document.createElement("script");
  loader.src = buildUrl + "/{{{ LOADER_FILENAME }}}";
  loader.onerror = () => showError("Unable to download Unity loader");
  loader.onload = () => {
    if (failed) return;
    loadingText.textContent = "Mengunduh ruang latihan...";
    try {
      createUnityInstance(canvas, config, (progress) => {
        if (failed) return;
        const percent = Math.min(100, Math.max(0, Math.round(progress * 100)));
        progressBar.value = percent;
        progressBar.textContent = percent + "%";
        progressText.textContent = percent + "%";
        if (percent >= 90) loadingText.textContent = "Membuka ruang latihan...";
      }).then((instance) => {
        if (failed) { instance.Quit(); return; }
        clearTimeout(slowLoadTimer);
        unityInstance = instance;
        loadingPanel.hidden = true;
        fullscreenButton.disabled = false;
        status.textContent = "Siap. Klik area permainan, lalu pilih Mulai latihan.";
      }).catch((error) => showError(error));
    } catch (error) { showError(error); }
  };
  document.body.appendChild(loader);
})();
