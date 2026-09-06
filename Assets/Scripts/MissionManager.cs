using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Siklus latihan, timer, skor, dan transisi yang dipakai input serta HUD.
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    public enum MissionPhase { Running, Complete, Failed, Ready, Paused }
    public MissionPhase Phase { get; private set; } = MissionPhase.Ready;

    public float missionTime = 60f;
    public int pointsPerFire = 100;
    public int timeBonusPerSecond = 2;
    public FireSource[] fires;
    public ExitDoor exitDoor;
    public MissionUI ui;

    public float TimeLeft { get; private set; }
    public int Score { get; private set; }
    public int FiresRemaining { get; private set; }
    public int FinalScore { get; private set; }
    public int TotalFires => fires.Length;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        TimeLeft = missionTime;
        if (fires == null || fires.Length == 0)
            fires = FindObjectsByType<FireSource>(FindObjectsSortMode.None);
        if (exitDoor == null) exitDoor = FindFirstObjectByType<ExitDoor>();
        if (ui == null) ui = FindFirstObjectByType<MissionUI>();
        FiresRemaining = fires.Length;
        foreach (var fire in fires)
            fire.Extinguished += OnFireExtinguished;
    }

    void Start() => SetPhase(MissionPhase.Ready);

    void OnDestroy()
    {
        if (fires != null)
            foreach (var fire in fires)
                if (fire != null) fire.Extinguished -= OnFireExtinguished;
        if (Instance == this)
        {
            Instance = null;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null)
        {
            if ((Phase == MissionPhase.Ready || Phase == MissionPhase.Paused) && kb.enterKey.wasPressedThisFrame)
                BeginOrResume();
            else if (kb.escapeKey.wasPressedThisFrame)
            {
                if (Phase == MissionPhase.Running) Pause();
                else if (Phase == MissionPhase.Paused) BeginOrResume();
            }
            if (Phase != MissionPhase.Running && Phase != MissionPhase.Ready && kb.rKey.wasPressedThisFrame)
                Restart();
        }
        if (Phase != MissionPhase.Running) return;
        TimeLeft = Mathf.Max(0f, TimeLeft - Time.deltaTime);
        if (TimeLeft <= 0f)
        {
            FinalScore = Score;
            SetPhase(MissionPhase.Failed);
        }
    }

    public void BeginOrResume()
    {
        if (Phase == MissionPhase.Ready || Phase == MissionPhase.Paused)
            SetPhase(MissionPhase.Running);
    }

    public void Pause()
    {
        if (Phase == MissionPhase.Running) SetPhase(MissionPhase.Paused);
    }

    void OnApplicationFocus(bool focused)
    {
        if (!focused) Pause();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetPhase(MissionPhase phase)
    {
        Phase = phase;
        // Nyala tetap hidup pada briefing dan hasil, tetapi berhenti saat pause.
        Time.timeScale = phase == MissionPhase.Paused ? 0f : 1f;
        bool playing = phase == MissionPhase.Running;
        Cursor.lockState = playing ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !playing;
        ui?.ShowPhase(phase);
    }

    void OnFireExtinguished(FireSource fire)
    {
        if (Phase != MissionPhase.Running) return;
        FiresRemaining = Mathf.Max(0, FiresRemaining - 1);
        Score += pointsPerFire;
        if (FiresRemaining == 0 && exitDoor != null)
        {
            exitDoor.Unlock();
            ui?.ShowCenterMessage("Semua api padam. Menuju pintu KELUAR.", 4f);
        }
        else ui?.ShowCenterMessage("Api padam  +" + pointsPerFire, 2f);
    }

    public void TryCompleteMission()
    {
        if (Phase != MissionPhase.Running || FiresRemaining > 0 || exitDoor == null || !exitDoor.IsUnlocked)
            return;
        FinalScore = Score + Mathf.RoundToInt(TimeLeft) * timeBonusPerSecond;
        SetPhase(MissionPhase.Complete);
    }
}
