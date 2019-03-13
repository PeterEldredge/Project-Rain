using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventExampleObj : GameEventUserObject
{
    private void DisplayFinalScore(Events.GameOverEventArgs eventArgs)
    {
        Debug.Log("Display " + eventArgs.Time.ToString());
    }

    public override void Subscribe()
    {
        EventManager.AddListener<Events.GameOverEventArgs>(DisplayFinalScore);
    }

    public override void Unsubscribe()
    {
        EventManager.RemoveListener<Events.GameOverEventArgs>(DisplayFinalScore);
    }
}

public class GameEventsTriggerer : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(EndGame(5f));
    }

    private IEnumerator EndGame(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        EventManager.TriggerEvent(new Events.GameOverEventArgs(seconds));
    }
}