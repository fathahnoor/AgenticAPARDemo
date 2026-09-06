using UnityEngine;
using UnityEngine.UI;

// Presentasi HUD. Semua referensi dan tata letak dibuat oleh Editor builder.
public class MissionUI : MonoBehaviour
{
    public Text timeText, firesText, scoreText, centerText, promptText;
    public Text objectiveText, equipmentText, targetText;
    public Text overlayTitle, overlayBody, primaryLabel;
    public GameObject overlay, hud, targetPanel;
    public Button primaryButton, retryButton;
    public Image timeFill, targetFill, reticle;
    public FireExtinguisher apar;
    float messageUntil;
    int lastSecond = -1, lastScore = -1, lastFires = -1;

    static readonly Color Mint = new Color(0.35f, 0.94f, 0.73f);
    static readonly Color Amber = new Color(1f, 0.72f, 0.3f);
    static readonly Color Danger = new Color(1f, 0.36f, 0.3f);

    void Awake()
    {
        primaryButton.onClick.AddListener(PrimaryAction);
        retryButton.onClick.AddListener(() => MissionManager.Instance.Restart());
    }

    void PrimaryAction()
    {
        var m = MissionManager.Instance;
        if (m.Phase == MissionManager.MissionPhase.Complete || m.Phase == MissionManager.MissionPhase.Failed) m.Restart();
        else m.BeginOrResume();
    }

    void LateUpdate()
    {
        var m = MissionManager.Instance;
        if (m == null) return;
        UpdateHUD(m.TimeLeft, m.FiresRemaining, m.Score);
        if (Time.unscaledTime > messageUntil) centerText.text = "";
        bool playing = m.Phase == MissionManager.MissionPhase.Running;
        FireSource target = playing && apar != null ? apar.CurrentTarget : null;
        targetPanel.SetActive(target != null);
        reticle.color = target != null ? Mint : new Color(1f, 1f, 1f, 0.8f);
        if (target != null)
        {
            targetFill.fillAmount = target.Intensity;
            targetText.text = "INTENSITAS API  " + Mathf.CeilToInt(target.Intensity * 100f) + "%";
        }
        bool equipped = apar != null && apar.IsEquipped;
        equipmentText.text = equipped ? (apar.IsSpraying ? "APAR  /  MENYEMPROT" : "APAR  /  SIAP") : "APAR  /  BELUM DIAMBIL";
        equipmentText.color = equipped ? Mint : Amber;
        objectiveText.text = m.FiresRemaining == 0 ? "03   MENUJU PINTU KELUAR" : equipped ? "02   PADAMKAN " + m.FiresRemaining + " TITIK API" : "01   AMBIL APAR MERAH";
    }

    public void UpdateHUD(float timeLeft, int firesRemaining, int score)
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        if (seconds != lastSecond)
        {
            timeText.text = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
            lastSecond = seconds;
        }
        var m = MissionManager.Instance;
        int total = m != null ? m.TotalFires : 3;
        if (lastFires != firesRemaining)
        {
            firesText.text = (total - firesRemaining) + " / " + total + " API PADAM";
            lastFires = firesRemaining;
        }
        if (lastScore != score)
        {
            scoreText.text = score.ToString("000") + "  POIN";
            lastScore = score;
        }
        timeText.color = timeLeft <= 10f ? Danger : Color.white;
        timeFill.color = timeLeft <= 10f ? Danger : Mint;
        timeFill.fillAmount = timeLeft / (m != null ? m.missionTime : 60f);
    }

    public void ShowPrompt(string message) => promptText.text = message ?? "";

    public void ShowCenterMessage(string message, float duration)
    {
        centerText.text = message;
        messageUntil = Time.unscaledTime + duration;
    }

    public void ShowPhase(MissionManager.MissionPhase phase)
    {
        bool running = phase == MissionManager.MissionPhase.Running;
        overlay.SetActive(!running);
        hud.SetActive(running);
        retryButton.gameObject.SetActive(phase == MissionManager.MissionPhase.Paused);
        if (running) return;
        var m = MissionManager.Instance;
        overlayTitle.color = Color.white;
        switch (phase)
        {
            case MissionManager.MissionPhase.Ready:
                overlayTitle.text = "Siap menghadapi api?";
                overlayBody.text = "Ambil APAR, padamkan tiga titik api, lalu capai pintu keluar.\nWaktu latihan 60 detik dimulai saat Anda siap.\n\n01   Dekati APAR merah, lalu tekan E.\n02   Bidik pangkal api dan tahan SPACE.\n03   Setelah api padam, ikuti tanda KELUAR.\n\nWASD / Panah   Bergerak       Mouse   Lihat sekitar\nESC   Jeda       ENTER   Mulai / lanjut";
                primaryLabel.text = "MULAI LATIHAN   [ ENTER ]";
                break;
            case MissionManager.MissionPhase.Paused:
                overlayTitle.text = "Latihan dijeda";
                overlayBody.text = "Waktu berhenti. Lanjutkan saat Anda siap.\n\nWASD / Panah   Bergerak\nMouse   Lihat sekitar\nE   Ambil APAR\nTahan SPACE   Semprot\n\nESC / ENTER   Lanjut       R   Ulang dari awal";
                primaryLabel.text = "LANJUTKAN   [ ENTER ]";
                break;
            default:
                bool complete = phase == MissionManager.MissionPhase.Complete;
                overlayTitle.text = complete ? "Latihan selesai" : "Waktu latihan habis";
                overlayTitle.color = complete ? Mint : Amber;
                overlayBody.text = (complete ? "Semua api padam. Anda telah mencapai pintu keluar." : "Coba lagi. Dekati api hingga bidikan berubah hijau.")
                    + "\n\n" + (m.TotalFires - m.FiresRemaining) + " / " + m.TotalFires + " api padam"
                    + "\nSkor pemadaman   " + m.Score
                    + "\nBonus waktu   " + (m.FinalScore - m.Score)
                    + "\n\nSKOR AKHIR   " + m.FinalScore;
                primaryLabel.text = "ULANGI LATIHAN   [ R ]";
                break;
        }
    }

    public static Text MakeText(string name, Transform parent, int fontSize, TextAnchor anchor)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = anchor;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }
}
