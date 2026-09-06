using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Membangun seluruh isi scene DemoAPAR lewat kode (bukan edit YAML).
// Dijalankan dari menu DemoAPAR/Build Scene atau via Unity CLI (menu).
// Aman dijalankan ulang: objek lama hasil build dibersihkan dulu.
public static class DemoAparBuilder
{
    const string MenuPath = "DemoAPAR/Build Scene";

    static Material floorMat;
    static Material wallMat;
    static Material darkMat;
    static Material redMat;

    [MenuItem(MenuPath)]
    public static void Build()
    {
        MakeMaterials();
        CleanupPreviousBuild();
        BuildRoom();
        BuildExit();
        BuildFire("Fire1", new Vector3(-4.5f, 0f, -2.5f));
        BuildFire("Fire2", new Vector3(4.5f, 0f, 1.5f));
        BuildFire("Fire3", new Vector3(0f, 0f, -1f));
        BuildApar();
        BuildPlayer();
        BuildMission();
        Debug.Log("[DemoAPAR] Build selesai. Simpan scene bila puas.");
    }

    // ---------- util ----------

    static void MakeMaterials()
    {
        Shader lit = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        floorMat = new Material(lit) { color = new Color(0.55f, 0.55f, 0.58f) };
        wallMat = new Material(lit) { color = new Color(0.7f, 0.7f, 0.72f) };
        darkMat = new Material(lit) { color = new Color(0.15f, 0.14f, 0.14f) };
        redMat = new Material(lit) { color = new Color(0.8f, 0.08f, 0.08f) };
    }

    static GameObject NewPrimitive(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.transform.localScale = scale;
        if (mat != null)
            go.GetComponent<Renderer>().material = mat;
        return go;
    }

    static void CleanupPreviousBuild()
    {
        // Kembalikan Main Camera ke root bila menempel di Player lama.
        var mainCam = GameObject.Find("Main Camera");
        if (mainCam != null)
            mainCam.transform.SetParent(null, true);
        foreach (string name in new[] { "Room", "Player", "APAR", "Fire1", "Fire2", "Fire3", "ExitDoor", "MissionManager", "MissionUI" })
        {
            var go = GameObject.Find(name);
            if (go != null)
                Undo.DestroyObjectImmediate(go);
        }
    }

    // ---------- ruangan ----------

    static void BuildRoom()
    {
        var room = new GameObject("Room");
        Transform t = room.transform;
        // Lantai 12.6 x 10.6, permukaan atas di y=0.
        NewPrimitive(PrimitiveType.Cube, "Floor", new Vector3(0f, -0.1f, 0f), new Vector3(12.6f, 0.2f, 10.6f), floorMat, t);
        // Plafon.
        NewPrimitive(PrimitiveType.Cube, "Ceiling", new Vector3(0f, 4.1f, 0f), new Vector3(12.6f, 0.2f, 10.6f), wallMat, t);
        // Dinding utara, timur, barat (tebal 0.3, tinggi 4).
        NewPrimitive(PrimitiveType.Cube, "WallN", new Vector3(0f, 2f, 5.15f), new Vector3(12.6f, 4f, 0.3f), wallMat, t);
        NewPrimitive(PrimitiveType.Cube, "WallE", new Vector3(6.15f, 2f, 0f), new Vector3(0.3f, 4f, 10.6f), wallMat, t);
        NewPrimitive(PrimitiveType.Cube, "WallW", new Vector3(-6.15f, 2f, 0f), new Vector3(0.3f, 4f, 10.6f), wallMat, t);
        // Dinding selatan dengan lubang pintu x di [3,5].
        NewPrimitive(PrimitiveType.Cube, "WallS1", new Vector3(-1.65f, 2f, -5.15f), new Vector3(9.3f, 4f, 0.3f), wallMat, t);
        NewPrimitive(PrimitiveType.Cube, "WallS2", new Vector3(5.65f, 2f, -5.15f), new Vector3(1.3f, 4f, 0.3f), wallMat, t);
        // Ambang atas pintu (pintu setinggi 3).
        NewPrimitive(PrimitiveType.Cube, "LintelS", new Vector3(4f, 3.5f, -5.15f), new Vector3(2f, 1f, 0.3f), wallMat, t);
    }

    // ---------- pintu keluar ----------

    static void BuildExit()
    {
        var exit = new GameObject("ExitDoor");
        exit.transform.position = new Vector3(4f, 0f, -5.15f);
        var panel = NewPrimitive(PrimitiveType.Cube, "DoorPanel", new Vector3(4f, 1.5f, -5.15f), new Vector3(2f, 3f, 0.2f), redMat, exit.transform);
        var point = new GameObject("ExitPoint");
        point.transform.SetParent(exit.transform, false);
        point.transform.position = new Vector3(4f, 1f, -6.2f);
        var comp = Undo.AddComponent<ExitDoor>(exit);
        comp.doorPanel = panel.transform;
        comp.exitPoint = point.transform;
        comp.panelRenderer = panel.GetComponent<Renderer>();
        comp.openOffset = new Vector3(2.2f, 0f, 0f);
    }

    // ---------- api ----------

    static void BuildFire(string name, Vector3 pos)
    {
        var fire = new GameObject(name);
        fire.transform.position = pos;
        // Tong sebagai badan api.
        NewPrimitive(PrimitiveType.Cylinder, "Barrel", pos + new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f), darkMat, fire.transform);
        // Visual api (diskala oleh FireSource mengikuti intensitas).
        var visual = new GameObject("FireVisual");
        visual.transform.SetParent(fire.transform, false);
        visual.transform.position = pos + new Vector3(0f, 1f, 0f);
        var ps = visual.AddComponent<ParticleSystem>();
        SetupFireParticles(ps);
        // Cahaya api.
        var lightGo = new GameObject("FireLight");
        lightGo.transform.SetParent(fire.transform, false);
        lightGo.transform.position = pos + new Vector3(0f, 1.8f, 0f);
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.5f, 0.1f);
        light.intensity = 2.5f;
        light.range = 8f;
        var src = Undo.AddComponent<FireSource>(fire);
        src.fireParticles = ps;
        src.fireLight = light;
        src.visualRoot = visual.transform;
    }

    static void SetupFireParticles(ParticleSystem ps)
    {
        var main = ps.main;
        main.duration = 1f;
        main.loop = true;
        main.startLifetime = 0.8f;
        main.startSpeed = 2.5f;
        main.startSize = 0.5f;
        main.startColor = new Color(1f, 0.45f, 0.05f);
        main.gravityModifier = -0.2f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 200;
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 60f;
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.3f;
        // Gradasi kuning ke merah transparan.
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] {
                new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0f),
                new GradientColorKey(new Color(1f, 0.3f, 0.05f), 0.6f),
                new GradientColorKey(new Color(0.3f, 0.05f, 0.02f), 1f)
            },
            new[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            });
        col.color = grad;
    }

    // ---------- APAR ----------

    static void BuildApar()
    {
        Vector3 basePos = new Vector3(1.5f, 0f, 2.5f);
        var apar = new GameObject("APAR");
        apar.transform.position = basePos;
        NewPrimitive(PrimitiveType.Cylinder, "Base", basePos + new Vector3(0f, 0.05f, 0f), new Vector3(0.7f, 0.1f, 0.7f), darkMat, apar.transform);
        NewPrimitive(PrimitiveType.Cylinder, "Body", basePos + new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 0.8f, 0.5f), redMat, apar.transform);
        NewPrimitive(PrimitiveType.Cylinder, "Top", basePos + new Vector3(0f, 0.95f, 0f), new Vector3(0.2f, 0.15f, 0.2f), darkMat, apar.transform);
        // Moncong semprot menghadap +z lokal.
        var tip = new GameObject("SprayTip");
        tip.transform.SetParent(apar.transform, false);
        tip.transform.localPosition = new Vector3(0f, 0.8f, 0.3f);
        tip.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        var spray = tip.AddComponent<ParticleSystem>();
        SetupSprayParticles(spray);
        var ext = Undo.AddComponent<FireExtinguisher>(apar);
        ext.sprayTip = tip.transform;
        ext.sprayParticles = spray;
    }

    static void SetupSprayParticles(ParticleSystem ps)
    {
        var main = ps.main;
        main.duration = 0.5f;
        main.loop = true;
        main.startLifetime = 0.5f;
        main.startSpeed = 8f;
        main.startSize = 0.15f;
        main.startColor = new Color(0.95f, 0.95f, 0.95f, 0.9f);
        main.gravityModifier = 0.3f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 300;
        var emission = ps.emission;
        emission.enabled = false;
        emission.rateOverTime = 150f;
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 12f;
        shape.radius = 0.08f;
    }

    // ---------- pemain ----------

    static void BuildPlayer()
    {
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 0f, 3.5f);
        player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        var cc = Undo.AddComponent<CharacterController>(player);
        cc.height = 1.8f;
        cc.radius = 0.4f;
        cc.center = new Vector3(0f, 0.9f, 0f);
        // Pakai Main Camera yang sudah ada sebagai kamera pemain.
        var camGo = GameObject.Find("Main Camera");
        Camera cam = camGo != null ? camGo.GetComponent<Camera>() : null;
        if (cam == null)
        {
            camGo = new GameObject("Main Camera");
            cam = camGo.AddComponent<Camera>();
            camGo.tag = "MainCamera";
        }
        cam.transform.SetParent(player.transform, false);
        cam.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        cam.transform.localRotation = Quaternion.identity;
        var mount = new GameObject("APARMount");
        mount.transform.SetParent(cam.transform, false);
        mount.transform.localPosition = new Vector3(0.35f, -0.5f, 0.7f);
        mount.transform.localRotation = Quaternion.identity;
        var ctrl = Undo.AddComponent<PlayerController>(player);
        ctrl.playerCamera = cam;
        var inter = Undo.AddComponent<PlayerInteractor>(player);
        inter.aparMount = mount.transform;
    }

    // ---------- misi + HUD ----------

    static void BuildMission()
    {
        var mm = new GameObject("MissionManager");
        Undo.AddComponent<MissionManager>(mm);

        var uiGo = new GameObject("MissionUI");
        var ui = Undo.AddComponent<MissionUI>(uiGo);

        var canvasGo = new GameObject("HUD");
        canvasGo.transform.SetParent(uiGo.transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        ui.timeText = AddHudText(canvasGo.transform, "TimeText", 28, TextAnchor.UpperLeft,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f));
        ui.firesText = AddHudText(canvasGo.transform, "FiresText", 28, TextAnchor.UpperLeft,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -60f));
        ui.scoreText = AddHudText(canvasGo.transform, "ScoreText", 28, TextAnchor.UpperLeft,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -100f));
        ui.centerText = AddHudText(canvasGo.transform, "CenterText", 56, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
        ui.centerText.fontStyle = FontStyle.Bold;
        ui.promptText = AddHudText(canvasGo.transform, "PromptText", 24, TextAnchor.LowerCenter,
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f));
        ui.UpdateHUD(60f, 3, 0);
    }

    static Text AddHudText(Transform parent, string name, int size, TextAnchor anchor,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos)
    {
        Text text = MissionUI.MakeText(name, parent, size, anchor);
        var rect = text.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(800f, 80f);
        if (name == "CenterText")
            rect.sizeDelta = new Vector2(1200f, 300f);
        return text;
    }
}
