using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPoints : NetworkBehaviour
{
    public void AddPoints(int _points)
    {
        if (!IsServer) return;

        EventSystemNew<string, int>.RaiseEvent(Event_Type.UPDATE_SCORE, Owner.CustomData.ToString(), _points);
    }
}
