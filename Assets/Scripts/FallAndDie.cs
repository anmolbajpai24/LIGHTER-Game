using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallAndDie : MonoBehaviour
{
    public GameObject Player;

    private void Awake()
    {
        this.GetComponent<BoxCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        
        //Debug.Log("YES");
        GameManager.instance.EndGame();
      

   

    }
}
