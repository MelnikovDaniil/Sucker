using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(/*typeof(SpringJoint),*/ typeof(Rigidbody))]

public class Sucker : MonoBehaviour
{
    public Vector3 springConnectionPosition;
    // Start is called before the first frame update

    //[NonSerialized]
    //public SpringJoint spring;
    [NonSerialized]
    public Rigidbody rigidbody;

    private void Awake()
    {
        //spring = GetComponent<SpringJoint>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        var springWorldPosition = transform.position + transform.rotation * springConnectionPosition;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(springWorldPosition, 0.5f);
    }
}
