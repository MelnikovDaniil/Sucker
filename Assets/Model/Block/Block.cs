using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    public event Action OnBreak;
    public float strength;

    private Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    public void Break()
    {
        OnBreak?.Invoke();
    }
}
