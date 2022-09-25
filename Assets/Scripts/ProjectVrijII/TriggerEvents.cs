using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
    public void GameEnded()
    {
        EventSystemNew.RaiseEvent(Event_Type.GAME_ENDED);
    }
}
