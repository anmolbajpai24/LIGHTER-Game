using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void StartGame() => Load("Game");
    public void RestartGame() => Load("Game");
    public void LoadMainMenu() => Load("MainMenu");
    public void QuitGame() { PlayerPrefs.Save(); Application.Quit(); }
    public static void Load(string scene) { Time.timeScale = 1; LighterInput.Reset(); SceneManager.LoadScene(scene); }
}
