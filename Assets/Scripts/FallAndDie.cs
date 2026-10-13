using UnityEngine;

public class FallAndDie : MonoBehaviour
{
    public GameObject Player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance != null)
            GameManager.instance.EndGame("You stepped off the arena.");
    }
}
