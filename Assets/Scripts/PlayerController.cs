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
        }
        public struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public ReconcileData(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
        //public struct ReconcileData
        //{
        //    public Vector3 Position;
        //    public ReconcileData(Vector3 position)
        //    {
        //        Position = position;
        //    }
        //}
        #endregion

        #region Serialized.
        [Header("Player Move")]
        [SerializeField]
        private float _moveRate = 5f;
        [SerializeField]
        private float _smoothInputSpeed = 0.2f;
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
        #endregion

        private void Awake()
        {
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
                ReconcileData rd = new ReconcileData(transform.position, transform.rotation);
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

                    Debug.Log("Input: " + curMovementInput);
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
            };
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, bool replaying = false)
        {
            // Move
            Vector3 move = (transform.forward * md.Vertical + transform.right * md.Horizontal).normalized;
            move.y = Physics.gravity.y;
            _characterController.Move(move * _moveRate * (float)base.TimeManager.TickDelta);

            if (!base.IsOwner)
                transform.rotation = md.Rotation;
            // Rotate
            //curCamRotX += md.RVertical * lookSensitivity;
            //curCamRotX = Mathf.Clamp(curCamRotX, minXLook, maxXLook);
            //cameraHolder.localEulerAngles = new Vector3(-curCamRotX, 0, 0);

            //playerTransform.eulerAngles += new Vector3(0, md.RHorizontal * lookSensitivity, 0);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
            //transform.rotation = rd.Rotation;
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

        public void OnLookInput(InputAction.CallbackContext context)
        {
            mouseDelta = context.ReadValue<Vector2>();
        }
    }
}