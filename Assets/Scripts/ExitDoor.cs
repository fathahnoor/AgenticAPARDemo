using UnityEngine;

// Pintu keluar darurat. Mulanya terkunci dan menutup lubang dinding.
// Setelah semua api padam, Unlock dipanggil MissionManager: panel
// bergeser terbuka dan collidernya mati agar pemain bisa lewat.
// Penyelesaian misi dicek via jarak pemain (tanpa fisika trigger).
public class ExitDoor : MonoBehaviour
{
    public bool IsUnlocked { get; private set; }
    public bool IsOpen { get; private set; }

    [Header("Refs (diisi builder)")]
    public Transform doorPanel;
    public Transform exitPoint;
    public Transform player;
    public Renderer panelRenderer;

    [Header("Tuning")]
    public Vector3 openOffset = new Vector3(0f, -2.6f, 0f);
    public float openSpeed = 2f;
    public float reachRadius = 1.6f;
    public Color lockedColor = new Color(0.7f, 0.15f, 0.12f);
    public Color unlockedColor = new Color(0.15f, 0.65f, 0.25f);

    Vector3 closedPos;
    Vector3 openPos;
    Collider panelCollider;

    void Awake()
    {
        if (doorPanel != null)
        {
            closedPos = doorPanel.position;
            openPos = closedPos + openOffset;
            panelCollider = doorPanel.GetComponent<Collider>();
        }
        if (player == null)
        {
            var pc = FindFirstObjectByType<PlayerController>();
            if (pc != null)
                player = pc.transform;
        }
        ApplyColor(lockedColor);
    }

    public void Unlock()
    {
        if (IsUnlocked)
            return;
        IsUnlocked = true;
        ApplyColor(unlockedColor);
    }

    void Update()
    {
        if (IsUnlocked && !IsOpen && doorPanel != null)
        {
            doorPanel.position = Vector3.MoveTowards(doorPanel.position, openPos, openSpeed * Time.deltaTime);
            if (Vector3.Distance(doorPanel.position, openPos) < 0.05f)
            {
                IsOpen = true;
                if (panelCollider != null)
                    panelCollider.enabled = false;
            }
        }
        if (IsUnlocked && !IsOpen && panelCollider != null && doorPanel != null)
        {
            // Longgarkan collider saat panel sudah setengah terbuka.
            if (Vector3.Distance(doorPanel.position, closedPos) > 1f)
                panelCollider.enabled = false;
        }
        CheckPlayerReached();
    }

    void CheckPlayerReached()
    {
        if (!IsUnlocked || player == null || exitPoint == null)
            return;
        if (MissionManager.Instance == null || MissionManager.Instance.Phase != MissionManager.MissionPhase.Running)
            return;
        float dist = Vector3.Distance(player.position, exitPoint.position);
        if (dist <= reachRadius)
            MissionManager.Instance.TryCompleteMission();
    }

    void ApplyColor(Color c)
    {
        if (panelRenderer != null)
            panelRenderer.material.color = c;
    }
}
