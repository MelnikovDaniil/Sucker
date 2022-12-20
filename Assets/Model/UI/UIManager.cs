using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Canvas finishCanvas;
    public Canvas deathCanvas;

    private void Awake()
    {
        Instance = this;
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
    }

    public void NextLevel()
    {
        GameManager.Instance.NextLevel();
    }

    public void ShowDeathPanel()
    {
        Time.timeScale = 0;
        deathCanvas.gameObject.SetActive(true);
    }

    public void ShowFinishPanel()
    {
        Time.timeScale = 0;
        finishCanvas.gameObject.SetActive(true);
    }
}
