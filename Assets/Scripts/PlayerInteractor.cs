using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public FireExtinguisher apar;
    public Transform aparMount;
    public MissionUI ui;

    void Awake()
    {
        if (apar == null) apar = FindFirstObjectByType<FireExtinguisher>();
        if (ui == null) ui = FindFirstObjectByType<MissionUI>();
    }

    void Update()
    {
        var mission = MissionManager.Instance;
        if (mission == null || mission.Phase != MissionManager.MissionPhase.Running || apar == null) return;
        if (mission.FiresRemaining == 0)
        {
            ui?.ShowPrompt("Ikuti tanda hijau menuju pintu KELUAR");
            return;
        }
        if (apar.IsEquipped)
        {
            ui?.ShowPrompt(apar.CurrentTarget != null ? "Tahan SPACE  /  Padamkan pangkal api" : "Bidik pangkal api  /  Dekati hingga 7 meter");
            return;
        }
        float distance = Vector3.Distance(transform.position, apar.transform.position);
        bool nearby = distance <= apar.pickupRadius;
        ui?.ShowPrompt(nearby ? "[ E ]  Ambil APAR" : "Dekati APAR merah di area PERALATAN");
        var kb = Keyboard.current;
        if (nearby && kb != null && kb.eKey.wasPressedThisFrame && aparMount != null)
        {
            apar.Equip(aparMount);
            ui?.ShowCenterMessage("APAR siap. Bidik pangkal api, lalu tahan SPACE.", 3f);
        }
    }
}
