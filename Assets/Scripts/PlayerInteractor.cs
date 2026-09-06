using UnityEngine;
using UnityEngine.InputSystem;

// Menangani ambil APAR dengan tombol E saat dekat, plus teks panduan.
// Semprotan sendiri ditangani FireExtinguisher.
public class PlayerInteractor : MonoBehaviour
{
    [Header("Refs (diisi builder atau otomatis)")]
    public FireExtinguisher apar;
    public Transform aparMount;
    public MissionUI ui;

    void Awake()
    {
        if (apar == null)
            apar = FindFirstObjectByType<FireExtinguisher>();
        if (ui == null)
            ui = FindFirstObjectByType<MissionUI>();
    }

    void Update()
    {
        if (MissionManager.Instance == null || MissionManager.Instance.Phase != MissionManager.MissionPhase.Running)
            return;
        if (apar == null)
            return;
        if (!apar.IsEquipped)
            HandlePickup();
        else
            ui?.ShowPrompt("Hold SPACE to spray");
    }

    void HandlePickup()
    {
        float dist = Vector3.Distance(transform.position, apar.transform.position);
        if (dist > apar.pickupRadius)
        {
            ui?.ShowPrompt("");
            return;
        }
        ui?.ShowPrompt("Press E to pick up extinguisher");
        var kb = Keyboard.current;
        if (kb != null && kb.eKey.wasPressedThisFrame && aparMount != null)
        {
            apar.Equip(aparMount);
            ui?.ShowPrompt("Hold SPACE to spray");
        }
    }
}
