using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SharpRotation : MonoBehaviour
{
    public float rotationSpeed = 180;
    public float pauseDuration = 1;
    public bool moveOnSuck;
    public List<float> turnAnglePatten;

    private Queue<float> turnAngleQueue = new Queue<float>();
    public float currentPauseDuration;
    private Quaternion? targetRotation;

    private void Start()
    {
        if (moveOnSuck && TryGetComponent(out Obstacle obstacle))
        {
            obstacle.OnConnect += Connect;
        }
        currentPauseDuration = pauseDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetRotation.HasValue && currentPauseDuration <= 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation.Value, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation.Value) < 2f)
            {
                targetRotation = null;
                currentPauseDuration = pauseDuration;
            }
        }
        else if (!moveOnSuck)
        {
            currentPauseDuration -= Time.deltaTime;
            if (currentPauseDuration <= 0)
            {
                targetRotation = Quaternion.Euler(0, 0, GetNextAngle());
            }
        }
    }

    public void Connect()
    {
        if (moveOnSuck)
        {
            currentPauseDuration = 0;
            targetRotation = Quaternion.Euler(0, 0, GetNextAngle());
        }
    }

    private float GetNextAngle()
    {
        if (!turnAngleQueue.Any())
        {
            turnAngleQueue = new Queue<float>(turnAnglePatten);
        }

        return turnAngleQueue.Dequeue();
    }
}
