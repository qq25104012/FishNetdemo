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
        #endregion

        #region Serialized.
        [SerializeField]
        private float _moveRate = 5f;
        [SerializeField]
        private float _smoothInputSpeed = 0.2f;
        [SerializeField]
        private CharacterController _characterController;
        #endregion

        #region Private.
        private Vector2 curMovementInput = Vector2.zero;
        private Vector2 smoothInputVelocity = Vector2.zero;
        private bool isMoving = false;
        #endregion

        private void Awake()
        {
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _characterController.enabled = (base.IsServer || base.IsOwner);
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

            //float horizontal = Input.GetAxisRaw("Horizontal");
            //float vertical = Input.GetAxisRaw("Vertical");

            if (curMovementInput == Vector2.zero)
                return;

            md = new MoveData()
            {
                Horizontal = curMovementInput.x,
                Vertical = curMovementInput.y
            };
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, bool replaying = false)
        {
            Vector3 move = new Vector3(md.Horizontal, 0f, md.Vertical).normalized + new Vector3(0f, Physics.gravity.y, 0f);
            _characterController.Move(move * _moveRate * (float)base.TimeManager.TickDelta);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
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
    }
}