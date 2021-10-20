using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDrainer
{
    abstract void SetDrainableTarget(IDrainable target);
}
