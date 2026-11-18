using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JetpackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChaosPlayerMovement player;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private TMP_Text fuelText;

    private void Update()
    {
        if (player == null)
            return;

        float currentFuel = player.CurrentFuel;
        float maxFuel = player.MaxFuel;

        // Update slider.
        fuelSlider.value = player.FuelNormalized;

        // Update numerical display.
        fuelText.text =
            currentFuel.ToString("F1") +
            " / " +
            maxFuel.ToString("F1") +
            " s";
    }
}

