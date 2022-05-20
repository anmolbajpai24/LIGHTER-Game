using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePerSecond : MonoBehaviour
{
    public static ScorePerSecond instance;

    public void Awake()
    {
        instance = this;
    }
    public Text score;
    public float scoreAmount;
  //  public Text highscoreText;

    public int highscore;

    void Start()
    {
        scoreAmount = 1f;
        PlayerPrefs.SetFloat("score", scoreAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
        score.text = (int)scoreAmount + "";
    

        StartScore();
    }

    void IncrementScore()
    {
        //scoreAmount = scoreAmount + 1;
    }

    public void StartScore()
    {
        
        scoreAmount = scoreAmount + Time.deltaTime;
    }

    public void StopScore()
    {
        //CancelInvoke("StartScore");

       
        
    }


}
