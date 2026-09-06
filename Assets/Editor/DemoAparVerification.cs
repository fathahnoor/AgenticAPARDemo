using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Integration checks run through real Input System frames in Play Mode.
public static class DemoAparVerification
{
    public static string Status { get; private set; } = "Not run";
    static Keyboard keyboard;
    static readonly System.Collections.Generic.List<string> results = new();

    public static async void Run()
    {
        if (!EditorApplication.isPlaying || Status == "Running") return;
        Status = "Running";
        results.Clear();
        bool background = Application.runInBackground;
        try
        {
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")).Focus();
            keyboard = InputSystem.AddDevice<Keyboard>();
            keyboard.MakeCurrent();
            await Task.Delay(300);
            var mission = MissionManager.Instance;
            Check(mission.Phase == MissionManager.MissionPhase.Ready && mission.TimeLeft == 60f, "Briefing holds timer at 60");
            await Press(Key.Enter);
            Check(mission.Phase == MissionManager.MissionPhase.Running, "Enter starts mission");
            await Task.Delay(400);
            Check(mission.TimeLeft < 60f, "Timer advances in live frames");
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerController>();
            Vector3 start = player.transform.position;
            await Hold(Key.W, 250);
            Check(Vector3.Distance(start, player.transform.position) > 0.2f, "W moves the player");
            await Press(Key.Escape);
            float pausedTime = mission.TimeLeft;
            Vector3 pausedPosition = player.transform.position;
            await Hold(Key.W, 200);
            Check(mission.Phase == MissionManager.MissionPhase.Paused && Mathf.Approximately(pausedTime, mission.TimeLeft)
                && Vector3.Distance(pausedPosition, player.transform.position) < 0.01f, "Pause freezes timer and movement");
            await Press(Key.Escape);
            Check(mission.Phase == MissionManager.MissionPhase.Running, "Escape resumes mission");
            mission.TryCompleteMission();
            Check(mission.Phase == MissionManager.MissionPhase.Running, "Cannot complete while fires remain");
            var apar = mission.ui.apar;
            Teleport(player, apar.transform.position + new Vector3(0, 0, 1.2f));
            await Press(Key.E);
            Check(apar.IsEquipped && apar.GetComponentsInChildren<Collider>().All(c => !c.enabled), "E picks up APAR and disables body collisions");
            var fire = mission.fires.OrderBy(f => f.transform.position.sqrMagnitude).First();
            Teleport(player, fire.transform.position + new Vector3(0, 0, 3f));
            Aim(player, fire.AimPoint);
            await Task.Delay(100);
            Check(apar.FindTarget() == fire, "Crosshair finds aimed fire");
            var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = "VerificationOccluder";
            obstacle.transform.position = (player.playerCamera.transform.position + fire.AimPoint) * 0.5f;
            obstacle.transform.localScale = new Vector3(3, 3, 0.25f);
            Physics.SyncTransforms();
            Check(apar.FindTarget() == null, "Solid obstacle blocks spray target");
            UnityEngine.Object.Destroy(obstacle);
            await Task.Delay(100);
            await Hold(Key.Space, 2400);
            Check(fire.IsExtinguished && mission.FiresRemaining == 2 && mission.Score == 100, "Space extinguishes aimed fire and awards 100 once");
            Check(mission.fires.Where(f => f != fire).All(f => Mathf.Approximately(f.Intensity, 1f)), "Other fires remain unaffected");
            foreach (var other in mission.fires.Where(f => !f.IsExtinguished))
            {
                Teleport(player, other.transform.position + new Vector3(0, 0, 3f));
                Aim(player, other.AimPoint);
                await Hold(Key.Space, 2400);
                Check(other.IsExtinguished, "Spray extinguishes " + other.name);
            }
            Check(mission.FiresRemaining == 0 && mission.Score == 300 && mission.exitDoor.IsUnlocked, "All fires unlock exit");
            await Task.Delay(1300);
            Teleport(player, mission.exitDoor.exitPoint.position);
            await Task.Delay(200);
            Check(mission.Phase == MissionManager.MissionPhase.Complete && mission.FinalScore >= 300, "Reaching exit completes mission with time bonus");
            await Press(Key.R);
            await Task.Delay(400);
            mission = MissionManager.Instance;
            Check(mission.Phase == MissionManager.MissionPhase.Ready && mission.Score == 0 && mission.FiresRemaining == 3 && !mission.ui.apar.IsEquipped, "R restarts cleanly with three fires and briefing");
            await Press(Key.Enter);
            // Advance the actual timer branch without a one-minute idle wait.
            typeof(MissionManager).GetProperty("TimeLeft").SetValue(mission, 0.05f);
            await Task.Delay(200);
            Check(mission.Phase == MissionManager.MissionPhase.Failed, "Timer expiry enters Failed");
            mission.ui.primaryButton.onClick.Invoke();
            await Task.Delay(400);
            Check(MissionManager.Instance.Phase == MissionManager.MissionPhase.Ready, "Result button restarts mission");
            Status = "PASS (" + results.Count + " checks)";
        }
        catch (Exception e)
        {
            Status = "FAIL: " + e.Message;
            Debug.LogError("[DemoAPAR QA] " + Status);
        }
        finally
        {
            if (keyboard != null) InputSystem.RemoveDevice(keyboard);
            Application.runInBackground = background;
            Directory.CreateDirectory("Documentation");
            File.WriteAllText("Documentation/ux-verification.txt", DateTimeOffset.Now.ToString("O") + "\n" + Status + "\n" + string.Join("\n", results));
            Debug.Log("[DemoAPAR QA] " + Status);
        }
    }

    static void Check(bool pass, string label)
    {
        results.Add((pass ? "PASS " : "FAIL ") + label);
        if (!pass) throw new Exception(label);
    }

    static async Task Press(Key key) => await Hold(key, 90);

    static async Task Hold(Key key, int milliseconds)
    {
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
        await Task.Delay(milliseconds);
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        await Task.Delay(90);
    }

    static void Teleport(PlayerController player, Vector3 position)
    {
        var cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.position = position;
        cc.enabled = true;
        Physics.SyncTransforms();
    }

    static void Aim(PlayerController player, Vector3 target)
    {
        Vector3 dir = target - player.playerCamera.transform.position;
        player.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        float pitch = -Mathf.Atan2(dir.y, new Vector2(dir.x, dir.z).magnitude) * Mathf.Rad2Deg;
        typeof(PlayerController).GetField("pitch", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(player, pitch);
        player.playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}
