using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class RopeGenerator : NetworkBehaviour
{
    [SerializeField] private float forwardForce = 1;
    [SerializeField] private float upwardForce = 1f;

    [SerializeField] private Camera cam;

    [Header("Raycast Settings")]

    [SerializeField] private LayerMask raycastLayers;

    [SerializeField] private float maxDistance = 1f;

    [Header("Rope Settings")]

    [SerializeField] private Transform target;

    [SerializeField] private Spider spiderScript;

    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private GameObject endPointPrefab;

    [SerializeField] private Transform startPoint;

    [SerializeField] private int amountOfPointsOffset = 0;

    [SerializeField] private Vector3 ropeOffset = Vector3.zero;

    [SerializeField] private float ropeSize = 0.25f;

    [SerializeField] private float distanceBetweenPoints = 1f;

    private List<GameObject> ropePoints = new List<GameObject>();

    private GameObject endPoint;

    private Rigidbody lastRopePointRB;

    private Vector3 instantiatePosition;

    private float lerpValue = 0f;

    private float lerpDistanceToAdd = 0f;

    private bool canSwing = true;

    private bool isCurrentlySwinging = false;

    private bool isForwardPressed = false;

    private bool canMove = false;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.COLLIDED, DestroyRope);
        EventSystemNew.Subscribe(Event_Type.GAME_STARTED, GameStarted);
        EventSystemNew.Subscribe(Event_Type.GAME_ENDED, GameEnded);

        // Input Events
        EventSystemNew<bool>.Subscribe(Event_Type.Swing, Swing);
        EventSystemNew<bool>.Subscribe(Event_Type.RopeForward, RopeForwardState);

        if (GameState.Instance.gameStarted && !GameState.Instance.gameEnded)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.COLLIDED, DestroyRope);
        EventSystemNew.Unsubscribe(Event_Type.GAME_STARTED, GameStarted);
        EventSystemNew.Unsubscribe(Event_Type.GAME_ENDED, GameEnded);

        // Input Events
        EventSystemNew<bool>.Unsubscribe(Event_Type.Swing, Swing);
        EventSystemNew<bool>.Unsubscribe(Event_Type.RopeForward, RopeForwardState);
    }

    private void OnDestroy()
    {
        if (!IsServer) return;

        if (endPoint != null)
        {
            foreach (var ropePoint in ropePoints)
            {
                InstanceFinder.ServerManager.Despawn(ropePoint);
            }

            InstanceFinder.ServerManager.Despawn(endPoint);
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        if (!canMove)
        {
            return;
        }

        if (isCurrentlySwinging && isForwardPressed)
        {
            lastRopePointRB.AddForce((cam.transform.forward * forwardForce) + (cam.transform.up * upwardForce));
        }
    }

    private void Swing(bool _isSwinging)
    {
        if (_isSwinging && canSwing && spiderScript.IsGrounded())
        {
            Debug.Log("SWING");

            canSwing = false;

            RaycastRope();
        }
        else if (!_isSwinging && endPoint != null)
        {
            DestroyRope();
        }
    }

    private void RopeForwardState(bool _isPressed)
    {
        isForwardPressed = _isPressed;
    }

    private void DestroyRope()
    {
        if (!IsServer) return;

        EventSystemNew<bool>.RaiseEvent(Event_Type.IS_SWINGING, false);

        target.GetComponent<Rigidbody>().isKinematic = false;
        target.transform.SetParent(null);

        StartCoroutine(DeleteRope());

        isCurrentlySwinging = false;
    }

    private void RaycastRope()
    {
        RaycastHit rayHit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out rayHit, maxDistance, raycastLayers, QueryTriggerInteraction.Ignore))
        {
            GenerateRope(rayHit.point);
        }
        else
        {
            canSwing = true;
        }
    }

    private void GenerateRope(Vector3 _endPointTransform)
    {
        if (!IsServer) return;

        // Rope Generation End to Start
        Vector3 newEndPointTransform = _endPointTransform + ropeOffset;

        EventSystemNew<bool>.RaiseEvent(Event_Type.IS_SWINGING, true);

        // Create an endpoint to attach the joint to
        GameObject endPoint = Instantiate(endPointPrefab, newEndPointTransform, Quaternion.identity);
        Spawn(endPoint);

        lerpValue = 0f;

        float distance = Vector3.Distance(startPoint.position, endPoint.transform.position);

        int amountOfPoints = (int)Mathf.Ceil(distance / distanceBetweenPoints);

        amountOfPoints -= amountOfPointsOffset;

        lerpDistanceToAdd = 1f / amountOfPoints;

        // Create all the points between the two vectors
        // i < amountOfPoints + 1
        for (int i = 0; i < amountOfPoints + 1; i++)
        {
            instantiatePosition = Vector3.Lerp(endPoint.transform.position, startPoint.position, lerpValue);

            GameObject ropePoint = Instantiate(spherePrefab, instantiatePosition, Quaternion.identity);
            Spawn(ropePoint);

            ropePoint.transform.localScale = new Vector3(ropeSize, ropeSize, ropeSize);

            if (i == 0)
            {
                ropePoint.GetComponent<HingeJoint>().connectedBody = endPoint.GetComponent<Rigidbody>();
            }
            else
            {
                ropePoint.GetComponent<HingeJoint>().connectedBody = ropePoints[i - 1].GetComponent<Rigidbody>();
            }

            // i == amountOfPoints - 1
            if (i == amountOfPoints)
            {
                lastRopePointRB = ropePoint.GetComponent<Rigidbody>();

                ropePoint.transform.localScale = new Vector3(ropeSize, ropeSize, ropeSize);

                target.SetParent(ropePoint.transform);
                target.GetComponent<Rigidbody>().isKinematic = true;
            }

            ropePoints.Add(ropePoint);

            lerpValue += lerpDistanceToAdd;
        }

        isCurrentlySwinging = true;
    }

    private IEnumerator DeleteRope()
    {
        yield return new WaitForSeconds(0.25f);

        foreach (var ropePoint in ropePoints)
        {
            InstanceFinder.ServerManager.Despawn(ropePoint);
        }

        InstanceFinder.ServerManager.Despawn(endPoint);

        ropePoints.Clear();

        yield return new WaitForSeconds(0.25f);

        canSwing = true;
    }

    private void GameStarted()
    {
        canMove = true;
    }

    private void GameEnded()
    {
        canMove = false;
    }
}
