using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChainElement : MonoBehaviour
{
    [NonSerialized]
    public SpringJoint springJoint;
    [NonSerialized]
    public SpringJoint suckerJoint;
    [NonSerialized]
    public CapsuleCollider capsuleCollider;
}
