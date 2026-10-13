using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Text Score;
    public Text Highscore;
    private void Start()
    {
        if (Score != null) Score.text = PlayerPrefs.GetInt(GameManager.LastKey, 0).ToString();
        if (Highscore != null) Highscore.text = GameManager.BestScore.ToString();
    }
}
