
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private TMP_Text resourceCounterText;

    private int resources;
    public int Resources => resources;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateUI();
    }

    public void AddResources(int amount)
    {
        resources += amount;
        UpdateUI();
    }
    public void ResetResources()
    {
        resources = 0;
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (resourceCounterText != null)
        {
            resourceCounterText.text = "Scrap: " + resources;
        }
    }
}
