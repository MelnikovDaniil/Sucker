using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Obstacle : MonoBehaviour
{
    public event Action OnConnect;
    public float rotationSpeed = 100;

    [NonSerialized]
    public Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
    }

    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void Connect()
    {
        OnConnect?.Invoke();
    }
}
