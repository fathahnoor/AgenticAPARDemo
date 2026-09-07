using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Owned geometry and materials, persisted by Editor APIs and reused by the builder.
public static class DemoAparArt
{
    const string Root = "Assets/Art/Generated";
    public static Material Concrete, Wall, Steel, Red;
    static Material rubber, silver, brass, white, yellow, teal, green, glow, warmGlow, soot, flame, smoke;
    static Mesh bevel, cylinder, bottle, drum, torus;

    public static void Prepare()
    {
        Directory.CreateDirectory(Root);
        AssetDatabase.Refresh();
        Concrete = Lit("Concrete", new Color(0.34f, 0.38f, 0.39f), 0.05f, 0.26f);
        Wall = Lit("Plaster", new Color(0.65f, 0.68f, 0.66f), 0, 0.18f);
        Steel = Lit("PaintedSteel", new Color(0.21f, 0.27f, 0.29f), 0.4f, 0.42f);
        Red = Lit("EnamelRed", new Color(0.65f, 0.025f, 0.018f), 0.28f, 0.7f);
        rubber = Lit("Rubber", new Color(0.018f, 0.023f, 0.026f), 0, 0.24f);
        silver = Lit("BrushedMetal", new Color(0.55f, 0.59f, 0.61f), 0.85f, 0.62f);
        brass = Lit("Brass", new Color(0.56f, 0.34f, 0.09f), 0.75f, 0.65f);
        white = Lit("LabelWhite", new Color(0.92f, 0.9f, 0.8f), 0, 0.36f);
        yellow = Lit("SafetyYellow", new Color(0.95f, 0.58f, 0.055f), 0.12f, 0.4f);
        teal = Lit("BlueWallPanels", new Color(0.10f, 0.21f, 0.24f), 0.25f, 0.35f);
        green = Lit("ExitGreen", new Color(0.045f, 0.36f, 0.19f), 0.1f, 0.4f);
        glow = Lit("CoolLamp", new Color(0.75f, 0.9f, 1f), 0, 0.45f, new Color(1.6f, 2.1f, 2.4f));
        warmGlow = Lit("WarmLamp", new Color(1f, 0.81f, 0.5f), 0, 0.5f, new Color(3f, 2.2f, 1.2f));
        soot = Lit("Charcoal", new Color(0.03f, 0.023f, 0.015f), 0, 0.1f);
        var concreteTex = SurfaceTexture("ConcreteGrain", 256, false);
        Concrete.SetTexture("_BaseMap", concreteTex);
        Concrete.SetTextureScale("_BaseMap", new Vector2(5, 5));
        Wall.SetTexture("_BaseMap", concreteTex);
        Wall.SetTextureScale("_BaseMap", new Vector2(3, 2));
        var metalTex = SurfaceTexture("SteelGrain", 256, true);
        Steel.SetTexture("_BaseMap", metalTex);
        silver.SetTexture("_BaseMap", metalTex);
        var normal = NormalTexture();
        Concrete.SetTexture("_BumpMap", normal);
        Concrete.SetFloat("_BumpScale", 0.25f);
        Concrete.EnableKeyword("_NORMALMAP");
        flame = ParticleMaterial("Flame", true);
        smoke = ParticleMaterial("Smoke", false);
        bevel = BevelMesh();
        cylinder = Lathe("Cylinder", new[] { new Vector2(0, -0.5f), new Vector2(0.48f, -0.5f), new Vector2(0.5f, -0.47f), new Vector2(0.5f, 0.47f), new Vector2(0.48f, 0.5f), new Vector2(0, 0.5f) });
        bottle = Lathe("ExtinguisherShell", new[] { new Vector2(0, 0.035f), new Vector2(0.13f, 0.035f), new Vector2(0.17f, 0.065f), new Vector2(0.18f, 0.11f), new Vector2(0.18f, 0.57f), new Vector2(0.175f, 0.61f), new Vector2(0.15f, 0.66f), new Vector2(0.09f, 0.7f), new Vector2(0.055f, 0.71f), new Vector2(0.055f, 0.76f), new Vector2(0, 0.76f) });
        drum = Lathe("TrainingDrum", new[] { new Vector2(0, 0.04f), new Vector2(0.44f, 0.04f), new Vector2(0.47f, 0.08f), new Vector2(0.47f, 0.2f), new Vector2(0.48f, 0.23f), new Vector2(0.48f, 0.26f), new Vector2(0.46f, 0.29f), new Vector2(0.46f, 0.7f), new Vector2(0.48f, 0.73f), new Vector2(0.48f, 0.77f), new Vector2(0.46f, 0.8f), new Vector2(0.46f, 0.99f), new Vector2(0.49f, 1.02f), new Vector2(0.49f, 1.06f), new Vector2(0.43f, 1.06f), new Vector2(0.43f, 0.98f), new Vector2(0, 0.98f) });
        var ring = new Vector2[17];
        for (int i = 0; i < ring.Length; i++)
        {
            float a = i * Mathf.PI * 2 / 16;
            ring[i] = new Vector2(0.48f + Mathf.Cos(a) * 0.02f, Mathf.Sin(a) * 0.02f);
        }
        torus = Lathe("Rim", ring);
        AssetDatabase.SaveAssets();
    }

    static Material Lit(string name, Color color, float metal, float smooth, Color emission = default)
    {
        var m = AssetDatabase.LoadAssetAtPath<Material>(Root + "/" + name + ".mat");
        if (m == null) { m = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(m, Root + "/" + name + ".mat"); }
        m.color = color;
        m.SetFloat("_Metallic", metal);
        m.SetFloat("_Smoothness", smooth);
        if (emission.maxColorComponent > 0)
        {
            m.SetColor("_EmissionColor", emission);
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            m.EnableKeyword("_EMISSION");
        }
        EditorUtility.SetDirty(m);
        return m;
    }

    static Texture2D SurfaceTexture(string name, int size, bool metal)
    {
        string path = Root + "/" + name + ".png";
        var existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (existing != null) return existing;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        var colors = new Color[size * size];
        var random = new System.Random(831);
        for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
        {
            float noise = Mathf.PerlinNoise(x / 31f, y / 31f) * 0.16f + (float)random.NextDouble() * 0.11f;
            float v = metal ? 0.8f + Mathf.PerlinNoise(x * 0.8f, y * 0.01f) * 0.2f : 0.72f + noise;
            colors[y * size + x] = new Color(v, v, v, 1);
        }
        tex.SetPixels(colors); tex.Apply(); File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        Object.DestroyImmediate(tex);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Texture2D NormalTexture()
    {
        string path = Root + "/ConcreteNormal.png";
        var existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (existing != null) return existing;
        var tex = new Texture2D(256, 256);
        for (int y = 0; y < 256; y++) for (int x = 0; x < 256; x++)
        {
            float dx = Mathf.PerlinNoise((x + 1) * 0.35f, y * 0.35f) - Mathf.PerlinNoise((x - 1) * 0.35f, y * 0.35f);
            float dy = Mathf.PerlinNoise(x * 0.35f, (y + 1) * 0.35f) - Mathf.PerlinNoise(x * 0.35f, (y - 1) * 0.35f);
            Vector3 n = new Vector3(-dx, -dy, 1).normalized;
            tex.SetPixel(x, y, new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f));
        }
        tex.Apply(); File.WriteAllBytes(path, tex.EncodeToPNG()); AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.NormalMap;
        importer.SaveAndReimport(); Object.DestroyImmediate(tex);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Material ParticleMaterial(string name, bool additive)
    {
        string texPath = Root + "/ParticleSoft.png";
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        if (texture == null)
        {
            var t = new Texture2D(128, 128);
            for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
            {
                float radius = new Vector2((x - 63.5f) / 63.5f, (y - 63.5f) / 63.5f).magnitude;
                float alpha = Mathf.Pow(Mathf.Clamp01(1 - radius), 1.8f) * (0.7f + 0.3f * Mathf.PerlinNoise(x * 0.1f, y * 0.1f));
                t.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
            t.Apply(); File.WriteAllBytes(texPath, t.EncodeToPNG()); AssetDatabase.ImportAsset(texPath); Object.DestroyImmediate(t);
            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        }
        string path = Root + "/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null) { m = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")); AssetDatabase.CreateAsset(m, path); }
        m.SetTexture("_BaseMap", texture); m.SetColor("_BaseColor", Color.white);
        m.SetFloat("_Surface", 1); m.SetFloat("_Blend", additive ? 2 : 0);
        m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        m.SetFloat("_DstBlend", (float)(additive ? BlendMode.One : BlendMode.OneMinusSrcAlpha));
        m.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
        m.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
        m.SetFloat("_ZWrite", 0); m.SetFloat("_Cull", 0);
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.SetOverrideTag("RenderType", "Transparent"); m.renderQueue = 3000;
        EditorUtility.SetDirty(m);
        return m;
    }

    static Mesh Lathe(string name, Vector2[] profile)
    {
        string path = Root + "/" + name + ".asset";
        var old = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (old != null) return old;
        const int sides = 64;
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        for (int j = 0; j < profile.Length; j++) for (int i = 0; i <= sides; i++)
        {
            float a = i * Mathf.PI * 2 / sides;
            vertices.Add(new Vector3(Mathf.Sin(a) * profile[j].x, profile[j].y, Mathf.Cos(a) * profile[j].x));
            uv.Add(new Vector2((float)i / sides, (float)j / (profile.Length - 1)));
            if (i < sides && j < profile.Length - 1)
            {
                int n = j * (sides + 1) + i;
                triangles.AddRange(new[] { n, n + 1, n + sides + 1, n + 1, n + sides + 2, n + sides + 1 });
            }
        }
        var mesh = new Mesh { name = name };
        mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0); mesh.RecalculateNormals(); mesh.RecalculateTangents();
        AssetDatabase.CreateAsset(mesh, path); return mesh;
    }

    static Mesh BevelMesh()
    {
        string path = Root + "/BeveledBox.asset";
        var old = AssetDatabase.LoadAssetAtPath<Mesh>(path); if (old != null) return old;
        var v = new List<Vector3>(); var normals = new List<Vector3>(); var uv = new List<Vector2>(); var tri = new List<int>();
        float[] steps = { -0.5f, -0.46f, 0.46f, 0.5f };
        Vector3[] faces = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        foreach (var normal in faces)
        {
            Vector3 axis = Mathf.Abs(normal.y) > 0.5f ? Vector3.forward : Vector3.up;
            Vector3 right = Vector3.Cross(axis, normal);
            int start = v.Count;
            for (int y = 0; y < 4; y++) for (int x = 0; x < 4; x++)
            {
                Vector3 p = normal * 0.5f + right * steps[x] + axis * steps[y];
                Vector3 core = new Vector3(Mathf.Clamp(p.x, -0.46f, 0.46f), Mathf.Clamp(p.y, -0.46f, 0.46f), Mathf.Clamp(p.z, -0.46f, 0.46f));
                Vector3 n = (p - core).normalized;
                v.Add(core + n * 0.04f); normals.Add(n); uv.Add(new Vector2(steps[x] + 0.5f, steps[y] + 0.5f));
                if (x < 3 && y < 3) { int i = start + y * 4 + x; tri.AddRange(new[] { i, i + 1, i + 4, i + 1, i + 5, i + 4 }); }
            }
        }
        var mesh = new Mesh { name = "BeveledBox" };
        mesh.SetVertices(v); mesh.SetNormals(normals); mesh.SetUVs(0, uv); mesh.SetTriangles(tri, 0); mesh.RecalculateTangents();
        AssetDatabase.CreateAsset(mesh, path); return mesh;
    }

    static Transform Model(string name, Transform parent, Mesh mesh, Vector3 position, Vector3 scale, Material material)
    {
        var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer)); go.transform.SetParent(parent, false);
        go.transform.localPosition = position; go.transform.localScale = scale;
        go.GetComponent<MeshFilter>().sharedMesh = mesh; go.GetComponent<MeshRenderer>().sharedMaterial = material;
        return go.transform;
    }

    static Transform Box(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool solid = false)
    {
        var t = Model(name, parent, bevel, position, scale, material);
        if (solid) t.gameObject.AddComponent<BoxCollider>();
        return t;
    }

    static Transform Tube(string name, Transform parent, Vector3 a, Vector3 b, float diameter, Material material)
    {
        var t = Model(name, parent, cylinder, (a + b) * 0.5f, new Vector3(diameter, (b - a).magnitude, diameter), material);
        t.localRotation = Quaternion.FromToRotation(Vector3.up, b - a); return t;
    }

    static void Text(string name, Transform parent, string text, Vector3 position, float size, Color color, Quaternion rotation)
    {
        // World-space uGUI respects scene depth, so signs cannot show through a closed door.
        var go = new GameObject(name, typeof(RectTransform), typeof(Canvas));
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localRotation = rotation * Quaternion.Euler(0, 180, 0);
        go.transform.localScale = Vector3.one * size * 0.04f;
        go.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        var rect = (RectTransform)go.transform;
        rect.sizeDelta = new Vector2(1800, 360);
        var label = MissionUI.MakeText("Label", go.transform, 80, TextAnchor.MiddleCenter);
        label.text = text;
        label.color = color;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.sizeDelta = Vector2.zero;
    }
    public static void DressRoom(Transform room)
    {
        // Floor joints and a painted walking route remain flat and collision-free.
        for (int x = -5; x <= 5; x += 2) Box("FloorJointX", room, new Vector3(x, 0.005f, 0), new Vector3(0.018f, 0.006f, 10), Steel);
        for (int z = -4; z <= 4; z += 2) Box("FloorJointZ", room, new Vector3(0, 0.006f, z), new Vector3(12, 0.006f, 0.018f), Steel);
        for (int side = -1; side <= 1; side += 2)
        {
            Box("LowerWall", room, new Vector3(side * 5.98f, 0.65f, 0), new Vector3(0.08f, 1.3f, 10), teal);
            Box("WallRail", room, new Vector3(side * 5.9f, 1.35f, 0), new Vector3(0.1f, 0.045f, 10), silver);
            Box("Skirting", room, new Vector3(side * 5.9f, 0.09f, 0), new Vector3(0.12f, 0.18f, 10), rubber);
            for (int z = -4; z <= 4; z += 2)
                Box("WallPost", room, new Vector3(side * 5.92f, 2, z), new Vector3(0.12f, 4, 0.1f), Steel);
        }
        Box("SouthBluePanel", room, new Vector3(-1.55f, 0.65f, -4.97f), new Vector3(9.1f, 1.3f, 0.07f), teal);
        Box("SouthRail", room, new Vector3(-1.55f, 1.35f, -4.9f), new Vector3(9.1f, 0.04f, 0.05f), silver);
        Box("NorthPanel", room, new Vector3(0, 0.65f, 4.96f), new Vector3(12, 1.3f, 0.08f), teal);
        for (int z = -4; z <= 4; z += 4)
        {
            Box("CeilingBeam", room, new Vector3(0, 3.84f, z), new Vector3(12, 0.25f, 0.15f), Steel);
            Box("BeamFlange", room, new Vector3(0, 3.7f, z), new Vector3(12, 0.05f, 0.26f), silver);
        }
        Tube("FireMainPipe", room, new Vector3(-5.3f, 3.55f, -4.9f), new Vector3(-5.3f, 3.55f, 4.9f), 0.12f, Red);
        for (int z = -4; z <= 4; z += 2)
        {
            Tube("PipeBracket", room, new Vector3(-5.3f, 3.55f, z), new Vector3(-5.3f, 4, z), 0.035f, silver);
            Tube("SprinklerDrop", room, new Vector3(-5.3f, 3.5f, z), new Vector3(-5.3f, 3.32f, z), 0.045f, brass);
            Model("SprinklerHead", room, cylinder, new Vector3(-5.3f, 3.32f, z), new Vector3(0.12f, 0.022f, 0.12f), silver);
        }
        Box("VentilationDuct", room, new Vector3(2.8f, 3.64f, 0), new Vector3(0.65f, 0.45f, 10), silver);
        for (int z = -4; z <= 4; z += 2)
        {
            Box("DuctSeam", room, new Vector3(2.8f, 3.64f, z), new Vector3(0.69f, 0.49f, 0.045f), Steel);
            for (int j = 0; j < 5; j++) Box("VentSlat", room, new Vector3(2.8f, 3.4f, z + j * 0.1f - 0.2f), new Vector3(0.42f, 0.014f, 0.035f), rubber);
        }
        // Illuminated side windows with mullions.
        for (int z = -2; z <= 2; z += 4)
        {
            Box("WindowFrame", room, new Vector3(5.87f, 2.55f, z), new Vector3(0.18f, 1.6f, 2.8f), Steel);
            Box("FrostedWindow", room, new Vector3(5.76f, 2.55f, z), new Vector3(0.025f, 1.4f, 2.6f), glow);
            for (int i = -1; i <= 1; i++) Box("Mullion", room, new Vector3(5.73f, 2.55f, z + i * 0.82f), new Vector3(0.04f, 1.4f, 0.04f), silver);
        }
        Box("TrainingSign", room, new Vector3(-1.6f, 2.6f, -4.88f), new Vector3(5.5f, 1.28f, 0.08f), Steel);
        Text("TrainingTitle", room, "PUSAT LATIHAN APAR", new Vector3(-1.6f, 2.82f, -4.81f), 0.115f, new Color(0.85f, 0.94f, 0.96f), Quaternion.Euler(0, 0, 0));
        Text("TrainingSubtitle", room, "RUANG 01   /   PEMADAMAN API AWAL", new Vector3(-1.6f, 2.4f, -4.81f), 0.05f, new Color(0.42f, 0.88f, 0.74f), Quaternion.identity);
        // Exit lane leaves the fire and equipment bays unobstructed.
        Box("ExitLane", room, new Vector3(4, 0.011f, -3.5f), new Vector3(1.8f, 0.009f, 3), green);
        for (int z = -4; z <= -2; z++)
        {
            var a = Box("RouteArrowL", room, new Vector3(3.86f, 0.022f, z), new Vector3(0.08f, 0.006f, 0.4f), white); a.localRotation = Quaternion.Euler(0, -40, 0);
            var b = Box("RouteArrowR", room, new Vector3(4.14f, 0.022f, z), new Vector3(0.08f, 0.006f, 0.4f), white); b.localRotation = Quaternion.Euler(0, 40, 0);
        }
        // Equipment bay, cabinet and ventilation grilles add scale without blocking the route.
        Box("EquipmentPad", room, new Vector3(-1.3f, 0.015f, 1.1f), new Vector3(1.2f, 0.02f, 1.25f), yellow);
        Box("EquipmentPlinth", room, new Vector3(-1.3f, 0.18f, 1.1f), new Vector3(0.7f, 0.34f, 0.7f), Steel, true);
        Box("EquipmentSignPost", room, new Vector3(-1.3f, 0.85f, 0.65f), new Vector3(0.05f, 1.7f, 0.05f), silver);
        Box("EquipmentSign", room, new Vector3(-1.3f, 1.55f, 0.65f), new Vector3(1.05f, 0.36f, 0.055f), Red);
        Text("EquipmentCaption", room, "APAR / ABC", new Vector3(-1.3f, 1.55f, 0.69f), 0.044f, Color.white, Quaternion.identity);
        Box("Cabinet", room, new Vector3(-5.45f, 0.9f, 3.8f), new Vector3(0.75f, 1.8f, 1.5f), Steel, true);
        for (int i = 0; i < 2; i++)
        {
            Box("CabinetDoor", room, new Vector3(-5.04f, 0.94f, 3.42f + i * 0.76f), new Vector3(0.04f, 1.66f, 0.69f), teal);
            Tube("Handle", room, new Vector3(-4.98f, 0.8f, 3.42f + i * 0.76f), new Vector3(-4.98f, 1.05f, 3.42f + i * 0.76f), 0.025f, silver);
        }
        Box("ExitLanding", room, new Vector3(4, -0.1f, -6), new Vector3(2.3f, 0.2f, 2.4f), Concrete, true);
        Box("ExitBackdrop", room, new Vector3(4, 1.65f, -7.15f), new Vector3(2.5f, 3.3f, 0.15f), teal);
        Text("SafeArea", room, "AREA AMAN", new Vector3(4, 1.8f, -7.04f), 0.11f, Color.white, Quaternion.identity);
    }

    public static void DressExtinguisher(FireExtinguisher apar)
    {
        foreach (var r in apar.GetComponentsInChildren<Renderer>()) if (!(r is ParticleSystemRenderer)) r.enabled = false;
        foreach (var c in apar.GetComponentsInChildren<Collider>()) c.enabled = false;
        var t = new GameObject("DetailedExtinguisher").transform; t.SetParent(apar.transform, false);
        Model("EnamelCylinder", t, bottle, Vector3.zero, Vector3.one, Red);
        Model("RubberFoot", t, cylinder, new Vector3(0, 0.042f, 0), new Vector3(0.36f, 0.065f, 0.36f), rubber);
        Model("Valve", t, cylinder, new Vector3(0, 0.78f, 0), new Vector3(0.1f, 0.12f, 0.1f), brass);
        Box("FixedHandle", t, new Vector3(0.055f, 0.875f, 0), new Vector3(0.29f, 0.035f, 0.07f), rubber);
        var lever = Box("SqueezeLever", t, new Vector3(0.045f, 0.92f, 0), new Vector3(0.28f, 0.025f, 0.065f), Red); lever.localRotation = Quaternion.Euler(0, 0, 8);
        var gauge = Model("PressureGauge", t, cylinder, new Vector3(0, 0.805f, -0.09f), new Vector3(0.12f, 0.035f, 0.12f), silver); gauge.localRotation = Quaternion.Euler(90, 0, 0);
        var dial = Model("GaugeFace", t, cylinder, new Vector3(0, 0.805f, -0.112f), new Vector3(0.098f, 0.006f, 0.098f), white); dial.localRotation = Quaternion.Euler(90, 0, 0);
        Tube("GaugeNeedle", t, new Vector3(0, 0.805f, -0.117f), new Vector3(0.023f, 0.83f, -0.117f), 0.006f, green);
        var pin = Model("SafetyPinRing", t, torus, new Vector3(-0.15f, 0.83f, 0), Vector3.one * 0.075f, brass); pin.localRotation = Quaternion.Euler(90, 0, 0);
        Tube("SafetyPin", t, new Vector3(-0.12f, 0.83f, 0), new Vector3(0.05f, 0.83f, 0), 0.013f, silver);
        Vector3[] hose = { new Vector3(0.065f, 0.79f, 0.03f), new Vector3(0.19f, 0.77f, 0.025f), new Vector3(0.27f, 0.64f, 0.015f), new Vector3(0.28f, 0.37f, 0.02f), new Vector3(0.27f, 0.18f, 0.08f), new Vector3(0.25f, 0.18f, 0.2f), new Vector3(0.22f, 0.31f, 0.3f), new Vector3(0.18f, 0.59f, 0.33f) };
        for (int i = 0; i < hose.Length - 1; i++) Tube("HoseSegment", t, hose[i], hose[i + 1], 0.045f, rubber);
        Tube("Nozzle", t, new Vector3(0.18f, 0.59f, 0.33f), new Vector3(0.18f, 0.62f, 0.51f), 0.073f, rubber);
        Tube("NozzleRim", t, new Vector3(0.18f, 0.62f, 0.49f), new Vector3(0.18f, 0.623f, 0.515f), 0.076f, silver);
        Box("FrontLabel", t, new Vector3(0, 0.39f, -0.174f), new Vector3(0.245f, 0.31f, 0.014f), white);
        Text("LabelAPAR", t, "APAR\nABC\n6 kg", new Vector3(0, 0.43f, -0.185f), 0.021f, new Color(0.12f, 0.1f, 0.07f), Quaternion.Euler(0, 180, 0));
        for (int i = 0; i < 5; i++) Box("InstructionLine", t, new Vector3(0, 0.28f + i * 0.012f, -0.185f), new Vector3(0.16f, 0.004f, 0.002f), Steel);
        apar.transform.position = new Vector3(-1.3f, 0.35f, 1.1f);
        apar.transform.rotation = Quaternion.Euler(0, 155, 0);
        apar.sprayTip.localPosition = new Vector3(0.18f, 0.623f, 0.53f);
        apar.sprayTip.localRotation = Quaternion.identity;
        var ps = apar.sprayParticles;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main; main.startLifetime = 0.55f; main.startSpeed = 12f; main.startSize = new ParticleSystem.MinMaxCurve(0.11f, 0.25f); main.startColor = new Color(0.92f, 0.97f, 1, 0.38f);
        var size = ps.sizeOverLifetime; size.enabled = true; size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, 0.3f, 1, 2.5f));
        ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = smoke;
    }

    public static void DressFire(FireSource fire, int index)
    {
        var t = fire.transform;
        t.Find("Barrel").GetComponent<Renderer>().enabled = false;
        var collider = t.Find("Barrel").GetComponent<Collider>(); collider.enabled = false;
        var body = Model("RibbedSteelDrum", t, drum, Vector3.zero, Vector3.one, Steel);
        var bc = body.gameObject.AddComponent<CapsuleCollider>(); bc.radius = 0.48f; bc.height = 1.05f; bc.center = new Vector3(0, 0.525f, 0);
        Model("UpperRolledRim", t, torus, new Vector3(0, 1.05f, 0), Vector3.one, silver);
        Model("LowerRim", t, torus, new Vector3(0, 0.075f, 0), Vector3.one * 0.97f, silver);
        Model("SootBed", t, cylinder, new Vector3(0, 0.995f, 0), new Vector3(0.86f, 0.025f, 0.86f), soot);
        for (int i = 0; i < 5; i++)
        {
            var log = Box("CharredFuel", t, new Vector3((i % 2 - 0.5f) * 0.28f, 1.02f, (i / 2 - 1) * 0.16f), new Vector3(0.14f, 0.08f, 0.5f), soot);
            log.localRotation = Quaternion.Euler(0, i * 38, 0);
        }
        Box("DrumLabel", t, new Vector3(0, 0.52f, 0.459f), new Vector3(0.3f, 0.24f, 0.018f), yellow);
        Text("DrumNumber", t, "0" + index, new Vector3(0, 0.53f, 0.475f), 0.065f, new Color(0.07f, 0.08f, 0.06f), Quaternion.identity);
        // Individual metal catch trays anchor the barrels to the floor.
        Box("CatchTray", t, new Vector3(0, 0.012f, 0), new Vector3(1.4f, 0.025f, 1.4f), Steel);
        for (int i = -1; i <= 1; i += 2)
        {
            Box("TrayEdge", t, new Vector3(i * 0.67f, 0.055f, 0), new Vector3(0.025f, 0.1f, 1.4f), yellow);
            Box("TrayEdge", t, new Vector3(0, 0.055f, i * 0.67f), new Vector3(1.4f, 0.1f, 0.025f), yellow);
        }
        var ps = fire.fireParticles; ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        var main = ps.main; main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1.05f); main.startSpeed = new ParticleSystem.MinMaxCurve(0.7f, 1.8f); main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.7f); main.startColor = new Color(2.4f, 1.5f, 0.5f, 0.9f); main.prewarm = true;
        var noise = ps.noise; noise.enabled = true; noise.strength = 0.25f; noise.frequency = 1.5f; noise.scrollSpeed = 0.6f;
        var size = ps.sizeOverLifetime; size.enabled = true; size.size = new ParticleSystem.MinMaxCurve(1, new AnimationCurve(new Keyframe(0, 0.65f), new Keyframe(0.2f, 1), new Keyframe(1, 0)));
        ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = flame;
        var smokeGo = new GameObject("Smoke", typeof(ParticleSystem)); smokeGo.transform.SetParent(t, false); smokeGo.transform.localPosition = new Vector3(0, 1.7f, 0); smokeGo.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        var sp = smokeGo.GetComponent<ParticleSystem>(); sp.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var sm = sp.main; sm.startLifetime = new ParticleSystem.MinMaxCurve(1.6f, 2.8f); sm.startSpeed = 0.65f; sm.startSize = new ParticleSystem.MinMaxCurve(0.55f, 1f); sm.startColor = new Color(0.12f, 0.13f, 0.14f, 0.28f); sm.simulationSpace = ParticleSystemSimulationSpace.World; sm.maxParticles = 80; sm.prewarm = true;
        var em = sp.emission; em.rateOverTime = 12;
        var shape = sp.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 14; shape.radius = 0.25f;
        var ss = sp.sizeOverLifetime; ss.enabled = true; ss.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, 0.5f, 1, 2));
        var color = sp.colorOverLifetime; color.enabled = true; var gradient = new Gradient(); gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(0.8f, 0.15f), new GradientAlphaKey(0, 1) }); color.color = gradient;
        sp.GetComponent<ParticleSystemRenderer>().sharedMaterial = smoke;
        fire.smokeParticles = sp;
        fire.fireLight.intensity = 4f; fire.fireLight.range = 4.5f;
    }

    public static void DressExit(ExitDoor exit)
    {
        Transform panel = exit.doorPanel;
        panel.GetComponent<Renderer>().sharedMaterial = green;
        exit.lockedColor = new Color(0.07f, 0.23f, 0.2f);
        exit.unlockedColor = new Color(0.12f, 0.5f, 0.27f);
        var room = exit.transform;
        Box("LeftFrame", room, new Vector3(-1.1f, 1.5f, 0.1f), new Vector3(0.15f, 3.15f, 0.25f), silver);
        Box("RightFrame", room, new Vector3(1.1f, 1.5f, 0.1f), new Vector3(0.15f, 3.15f, 0.25f), silver);
        Box("ExitSign", room, new Vector3(0, 3.3f, 0.2f), new Vector3(2.05f, 0.42f, 0.12f), green);
        Text("ExitLabel", room, "KELUAR  >", new Vector3(0, 3.3f, 0.28f), 0.115f, new Color(0.7f, 1f, 0.8f), Quaternion.identity);
        // Child transforms are normalized so details retain real-world proportions on the scaled door.
        var detail = new GameObject("DoorDetails").transform; detail.SetParent(panel, false); detail.localScale = new Vector3(0.5f, 1f / 3f, 5);
        Box("InspectionWindowFrame", detail, new Vector3(0, 0.65f, 0.16f), new Vector3(0.52f, 0.7f, 0.08f), silver);
        Box("InspectionWindow", detail, new Vector3(0, 0.65f, 0.21f), new Vector3(0.4f, 0.58f, 0.03f), rubber);
        Box("PanicBar", detail, new Vector3(0, -0.3f, 0.25f), new Vector3(1.45f, 0.09f, 0.09f), silver);
        Box("KickPlate", detail, new Vector3(0, -1.18f, 0.16f), new Vector3(1.75f, 0.42f, 0.025f), silver);
    }

    public static void Lighting(Transform room, Camera camera)
    {
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.48f, 0.56f, 0.63f);
        RenderSettings.ambientEquatorColor = new Color(0.36f, 0.42f, 0.44f);
        RenderSettings.ambientGroundColor = new Color(0.18f, 0.2f, 0.22f);
        RenderSettings.fog = true; RenderSettings.fogMode = FogMode.ExponentialSquared; RenderSettings.fogDensity = 0.014f; RenderSettings.fogColor = new Color(0.21f, 0.25f, 0.28f);
        var sun = GameObject.Find("Directional Light").GetComponent<Light>(); sun.intensity = 0.6f; sun.color = new Color(0.73f, 0.86f, 1f); sun.shadows = LightShadows.Soft;
        for (int x = -3; x <= 3; x += 6) for (int z = -3; z <= 3; z += 3)
        {
            Box("LuminaireHousing", room, new Vector3(x, 3.73f, z), new Vector3(1.8f, 0.13f, 0.42f), Steel);
            Box("Diffuser", room, new Vector3(x, 3.65f, z), new Vector3(1.62f, 0.025f, 0.3f), x < 0 ? warmGlow : glow);
            var lightGo = new GameObject("PracticalLight", typeof(Light)); lightGo.transform.SetParent(room, false); lightGo.transform.localPosition = new Vector3(x, 3.52f, z); lightGo.transform.localRotation = Quaternion.Euler(90, 0, 0);
            var light = lightGo.GetComponent<Light>(); light.type = LightType.Spot; light.spotAngle = 118; light.innerSpotAngle = 85; light.range = 9; light.intensity = 18;
            light.color = x < 0 ? new Color(1f, 0.84f, 0.63f) : new Color(0.68f, 0.84f, 1f);
            light.shadows = z == 0 ? LightShadows.Soft : LightShadows.None;
        }
        camera.fieldOfView = 68; camera.nearClipPlane = 0.08f;
        var cameraData = camera.GetUniversalAdditionalCameraData();
        cameraData.renderPostProcessing = true;
        cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        cameraData.antialiasingQuality = AntialiasingQuality.High;
        var volume = Object.FindAnyObjectByType<Volume>();
        string path = Root + "/TrainingVolume.asset";
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
        if (profile == null) { profile = ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(profile, path); }
        if (!profile.TryGet<Bloom>(out var bloom)) { bloom = profile.Add<Bloom>(); AssetDatabase.AddObjectToAsset(bloom, profile); }
        bloom.intensity.Override(0.22f); bloom.threshold.Override(1.1f); bloom.scatter.Override(0.55f);
        if (!profile.TryGet<Tonemapping>(out var tone)) { tone = profile.Add<Tonemapping>(); AssetDatabase.AddObjectToAsset(tone, profile); }
        tone.mode.Override(TonemappingMode.ACES);
        if (!profile.TryGet<ColorAdjustments>(out var grade)) { grade = profile.Add<ColorAdjustments>(); AssetDatabase.AddObjectToAsset(grade, profile); }
        grade.postExposure.Override(0.6f); grade.contrast.Override(8); grade.saturation.Override(-5);
        volume.sharedProfile = profile; EditorUtility.SetDirty(profile);
        var probeGo = new GameObject("RoomReflection", typeof(ReflectionProbe)); probeGo.transform.SetParent(room, false); probeGo.transform.localPosition = new Vector3(0, 1.8f, 0);
        var probe = probeGo.GetComponent<ReflectionProbe>(); probe.mode = ReflectionProbeMode.Realtime; probe.refreshMode = ReflectionProbeRefreshMode.OnAwake; probe.resolution = 128; probe.size = new Vector3(12, 4, 10); probe.boxProjection = true; probe.intensity = 0.8f;
        AssetDatabase.SaveAssets();
    }
}
