using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public void Awake()
    {
        instance = this;
    }

    public bool GameOver;

    // Start is called before the first frame update
    void Start()
    {
        GameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        ScorePerSecond.instance.StartScore();
        

    }

    public void EndGame()
    {
        SceneManager.LoadScene("GameOver");
       // ScorePerSecond.instance.StopScore();

    }
}
