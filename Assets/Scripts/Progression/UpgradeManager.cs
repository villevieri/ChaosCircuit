
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Jetpack Upgrade")]
    [SerializeField] private float fuelIncreasePerLevel = 0.5f;
    [SerializeField] private int baseUpgradeCost = 10;

    private int jetpackLevel;

    private const string JETPACK_LEVEL_KEY = "JetpackLevel";

    public int JetpackLevel => jetpackLevel;

    public int JetpackUpgradeCost =>
        baseUpgradeCost * (jetpackLevel + 1);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        jetpackLevel = PlayerPrefs.GetInt(
            JETPACK_LEVEL_KEY, 0
        );
    }

    public void ApplyJetpackUpgrade()
    {
        ChaosPlayerMovement player =
            FindAnyObjectByType<ChaosPlayerMovement>();

        if (player != null)
        {
            player.UpgradeMaxFuel(fuelIncreasePerLevel);
        }
    }

    public bool PurchaseJetpackUpgrade()
    {
        int cost = JetpackUpgradeCost;

        if (ResourceManager.Instance.TotalResources < cost)
        {
            Debug.Log("Not enough Scrap!");
            return false;
        }

        jetpackLevel++;

        PlayerPrefs.SetInt(JETPACK_LEVEL_KEY, jetpackLevel);
        PlayerPrefs.Save();

        ApplyJetpackUpgrade();

        Debug.Log("Jetpack upgraded to level " + jetpackLevel);

        return true;
    }
}

