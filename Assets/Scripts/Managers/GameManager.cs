using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Restart()
    {
        var sceen = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sceen.name);
        Time.timeScale = 1;
    }
}
