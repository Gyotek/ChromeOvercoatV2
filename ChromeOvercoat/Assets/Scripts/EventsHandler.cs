using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EventsHandler : SerializedMonoBehaviour
{
    public static EventsHandler instance;
    private void Awake() { instance = this; }

    public enum events {BulletImpact, BulletSpawn, Drain, GunReloadStart, GunReloadEnd, GunShoot, Jump, VesselDestroyed, VesselPossessed };
    [SerializeField] Dictionary<events, GameEvent> GameEvents = new Dictionary<events, GameEvent>();

    public void TriggerEvent(events _event)
    {
        GameEvents[_event].Raise();
    }

}
