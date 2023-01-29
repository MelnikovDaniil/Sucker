using System;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public event Action<Block> OnCollision;
    public event Action OnBreak;


    public float strength;

    [NonSerialized]
    public Rigidbody2D rigidbody;

    private readonly List<Sucker> connectedSuckers = new();
    private bool ableToCollide;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    public void Connect(Sucker sucker)
    {
        connectedSuckers.Add(sucker);
    }

    public void SetResistance(float liftingStrength)
    {
        ableToCollide = liftingStrength >= strength;
        rigidbody.mass *= Mathf.Clamp(strength / liftingStrength, 1,  100);
    }

    public void Break()
    {
        foreach (var sucker in connectedSuckers)
        {
            sucker.Disconnect();
        }
        OnBreak?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!rigidbody.isKinematic 
            && ableToCollide
            && collision.relativeVelocity.y > 5f 
            && collision.gameObject.TryGetComponent(out Block block))
        {
            OnCollision?.Invoke(block);
        }
    }
}
