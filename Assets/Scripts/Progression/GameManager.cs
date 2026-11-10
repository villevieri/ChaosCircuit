
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Results UI")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text resultsText;
    [SerializeField] private Button restartButton;

    private bool runEnded;

    private void Start()
    {
        resultsPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartRun);
    }

    private void Update()
    {
        // Temporary test: press Escape to end the run.
        if (!runEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            EndRun();
        }
    }

    public void EndRun()
    {
        if (runEnded)
            return;

        runEnded = true;

        int earnedScrap = ResourceManager.Instance.RunResources;

        ResourceManager.Instance.CompleteRun();

        resultsText.text =
            "RUN OVER\n\n" +
            "Scrap collected: " + earnedScrap +
            "\nTotal Scrap: " +
            ResourceManager.Instance.TotalResources;

        resultsPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}

