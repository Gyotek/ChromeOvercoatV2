using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDrain : MonoBehaviour, IDrainer
{
    bool canDrain;

    private IDrainable drainableTarget = null;
    public void SetDrainableTarget(IDrainable target, bool onRange)
    {
        if (onRange)
        {
            drainableTarget = target;
            canDrain = true;
        }
        else if (!onRange && drainableTarget == target)
        {
            drainableTarget = null;
            canDrain = false;
        }
    }

    private void Update()
    {
        if (canDrain && Input.GetButtonDown("Drain"))
        {
            drainableTarget.Drain();

            EventsHandler.instance.TriggerEvent(EventsHandler.events.Drain);
        }
    }
}
