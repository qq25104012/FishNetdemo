using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartGame : MonoBehaviour
{
    [SerializeField] GameObject waitingForHostText;

    [SerializeField] GameObject[] objectToDisableOnStart;
    [SerializeField] GameObject[] objectToEnableOnStart;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.PRE_GAME, GameStarted);

        EventSystemNew.Subscribe(Event_Type.GAME_ENDED, GameEnded);

        if (GameManager.Instance.gameStarted && !GameManager.Instance.gameEnded)
        {
            foreach (var item in objectToDisableOnStart)
            {
                item.SetActive(false);
            }

            foreach (var item in objectToEnableOnStart)
            {
                item.SetActive(true);
            }
        }
        else if (GameManager.Instance.gameEnded)
        {
            foreach (var item in objectToDisableOnStart)
            {
                item.SetActive(true);
            }

            foreach (var item in objectToEnableOnStart)
            {
                item.SetActive(false);
            }

            waitingForHostText.SetActive(false);
        }
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.PRE_GAME, GameStarted);

        EventSystemNew.Unsubscribe(Event_Type.GAME_ENDED, GameEnded);
    }

    private void Awake()
    {
        foreach (var item in objectToEnableOnStart)
        {
            item.SetActive(false);
        }
    }

    private void GameStarted()
    {
        foreach (var item in objectToDisableOnStart)
        {
            item.SetActive(false);
        }

        foreach (var item in objectToEnableOnStart)
        {
            item.SetActive(true);
        }
    }

    private void GameEnded()
    {
        foreach (var item in objectToDisableOnStart)
        {
            item.SetActive(true);
        }

        foreach (var item in objectToEnableOnStart)
        {
            item.SetActive(false);
        }

        waitingForHostText.SetActive(false);
    }
}
