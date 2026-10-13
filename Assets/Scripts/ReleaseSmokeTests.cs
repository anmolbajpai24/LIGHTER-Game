#if LIGHTER_QA
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Included only in the QA player. Release packages contain no automation entry point.
public class ReleaseSmokeTests : MonoBehaviour
{
    public static bool Running { get; private set; }
    private static readonly List<string> Errors = new List<string>();
    private int passed;
    private string folder;
    private float started;
    private int previousBest;
    private int previousLast;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Listen()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-lighter-smoke-test") < 0) return;
        Errors.Clear();
        Running = true;
        Application.logMessageReceived += (message, stack, type) => { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) Errors.Add(message); };
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Begin()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-lighter-smoke-test") < 0) return;
        var go = new GameObject("Release smoke tests");
        DontDestroyOnLoad(go);
        go.AddComponent<ReleaseSmokeTests>();
    }
    private IEnumerator Start()
    {
        started = Time.realtimeSinceStartup;
        Application.runInBackground = true;
        var args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, "-evidencePath");
        folder = index >= 0 && index + 1 < args.Length ? args[index + 1] : Path.Combine(Application.persistentDataPath, "QA");
        Directory.CreateDirectory(folder);
        previousBest = PlayerPrefs.GetInt(GameManager.BestKey, 0);
        previousLast = PlayerPrefs.GetInt(GameManager.LastKey, 0);
        yield return new WaitForSecondsRealtime(.5f);
        Check(SceneManager.GetActiveScene().name == "MainMenu", "Player boots to menu");
        Check(GameObject.Find("LIGHTER Interface") != null && GameObject.Find("Play") != null, "Menu initialized without stripped shader failure");
        yield return Capture("release-menu.png");
        Click("Play");
        yield return null;
        yield return null;
        Check(GameManager.instance != null && GameManager.instance.Session.Score == 0, "Play loads a fresh run");
        Check(GameManager.instance.Session.State == RunSession.Phase.Ready, "Countdown precedes play");
        yield return new WaitForSecondsRealtime(3.4f);
        if (!GameManager.instance.IsPlaying) GameManager.instance.TogglePause();
        Check(GameManager.instance.IsPlaying, "Countdown starts gameplay");
        var player = GameObject.FindGameObjectWithTag("Player");
        var original = player.transform.position;
        var joystick = FindFirstObjectByType<LighterJoystick>();
        var pointer = new PointerEventData(EventSystem.current) { pointerId = 42 };
        if (joystick != null)
        {
            var rect = (RectTransform)joystick.transform;
            pointer.position = RectTransformUtility.WorldToScreenPoint(null, rect.position);
            joystick.OnPointerDown(pointer);
            pointer.position = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(new Vector3(rect.rect.width, 0, 0)));
            joystick.OnDrag(pointer);
            Check(Mathf.Abs(LighterInput.Direction.x - 1) < .01f && Mathf.Abs(LighterInput.Direction.y) < .01f, "Joystick clamps drag to unit movement");
            joystick.OnPointerUp(new PointerEventData(EventSystem.current) { pointerId = 99 });
            Check(LighterInput.Direction.x > .99f, "Second finger cannot release movement finger");
        }
        else LighterInput.Direction = Vector2.right;
        yield return new WaitForSeconds(.3f);
        if (joystick != null) { joystick.OnPointerUp(pointer); Check(LighterInput.Direction == Vector2.zero, "Releasing joystick stops input"); }
        else LighterInput.Reset();
        Check(Vector3.Distance(player.transform.position, original) > .45f, "Touch movement reaches the real character");
        yield return new WaitForSeconds(.25f);
        float beforeJump = player.transform.position.y;
        if (GameObject.Find("Jump") != null) Click("Jump");
        else LighterInput.Jump = true;
        yield return new WaitForSeconds(.2f);
        Check(player.transform.position.y > beforeJump + .15f, "Jump lifts the real rigidbody");
        yield return new WaitForSeconds(.7f);
        Click("Pause");
        yield return null;
        float frozen = GameManager.instance.Session.Seconds;
        original = player.transform.position;
        yield return new WaitForSecondsRealtime(.4f);
        Check(Time.timeScale == 0 && GameManager.instance.Session.Seconds == frozen, "Pause freezes score");
        Check(Vector3.Distance(original, player.transform.position) < .01f, "Pause freezes physics");
        yield return Capture("release-pause.png");
        Click("Resume");
        yield return null;
        Check(Time.timeScale == 1 && GameManager.instance.IsPlaying, "Resume restores simulation");
        var harmless = new GameObject("Non-player collider").AddComponent<BoxCollider>();
        FindFirstObjectByType<FallAndDie>().SendMessage("OnTriggerEnter", harmless);
        yield return null;
        Check(SceneManager.GetActiveScene().name == "Game", "Fall trigger ignores non-player objects");
        Destroy(harmless.gameObject);
        var spawner = FindFirstObjectByType<LighterSpawn>();
        for (int i = 0; i < 100; i++)
        {
            Check(spawner.TryPosition(player.transform.position, out var point) && Vector3.Distance(point, player.transform.position) >= LighterSpawn.SafeDistance, "Safe NavMesh spawn " + i);
        }
        yield return Capture("release-gameplay.png");
        // Actual physics fall verifies the full scene transition and score save.
        GameManager.instance.Session.Tick(11);
        int expected = GameManager.instance.Session.Score;
        player.GetComponent<Rigidbody>().position = new Vector3(0, -5, 0);
        Physics.SyncTransforms();
        yield return new WaitForSecondsRealtime(.2f);
        Check(SceneManager.GetActiveScene().name == "GameOver", "Falling ends the run");
        Check(GameManager.LastScore == expected && PlayerPrefs.GetInt(GameManager.LastKey) == expected, "Result preserves final elapsed score");
        Check(GameManager.BestScore >= expected, "Personal best persisted");
        yield return Capture("release-results.png");
        Click("Retry");
        yield return null;
        yield return null;
        Check(GameManager.instance.Session.Score == 0 && Time.timeScale == 1, "Retry resets score and time scale");
        GameManager.instance.StartGame();
        player = GameObject.FindGameObjectWithTag("Player");
        yield return new WaitForSeconds(1.4f);
        var enemies = FindObjectsByType<EnemyFollowing>(FindObjectsSortMode.None);
        Check(enemies.Length > 0 && enemies[0].enemy.isOnNavMesh, "Spawned lighter can navigate");
        var enemy = enemies[0];
        var before = enemy.transform.position;
        yield return new WaitForSeconds(.4f);
        Check(Vector3.Distance(before, enemy.transform.position) > .05f, "Enemy pursues player");
        player.GetComponent<Rigidbody>().position = enemy.transform.position;
        Physics.SyncTransforms();
        yield return new WaitForSeconds(.3f);
        Check(SceneManager.GetActiveScene().name == "GameOver", "Real lighter collision ends the run");
        Click("Retry");
        yield return null;
        yield return null;
        GameManager.instance.StartGame();
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<LighterPlayerController>().enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.transform.position = new Vector3(0, 3, 0);
        Time.timeScale = 12;
        yield return new WaitForSeconds(80);
        Time.timeScale = 1;
        int count = FindObjectsByType<EnemyFollowing>(FindObjectsSortMode.None).Length;
        Check(count == LighterSpawn.MaxEnemies, "Long run reaches but does not exceed enemy cap");
        Check(GameManager.instance.Session.Heat == 6, "Long-run difficulty is bounded");
        GameManager.instance.TogglePause();
        yield return null;
        Click("Abandon run");
        yield return null;
        yield return null;
        Check(SceneManager.GetActiveScene().name == "MainMenu" && Time.timeScale == 1, "Leaving a paused run restores menu");
        Check(Errors.Count == 0, "No runtime errors across menu, gameplay, pause, death, retry, and long run");
        Complete(true);
    }
    private void Update() { if (started > 0 && Time.realtimeSinceStartup - started > 90) { Debug.LogError("[LIGHTER QA] Timeout"); Complete(false); } }
    private void Check(bool condition, string description)
    {
        if (!condition) { Debug.LogError("[LIGHTER QA] FAIL: " + description); Complete(false); throw new Exception(description); }
        passed++;
        if (!description.StartsWith("Safe NavMesh")) Debug.Log("[LIGHTER QA] PASS: " + description);
    }
    private void Click(string name)
    {
        var obj = GameObject.Find(name);
        Check(obj != null && obj.GetComponent<Button>() != null, "Button available: " + name);
        obj.GetComponent<Button>().onClick.Invoke();
    }
    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(Path.Combine(folder, name));
        yield return null;
    }
    private void Complete(bool success)
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt(GameManager.BestKey, previousBest);
        PlayerPrefs.SetInt(GameManager.LastKey, previousLast);
        PlayerPrefs.Save();
        if (folder != null) File.WriteAllText(Path.Combine(folder, "smoke-test-result.txt"), $"{DateTimeOffset.Now:O}\n{(success ? "PASS" : "FAIL")} | {passed} assertions\n" + string.Join("\n", Errors));
        Debug.Log($"[LIGHTER QA] {(success ? "PASS" : "FAIL")} | {passed} assertions");
        Application.Quit(success ? 0 : 1);
    }
}
#endif
