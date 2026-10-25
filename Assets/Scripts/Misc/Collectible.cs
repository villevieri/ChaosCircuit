using System.Resources;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int resourceValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResourceManager.Instance.AddResources(resourceValue);
            Destroy(gameObject);
        }
    }
}
