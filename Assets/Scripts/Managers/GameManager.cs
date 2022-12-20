using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Vector3 deathColliderSize;
    public Vector3 deathColliderOffset;

    private BoxCollider deathCollider;
    private bool isDead;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var deathColliderObj = new GameObject("DeathCollider", typeof(BoxCollider));
        deathColliderObj.transform.localPosition = Vector3.zero;
        deathColliderObj.tag = "deathCollider";
        deathCollider = deathColliderObj.GetComponent<BoxCollider>();
        deathCollider.size = deathColliderSize;
        deathCollider.transform.position = deathColliderOffset;
        deathCollider.isTrigger = true;
    }

    public void DisableDeathCollider()
    {
        deathCollider.gameObject.SetActive(false);
    }

    public void Death()
    {
        if (!isDead)
        {
            isDead = true;
            UIManager.Instance.ShowDeathPanel();
        }
    }

    public void NextLevel()
    {
        var sceen = SceneManager.GetActiveScene();
        var nextLevelNumber = int.Parse(sceen.name) % 5;
        SceneManager.LoadScene(nextLevelNumber);
        Time.timeScale = 1;
    }

    public void MoveDeathCollider(Vector3 position)
    {
        deathCollider.gameObject.transform.position = position + deathColliderOffset;
    }

    public void Restart()
    {
        var sceen = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sceen.name);
        Time.timeScale = 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(deathColliderOffset, deathColliderSize);
    }
}
