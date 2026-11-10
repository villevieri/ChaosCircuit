
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager =
                FindAnyObjectByType<GameManager>();

            if (gameManager != null)
            {
                gameManager.EndRun();
            }
        }
    }
}

