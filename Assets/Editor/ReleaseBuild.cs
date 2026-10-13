using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public static class ReleaseBuild
{
    private static readonly string[] Scenes = { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Game.unity", "Assets/Scenes/GameOver.unity" };

    [MenuItem("LIGHTER/Prepare release settings")]
    public static void Prepare()
    {
        PlayerSettings.companyName = "Anmol Bajpai";
        PlayerSettings.productName = "LIGHTER";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.anmolbajpai.lighter");
        PlayerSettings.defaultScreenWidth = 1360;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.runInBackground = false;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new[] { GraphicsDeviceType.Direct3D11 });
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 2;
        QualitySettings.shadowDistance = 25;
        QualitySettings.pixelLightCount = 2;
        EditorBuildSettings.scenes = Scenes.Select(p => new EditorBuildSettingsScene(p, true)).ToArray();

        Directory.CreateDirectory("Assets/Resources");
        Directory.CreateDirectory("Assets/ReleaseAssets");
        const string displayPath = "Assets/ReleaseAssets/LighterDisplay.prefab";
        if (!File.Exists(displayPath))
        {
            var preview = PrefabUtility.LoadPrefabContents("Assets/devotid/Lighter/Prefabs/Lighter_Prefab.prefab");
            UnityEngine.Object.DestroyImmediate(preview.GetComponent<EnemyFollowing>());
            UnityEngine.Object.DestroyImmediate(preview.GetComponent<NavMeshAgent>());
            UnityEngine.Object.DestroyImmediate(preview.GetComponent<Rigidbody>());
            foreach (var collider in preview.GetComponents<Collider>()) UnityEngine.Object.DestroyImmediate(collider);
            PrefabUtility.SaveAsPrefabAsset(preview, displayPath);
            PrefabUtility.UnloadPrefabContents(preview);
        }
        const string catalogPath = "Assets/Resources/ReleaseCatalog.asset";
        var catalog = AssetDatabase.LoadAssetAtPath<ReleaseCatalog>(catalogPath);
        if (catalog == null) { catalog = ScriptableObject.CreateInstance<ReleaseCatalog>(); AssetDatabase.CreateAsset(catalog, catalogPath); }
        catalog.DisplayPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(displayPath);
        catalog.LitShader = Shader.Find("Standard");
        catalog.UnlitShader = Shader.Find("Unlit/Color");
        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        Debug.Log("[LIGHTER BUILD] Prepared 0.1.0 for Windows x64 and Android ARM64.");
    }

    [MenuItem("LIGHTER/Build Windows release")]
    public static void Windows() => Build(BuildTarget.StandaloneWindows64, "Builds/Windows/LIGHTER.exe", false);
    public static void WindowsQA() => Build(BuildTarget.StandaloneWindows64, "Builds/QA/LIGHTER.exe", true);
    [MenuItem("LIGHTER/Build Android test APK")]
    public static void Android()
    {
        EditorUserBuildSettings.buildAppBundle = false;
        Build(BuildTarget.Android, "Builds/Android/LIGHTER.apk", false);
    }
    public static void AndroidBundle()
    {
        if (!PlayerSettings.Android.useCustomKeystore) throw new BuildFailedException("A store bundle requires a configured release keystore. Use ReleaseBuild.Android for the locally signed test APK.");
        EditorUserBuildSettings.buildAppBundle = true;
        Build(BuildTarget.Android, "Builds/Android/LIGHTER.aab", false);
    }

    private static void Build(BuildTarget target, string fallback, bool qa)
    {
        Prepare();
        Validate();
        var args = Environment.GetCommandLineArgs();
        var index = Array.IndexOf(args, "-customBuildPath");
        var output = index >= 0 && index + 1 < args.Length ? args[index + 1] : fallback;
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        Debug.Log($"[LIGHTER BUILD] Started {DateTimeOffset.Now:O} | {Application.unityVersion} | {target} | {output}");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = Scenes, target = target, locationPathName = output,
            options = qa ? BuildOptions.Development : BuildOptions.None,
            extraScriptingDefines = qa ? new[] { "LIGHTER_QA" } : Array.Empty<string>()
        });
        var summary = report.summary;
        Debug.Log($"[LIGHTER BUILD] Finished {DateTimeOffset.Now:O} | {summary.result} | {summary.totalTime} | {summary.totalSize} bytes | {summary.totalErrors} errors");
        if (summary.result != BuildResult.Succeeded) throw new BuildFailedException("LIGHTER build failed. See the first compiler or build error above.");
    }

    public static void Validate()
    {
        foreach (var path in Scenes) if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null) throw new BuildFailedException("Required scene is missing: " + path);
        if (Resources.Load<ReleaseCatalog>("ReleaseCatalog") == null) throw new BuildFailedException("ReleaseCatalog is missing. Run ReleaseBuild.Prepare.");
        // Catch scene serialization breakage before a green build can hide it.
        var setup = UnityEditor.SceneManagement.EditorSceneManager.GetSceneManagerSetup();
        try
        {
            foreach (var path in Scenes)
            {
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path);
                foreach (var root in scene.GetRootGameObjects()) foreach (var t in root.GetComponentsInChildren<Transform>(true))
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0) throw new BuildFailedException("Missing script: " + path + " / " + t.name);
            }
        }
        finally { if (setup.Length > 0) UnityEditor.SceneManagement.EditorSceneManager.RestoreSceneManagerSetup(setup); }
        RulesTests();
    }

    public static void RulesTests()
    {
        var run = new RunSession();
        Check(run.Score == 0 && !run.Finish(), "No score or death before countdown completes");
        run.Tick(4); Check(run.Score == 0, "Countdown excluded from score");
        Check(run.Start() && !run.Start(), "Start is idempotent");
        for (int i = 0; i < 100; i++) run.Tick(.1f);
        Check(run.Score >= 9 && run.Score <= 10, "Elapsed time accumulates");
        Check(run.Pause(), "Pause succeeds");
        var frozen = run.Seconds; run.Tick(100); Check(run.Seconds == frozen, "Paused time excluded");
        Check(run.Resume(), "Resume succeeds");
        run.Tick(80); Check(run.Heat == 6, "Difficulty capped");
        Check(run.Finish() && !run.Finish(), "Death is idempotent");
        frozen = run.Seconds; run.Tick(3); Check(run.Seconds == frozen, "Finished score frozen");
        Check(new RunSession().Score == 0, "Retry starts from zero");
        Debug.Log("[LIGHTER TEST] Rules: 10 passed.");
    }
    private static void Check(bool condition, string name) { if (!condition) throw new BuildFailedException("Rule failed: " + name); }
}
