using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowing : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform Player;
    private float nextPath;
    private void Awake()
    {
        enemy = GetComponent<NavMeshAgent>();
        var body = GetComponent<Rigidbody>();
        if (body != null) { body.isKinematic = true; body.useGravity = false; }
        enemy.radius = .18f;
        enemy.height = .9f;
        enemy.angularSpeed = 360;
        enemy.acceleration = 10;
        enemy.avoidancePriority = Random.Range(25, 75);
    }
    private void Start() { var player = GameObject.FindGameObjectWithTag("Player"); if (player != null) Player = player.transform; }
    private void Update()
    {
        if (enemy == null || !enemy.isOnNavMesh) return;
        bool playing = GameManager.instance != null && GameManager.instance.IsPlaying && Player != null;
        enemy.isStopped = !playing;
        if (!playing) return;
        enemy.speed = 1.15f + (GameManager.instance.Session.Heat - 1) * .19f;
        if (Time.time >= nextPath) { enemy.SetDestination(Player.position); nextPath = Time.time + .15f; }
    }
}
