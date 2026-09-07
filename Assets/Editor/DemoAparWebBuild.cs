using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class DemoAparWebBuild
{
    public static string Status { get; private set; } = "Idle";

    [MenuItem("DemoAPAR/Configure WebGL")]
    public static void Configure()
    {
        PlayerSettings.companyName = "Fathah Noor Prawita";
        PlayerSettings.productName = "Latihan APAR";
        PlayerSettings.defaultWebScreenWidth = 1280;
        PlayerSettings.defaultWebScreenHeight = 720;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.nameFilesAsHashes = false;
        PlayerSettings.WebGL.template = "PROJECT:APAR";
        PlayerSettings.WebGL.threadsSupport = false;
        PlayerSettings.WebGL.initialMemorySize = 128;
        PlayerSettings.WebGL.maximumMemorySize = 512;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new[] { GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/DemoAPAR.unity", true) };

        const string pipelinePath = "Assets/Settings/Web_RPAsset.asset";
        const string rendererPath = "Assets/Settings/Web_Renderer.asset";
        var web = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
        if (web == null)
        {
            web = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/PC_RPAsset.asset"));
            web.name = "Web_RPAsset";
            AssetDatabase.CreateAsset(web, pipelinePath);
        }
        var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
        if (renderer == null)
        {
            renderer = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/PC_Renderer.asset"));
            renderer.name = "Web_Renderer";
            renderer.rendererFeatures.Clear();
            AssetDatabase.CreateAsset(renderer, rendererPath);
        }
        renderer.renderingMode = RenderingMode.Forward;

        web.maxAdditionalLightsCount = 4;
        web.shadowDistance = 20;
        web.shadowCascadeCount = 1;
        web.msaaSampleCount = 1;
        web.renderScale = 1f;
        var serialized = new SerializedObject(web);
        serialized.FindProperty("m_AdditionalLightShadowsSupported").boolValue = false;
        serialized.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue = renderer;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        GraphicsSettings.defaultRenderPipeline = web;
        int selected = QualitySettings.GetQualityLevel();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            QualitySettings.SetQualityLevel(i, false);
            QualitySettings.renderPipeline = web;
        }
        QualitySettings.SetQualityLevel(selected, false);
        EditorUtility.SetDirty(web);
        EditorUtility.SetDirty(renderer);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("DemoAPAR/Build WebGL for GitHub Pages")]
    public static void StartBuild()
    {
        if (EditorApplication.isPlaying || BuildPipeline.isBuildingPlayer)
            throw new InvalidOperationException("Stop Play Mode and wait for any active build first.");
        Configure();
        Status = "Queued";
        Directory.CreateDirectory("Documentation");
        File.WriteAllText("Documentation/webgl-build.txt", DateTimeOffset.Now.ToString("O") + "\nQueued");
        EditorApplication.update -= BuildWhenReady;
        EditorApplication.update += BuildWhenReady;
    }

    static void BuildWhenReady()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        EditorApplication.update -= BuildWhenReady;
        Build();
    }

    static void Build()
    {
        Status = "Building";
        File.AppendAllText("Documentation/webgl-build.txt", "\nBuilding");
        try
        {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/DemoAPAR.unity" },
                locationPathName = "docs",
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });
            Status = report.summary.result.ToString();
            File.WriteAllText("Documentation/webgl-build.txt", DateTimeOffset.Now.ToString("O") + "\n" + Status
                + "\nErrors: " + report.summary.totalErrors + "\nWarnings: " + report.summary.totalWarnings
                + "\nBytes: " + report.summary.totalSize + "\nDuration: " + report.summary.totalTime);
            if (report.summary.result == BuildResult.Succeeded)
                File.WriteAllText("docs/.nojekyll", "");
        }
        catch (Exception e)
        {
            Status = "Failed: " + e.Message;
            File.AppendAllText("Documentation/webgl-build.txt", "\n" + Status);
            Debug.LogException(e);
        }
    }
}
