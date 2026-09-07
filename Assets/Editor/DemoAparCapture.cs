using System.IO;
using UnityEngine;

// Native-resolution evidence including HUD, rendered through the active URP camera.
public static class DemoAparCapture
{
    public static string Capture(string path, int width = 1600, int height = 900)
    {
        Camera camera = Camera.main;
        Canvas canvas = MissionManager.Instance.ui.GetComponentInChildren<Canvas>();
        var oldMode = canvas.renderMode;
        var oldCamera = canvas.worldCamera;
        float oldPlane = canvas.planeDistance;
        var oldTarget = camera.targetTexture;
        var oldActive = RenderTexture.active;
        var target = new RenderTexture(width, height, 24);
        var image = new Texture2D(width, height, TextureFormat.RGB24, false);
        try
        {
            camera.targetTexture = target;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = camera.nearClipPlane + 0.1f;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, image.EncodeToPNG());
            return Path.GetFullPath(path);
        }
        finally
        {
            canvas.renderMode = oldMode;
            canvas.worldCamera = oldCamera;
            canvas.planeDistance = oldPlane;
            camera.targetTexture = oldTarget;
            RenderTexture.active = oldActive;
            target.Release();
            Object.DestroyImmediate(target);
            Object.DestroyImmediate(image);
            Canvas.ForceUpdateCanvases();
        }
    }
}
