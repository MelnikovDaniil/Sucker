using System;
using UnityEngine;

public class SuckerController : MonoBehaviour
{
    public static SuckerController Instance;

    public Chain chain;

    [NonSerialized]
    public Sucker sucker1;
    [NonSerialized]
    public Sucker sucker2;

    void Awake()
    {
        Instance = this;
        sucker1 = chain.sucker1;
        sucker2 = chain.sucker2;
        sucker1.OnSuck += SuckObstacle;
        sucker2.OnSuck += SuckObstacle;
        sucker1.OnUnSuck += UnSuckObstacle;
        sucker2.OnUnSuck += UnSuckObstacle;
    }

    private void Start()
    {
        sucker1.OnDeath += GameManager.Instance.Death;
        sucker2.OnDeath += GameManager.Instance.Death;
    }

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
        //{
        //    sucker1.UnSuck();
        //    sucker2.UnSuck();
        //}
    }

    private void UnSuckObstacle()
    {

    }

    public void SuckObstacle(Obstacle obstacle)
    {
        if (!CameraManager.Instance.followByPlunger)
        {
            CameraManager.Instance.SetTarget(obstacle.transform.position);
        }

        GameManager.Instance.MoveDeathCollider(obstacle.transform.position);
    }
}
