using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckerController : MonoBehaviour
{
    public Sucker sucker1;
    public Sucker sucker2;
    public Chain chain;
    
    void Start()
    {
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
    }
}
