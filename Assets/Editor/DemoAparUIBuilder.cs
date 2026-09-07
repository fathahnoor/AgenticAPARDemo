using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public static class DemoAparUIBuilder
{
    static readonly Color Panel = new Color(0.035f, 0.065f, 0.085f, 0.94f);
    static readonly Color Muted = new Color(0.62f, 0.72f, 0.75f);
    static readonly Color Mint = new Color(0.35f, 0.94f, 0.73f);

    public static MissionUI Build()
    {
        var root = new GameObject("MissionUI");
        var ui = root.AddComponent<MissionUI>();
        var canvas = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.transform.SetParent(root.transform, false);
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.GetComponent<Canvas>().sortingOrder = 100;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600, 900);
        scaler.matchWidthOrHeight = 0.5f;
        var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        events.transform.SetParent(root.transform, false);
        events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();

        ui.hud = Group("InGame", canvas.transform);
        Transform hud = ui.hud.transform;
        var mission = Box("MissionCard", hud, new Vector2(0, 1), new Vector2(28, -28), new Vector2(430, 132), Panel);
        Label("Brand", mission.transform, "APAR  /  RUANG LATIHAN 01", 18, new Vector2(22, -18), new Vector2(390, 24), Muted);
        ui.objectiveText = Label("Objective", mission.transform, "01   AMBIL APAR MERAH", 23, new Vector2(22, -53), new Vector2(390, 32), Color.white);
        ui.firesText = Label("Progress", mission.transform, "0 / 3 API PADAM", 17, new Vector2(22, -99), new Vector2(215, 22), Mint);
        ui.scoreText = Label("Score", mission.transform, "000 POIN", 17, new Vector2(270, -99), new Vector2(140, 22), Color.white);

        var timer = Box("TimerCard", hud, Vector2.one, new Vector2(-28, -28), new Vector2(172, 130), Panel);
        Label("Caption", timer.transform, "WAKTU TERSISA", 15, new Vector2(18, -17), new Vector2(145, 22), Muted);
        ui.timeText = Label("Time", timer.transform, "01:00", 43, new Vector2(18, -43), new Vector2(145, 54), Color.white);
        Box("TimeTrack", timer.transform, new Vector2(0, 1), new Vector2(18, -110), new Vector2(136, 4), new Color(0.2f, 0.28f, 0.3f));
        ui.timeFill = Box("TimeFill", timer.transform, new Vector2(0, 1), new Vector2(18, -110), new Vector2(136, 4), Mint);
        Fill(ui.timeFill);

        var prompt = Box("PromptCard", hud, new Vector2(0.5f, 0), new Vector2(0, 35), new Vector2(660, 58), Panel);
        ui.promptText = Label("Prompt", prompt.transform, "", 21, new Vector2(16, -14), new Vector2(628, 30), Color.white);
        ui.promptText.alignment = TextAnchor.MiddleCenter;
        var equipment = Box("EquipmentCard", hud, new Vector2(0, 0), new Vector2(28, 116), new Vector2(310, 65), Panel);
        ui.equipmentText = Label("Equipment", equipment.transform, "APAR / BELUM DIAMBIL", 17, new Vector2(16, -12), new Vector2(280, 24), Mint);
        Label("PauseHint", equipment.transform, "ESC  Jeda & kontrol", 15, new Vector2(16, -39), new Vector2(280, 21), Muted);

        ui.reticle = Box("AimDot", hud, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(5, 5), Color.white);
        for (int i = 0; i < 4; i++)
        {
            Vector2 offset = i < 2 ? new Vector2(i == 0 ? -13 : 13, 0) : new Vector2(0, i == 2 ? -13 : 13);
            Box("AimTick" + i, hud, new Vector2(0.5f, 0.5f), offset, i < 2 ? new Vector2(6, 2) : new Vector2(2, 6), new Color(1, 1, 1, 0.65f));
        }
        ui.targetPanel = Box("Target", hud, new Vector2(0.5f, 0.5f), new Vector2(0, -70), new Vector2(240, 48), Panel).gameObject;
        ui.targetText = Label("Intensity", ui.targetPanel.transform, "", 14, new Vector2(12, -8), new Vector2(220, 20), Color.white);
        ui.targetFill = Box("IntensityFill", ui.targetPanel.transform, new Vector2(0, 1), new Vector2(12, -34), new Vector2(216, 4), Mint);
        Fill(ui.targetFill);
        ui.centerText = Label("Notification", hud, "", 23, Vector2.zero, new Vector2(850, 45), Mint);
        Place(ui.centerText.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -190), new Vector2(850, 45));
        ui.centerText.alignment = TextAnchor.MiddleCenter;

        ui.overlay = Group("Overlay", canvas.transform);
        var dim = Box("Dim", ui.overlay.transform, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.012f, 0.027f, 0.035f, 0.72f));
        dim.rectTransform.anchorMin = Vector2.zero;
        dim.rectTransform.anchorMax = Vector2.one;
        dim.raycastTarget = true;
        var card = Box("BriefingCard", ui.overlay.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(740, 590), Panel);
        Box("Accent", card.transform, new Vector2(0, 1), Vector2.zero, new Vector2(740, 4), Mint);
        Label("Eyebrow", card.transform, "APAR   /   SIMULASI KEBAKARAN", 17, new Vector2(38, -32), new Vector2(665, 26), Mint);
        ui.overlayTitle = Label("Title", card.transform, "Siap menghadapi api?", 36, new Vector2(38, -80), new Vector2(665, 55), Color.white);
        ui.overlayTitle.fontStyle = FontStyle.Bold;
        ui.overlayBody = Label("Instructions", card.transform, "", 21, new Vector2(38, -155), new Vector2(665, 306), new Color(0.82f, 0.88f, 0.9f));
        ui.overlayBody.lineSpacing = 1.2f;
        ui.primaryButton = Button("Primary", card.transform, new Vector2(38, -476), new Vector2(460, 59), Mint, out ui.primaryLabel);
        ui.retryButton = Button("Retry", card.transform, new Vector2(514, -476), new Vector2(188, 59), new Color(0.2f, 0.3f, 0.33f), out var retryLabel);
        retryLabel.text = "ULANG [ R ]";
        retryLabel.color = Color.white;
        Label("Footnote", card.transform, "Demo interaktif untuk pembelajaran pengoperasian kontrol APAR.", 15, new Vector2(38, -552), new Vector2(665, 24), Muted);
        return ui;
    }

    static GameObject Group(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var r = (RectTransform)go.transform;
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.sizeDelta = Vector2.zero;
        return go;
    }

    static void Place(RectTransform r, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        r.anchorMin = r.anchorMax = r.pivot = anchor;
        r.anchoredPosition = pos;
        r.sizeDelta = size;
    }

    static Image Box(string name, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        Place(image.rectTransform, anchor, pos, size);
        return image;
    }

    static Text Label(string name, Transform parent, string value, int size, Vector2 pos, Vector2 dimensions, Color color)
    {
        var t = MissionUI.MakeText(name, parent, size, TextAnchor.UpperLeft);
        t.text = value;
        t.color = color;
        Place(t.rectTransform, new Vector2(0, 1), pos, dimensions);
        return t;
    }

    static void Fill(Image image)
    {
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = 0;
        // A built-in white sprite enables Image's filled mesh path.
        image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    static Button Button(string name, Transform parent, Vector2 pos, Vector2 size, Color color, out Text label)
    {
        var image = Box(name, parent, new Vector2(0, 1), pos, size, color);
        image.raycastTarget = true;
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors;
        colors.highlightedColor = new Color(0.85f, 1f, 0.94f);
        colors.pressedColor = new Color(0.55f, 0.8f, 0.7f);
        button.colors = colors;
        label = Label("Label", image.transform, "", 18, Vector2.zero, size, new Color(0.025f, 0.075f, 0.08f));
        label.alignment = TextAnchor.MiddleCenter;
        label.fontStyle = FontStyle.Bold;
        return button;
    }
}
