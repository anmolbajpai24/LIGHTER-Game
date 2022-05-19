using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterSpawn : MonoBehaviour
{
    public GameObject Lighter;
    public Vector3 MinSpawnLighter;
    public Vector3 MaxSpawnLighter;

    [SerializeField]
    private bool StopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LightersSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
       IEnumerator LightersSpawn()
            {

        while (!StopSpawning)
        {
            yield return new WaitForSeconds(2f);


            Vector3 spawnPosition = new Vector3(Random.Range(MinSpawnLighter.x, MaxSpawnLighter.x), 0, Random.Range(MinSpawnLighter.z, MaxSpawnLighter.z));
            Instantiate(Lighter, spawnPosition, Quaternion.identity);


        }
        
    }

    public void SetStopSpawningTrue()
    {
        StopSpawning = true;
    }

}
