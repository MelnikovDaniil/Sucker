using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Sucker : MonoBehaviour
{
    public event Action<Obstacle> OnSuck;
    public event Action OnUnSuck;
    public Vector3 springConnectionPosition;

    [NonSerialized]
    public bool isSucked;
    [NonSerialized]
    public Rigidbody rigidbody;

    private FixedJoint connectionPlace;
    private bool blocked;

    private void Awake()
    {
        //spring = GetComponent<SpringJoint>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void UnSuck()
    {
        if (connectionPlace != null)
        {
            StartCoroutine(UnSuckRoutine());
            //rigidbody.isKinematic = false;
            isSucked = false;
            Destroy(connectionPlace.gameObject);
            connectionPlace = null;
            OnUnSuck?.Invoke();
        }
    }

    private IEnumerator UnSuckRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        blocked = false;
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (!isSucked && other.gameObject.TryGetComponent(out Obstacle obstacle))
    //    {
    //        isSucked = true;
    //        var contactPoint = other.GetContact(0);
    //        connectionPlace = new GameObject($"ConnectionPlace of {this.name}", typeof(FixedJoint)).GetComponent<FixedJoint>();
    //        connectionPlace.GetComponent<Rigidbody>().isKinematic = true;
    //        connectionPlace.transform.parent = obstacle.transform;
    //        connectionPlace.transform.position = new Vector3(contactPoint.point.x, contactPoint.point.y, 0);
    //        connectionPlace.transform.localRotation = Quaternion.identity;
    //        connectionPlace.connectedBody = rigidbody;
    //        //rigidbody.isKinematic = true;
    //        OnSuck?.Invoke(obstacle);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!blocked && !isSucked && other.gameObject.TryGetComponent(out Obstacle obstacle))
        {
            blocked = true;
            isSucked = true;
            connectionPlace = new GameObject($"ConnectionPlace of {this.name}", typeof(FixedJoint)).GetComponent<FixedJoint>();
            var placeRigidbody = connectionPlace.GetComponent<Rigidbody>();
            placeRigidbody.isKinematic = true;
            placeRigidbody.constraints = rigidbody.constraints;
            connectionPlace.transform.parent = obstacle.transform;
            connectionPlace.transform.position = transform.position;
            connectionPlace.transform.localRotation = Quaternion.identity;
            connectionPlace.connectedBody = rigidbody;
            //rigidbody.isKinematic = true;
            OnSuck?.Invoke(obstacle);
        }
    }

    private void OnDrawGizmos()
    {
        var springWorldPosition = transform.position + transform.rotation * springConnectionPosition;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(springWorldPosition, 0.5f);
    }
}
