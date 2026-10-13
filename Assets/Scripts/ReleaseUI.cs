using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReleaseUI : MonoBehaviour
{
    private static readonly Color Ink = new Color(.035f, .047f, .073f);
    private static readonly Color Paper = new Color(.94f, .93f, .89f);
    private static readonly Color Muted = new Color(.57f, .64f, .72f);
    private static readonly Color Amber = new Color(1, .57f, .22f);
    private RectTransform root;
    private Rect lastSafeArea;
    private Font font;
    private Text timer, heat, countdown, audioLabel;
    private GameObject pausePanel, touchPanel;
    private bool gameplay;
    private bool UseTouch
    {
        get
        {
#if LIGHTER_QA
            if (System.Array.IndexOf(System.Environment.GetCommandLineArgs(), "-lighter-touch") >= 0) return true;
#endif
            return Application.isMobilePlatform;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register() { SceneManager.sceneLoaded -= OnScene; SceneManager.sceneLoaded += OnScene; }
    private static void OnScene(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu" && scene.name != "Game" && scene.name != "GameOver") return;
        foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None)) canvas.gameObject.SetActive(false);
        var ui = new GameObject("LIGHTER Interface").AddComponent<ReleaseUI>();
        ui.Create(scene.name);
    }

    private void Create(string scene)
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        gameplay = scene == "Game";
        var catalog = Resources.Load<ReleaseCatalog>("ReleaseCatalog");
        font = catalog != null && catalog.InterfaceFont != null ? catalog.InterfaceFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        gameObject.AddComponent<LighterAudio>();
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1360, 768);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        gameObject.AddComponent<GraphicRaycaster>();
        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        root = Rect("Safe area", transform, 0, 0, 1, 1);
        UpdateSafeArea();
        gameObject.AddComponent<ArenaPresentation>().Setup(gameplay);
        if (gameplay) GameHUD();
        else if (scene == "MainMenu") Menu();
        else Results();
    }

    private RectTransform Rect(string name, Transform parent, float x0, float y0, float x1, float y1)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        var rect = obj.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(x0, y0); rect.anchorMax = new Vector2(x1, y1);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        return rect;
    }
    private Image Panel(string name, Transform parent, float x0, float y0, float x1, float y1, Color color, bool blocks = false)
    {
        var image = Rect(name, parent, x0, y0, x1, y1).gameObject.AddComponent<Image>();
        image.color = color; image.raycastTarget = blocks;
        return image;
    }
    private Text Label(string name, string words, Transform parent, float x0, float y0, float x1, float y1, int size, Color color, TextAnchor align = TextAnchor.MiddleLeft)
    {
        var text = Rect(name, parent, x0, y0, x1, y1).gameObject.AddComponent<Text>();
        text.font = font; text.text = words; text.fontSize = size; text.color = color; text.alignment = align;
        text.resizeTextForBestFit = true; text.resizeTextMinSize = Mathf.Max(12, size - 8); text.resizeTextMaxSize = size;
        text.raycastTarget = false;
        return text;
    }
    private Button Button(string name, string words, Transform parent, float x0, float y0, float x1, float y1, UnityEngine.Events.UnityAction action, bool primary = false)
    {
        var image = Panel(name, parent, x0, y0, x1, y1, primary ? Amber : new Color(.10f, .14f, .19f), true);
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors; colors.highlightedColor = new Color(1, .9f, .76f); colors.pressedColor = new Color(.72f, .77f, .83f); button.colors = colors;
        Label("Label", words, image.transform, .04f, 0, .96f, 1, 23, primary ? Ink : Paper, TextAnchor.MiddleCenter);
        button.onClick.AddListener(() => { LighterAudio.Play(540, .06f); action(); });
        return button;
    }
    private void Eyebrow(string value) => Label("Eyebrow", value, root, .055f, .86f, .8f, .91f, 17, Amber);
    private void Footer()
    {
        Label("Version", "ANMOL BAJPAI  /  LIGHTER 0.1", root, .055f, .035f, .53f, .085f, 14, Muted);
        Label("Footer", "ONE MORE TRY.", root, .7f, .035f, .945f, .085f, 14, Muted, TextAnchor.MiddleRight);
    }
    private void Menu()
    {
        Eyebrow("A SMALL ARENA. A GROWING PROBLEM.");
        Label("Title", "LIGHTER", root, .05f, .64f, .60f, .85f, 112, Paper);
        Label("Tagline", "Stay cool. Stay alive.", root, .055f, .55f, .53f, .64f, 30, Amber);
        Label("Description", "Dodge the chasing lighters. Keep your footing.\nSurvive a little longer every run.", root, .055f, .42f, .54f, .54f, 23, Muted);
        Button("Play", "LET'S PLAY   >", root, .055f, .285f, .34f, .39f, () => LevelManager.Load("Game"), true);
        Label("Best", "PERSONAL BEST\n" + GameManager.BestScore + " seconds", root, .375f, .285f, .56f, .39f, 23, Paper);
        Label("Controls", UseTouch ? "LEFT THUMB  Move    /    RIGHT THUMB  Jump" : "WASD / ARROWS  Move     SPACE  Jump     ESC  Pause", root, .055f, .19f, .62f, .245f, 17, Muted);
        var audio = Button("Sound", SoundText(), root, .055f, .105f, .225f, .16f, ToggleSound);
        audioLabel = audio.GetComponentInChildren<Text>();
        if (!Application.isMobilePlatform) Button("Quit", "QUIT", root, .245f, .105f, .36f, .16f, Application.Quit);
        Label("Edition", "SURVIVAL ARCADE\n01 / ENDLESS", root, .73f, .80f, .945f, .91f, 17, Muted, TextAnchor.MiddleRight);
        Label("Model caption", "THEY LOOK HARMLESS.", root, .64f, .16f, .945f, .22f, 17, Amber, TextAnchor.MiddleCenter);
        Footer();
    }
    private void Results()
    {
        Eyebrow(GameManager.NewBest ? "A NEW PERSONAL BEST." : "THAT GOT HEATED.");
        Label("Title", "NICE RUN.", root, .05f, .66f, .64f, .84f, 90, Paper);
        Label("Result", GameManager.LastScore + " seconds", root, .055f, .52f, .58f, .67f, 64, Amber);
        Label("Reason", GameManager.LastReason, root, .055f, .44f, .56f, .52f, 23, Muted);
        Label("Best", "PERSONAL BEST   /   " + GameManager.BestScore + "s", root, .055f, .36f, .55f, .42f, 21, Paper);
        Button("Retry", "GO AGAIN   >", root, .055f, .22f, .345f, .325f, () => LevelManager.Load("Game"), true);
        Button("Main menu", "MAIN MENU", root, .365f, .22f, .55f, .325f, () => LevelManager.Load("MainMenu"));
        Label("Tip", "TIP  Orange circles warn you where the next lighter will appear.", root, .055f, .115f, .61f, .19f, 18, Muted);
        Footer();
        LighterAudio.Play(180, .22f, .18f);
    }
    private void GameHUD()
    {
        Panel("Header shade", root, 0, .86f, 1, 1, new Color(.025f, .035f, .06f, .94f));
        Label("Brand", "LIGHTER", root, .04f, .905f, .25f, .97f, 32, Paper);
        Label("Best", "BEST  " + GameManager.BestScore + "s", root, .04f, .865f, .25f, .908f, 15, Muted);
        timer = Label("Timer", "0s", root, .39f, .90f, .61f, .98f, 42, Paper, TextAnchor.MiddleCenter);
        heat = Label("Heat", "HEAT 01", root, .39f, .858f, .61f, .907f, 15, Amber, TextAnchor.MiddleCenter);
        Button("Pause", "II  PAUSE", root, .82f, .895f, .96f, .965f, () => GameManager.instance.TogglePause());
        countdown = Label("Countdown", "3", root, .25f, .45f, .75f, .68f, 90, Amber, TextAnchor.MiddleCenter);
        if (UseTouch)
        {
            touchPanel = Rect("Touch controls", root, 0, 0, 1, .38f).gameObject;
            var baseImage = Panel("Move pad", touchPanel.transform, .045f, .14f, .175f, .80f, new Color(.16f, .21f, .27f, .8f), true);
            var aspect = baseImage.gameObject.AddComponent<AspectRatioFitter>(); aspect.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight; aspect.aspectRatio = 1;
            var stick = baseImage.gameObject.AddComponent<LighterJoystick>();
            var knob = Panel("Thumb", baseImage.transform, .33f, .33f, .67f, .67f, Amber);
            stick.Knob = (RectTransform)knob.transform;
            Label("Move", "MOVE", touchPanel.transform, .045f, .01f, .175f, .12f, 15, Muted, TextAnchor.MiddleCenter);
            Button("Jump", "JUMP", touchPanel.transform, .82f, .16f, .955f, .67f, () => LighterInput.Jump = true, true);
        }
        else Label("Controls", "WASD / ARROWS   MOVE       SPACE   JUMP       ESC   PAUSE", root, .18f, .035f, .82f, .095f, 16, Muted, TextAnchor.MiddleCenter);
        pausePanel = Panel("Pause overlay", root, 0, 0, 1, 1, new Color(.02f, .03f, .05f, .96f), true).gameObject;
        Label("Pause title", "TAKE A BREATHER.", pausePanel.transform, .18f, .64f, .82f, .79f, 58, Paper, TextAnchor.MiddleCenter);
        Label("Pause hint", "Your run is paused. The lighters can wait.", pausePanel.transform, .2f, .54f, .8f, .64f, 23, Muted, TextAnchor.MiddleCenter);
        Button("Resume", "KEEP GOING   >", pausePanel.transform, .34f, .395f, .66f, .50f, () => GameManager.instance.TogglePause(), true);
        Button("Abandon run", "BACK TO MENU", pausePanel.transform, .34f, .275f, .66f, .365f, () => LevelManager.Load("MainMenu"));
        var audio = Button("Sound", SoundText(), pausePanel.transform, .40f, .17f, .60f, .235f, ToggleSound);
        audioLabel = audio.GetComponentInChildren<Text>();
        pausePanel.SetActive(false);
    }
    private string SoundText() => LighterAudio.Muted ? "SOUND  OFF" : "SOUND  ON";
    private void ToggleSound() { LighterAudio.Toggle(); audioLabel.text = SoundText(); }
    private void UpdateSafeArea()
    {
        lastSafeArea = Screen.safeArea;
        root.anchorMin = new Vector2(lastSafeArea.xMin / Screen.width, lastSafeArea.yMin / Screen.height);
        root.anchorMax = new Vector2(lastSafeArea.xMax / Screen.width, lastSafeArea.yMax / Screen.height);
    }
    private void Update()
    {
        if (lastSafeArea != Screen.safeArea) UpdateSafeArea();
        if (!gameplay)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.R)) LevelManager.Load("Game");
            return;
        }
        var manager = GameManager.instance;
        if (manager == null) return;
        timer.text = manager.Session.Score + "s";
        heat.text = "HEAT 0" + manager.Session.Heat;
        countdown.text = manager.Session.State == RunSession.Phase.Ready ? Mathf.CeilToInt(Mathf.Max(.1f, manager.Countdown)).ToString() : "";
        bool paused = manager.Session.State == RunSession.Phase.Paused;
        if (pausePanel.activeSelf != paused) pausePanel.SetActive(paused);
        if (touchPanel != null && touchPanel.activeSelf == paused) touchPanel.SetActive(!paused);
    }
}
