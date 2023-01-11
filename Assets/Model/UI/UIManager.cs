using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public bool showPlayerMoves;
    public Canvas finishCanvas;
    public Canvas deathCanvas;
    public Animator handAnimator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        handAnimator.gameObject.SetActive(showPlayerMoves);
    }

    private void Update()
    {
        if (showPlayerMoves && Time.timeScale > 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            {
                handAnimator.SetTrigger("tap");
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Space))
            {
                handAnimator.SetTrigger("release");
            }
        }
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
