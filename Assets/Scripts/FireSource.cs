using System;
using System.Collections.Generic;
using UnityEngine;

// Satu sumber api independen. Intensitas 1 = api penuh, 0 = padam.
// Semprotan APAR memanggil ApplySpray. Event Extinguished hanya
// dikirim sekali dan dipakai MissionManager untuk skor.
public class FireSource : MonoBehaviour
{
    public static readonly HashSet<FireSource> All = new HashSet<FireSource>();

    public float Intensity { get; private set; } = 1f;
    public bool IsExtinguished => Intensity <= 0f;
    public Vector3 AimPoint => transform.position + Vector3.up * 1.05f;

    public event Action<FireSource> Extinguished;

    [Header("Visual refs (diisi builder)")]
    public ParticleSystem fireParticles;
    public Light fireLight;
    public ParticleSystem smokeParticles;
    public Transform visualRoot;

    float baseEmissionRate = 60f;
    float baseLightIntensity = 2.5f;
    Vector3 baseVisualScale = Vector3.one;
    bool eventSent;
    float flickerSeed;

    void Awake()
    {
        flickerSeed = UnityEngine.Random.Range(0f, 100f);
        if (fireParticles != null)
            baseEmissionRate = fireParticles.emission.rateOverTime.constant;
        if (fireLight != null)
            baseLightIntensity = fireLight.intensity;
        if (visualRoot != null)
            baseVisualScale = visualRoot.localScale;
    }

    void OnEnable()
    {
        All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    void Update()
    {
        if (IsExtinguished)
            return;
        // Kedip ringan agar api terlihat hidup, skala ikut intensitas.
        if (fireLight != null)
        {
            float flicker = 0.9f + 0.1f * Mathf.Sin(Time.time * 25f + flickerSeed);
            fireLight.intensity = baseLightIntensity * Intensity * flicker;
        }
    }

    // amount = rate semprot * deltaTime dari FireExtinguisher.
    public void ApplySpray(float amount)
    {
        if (IsExtinguished || amount <= 0f)
            return;
        Intensity = Mathf.Max(0f, Intensity - amount);
        RefreshVisuals();
        if (IsExtinguished && !eventSent)
        {
            eventSent = true;
            if (fireParticles != null)
                fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            if (smokeParticles != null) smokeParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            if (fireLight != null)
                fireLight.intensity = 0f;
            Extinguished?.Invoke(this);
        }
    }

    void RefreshVisuals()
    {
        if (fireParticles != null)
        {
            var emission = fireParticles.emission;
            emission.rateOverTime = baseEmissionRate * Intensity;
        }
        if (visualRoot != null)
        {
            float s = 0.3f + 0.7f * Intensity;
            visualRoot.localScale = new Vector3(
                baseVisualScale.x * s,
                baseVisualScale.y * (0.2f + 0.8f * Intensity),
                baseVisualScale.z * s);
        }
    }
}
