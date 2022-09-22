//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RopeGenerator : MonoBehaviour
//{
//    [SerializeField] float forwardForce = 1;
//    [SerializeField] float upwardForce = 1f;

//    [SerializeField] Camera cam;

//    [SerializeField] string spherePrefabName;

//    [Header("Raycast Settings")]

//    [SerializeField] LayerMask raycastLayers;

//    [SerializeField] float maxDistance = 1f;

//    [Header("Rope Settings")]

//    [SerializeField] Transform target;

//    [SerializeField] Spider spiderScript;

//    [SerializeField] GameObject spherePrefab;

//    [SerializeField] Transform startPoint;

//    [SerializeField] int amountOfPointsOffset = 0;

//    [SerializeField] Vector3 ropeOffset = Vector3.zero;

//    [SerializeField] float ropeSize = 0.25f;

//    [SerializeField] float distanceBetweenPoints = 1f;

//    //SOUND
//    private FMOD.Studio.EventInstance spiderPullSound;
//    private FMOD.Studio.EventInstance spiderHangSound;

//    List<GameObject> ropePoints = new List<GameObject>();

//    List<GameObject> endPoints = new List<GameObject>();

//    Rigidbody lastRopePointRB;

//    Vector3 instantiatePosition;

//    float lerpValue = 0f;

//    float lerpDistanceToAdd = 0f;

//    bool canSwing = true;

//    bool isCurrentlySwinging = false;

//    bool isForwardPressed = false;

//    bool canMove = false;

//    private void OnEnable()
//    {
//        EventSystemNew.Subscribe(Event_Type.COLLIDED, DestroyRope);
//        EventSystemNew.Subscribe(Event_Type.PRE_GAME, GameStarted);
//        EventSystemNew.Subscribe(Event_Type.GAME_ENDED, GameEnded);

//        // Input Events
//        EventSystemNew<bool>.Subscribe(Event_Type.Swing, Swing);
//        EventSystemNew<bool>.Subscribe(Event_Type.RopeForward, RopeForwardState);

//        if (GameManager.Instance.gameStarted && !GameManager.Instance.gameEnded)
//        {
//            canMove = true;
//        }
//        else
//        {
//            canMove = false;
//        }
//    }

//    private void OnDisable()
//    {
//        EventSystemNew.Unsubscribe(Event_Type.COLLIDED, DestroyRope);
//        EventSystemNew.Unsubscribe(Event_Type.PRE_GAME, GameStarted);
//        EventSystemNew.Unsubscribe(Event_Type.GAME_ENDED, GameEnded);

//        // Input Events
//        EventSystemNew<bool>.Unsubscribe(Event_Type.Swing, Swing);
//        EventSystemNew<bool>.Unsubscribe(Event_Type.RopeForward, RopeForwardState);
//    }

//    private void OnDestroy()
//    {
//        if (endPoints.Count > 0)
//        {
//            foreach (var ropePoint in ropePoints)
//            {
//                PhotonNetwork.Destroy(ropePoint);
//            }

//            foreach (var endPoint in endPoints)
//            {
//                PhotonNetwork.Destroy(endPoint);
//            }
//        }
//    }

//    private void Update()
//    {
//        if (!PV.IsMine)
//            return;

//        if (!canMove)
//        {
//            return;
//        }

//        if (isCurrentlySwinging && isForwardPressed)
//        {
//            lastRopePointRB.AddForce((cam.transform.forward * forwardForce) + (cam.transform.up * upwardForce));
//        }
//    }

//    private void Swing(bool _isSwinging)
//    {
//        if (!PV.IsMine)
//        {
//            return;
//        }

//        if (_isSwinging && canSwing && !spiderScript.IsGrounded())
//        {
//            canSwing = false;

//            RaycastRope();
//        }
//        else if (!_isSwinging && endPoints.Count > 0)
//        {
//            DestroyRope();
//        }
//    }

//    private void RopeForwardState(bool _isPressed)
//    {
//        if (!PV.IsMine)
//        {
//            return;
//        }

//        isForwardPressed = _isPressed;
//    }

//    private void DestroyRope()
//    {
//        if (!PV.IsMine)
//            return;

//        EventSystemNew<bool>.RaiseEvent(Event_Type.IS_SWINGING, false);

//        PV.RPC("RPC_SyncTarget", RpcTarget.All, target.GetComponent<PhotonView>().ViewID, false);
//        PV.RPC("RPC_UnParentTarget", RpcTarget.All, target.GetComponent<PhotonView>().ViewID);

//        StartCoroutine(DeleteRope());

//        //SOUND
//        spiderHangSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

//        isCurrentlySwinging = false;
//    }

//    private void RaycastRope()
//    {
//        RaycastHit rayHit;

//        if (Physics.Raycast(startPoint.position, startPoint.forward, out rayHit, maxDistance, raycastLayers, QueryTriggerInteraction.Ignore))
//        {
//            GenerateRope(rayHit.point);
//        }
//        else
//        {
//            canSwing = true;
//        }
//    }

//    private void GenerateRope(Vector3 _endPointTransform)
//    {
//        //SOUND
//        spiderPullSound = FMODUnity.RuntimeManager.CreateInstance("event:/SpiderPull");
//        spiderPullSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
//        spiderPullSound.start();
//        spiderHangSound = FMODUnity.RuntimeManager.CreateInstance("event:/SpiderHang");
//        spiderHangSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
//        spiderHangSound.start();

//        // Rope Generation End to Start

//        Vector3 newEndPointTransform = _endPointTransform + ropeOffset;

//        EventSystemNew<bool>.RaiseEvent(Event_Type.IS_SWINGING, true);

//        // Create an endpoint to attach the joint to
//        GameObject endPoint = PhotonNetwork.Instantiate("EndPoint", newEndPointTransform, Quaternion.identity);

//        endPoints.Add(endPoint);

//        lerpValue = 0f;

//        float distance = Vector3.Distance(startPoint.position, endPoint.transform.position);

//        int amountOfPoints = (int)Mathf.Ceil(distance / distanceBetweenPoints);

//        amountOfPoints -= amountOfPointsOffset;

//        lerpDistanceToAdd = 1f / amountOfPoints;

//        // Create all the points between the two vectors
//        // i < amountOfPoints + 1
//        for (int i = 0; i < amountOfPoints + 1; i++)
//        {
//            instantiatePosition = Vector3.Lerp(endPoint.transform.position, startPoint.position, lerpValue);

//            GameObject ropePoint = PhotonNetwork.Instantiate(spherePrefabName, instantiatePosition, Quaternion.identity);

//            if (i == 0)
//            {
//                PV.RPC("RPC_SyncRopePoint", RpcTarget.All, ropePoint.GetComponent<PhotonView>().ViewID, endPoint.GetComponent<PhotonView>().ViewID, ropeSize);
//            }
//            else
//            {
//                PV.RPC("RPC_SyncRopePoint", RpcTarget.All, ropePoint.GetComponent<PhotonView>().ViewID, ropePoints[i - 1].GetComponent<PhotonView>().ViewID, ropeSize);
//            }

//            // i == amountOfPoints - 1
//            if (i == amountOfPoints)
//            {
//                lastRopePointRB = ropePoint.GetComponent<Rigidbody>();

//                PV.RPC("RPC_ParentTarget", RpcTarget.All, target.GetComponent<PhotonView>().ViewID, ropePoint.GetComponent<PhotonView>().ViewID, ropeSize);
//            }

//            ropePoints.Add(ropePoint);

//            lerpValue += lerpDistanceToAdd;
//        }

//        isCurrentlySwinging = true;
//    }

//    [PunRPC]
//    public void RPC_SyncRopePoint(int _ropePointID, int _otherRopePointID, float _ropeSize)
//    {
//        Transform ropePoint = PhotonView.Find(_ropePointID).transform;
//        Transform otherRopePoint = PhotonView.Find(_otherRopePointID).transform;

//        ropePoint.transform.localScale = new Vector3(_ropeSize, _ropeSize, _ropeSize);

//        ropePoint.GetComponent<HingeJoint>().connectedBody = otherRopePoint.GetComponent<Rigidbody>();
//    }

//    [PunRPC]
//    public void RPC_SyncTarget(int _targetID, bool _isKinematic)
//    {
//        PhotonView.Find(_targetID).GetComponent<Rigidbody>().isKinematic = _isKinematic;
//    }

//    [PunRPC]
//    public void RPC_ParentTarget(int _targetID, int _ropePointID, float _ropeSize)
//    {
//        Transform target = PhotonView.Find(_targetID).transform;
//        Transform ropePoint = PhotonView.Find(_ropePointID).transform;

//        ropePoint.transform.localScale = new Vector3(_ropeSize, _ropeSize, _ropeSize);

//        target.SetParent(ropePoint);
//        target.GetComponent<Rigidbody>().isKinematic = true;
//    }

//    [PunRPC]
//    public void RPC_UnParentTarget(int _targetID)
//    {
//        PhotonView.Find(_targetID).transform.SetParent(null);

//        if (PV.IsMine)
//        {
//            StartCoroutine(DeleteRope());
//        }
//    }

//    private IEnumerator DeleteRope()
//    {
//        yield return new WaitForSeconds(0.25f);

//        foreach (var ropePoint in ropePoints)
//        {
//            PhotonNetwork.Destroy(ropePoint);
//        }

//        foreach (var endPoint in endPoints)
//        {
//            PhotonNetwork.Destroy(endPoint);
//        }

//        ropePoints.Clear();

//        endPoints.Clear();

//        yield return new WaitForSeconds(0.25f);

//        canSwing = true;
//    }

//    private void GameStarted()
//    {
//        canMove = true;
//    }

//    private void GameEnded()
//    {
//        canMove = false;
//    }
//}
