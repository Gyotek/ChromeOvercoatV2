using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselManager : MonoBehaviour
{
    public static VesselManager instance;
    private void Awake() { instance = this; }


    [SerializeField] List<Vessel>vessels = new List<Vessel>();

    public Vessel GetRandomVessel()
    {
        int rdm = Random.Range(0, vessels.Count);
        return vessels[rdm];
    }
    public Vessel GetAntoherRandomVessel(Vessel vessel)
    {
        var _vessels = new List<Vessel>();
        _vessels.AddRange(vessels);
        _vessels.Remove(vessel);

        int rdm = Random.Range(0, _vessels.Count);
        return _vessels[rdm];
    }
}
