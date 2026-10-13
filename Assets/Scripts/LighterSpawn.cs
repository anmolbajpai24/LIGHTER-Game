using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LighterSpawn : MonoBehaviour
{
    public GameObject Lighter;
    public Vector3 MinSpawnLighter;
    public Vector3 MaxSpawnLighter;
    public const int MaxEnemies = 18;
    public const float SafeDistance = 2.5f;
    private bool stopped;
    private IEnumerator Start()
    {
        // Existing starter lighter is replaced by the same safe, telegraphed spawn path.
        foreach (var enemy in FindObjectsByType<EnemyFollowing>(FindObjectsSortMode.None)) Destroy(enemy.gameObject);
        while (!stopped)
        {
            yield return new WaitForSeconds(.2f);
            if (GameManager.instance == null || !GameManager.instance.IsPlaying) continue;
            if (FindObjectsByType<EnemyFollowing>(FindObjectsSortMode.None).Length >= MaxEnemies) continue;
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null || !TryPosition(player.transform.position, out var position)) continue;
            var marker = ArenaPresentation.Marker(position, new Color(1, .46f, .12f));
            yield return new WaitForSeconds(.9f);
            Destroy(marker);
            if (stopped || GameManager.instance == null || !GameManager.instance.IsPlaying) continue;
            // A player can move into a warning circle; cancel rather than spawn on them.
            if (Vector3.Distance(player.transform.position, position) < SafeDistance) continue;
            Instantiate(Lighter, position, Quaternion.identity);
            yield return new WaitForSeconds(Mathf.Max(.8f, 2.8f - GameManager.instance.Session.Heat * .3f));
        }
    }
    public bool TryPosition(Vector3 player, out Vector3 result)
    {
        for (int i = 0; i < 24; i++)
        {
            Vector3 candidate = new Vector3(Random.Range(-4.2f, 4.2f), 0, Random.Range(-4.2f, 4.2f));
            if (NavMesh.SamplePosition(candidate, out var hit, .6f, NavMesh.AllAreas) && Vector3.Distance(player, hit.position) >= SafeDistance)
            { result = hit.position; return true; }
        }
        result = Vector3.zero;
        return false;
    }
    public void SetStopSpawningTrue() { stopped = true; StopAllCoroutines(); }
}
