///* 
// * This file is part of Unity-Procedural-IK-Wall-Walking-Spider on github.com/PhilS94
// * Copyright (C) 2020 Philipp Schofield - All Rights Reserved
// */

//using UnityEngine;
//using System.Collections;
//using Raycasting;
//using UnityEngine.InputSystem;
//using Photon.Realtime;
//using Photon.Pun;
//using ExitGames.Client.Photon;

///*
// * This class needs a reference to the Spider class and calls the walk and turn functions depending on player input.
// * So in essence, this class translates player input to spider movement. The input direction is relative to a camera and so a 
// * reference to one is needed.
// */

//[DefaultExecutionOrder(-1)] // Make sure the players input movement is applied before the spider itself will do a ground check and possibly add gravity
//public class SpiderController : MonoBehaviour
//{

//    [Header("Settings")]

//    [SerializeField] float isFallingCheckTime = 0.25f;

//    [SerializeField] private float smoothInputSpeed = 0.2f;

//    public Spider spider;

//    [Header("Camera")]
//    public SmoothCamera smoothCam;

//    [SerializeField] private PhotonView PV;

//    Vector2 moveInput = Vector2.zero;
//    Vector2 currentInputVector = Vector2.zero;
//    Vector2 smoothInputVelocity = Vector2.zero;

//    bool isFalling = false;

//    bool canMove = false;

//    private void OnEnable()
//    {
//        EventSystemNew.Subscribe(Event_Type.PRE_GAME, GameStarted);
//        EventSystemNew.Subscribe(Event_Type.GAME_ENDED, GameEnded);

//        // Input Events
//        EventSystemNew<float, float>.Subscribe(Event_Type.Move, Move);
//        EventSystemNew.Subscribe(Event_Type.Jump, Jump);
//        EventSystemNew<bool>.Subscribe(Event_Type.Fall, Fall);
//        EventSystemNew.Subscribe(Event_Type.ForceRespawn, ForceRespawn);

//        if (GameManager.Instance.gameStarted || GameManager.Instance.preGame && !GameManager.Instance.gameEnded)
//        {
//            canMove = true;

//        }
//        else if (GameManager.Instance.gameEnded)
//        {
//            canMove = false;

//            smoothCam.XSensitivity = 0;
//            smoothCam.YSensitivity = 0;
//        }
//    }

//    private void OnDisable()
//    {
//        EventSystemNew.Unsubscribe(Event_Type.PRE_GAME, GameStarted);
//        EventSystemNew.Unsubscribe(Event_Type.GAME_ENDED, GameEnded);

//        // Input Events
//        EventSystemNew<float, float>.Unsubscribe(Event_Type.Move, Move);
//        EventSystemNew.Unsubscribe(Event_Type.Jump, Jump);
//        EventSystemNew<bool>.Unsubscribe(Event_Type.Fall, Fall);
//        EventSystemNew.Unsubscribe(Event_Type.ForceRespawn, ForceRespawn);
//    }

//    void FixedUpdate()
//    {
//        //** Movement **//
//        Vector3 input = getInput();

//        spider.walk(input, moveInput);

//        Quaternion tempCamTargetRotation = smoothCam.getCamTargetRotation();
//        Vector3 tempCamTargetPosition = smoothCam.getCamTargetPosition();
//        spider.turn(input, moveInput);
//        smoothCam.setTargetRotation(tempCamTargetRotation);
//        smoothCam.setTargetPosition(tempCamTargetPosition);

//        if (isFalling)
//        {
//            if (spider.GroundCheckFalling())
//            {
//                isFalling = false;

//                spider.setGroundcheck(true);

//                CancelInvoke();
//            }
//        }
//    }

//    private Vector3 getInput()
//    {
//        if (canMove)
//        {
//            currentInputVector = Vector2.SmoothDamp(currentInputVector, moveInput, ref smoothInputVelocity, smoothInputSpeed);

//            Vector3 up = spider.transform.up;
//            Vector3 right = spider.transform.right;
//            Vector3 input = Vector3.ProjectOnPlane(smoothCam.getCameraTarget().forward, up).normalized * currentInputVector.y + (Vector3.ProjectOnPlane(smoothCam.getCameraTarget().right, up).normalized * currentInputVector.x);
//            Quaternion fromTo = Quaternion.AngleAxis(Vector3.SignedAngle(up, spider.getGroundNormal(), right), right);
//            input = fromTo * input;
//            float magnitude = input.magnitude;
//            return (magnitude <= 1) ? input : input /= magnitude;
//        }

//        return Vector3.zero;
//    }

//    #region Input Events
//    private void Move(float _x, float _y)
//    {
//        if (!canMove)
//        {
//            return;
//        }

//        moveInput = new Vector2(_x, _y);
//    }

//    private void Jump()
//    {
//        if (!canMove)
//        {
//            return;
//        }

//        spider.Jump();
//    }

//    private void Fall(bool _isFalling)
//    {
//        if (!canMove)
//        {
//            return;
//        }

//        if (_isFalling && spider.IsGrounded())
//        {
//            Invoke("SetFalling", isFallingCheckTime);

//            spider.setGroundcheck(false);
//        }
//        else if (!_isFalling)
//        {
//            isFalling = false;

//            spider.setGroundcheck(true);

//            CancelInvoke();
//        }
//    }

//    private void ForceRespawn()
//    {
//        object[] content = new object[] { PV.ViewID, false };

//        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

//        PhotonNetwork.RaiseEvent((int)Event_Code.DestroySpider, content, raiseEventOptions, SendOptions.SendReliable);
//    }
//    #endregion

//    private void SetFalling()
//    {
//        isFalling = true;
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