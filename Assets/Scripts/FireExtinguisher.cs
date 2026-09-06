using UnityEngine;
using UnityEngine.InputSystem;

// Tabung APAR di scene. Mulanya tergeletak, diambil pemain dengan tombol E
// lewat PlayerInteractor, lalu menempel di mount kamera. Semprotan aktif
// selama Space ditahan dan misi berjalan. Kena api dihitung dengan
// pendekatan kerucut (jarak + sudut), andal tanpa raycast.
public class FireExtinguisher : MonoBehaviour
{
    public bool IsEquipped { get; private set; }
    public bool IsSpraying { get; private set; }

    [Header("Tuning semprotan")]
    public float sprayRange = 7f;
    public float sprayAngle = 30f;
    public float extinguishRate = 0.5f;

    [Header("Refs (diisi builder)")]
    public Transform sprayTip;
    public ParticleSystem sprayParticles;

    public float pickupRadius = 3f;

    public void Equip(Transform mount)
    {
        if (IsEquipped)
            return;
        IsEquipped = true;
        transform.SetParent(mount);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        bool wantSpray = false;
        if (IsEquipped && MissionManager.Instance != null && MissionManager.Instance.Phase == MissionManager.MissionPhase.Running)
        {
            var kb = Keyboard.current;
            if (kb != null && kb.spaceKey.isPressed)
                wantSpray = true;
        }
        IsSpraying = wantSpray;
        SetSprayVisual(wantSpray);
        if (wantSpray)
            SprayFires(Time.deltaTime);
    }

    void SetSprayVisual(bool on)
    {
        if (sprayParticles == null)
            return;
        var emission = sprayParticles.emission;
        emission.enabled = on;
    }

    void SprayFires(float deltaTime)
    {
        if (sprayTip == null)
            return;
        Vector3 origin = sprayTip.position;
        Vector3 forward = sprayTip.forward;
        float maxAmount = extinguishRate * deltaTime;
        foreach (var fire in FireSource.All)
        {
            if (fire == null || fire.IsExtinguished)
                continue;
            // Bidik ke tengah api, sedikit di atas lantai.
            Vector3 target = fire.transform.position + Vector3.up * 1f;
            Vector3 toTarget = target - origin;
            float dist = toTarget.magnitude;
            if (dist > sprayRange)
                continue;
            float angle = Vector3.Angle(forward, toTarget / Mathf.Max(dist, 0.001f));
            if (angle <= sprayAngle)
                fire.ApplySpray(maxAmount);
        }
    }
}
