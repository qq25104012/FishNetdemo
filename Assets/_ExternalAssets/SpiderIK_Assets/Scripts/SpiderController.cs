/* 
 * This file is part of Unity-Procedural-IK-Wall-Walking-Spider on github.com/PhilS94
 * Copyright (C) 2020 Philipp Schofield - All Rights Reserved
 */

using UnityEngine;
using System.Collections;
using Raycasting;
using UnityEngine.InputSystem;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;

/*
 * This class needs a reference to the Spider class and calls the walk and turn functions depending on player input.
 * So in essence, this class translates player input to spider movement. The input direction is relative to a camera and so a 
 * reference to one is needed.
 */

[DefaultExecutionOrder(-1)] // Make sure the players input movement is applied before the spider itself will do a ground check and possibly add gravity
public class SpiderController : NetworkBehaviour
{
    #region Types
    public struct MoveData
    {
        public Vector3 Input;
        public float Horizontal;
        public float Vertical;
        public bool Jump;
        public MoveData(Vector3 _input, float _horizontal, float _vertical, bool _jump)
        {
            Input = _input;
            Horizontal = _horizontal;
            Vertical = _vertical;
            Jump = _jump;
        }
    }
    public struct ReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public ReconcileData(Vector3 _position, Quaternion _rotation, Vector3 _velocity, Vector3 _angularVelocity)
        {
            Position = _position;
            Rotation = _rotation;
            Velocity = _velocity;
            AngularVelocity = _angularVelocity;
        }
    }
    #endregion

    [Header("Settings")]
    [SerializeField] float isFallingCheckTime = 0.25f;

    [SerializeField] private float smoothInputSpeed = 0.2f;

    [SerializeField] Spider spider;

    [Header("Camera")]
    public SmoothCamera smoothCam;

    Vector2 moveInput = Vector2.zero;
    Vector2 currentInputVector = Vector2.zero;
    Vector2 smoothInputVelocity = Vector2.zero;

    bool isFalling = false;

    public bool canMove = false;

    private bool jumpQueued = false;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.GAME_STARTED, GameStarted);
        EventSystemNew.Subscribe(Event_Type.GAME_ENDED, GameEnded);

        // Input Events
        EventSystemNew<float, float>.Subscribe(Event_Type.Move, Move);
        EventSystemNew.Subscribe(Event_Type.Jump, Jump);
        EventSystemNew<bool>.Subscribe(Event_Type.Fall, Fall);
        EventSystemNew.Subscribe(Event_Type.ForceRespawn, ForceRespawn);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.GAME_STARTED, GameStarted);
        EventSystemNew.Unsubscribe(Event_Type.GAME_ENDED, GameEnded);

        // Input Events
        EventSystemNew<float, float>.Unsubscribe(Event_Type.Move, Move);
        EventSystemNew.Unsubscribe(Event_Type.Jump, Jump);
        EventSystemNew<bool>.Unsubscribe(Event_Type.Fall, Fall);
        EventSystemNew.Unsubscribe(Event_Type.ForceRespawn, ForceRespawn);
    }

    private void Awake()
    {
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;

        if (GameState.Instance.gameStarted && !GameState.Instance.gameEnded)
        {
            canMove = true;
        }
        else if (GameState.Instance.gameEnded)
        {
            canMove = false;

            smoothCam.XSensitivity = 0;
            smoothCam.YSensitivity = 0;
        }
    }

    private void OnDestroy()
    {
        if (InstanceFinder.TimeManager != null)
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (isFalling)
        {
            if (spider.GroundCheckFalling())
            {
                isFalling = false;

                spider.setGroundcheck(true);

                CancelInvoke();
            }
        }
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliation(default, false);
            CheckInput(out MoveData md);
            Move(md, false);
        }
        if (base.IsServer)
        {
            Move(default, true);
        }
    }

    private void TimeManager_OnPostTick()
    {
        if (base.IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, spider.rb.velocity, spider.rb.angularVelocity);
            Reconciliation(rd, true);
        }
    }

    private void CheckInput(out MoveData md)
    {
        md = default;

        Vector3 input = getInput();

        md = new MoveData(input, moveInput.x, moveInput.y, jumpQueued);
        jumpQueued = false;
    }

    [Replicate]
    private void Move(MoveData md, bool asServer, bool replaying = false)
    {
        //** Movement **//
        spider.walk(md.Input, moveInput);

        if (IsOwner)
        {
            Quaternion tempCamTargetRotation = smoothCam.getCamTargetRotation();
            Vector3 tempCamTargetPosition = smoothCam.getCamTargetPosition();
            smoothCam.setTargetRotation(tempCamTargetRotation);
            smoothCam.setTargetPosition(tempCamTargetPosition);
        }

        spider.turn(md.Input, moveInput);

        if (md.Jump)
            spider.Jump();
    }

    [Reconcile]
    private void Reconciliation(ReconcileData rd, bool asServer)
    {
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;
        spider.rb.velocity = rd.Velocity;
        spider.rb.angularVelocity = rd.AngularVelocity;
    }

    private Vector3 getInput()
    {
        if (canMove)
        {
            currentInputVector = Vector2.SmoothDamp(currentInputVector, moveInput, ref smoothInputVelocity, smoothInputSpeed);

            Vector3 up = spider.transform.up;
            Vector3 right = spider.transform.right;
            Vector3 input = Vector3.ProjectOnPlane(smoothCam.getCameraTarget().forward, up).normalized * currentInputVector.y + (Vector3.ProjectOnPlane(smoothCam.getCameraTarget().right, up).normalized * currentInputVector.x);
            Quaternion fromTo = Quaternion.AngleAxis(Vector3.SignedAngle(up, spider.getGroundNormal(), right), right);
            input = fromTo * input;
            float magnitude = input.magnitude;
            return (magnitude <= 1) ? input : input /= magnitude;
        }

        return Vector3.zero;
    }

    #region Input Events
    private void Move(float _x, float _y)
    {
        if (!canMove)
        {
            return;
        }

        moveInput = new Vector2(_x, _y);
    }

    private void Jump()
    {
        if (!canMove)
        {
            return;
        }

        spider.Jump();
    }

    private void Fall(bool _isFalling)
    {
        if (!canMove)
        {
            return;
        }

        if (_isFalling && spider.IsGrounded())
        {
            Invoke("SetFalling", isFallingCheckTime);

            spider.setGroundcheck(false);
        }
        else if (!_isFalling)
        {
            isFalling = false;

            spider.setGroundcheck(true);

            CancelInvoke();
        }
    }

    private void ForceRespawn()
    {
        EventSystemNew.RaiseEvent(Event_Type.Respawn_Player);
    }
    #endregion

    private void SetFalling()
    {
        isFalling = true;
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