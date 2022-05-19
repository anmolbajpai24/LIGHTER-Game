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
        /*
        highscore = (int) scoreAmount;

        if(PlayerPrefs.GetInt("scoreAmount")<=highscore)
        {
            PlayerPrefs.SetInt("scoreAmount", highscore);
        }
        PlayerPrefs.SetInt("score", highscore);

        highscoreText.text = (int)highscore + "";
        */

        StartScore();
    }

    void IncrementScore()
    {
        //scoreAmount = scoreAmount + 1;
    }

    public void StartScore()
    {
        //InvokeRepeating("IncrementScore", 1f, 1f);
        scoreAmount = scoreAmount + Time.deltaTime;
    }

    public void StopScore()
    {
        //CancelInvoke("StartScore");

        

        PlayerPrefs.SetFloat("score", scoreAmount);

        if(PlayerPrefs.HasKey("highscore"))
        {
            if (scoreAmount > PlayerPrefs.GetInt("highscore"))
            {
                PlayerPrefs.SetFloat("highscore", scoreAmount);

            }
        }

        else
        {
            PlayerPrefs.SetFloat("highscore", scoreAmount);
        }
        
    }


}
