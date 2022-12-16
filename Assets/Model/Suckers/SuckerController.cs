using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckerController : MonoBehaviour
{
    public static SuckerController Instance;

    public Sucker sucker1;
    public Sucker sucker2;
    public Chain chain;
    
    void Awake()
    {
        Instance = this;
        sucker1.OnSuck += SuckObstacle;
        sucker2.OnSuck += SuckObstacle;
        sucker1.OnUnSuck += UnSuckObstacle;
        sucker2.OnUnSuck += UnSuckObstacle;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            sucker1.UnSuck();
            sucker2.UnSuck();
        }
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
    }
}
