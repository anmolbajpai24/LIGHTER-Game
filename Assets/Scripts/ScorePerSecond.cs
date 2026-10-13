using UnityEngine;
using UnityEngine.UI;

// Retains the original scene's serialized references; GameManager owns the clock.
public class ScorePerSecond : MonoBehaviour
{
    public static ScorePerSecond instance;
    public Text score;
    public float scoreAmount;
    public int highscore;
    private void Awake() { instance = this; }
    private void Update()
    {
        if (GameManager.instance == null) return;
        scoreAmount = GameManager.instance.Session.Seconds;
        highscore = GameManager.BestScore;
        if (score != null) score.text = ((int)scoreAmount).ToString();
    }
    public void StartScore() { if (GameManager.instance != null) GameManager.instance.StartGame(); }
    public void StopScore() { }
    private void OnDestroy() { if (instance == this) instance = null; }
}
