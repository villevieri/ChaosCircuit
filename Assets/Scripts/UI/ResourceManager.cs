
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text resourceCounterText;

    private int runResources;
    private int totalResources;

    public int RunResources => runResources;
    public int TotalResources => totalResources;

    private const string TOTAL_RESOURCES_KEY = "TotalScrap";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        totalResources = PlayerPrefs.GetInt(TOTAL_RESOURCES_KEY, 0);

        UpdateUI();
    }

    public void AddResources(int amount)
    {
        runResources += amount;
        UpdateUI();
    }

    public void CompleteRun()
    {
        totalResources += runResources;

        PlayerPrefs.SetInt(TOTAL_RESOURCES_KEY, totalResources);
        PlayerPrefs.Save();

        runResources = 0;

        UpdateUI();
    }

    public void ResetRunResources()
    {
        runResources = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (resourceCounterText != null)
        {
            resourceCounterText.text = "Scrap: " + runResources;
        }
    }
}

