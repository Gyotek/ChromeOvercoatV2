using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{

	[SerializeField] EnergyEel eelPrefab;
	[SerializeField] public Transform eelSpawnPoint;

	// FX
	public DeathCube deathC;

	float randomTime;

	//Used to check if the target has been hit
	public bool isDown = false;

	[Header("Customizable Options")]
	//Minimum time before the target goes back up
	public float minTime;
	//Maximum time before the target goes back up
	public float maxTime;

	[Header("Audio")]
	public AudioClip upSound;
	public AudioClip downSound;

	public AudioSource audioSource;

	private void Update()
	{

		//Generate random time based on min and max time values
		randomTime = Random.Range(minTime, maxTime);

	}

	public void Destroyed()
	{
		if (isDown == true) return;

		//Animate the target "down"
		//gameObject.GetComponent<Animation>().Play("target_down");
		deathC?.ActivateCall(false);
		isDown = true;
		//Set the downSound as current sound, and play it
		audioSource.GetComponent<AudioSource>().clip = downSound;
		audioSource.Play();

		var eel = Instantiate(eelPrefab, eelSpawnPoint.position, Quaternion.identity);
		eel.Appartion(this);
		EventsHandler.instance.TriggerEvent(EventsHandler.events.VesselDestroyed);
	}
	public void Possessed(EnergyEel eel)
	{
		isDown = false;
		//Animate the target "up"
		//gameObject.GetComponent<Animation>().Play("target_up");
		deathC?.ActivateCall(true);
		//Set the downSound as current sound, and play it
		audioSource.GetComponent<AudioSource>().clip = upSound;
		audioSource.Play();

		Destroy(eel.gameObject);
		EventsHandler.instance.TriggerEvent(EventsHandler.events.VesselPossessed);
	}

}
