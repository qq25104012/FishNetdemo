using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishNet.Example.Prediction.CharacterControllers
{
    public class PlayerController : NetworkBehaviour
    {
        #region Types.
        public struct MoveData
        {
            public float Horizontal;
            public float Vertical;
            public Quaternion Rotation;
            public bool jump;
        }
        public struct ReconcileData
        {
            public Vector3 Position;
            public ReconcileData(Vector3 position)
            {
                Position = position;
            }
        }
        #endregion

        #region Serialized.
        [Header("Player Move")]
        [SerializeField]
        private float gravity = -9.81f;
        [SerializeField]
        private float _moveRate = 5f;
        [SerializeField]
        private float _smoothInputSpeed = 0.2f;
        [SerializeField]
        private float jumpHeight = 2f;
        [SerializeField]
        private CharacterController _characterController;
        [SerializeField]
        private PlayerInput input;

        [Header("Player Look")]
        [SerializeField]
        Transform cameraHolder;
        [SerializeField]
        Transform playerTransform;
        [SerializeField]
        float minXLook;
        [SerializeField]
        float maxXLook;
        [SerializeField]
        float lookSensitivity;
        #endregion

        #region Private.
        private Vector2 curMovementInput = Vector2.zero;
        private Vector2 smoothInputVelocity = Vector2.zero;
        private float curCamRotX;
        private Vector2 mouseDelta;
        private bool isMoving = false;
        private bool jumpQueued = false;
        public Vector3 velocity = Vector3.zero;
        private float distanceToGround;
        #endregion

        private void Awake()
        {
            distanceToGround = _characterController.bounds.extents.y;

            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _characterController.enabled = (base.IsServer || base.IsOwner);
            input.enabled = (base.IsOwner);
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
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
                ReconcileData rd = new ReconcileData(transform.position);
                Reconciliation(rd, true);
            }
        }

        private void LateUpdate()
        {
            if (base.IsOwner)
            {
                curCamRotX += mouseDelta.y * lookSensitivity;
                curCamRotX = Mathf.Clamp(curCamRotX, minXLook, maxXLook);
                cameraHolder.localEulerAngles = new Vector3(-curCamRotX, 0, 0);

                playerTransform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
            }
        }

        private void FixedUpdate()
        {
            if (base.IsOwner)
            {
                if (!isMoving)
                {
                    curMovementInput = Vector2.SmoothDamp(curMovementInput, Vector2.zero, ref smoothInputVelocity, _smoothInputSpeed);
                }
            }
        }

        private void CheckInput(out MoveData md)
        {
            md = default;

            //if (curMovementInput == Vector2.zero)
            //    return;

            md = new MoveData()
            {
                Horizontal = curMovementInput.x,
                Vertical = curMovementInput.y,
                Rotation = transform.rotation,
                jump = jumpQueued,
            };

            jumpQueued = false;
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, bool replaying = false)
        {
            if (_characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
            }

            // Move
            Vector3 move = (transform.forward * md.Vertical + transform.right * md.Horizontal).normalized;
            _characterController.Move(move * _moveRate * (float)base.TimeManager.TickDelta);

            if (md.jump && IsGrounded())
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            }

            velocity.y += gravity * (float)base.TimeManager.TickDelta;
            _characterController.Move(velocity * (float)base.TimeManager.TickDelta);

            if (!base.IsOwner)
                transform.rotation = md.Rotation;
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
        }

        // Inputs
        public void OnMoveInput(InputAction.CallbackContext _context)
        {
            if (base.IsOwner)
            {
                if (_context.phase == InputActionPhase.Performed)
                {
                    isMoving = true;
                    curMovementInput = _context.ReadValue<Vector2>();
                }
                else if (_context.phase == InputActionPhase.Canceled)
                {
                    isMoving = false;
                }
            }
        }

        public void OnLookInput(InputAction.CallbackContext _context)
        {
            mouseDelta = _context.ReadValue<Vector2>();
        }

        public void OnJumpInput(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started)
            {
                jumpQueued = true;
            }
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
        }
    }
}