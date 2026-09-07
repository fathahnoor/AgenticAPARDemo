using UnityEngine;

// Mengatur musik latar dan bunyi respons misi tanpa mengubah alur gameplay.
public class MissionAudio : MonoBehaviour
{
    public MissionManager mission;
    public FireExtinguisher apar;
    public ExitDoor exitDoor;

    [Header("Clips")]
    public AudioClip musicLoop;
    public AudioClip fireLoop;
    public AudioClip sprayLoop;
    public AudioClip pickupClip;
    public AudioClip missionStartClip;
    public AudioClip extinguishedClip;
    public AudioClip exitUnlockClip;
    public AudioClip completeClip;
    public AudioClip failedClip;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource ambienceSource;
    public AudioSource spraySource;
    public AudioSource sfxSource;

    MissionManager.MissionPhase lastPhase;
    bool lastEquipped;
    bool lastUnlocked;
    bool musicStarted;
    bool ambienceStarted;

    void Awake()
    {
        if (mission == null) mission = FindFirstObjectByType<MissionManager>();
        if (apar == null) apar = FindFirstObjectByType<FireExtinguisher>();
        if (exitDoor == null) exitDoor = FindFirstObjectByType<ExitDoor>();
        ConfigureSources();
        PreloadClip(musicLoop);
        PreloadClip(fireLoop);
        PreloadClip(sprayLoop);
        PreloadClip(pickupClip);
        PreloadClip(missionStartClip);
        PreloadClip(extinguishedClip);
        PreloadClip(exitUnlockClip);
        PreloadClip(completeClip);
        PreloadClip(failedClip);
    }

    void Start()
    {
        lastPhase = mission != null ? mission.Phase : MissionManager.MissionPhase.Ready;
        lastEquipped = apar != null && apar.IsEquipped;
        lastUnlocked = exitDoor != null && exitDoor.IsUnlocked;
        SubscribeFires();

        if (musicSource != null && musicLoop != null)
        {
            musicSource.clip = musicLoop;
            musicSource.loop = true;
        }
        if (ambienceSource != null && fireLoop != null)
        {
            ambienceSource.clip = fireLoop;
            ambienceSource.loop = true;
        }
        if (spraySource != null && sprayLoop != null)
        {
            spraySource.clip = sprayLoop;
            spraySource.loop = true;
        }
    }

    void OnDestroy()
    {
        foreach (var fire in FireSource.All)
            if (fire != null) fire.Extinguished -= OnFireExtinguished;
    }

    void Update()
    {
        if (mission == null) return;

        bool equipped = apar != null && apar.IsEquipped;
        if (equipped && !lastEquipped)
            PlaySfx(pickupClip);
        lastEquipped = equipped;

        bool spraying = equipped && mission.Phase == MissionManager.MissionPhase.Running && apar.IsSpraying;
        if (spraying)
        {
            if (spraySource != null && !spraySource.isPlaying && sprayLoop != null)
                spraySource.Play();
        }
        else if (spraySource != null && spraySource.isPlaying)
        {
            spraySource.Stop();
        }

        if (exitDoor != null && exitDoor.IsUnlocked && !lastUnlocked)
            PlaySfxAt(exitUnlockClip, exitDoor.transform.position, 0.85f);
        if (exitDoor != null) lastUnlocked = exitDoor.IsUnlocked;

        if (mission.Phase != lastPhase)
        {
            HandlePhaseChanged(lastPhase, mission.Phase);
            lastPhase = mission.Phase;
        }
    }

    void SubscribeFires()
    {
        foreach (var fire in FireSource.All)
            if (fire != null) fire.Extinguished += OnFireExtinguished;
    }

    void OnFireExtinguished(FireSource fire)
    {
        if (fire != null)
            PlaySfxAt(extinguishedClip, fire.transform.position, 0.75f);
    }

    void HandlePhaseChanged(MissionManager.MissionPhase previous, MissionManager.MissionPhase current)
    {
        if (current == MissionManager.MissionPhase.Paused)
        {
            if (musicSource != null) musicSource.Pause();
            if (ambienceSource != null) ambienceSource.Pause();
            if (spraySource != null) spraySource.Stop();
            return;
        }
        if (current == MissionManager.MissionPhase.Running)
        {
            if (musicSource != null && musicLoop != null)
            {
                if (!musicStarted) musicSource.Play();
                else musicSource.UnPause();
                musicStarted = true;
            }
            if (ambienceSource != null && fireLoop != null)
            {
                if (!ambienceStarted) ambienceSource.Play();
                else ambienceSource.UnPause();
                ambienceStarted = true;
            }
            if (previous == MissionManager.MissionPhase.Ready)
                PlaySfx(missionStartClip);
            return;
        }
        if (current == MissionManager.MissionPhase.Complete)
        {
            if (musicSource != null) musicSource.Stop();
            if (ambienceSource != null) ambienceSource.Stop();
            musicStarted = false;
            ambienceStarted = false;
            PlaySfx(completeClip);
        }
        else if (current == MissionManager.MissionPhase.Failed)
        {
            if (musicSource != null) musicSource.Stop();
            if (ambienceSource != null) ambienceSource.Stop();
            musicStarted = false;
            ambienceStarted = false;
            PlaySfx(failedClip);
        }
    }

    void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, 0.8f);
    }

    void PlaySfxAt(AudioClip clip, Vector3 position, float volume)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    void ConfigureSources()
    {
        ConfigureSource(musicSource, 0.12f, 0f, true);
        ConfigureSource(ambienceSource, 0.09f, 0.25f, true);
        ConfigureSource(spraySource, 0.28f, 0.35f, true);
        ConfigureSource(sfxSource, 0.7f, 0f, false);
    }

    static void ConfigureSource(AudioSource source, float volume, float spatialBlend, bool loop)
    {
        if (source == null) return;
        source.playOnAwake = false;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.loop = loop;
        source.dopplerLevel = 0f;
    }

    static void PreloadClip(AudioClip clip)
    {
        if (clip != null && clip.loadState == AudioDataLoadState.Unloaded)
            clip.LoadAudioData();
    }
}
