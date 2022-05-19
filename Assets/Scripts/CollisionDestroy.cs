using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDestroy : MonoBehaviour
{
    private LighterSpawn lighterSpawn;
    private ScorePerSecond scorePerSecond;

    void Start()
    {
        lighterSpawn = GameObject.Find("LighterSpawner").GetComponent<LighterSpawn>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Lighter_Prefab"|| collision.gameObject.name == "Lighter_Prefab(Clone)")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);

            lighterSpawn.SetStopSpawningTrue();

            ScorePerSecond.instance.StopScore();

            GameOver();
        }
    }
    
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
        //ScorePerSecond.instance.StopScore();
        //GameManager.instance.EndGame();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
}
