using UnityEngine;

public class CollisionDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<EnemyFollowing>() != null) GameOver();
    }
    public void GameOver() { if (GameManager.instance != null) GameManager.instance.EndGame(); }
}
