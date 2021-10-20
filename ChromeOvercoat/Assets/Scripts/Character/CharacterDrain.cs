using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDrain : MonoBehaviour, IDrainer
{
    bool canDrain;

    private IDrainable drainableTarget = null;
    public void SetDrainableTarget(IDrainable target)
    {
        drainableTarget = target;
        if (target != null)
            canDrain = true;
        else
            canDrain = false;

        Debug.Log(canDrain);
    }

    private void Update()
    {
        if (canDrain && Input.GetButtonDown("Drain"))
        {
            drainableTarget.Drain();
        }
    }
}
