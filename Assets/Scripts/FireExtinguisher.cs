using UnityEngine;
using UnityEngine.InputSystem;

// Bidikan kamera adalah sumber kebenaran untuk target, HUD, dan semprotan.
public class FireExtinguisher : MonoBehaviour
{
    public bool IsEquipped { get; private set; }
    public bool IsSpraying { get; private set; }
    public FireSource CurrentTarget { get; private set; }
    public float sprayRange = 7f;
    public float sprayAngle = 9f;
    public float extinguishRate = 0.5f;
    public float pickupRadius = 2.5f;
    public Transform sprayTip;
    public ParticleSystem sprayParticles;
    public Camera aimCamera;
    Collider[] bodyColliders;

    void Awake() => bodyColliders = GetComponentsInChildren<Collider>();

    public void Equip(Transform mount)
    {
        if (IsEquipped || mount == null) return;
        IsEquipped = true;
        foreach (var col in bodyColliders) col.enabled = false;
        transform.SetParent(mount, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        bool running = MissionManager.Instance != null && MissionManager.Instance.Phase == MissionManager.MissionPhase.Running;
        CurrentTarget = IsEquipped && running ? FindTarget() : null;
        var kb = Keyboard.current;
        IsSpraying = IsEquipped && running && kb != null && kb.spaceKey.isPressed;
        if (sprayTip != null && aimCamera != null && IsEquipped)
        {
            Vector3 target = CurrentTarget != null ? CurrentTarget.AimPoint : aimCamera.transform.position + aimCamera.transform.forward * sprayRange;
            sprayTip.rotation = Quaternion.LookRotation(target - sprayTip.position);
        }
        if (sprayParticles != null)
        {
            var emission = sprayParticles.emission;
            emission.enabled = IsSpraying;
        }
        if (IsSpraying && CurrentTarget != null)
            CurrentTarget.ApplySpray(extinguishRate * Time.deltaTime);
    }

    public FireSource FindTarget()
    {
        if (aimCamera == null) return null;
        Vector3 origin = aimCamera.transform.position;
        Vector3 forward = aimCamera.transform.forward;
        FireSource best = null;
        float bestAngle = sprayAngle;
        foreach (var fire in FireSource.All)
        {
            if (fire == null || fire.IsExtinguished) continue;
            Vector3 direction = fire.AimPoint - origin;
            float distance = direction.magnitude;
            if (distance > sprayRange) continue;
            float angle = Vector3.Angle(forward, direction);
            if (angle > bestAngle) continue;
            if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, ~0, QueryTriggerInteraction.Ignore)
                && !hit.transform.IsChildOf(fire.transform)) continue;
            if (sprayTip != null && Physics.Linecast(sprayTip.position, fire.AimPoint, out hit, ~0, QueryTriggerInteraction.Ignore)
                && !hit.transform.IsChildOf(fire.transform)) continue;
            bestAngle = angle;
            best = fire;
        }
        return best;
    }
}
