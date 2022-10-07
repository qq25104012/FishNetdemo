using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiderInputHandler : NetworkBehaviour
{
    public void OnMoveInput(InputAction.CallbackContext _context)
    {
        Vector2 input = _context.ReadValue<Vector2>();
        EventSystemNew<float, float>.RaiseEvent(Event_Type.Move, input.x, input.y);
    }

    public void OnJumpInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            EventSystemNew.RaiseEvent(Event_Type.Jump);
        }
    }

    public void OnShootInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            EventSystemNew.RaiseEvent(Event_Type.Shoot);
        }
    }

    public void OnSwingInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.Swing, true);
        }
        else if (_context.phase == InputActionPhase.Canceled)
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.Swing, false);
        }
    }

    public void OnFallInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.Fall, true);
        }
        else if (_context.phase == InputActionPhase.Canceled)
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.Fall, false);
        }
    }

    public void OnChangeSpectatorInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            float value = _context.ReadValue<Vector2>().x;

            EventSystemNew<int>.RaiseEvent(Event_Type.ChangeSpectator, (int)value);
        }
    }

    public void OnForceRespawnInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            EventSystemNew.RaiseEvent(Event_Type.ForceRespawn);
        }
    }

    public void OnRopeForwardInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.RopeForward, true);
        }
        else if (_context.phase == InputActionPhase.Canceled)
        {
            EventSystemNew<bool>.RaiseEvent(Event_Type.RopeForward, false);
        }
    }
}
