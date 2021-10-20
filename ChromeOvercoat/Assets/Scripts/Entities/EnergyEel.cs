using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyEel : MonoBehaviour, IDrainable
{
    private bool weakened = false;
    [SerializeField] Animation anim;
    

    public bool Drain()
    {
        if (weakened)
        {
            weakened = false;
            anim.Play("EnergyEel_Drained");
            return true;
        }
        else
            return false;
    }

    private void OnEnable()
    {
        weakened = true;
        anim.Play("EnergyEel_Apparition");

    }
    private void OnDisable()
    {
    }
}
