using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Sucker : MonoBehaviour
{
    public event Action<Obstacle> OnSuck;
    public event Action OnUnSuck;
    public Vector3 springConnectionPosition;

    [Space]
    public Vector3 suckerPosition;
    public float maxRotationDuringSuck;

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

    private void OnTriggerStay(Collider other)
    {
        if (!blocked && !isSucked && other.gameObject.TryGetComponent(out Obstacle obstacle))
        {
            Suck(other, obstacle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!blocked && !isSucked && other.gameObject.TryGetComponent(out Obstacle obstacle))
        {
            Suck(other, obstacle);
        }
    }

    private void Suck(Collider collider, Obstacle obstacle)
    {
        var leftAngle = CalculateRotationAngle(collider, false) ?? float.MaxValue;
        var rightAngle = CalculateRotationAngle(collider, true) ?? float.MaxValue;

        var rotationAngle = Mathf.Abs(leftAngle) >= Mathf.Abs(rightAngle) ? rightAngle : leftAngle;

        if (rotationAngle != float.MaxValue)
        {
            blocked = true;
            isSucked = true;

            var previousSuckPositions = GetSuckerPositions();
            var nextRotationPositions = GetSuckerPositions(rotationAngle);
            var previousMiddleSuckPoint = Vector3.Lerp(previousSuckPositions.left, previousSuckPositions.right, 0.5f);
            var nextMiddleSuckPoint = Vector3.Lerp(nextRotationPositions.left, nextRotationPositions.right, 0.5f);
            var closiestPoint = Physics.ClosestPoint(nextMiddleSuckPoint, collider, collider.transform.position, collider.transform.rotation);
            var moveOffset = closiestPoint - previousMiddleSuckPoint;
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAngle);
            transform.position += moveOffset;

            connectionPlace = new GameObject($"ConnectionPlace of {this.name}", typeof(FixedJoint)).GetComponent<FixedJoint>();
            var placeRigidbody = connectionPlace.GetComponent<Rigidbody>();
            placeRigidbody.isKinematic = true;
            placeRigidbody.constraints = rigidbody.constraints;
            connectionPlace.transform.parent = obstacle.transform;
            connectionPlace.transform.position = transform.position;
            connectionPlace.transform.localRotation = Quaternion.identity;
            connectionPlace.connectedBody = rigidbody;
            //rigidbody.isKinematic = true;

            obstacle.Connect();
            OnSuck?.Invoke(obstacle);
        }
    }

    private float? CalculateRotationAngle(Collider searchingCollider, bool isRightPositionPined)
    {
        var suckerCurrentPositions = GetSuckerPositions();

        var pinnedPoint = isRightPositionPined ? suckerCurrentPositions.right : suckerCurrentPositions.left;
        var movingPoint = isRightPositionPined ? suckerCurrentPositions.left : suckerCurrentPositions.right;
        var colliders = Physics.OverlapSphere(pinnedPoint, 0.1f, LayerMask.GetMask("Obstacle"));
        
        if (colliders.Contains(searchingCollider))
        {
            var differenceVector = movingPoint - pinnedPoint;

            return FindSuckAngle(searchingCollider, pinnedPoint, differenceVector, isRightPositionPined ? 1 : -1);
        }

        return null;
    }

    private float? FindSuckAngle(Collider searchingCollider, Vector3 pinnedPoint, Vector3 differenceVector, float angle = 0)
    {
        var potentialConnectionPoint = pinnedPoint + Quaternion.Euler(0, 0, angle) * differenceVector;
        var colliders = Physics.OverlapSphere(potentialConnectionPoint, 0.1f, LayerMask.GetMask("Obstacle"));

        if (!colliders.Contains(searchingCollider))
        {
            if (Mathf.Abs(angle) < maxRotationDuringSuck)
            {
                return FindSuckAngle(searchingCollider, pinnedPoint, differenceVector, angle + Mathf.Sign(angle));
            }
            return null;
        }

        var closiestToPinnedPoint = Physics.ClosestPoint(pinnedPoint, searchingCollider, searchingCollider.transform.position, searchingCollider.transform.rotation);
        var closiestToMovingPoint = Physics.ClosestPoint(potentialConnectionPoint, searchingCollider, searchingCollider.transform.position, searchingCollider.transform.rotation);

        var middlePoint = Vector3.Lerp(closiestToMovingPoint, closiestToPinnedPoint, 0.5f);
        var relativePos = middlePoint - transform.position;

        var finalAngle = Quaternion.LookRotation(Vector3.forward, relativePos);
        angle = Quaternion.Angle(finalAngle, transform.rotation);

        return angle;
    }

    //private float? FindSuckAngle(Collider searchingSolider, float angle)
    //{
    //    var suckerPositions = GetSuckerPositions(angle);

    //    var rightCollider = Physics.OverlapSphere(suckerPositions.right, 0.1f, LayerMask.GetMask("Obstacle"));
    //    var leftCollider = Physics.OverlapSphere(suckerPositions.left, 0.1f, LayerMask.GetMask("Obstacle"));

    //    if (rightCollider.FirstOrDefault(x => leftCollider.Contains(x)) != searchingSolider)
    //    {
    //        if (Mathf.Abs(angle) < maxRotationDuringSuck)
    //        {
    //            return FindSuckAngle(searchingSolider, angle + Mathf.Sign(angle));
    //        }
    //        return null;
    //    }

    //    return angle;
    //} 

    private (Vector3 left, Vector3 right) GetSuckerPositions(float angle = 0)
    {
        var rotationOfSuck = transform.rotation * Quaternion.Euler(0, 0, angle);
        var suckMiddlePosition = transform.position + rotationOfSuck * Vector3.up * suckerPosition.y;
        var suckRightPositions = suckMiddlePosition + (rotationOfSuck * Vector2.right * suckerPosition.x);
        var suckLeftPosition = suckMiddlePosition + (rotationOfSuck * Vector2.left * suckerPosition.x);
        return (suckLeftPosition, suckRightPositions);
    }

    private void OnDrawGizmos()
    {
        var springWorldPosition = transform.position + transform.rotation * springConnectionPosition;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(springWorldPosition, 0.5f);

        Gizmos.color = Color.yellow;
        var suckerPositions = GetSuckerPositions();
        
        Gizmos.DrawWireSphere(suckerPositions.left, 0.1f);
        Gizmos.DrawWireSphere(suckerPositions.right, 0.1f);
    }
}
