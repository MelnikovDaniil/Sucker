using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpRotation : MonoBehaviour
{
    public float rotationSpeed = 180;
    public float pauseDuration = 1;
    public float turningStepAngle = 120;
    public bool moveOnSuck;

    private float currentPauseDuration;
    private Quaternion targetRotation = Quaternion.identity;

    private void Start()
    {
        if (moveOnSuck && TryGetComponent(out Obstacle obstacle))
        {
            obstacle.OnConnect += Connect;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (targetRotation != Quaternion.identity && currentPauseDuration <= 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (transform.rotation == targetRotation)
            {
                currentPauseDuration = pauseDuration;
            }
        }
        else if (!moveOnSuck)
        {
            currentPauseDuration -= Time.deltaTime;
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, turningStepAngle);
        }
    }

    public void Connect()
    {
        if (moveOnSuck)
        {
            currentPauseDuration = 0;
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, turningStepAngle);
        }
    }
}
