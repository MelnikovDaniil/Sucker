using System;
using UnityEngine;

public class ChainElement : MonoBehaviour
{
    [NonSerialized]
    public SpringJoint2D springJoint;
    [NonSerialized]
    public SpringJoint2D suckerJoint;
    [NonSerialized]
    public CapsuleCollider2D capsuleCollider;
}
