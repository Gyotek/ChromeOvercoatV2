using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{

	[SerializeField] EnergyEel eelPrefab;
	[SerializeField] Transform eelSpawnPoint;


	float randomTime;
	bool routineStarted = false;

	//Used to check if the target has been hit
	public bool isHit = false;

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

		//If the target is hit
		if (isHit == true)
		{
			if (routineStarted == false)
			{
				Destroyed();
			}
		}
	}

	private void Destroyed()
	{
		//Animate the target "down"
		gameObject.GetComponent<Animation>().Play("target_down");

		//Set the downSound as current sound, and play it
		audioSource.GetComponent<AudioSource>().clip = downSound;
		audioSource.Play();

		//Start the timer
		StartCoroutine(DelayTimer());
		routineStarted = true;

		var eel = Instantiate(eelPrefab, eelSpawnPoint.position, Quaternion.identity);
		eel.weakened = true;
	}

	//Time before the target pops back up
	private IEnumerator DelayTimer()
	{
		//Wait for random amount of time
		yield return new WaitForSeconds(randomTime);
		//Animate the target "up" 
		gameObject.GetComponent<Animation>().Play("target_up");

		//Set the upSound as current sound, and play it
		audioSource.GetComponent<AudioSource>().clip = upSound;
		audioSource.Play();

		//Target is no longer hit
		isHit = false;
		routineStarted = false;
	}
}
