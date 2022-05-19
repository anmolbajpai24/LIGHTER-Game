using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AI;

public class EnemyFollowing : MonoBehaviour

{
    public NavMeshAgent enemy;

    public Transform Player;

    

    //public int speed;

    //public GameObject player;

    void Start()

    {
        
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        
        //Player = GameObject.FindWithTag("Player");

        //transform.Rotate(180, 0, 0);

    }

    void Update()

    {
        if (Player)
        {
            enemy.SetDestination(Player.position);

        }
        //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

    }

}