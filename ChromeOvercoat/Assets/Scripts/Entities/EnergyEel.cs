using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyEel : MonoBehaviour, IDrainable
{
    private bool weakened = false;
    private bool moving = true;
    [SerializeField] Animation anim;
    private Vessel nextVessel;
    private Vessel previousVessel;
    [SerializeField] EelDatas eelData = default;



    private void Update()
    {
        if (!weakened && moving)
            MoveToNextVessel();
    }

    public bool Drain()
    {
        if (weakened)
        {
            weakened = false;
            moving = false;
            anim.Play("EnergyEel_Drained");
            StartCoroutine(DestroyCoroutine());
            return true;
        }
        else
            return false;
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }

    public void Appartion(Vessel vessel)
    {
        anim.Play("EnergyEel_Apparition");
        nextVessel = null;
        previousVessel = vessel;
        weakened = true;
        moving = false;
        StartCoroutine(WeakCoroutine());
    }

    void StartMovingToNextVessel()
    {
        if (previousVessel != null)
            nextVessel = VesselManager.instance.GetAntoherRandomVessel(previousVessel);
        else
            nextVessel = VesselManager.instance.GetRandomVessel();

        weakened = false;
        moving = true;
    }

    void MoveToNextVessel()
    {
        if (nextVessel == null)
            nextVessel = VesselManager.instance.GetRandomVessel();

        transform.position = Vector3.MoveTowards(transform.position, nextVessel.eelSpawnPoint.position, eelData.speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextVessel.eelSpawnPoint.position) < 1f)
            nextVessel.Possessed(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDrainer>()?.SetDrainableTarget(this, true);
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<IDrainer>()?.SetDrainableTarget(this, false);
    }

    IEnumerator WeakCoroutine()
    {
        yield return new WaitForSeconds(2f);
        StartMovingToNextVessel();
    }
}
