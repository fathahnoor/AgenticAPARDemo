using UnityEngine;

// Otak misi: timer 60 detik, skor +100 per api padam, buka pintu keluar
// saat api habis, kapan COMPLETE dan FAILED. Referensi api, pintu,
// dan UI dicari otomatis agar builder tetap sederhana.
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public enum MissionPhase { Running, Complete, Failed }
    public MissionPhase Phase { get; private set; } = MissionPhase.Running;

    [Header("Tuning")]
    public float missionTime = 60f;
    public int pointsPerFire = 100;
    public int timeBonusPerSecond = 2;

    public float TimeLeft { get; private set; }
    public int Score { get; private set; }
    public int FiresRemaining { get; private set; }
    public int FinalScore { get; private set; }

    FireSource[] fires;
    ExitDoor exitDoor;
    MissionUI ui;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        TimeLeft = missionTime;
        fires = FindObjectsByType<FireSource>(FindObjectsSortMode.None);
        exitDoor = FindFirstObjectByType<ExitDoor>();
        ui = FindFirstObjectByType<MissionUI>();
        FiresRemaining = fires.Length;
        foreach (var fire in fires)
            fire.Extinguished += OnFireExtinguished;
    }

    void OnDestroy()
    {
        if (fires != null)
        {
            foreach (var fire in fires)
            {
                if (fire != null)
                    fire.Extinguished -= OnFireExtinguished;
            }
        }
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (Phase != MissionPhase.Running)
            return;
        TimeLeft = Mathf.Max(0f, TimeLeft - Time.deltaTime);
        if (TimeLeft <= 0f)
        {
            FailMission();
            return;
        }
        ui?.UpdateHUD(TimeLeft, FiresRemaining, Score);
    }

    void OnFireExtinguished(FireSource fire)
    {
        if (Phase != MissionPhase.Running)
            return;
        FiresRemaining = Mathf.Max(0, FiresRemaining - 1);
        Score += pointsPerFire;
        ui?.UpdateHUD(TimeLeft, FiresRemaining, Score);
        if (FiresRemaining <= 0 && exitDoor != null)
        {
            exitDoor.Unlock();
            ui?.ShowCenterMessage("EXIT UNLOCKED", 3f);
        }
    }

    public void TryCompleteMission()
    {
        if (Phase != MissionPhase.Running)
            return;
        if (exitDoor != null && !exitDoor.IsUnlocked)
            return;
        Phase = MissionPhase.Complete;
        FinalScore = Score + Mathf.RoundToInt(TimeLeft) * timeBonusPerSecond;
        ui?.UpdateHUD(TimeLeft, FiresRemaining, Score);
        ui?.ShowMissionEnd(true, FinalScore);
    }

    void FailMission()
    {
        Phase = MissionPhase.Failed;
        FinalScore = Score;
        ui?.UpdateHUD(0f, FiresRemaining, Score);
        ui?.ShowMissionEnd(false, FinalScore);
    }
}
