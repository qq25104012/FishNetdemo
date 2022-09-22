using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    [SerializeField] HingeJoint joint;

    private void OnCollisionEnter(Collision collision)
    {
        joint.connectedBody = null;
    }
}
