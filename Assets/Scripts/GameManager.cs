using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        ScorePerSecond.instance.StopScore();

    }
}
