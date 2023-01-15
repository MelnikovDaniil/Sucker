using System;
using UnityEngine;

public class FinishReward : MonoBehaviour
{
    public GameObject chestPrefab;

    [NonSerialized]
    public Rigidbody2D rigidbody;

    private GameObject chest;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        chest = Instantiate(chestPrefab);
        chest.transform.position = transform.position;
        chest.SetActive(false);
    }

    public void ShowChest()
    {
        chest.SetActive(true);
    }
}
