using UnityEngine;
using UnityEngine.UI;

// HUD sederhana: TIME, FIRES, SCORE di kiri atas, pesan tengah,
// dan prompt bawah. Dibangun via kode oleh builder, font Arial bawaan.
public class MissionUI : MonoBehaviour
{
    [Header("Refs (diisi builder)")]
    public Text timeText;
    public Text firesText;
    public Text scoreText;
    public Text centerText;
    public Text promptText;

    public void UpdateHUD(float timeLeft, int firesRemaining, int score)
    {
        if (timeText != null)
            timeText.text = "TIME: " + Mathf.CeilToInt(timeLeft);
        if (firesText != null)
            firesText.text = "FIRES: " + firesRemaining;
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }

    public void ShowPrompt(string message)
    {
        if (promptText != null)
            promptText.text = message ?? "";
    }

    // Pesan sementara seperti EXIT UNLOCKED.
    public void ShowCenterMessage(string message, float duration)
    {
        if (centerText == null)
            return;
        centerText.text = message;
        centerText.color = Color.yellow;
        CancelInvoke(nameof(ClearCenterMessage));
        Invoke(nameof(ClearCenterMessage), duration);
    }

    void ClearCenterMessage()
    {
        if (centerText != null)
            centerText.text = "";
    }

    // Pesan akhir permanen: COMPLETE hijau atau FAILED merah.
    public void ShowMissionEnd(bool complete, int finalScore)
    {
        if (centerText == null)
            return;
        CancelInvoke(nameof(ClearCenterMessage));
        if (complete)
        {
            centerText.text = "MISSION COMPLETE\nScore: " + finalScore;
            centerText.color = Color.green;
        }
        else
        {
            centerText.text = "MISSION FAILED\nScore: " + finalScore;
            centerText.color = Color.red;
        }
        ShowPrompt("");
    }

    public static Text MakeText(string name, Transform parent, int fontSize, TextAnchor anchor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = anchor;
        text.color = Color.white;
        text.raycastTarget = false;
        // Bayangan agar terbaca di atas api terang.
        var shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
        shadow.effectDistance = new Vector2(2f, -2f);
        return text;
    }
}
