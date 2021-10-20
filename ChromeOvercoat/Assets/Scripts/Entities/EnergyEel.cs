using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyEel : MonoBehaviour, IDrainable
{
    public bool weakened = false;
    [SerializeField] Animation anim;
    [SerializeField] float speed;


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
        anim.Play("EnergyEel_Apparition");
    }
    private void OnDisable()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDrainer>()?.SetDrainableTarget(this);
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<IDrainer>()?.SetDrainableTarget(null);
    }
}
