using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public const string BestKey = "lighter.best.v1";
    public const string LastKey = "lighter.last.v1";
    public static int LastScore { get; private set; }
    public static bool NewBest { get; private set; }
    public static string LastReason { get; private set; } = "The lighters caught you.";
    public bool GameOver;
    public RunSession Session { get; private set; } = new RunSession();
    public float Countdown { get; private set; } = 3;
    public bool IsPlaying => Session.State == RunSession.Phase.Playing;
    public static int BestScore => PlayerPrefs.GetInt(BestKey, 0);

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
        GameOver = false;
        LighterInput.Reset();
    }

    private void Update()
    {
        if (Session.State == RunSession.Phase.Ready)
        {
            Countdown -= Time.deltaTime;
            if (Countdown <= 0) StartGame();
        }
        Session.Tick(Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
    }

    public void StartGame() { if (Session.Start()) LighterAudio.Play(660, .12f); }

    public void TogglePause()
    {
        if (Session.Pause()) { Time.timeScale = 0; LighterInput.Reset(); }
        else if (Session.Resume()) Time.timeScale = 1;
    }

    private void OnApplicationPause(bool paused) { if (paused) Pause(); }
    private void OnApplicationFocus(bool focused)
    {
#if LIGHTER_QA
        if (ReleaseSmokeTests.Running) return;
#endif
        if (!focused) Pause();
    }
    private void Pause() { if (Session.Pause()) { Time.timeScale = 0; LighterInput.Reset(); } }

    public void EndGame() => EndGame("The lighters caught you.");
    public void EndGame(string reason)
    {
        if (!Session.Finish()) return;
        GameOver = true;
        LastScore = Session.Score;
        LastReason = reason;
        NewBest = LastScore > BestScore;
        PlayerPrefs.SetInt(LastKey, LastScore);
        if (NewBest) PlayerPrefs.SetInt(BestKey, LastScore);
        PlayerPrefs.Save();
        Time.timeScale = 1;
        LighterInput.Reset();
        SceneManager.LoadScene("GameOver");
    }

    private void OnDestroy() { if (instance == this) instance = null; }
}
