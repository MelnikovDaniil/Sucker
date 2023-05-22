using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Sucker : MonoBehaviour
{
    public event Action<Obstacle> OnSuckObstacle;
    public event Action<Block> OnSuckBlock;
    public event Action OnUnSuck;
    public event Action OnUnSuckTry;
    public Vector3 springConnectionPosition;
    public ParticleSystem suckParticles;

    [Space]
    public Vector3 suckerPosition;
    public float maxRotationDuringSuck;

    [Space]
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    [NonSerialized]
    public bool isSucked;
    [NonSerialized]
    public bool ableToSuck;
    [NonSerialized]
    public bool ableToUnSuck;

    [NonSerialized]
    public Rigidbody2D rigidbody;

    private Animator animator;
    private FixedJoint2D connectionPlace;

    private void Awake()
    {
        var suckerObj = transform.GetChild(0).GetChild(0);

        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        meshFilter = suckerObj.GetComponent<MeshFilter>();
        meshRenderer = suckerObj.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        ableToSuck = true;
        ableToUnSuck = true;
    }

    public void UnSuck()
    {
        if (connectionPlace != null)
        {
            if (ableToUnSuck)
            {
                //transform.parent = chain;
                //rigidbody.isKinematic = false;
                animator.SetTrigger("unsuck");
                StartCoroutine(UnSuckRoutine());
                OnUnSuck?.Invoke();
            }
            else
            {
                OnUnSuckTry?.Invoke();
            }
        }
    }

    public void Disconnect()
    {
        isSucked = false;
        ableToSuck = true;
        Destroy(connectionPlace);
        connectionPlace = null;
    }

    private IEnumerator UnSuckRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        ableToSuck = true;
    }

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (ableToSuck && !isSucked 
    //        && other.attachedRigidbody != null)
    //    {
    //        if (other.attachedRigidbody.gameObject.TryGetComponent(out Obstacle obstacle))
    //        {
    //            StartCoroutine(SuckObstacleRoutine(other, obstacle));
    //        }
    //        else
    //        {
                
    //        }
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ableToSuck && !isSucked
            && other.attachedRigidbody?.gameObject != null)
        {
            if (other.attachedRigidbody.gameObject.TryGetComponent(out Block block))
            {
                ConnectBlock(block);
            }
            else if (other.attachedRigidbody.gameObject.TryGetComponent(out Obstacle obstacle))
            {
                StartCoroutine(SuckObstacleRoutine(other, obstacle));
            }
        }
    }

    private void ConnectBlock(Block block)
    {
        ableToSuck = false;
        isSucked = true;

        connectionPlace = block.gameObject.AddComponent<FixedJoint2D>();
        connectionPlace.connectedBody = rigidbody;
        connectionPlace.autoConfigureConnectedAnchor = false;

        animator.SetTrigger("suck");
        suckParticles.Play();

        block.Connect(this);
        OnSuckBlock?.Invoke(block);
    }

    private IEnumerator SuckObstacleRoutine(Collider2D collider, Obstacle obstacle)
    {
        var leftAngle = CalculateRotationAngle(collider, false) ?? float.MaxValue;
        var rightAngle = CalculateRotationAngle(collider, true) ?? float.MaxValue;

        var rotationAngle = Mathf.Abs(leftAngle) >= Mathf.Abs(rightAngle) ? rightAngle : leftAngle;

        if (rotationAngle != float.MaxValue)
        {
            ableToSuck = false;
            isSucked = true;

            var previousSuckPositions = GetSuckerPositions();
            var nextRotationPositions = GetSuckerPositions(rotationAngle);
            var previousMiddleSuckPoint = Vector2.Lerp(previousSuckPositions.left, previousSuckPositions.right, 0.5f);
            var nextMiddleSuckPoint = Vector2.Lerp(nextRotationPositions.left, nextRotationPositions.right, 0.5f);
            var closiestPoint = Physics2D.ClosestPoint(nextMiddleSuckPoint, collider);
            Vector3 moveOffset = closiestPoint - previousMiddleSuckPoint;
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationAngle);
            transform.position += moveOffset;
            rigidbody.velocity = Vector3.zero;

            yield return new WaitForEndOfFrame();
            connectionPlace = new GameObject($"ConnectionPlace of {this.name}", typeof(FixedJoint2D)).GetComponent<FixedJoint2D>();
            var placeRigidbody = connectionPlace.GetComponent<Rigidbody2D>();

            connectionPlace.autoConfigureConnectedAnchor = false;
            placeRigidbody.isKinematic = true;
            //connectionPlace.dampingRatio = 1;
            //connectionPlace.frequency = 300f;
            connectionPlace.transform.parent = obstacle.transform;
            //connectionPlace.transform.localScale = Vector3.one;
            connectionPlace.transform.position = closiestPoint;
            //connectionPlace.connectedAnchor = (closiestPoint - (Vector2)transform.position);
            connectionPlace.connectedBody = rigidbody;

            //connectionPlace.transform.localRotation = Quaternion.identity;
            //connectionPlace.autoConfigureConnectedAnchor = false;

            //transform.parent = connectionPlace.transform;

            //rigidbody.isKinematic = true;

            animator.SetTrigger("suck");
            suckParticles.Play();
            obstacle.Connect();
            OnSuckObstacle?.Invoke(obstacle);
        }
    }

    private float? CalculateRotationAngle(Collider2D searchingCollider, bool isRightPositionPined)
    {
        var suckerCurrentPositions = GetSuckerPositions();

        var pinnedPoint = isRightPositionPined ? suckerCurrentPositions.right : suckerCurrentPositions.left;
        var movingPoint = isRightPositionPined ? suckerCurrentPositions.left : suckerCurrentPositions.right;
        var colliders = Physics2D.OverlapCircleAll(pinnedPoint, 0.1f, LayerMask.GetMask("Obstacle"));
        
        if (colliders.Contains(searchingCollider))
        {
            var differenceVector = movingPoint - pinnedPoint;

            return FindSuckAngle(searchingCollider, pinnedPoint, differenceVector, isRightPositionPined ? 1 : -1);
        }

        return null;
    }

    private float? FindSuckAngle(Collider2D searchingCollider, Vector3 pinnedPoint, Vector3 differenceVector, float angle = 0)
    {
        var potentialConnectionPoint = pinnedPoint + Quaternion.Euler(0, 0, angle) * differenceVector;
        var colliders = Physics2D.OverlapCircleAll(potentialConnectionPoint, 0.1f, LayerMask.GetMask("Obstacle"));

        if (!colliders.Contains(searchingCollider))
        {
            if (Mathf.Abs(angle) < maxRotationDuringSuck)
            {
                return FindSuckAngle(searchingCollider, pinnedPoint, differenceVector, angle + Mathf.Sign(angle));
            }
            return null;
        }

        var closiestToPinnedPoint = Physics2D.ClosestPoint(pinnedPoint, searchingCollider);
        var closiestToMovingPoint = Physics2D.ClosestPoint(potentialConnectionPoint, searchingCollider);

        var middlePoint = Vector3.Lerp(closiestToMovingPoint, closiestToPinnedPoint, 0.5f);
        var relativePos = middlePoint - transform.position;

        var finalAngle = Quaternion.LookRotation(Vector3.forward, relativePos);
        angle = Quaternion.Angle(finalAngle, transform.rotation);

        return angle;
    }

    public (Vector3 left, Vector3 right) GetSuckerPositions(float angle = 0)
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
