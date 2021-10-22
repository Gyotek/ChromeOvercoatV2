using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{

	[SerializeField] EnergyEel eelPrefab;
	[SerializeField] public Transform eelSpawnPoint;
	[SerializeField] int maxLifePoints = 3;
	int lifePoints = 3;

	// FX
	public DeathCube deathC;
	public void TakeDamage(int damage)
    {
		if (lifePoints <= 0 || isDown) return;
		lifePoints -= damage;

		if (lifePoints <= 0)
			Destroyed();

	}

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

    private void Start()
    {
		targetWaypoint = firstWaypoint;
		lifePoints = maxLifePoints;
    }

	Vector3 playerPosition;
	void FixedUpdate()
	{
		if (isDown) return;
		playerPosition = Chrome.GunController.instance.transform.position;

		CheckForPlayer();
		moveToNextPoint();
		Shoot();
	}

	[SerializeField] float shootRange = 1000;
	[SerializeField] float shootCooldown = 1f;
	void CheckForPlayer()
	{
		LayerMask layerMask = LayerMask.GetMask("Environement") + LayerMask.GetMask("Player");

		RaycastHit hit;
		Vector3 direction = (playerPosition - transform.position);


		if (Physics.Raycast(transform.position, direction, out hit, shootRange, layerMask))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
			isShooting = true;
		}
		else
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(direction) * shootRange, Color.white);
			isShooting = false;
		}
	}


	[SerializeField] Transform root;
	void Shoot()
    {
		if (!isShooting) return;
		if (root != null) root.LookAt(new Vector3(playerPosition.x, root.position.y, playerPosition.z));
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
		lifePoints = maxLifePoints;
		//Animate the target "up"
		//gameObject.GetComponent<Animation>().Play("target_up");
		deathC?.ActivateCall(true);
		//Set the downSound as current sound, and play it
		audioSource.GetComponent<AudioSource>().clip = upSound;
		audioSource.Play();

		Destroy(eel.gameObject);
		EventsHandler.instance.TriggerEvent(EventsHandler.events.VesselPossessed);
	}


	[SerializeField] Transform firstWaypoint;
	[SerializeField] Transform secondWaypoint;
	Transform targetWaypoint;
	[SerializeField] float speed = 15.0f;
	bool isShooting = false;
	bool movingToFirstPoint = true;
	void moveToNextPoint()
    {
		if (isShooting || isDown || targetWaypoint == null) return;

		transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

		if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
		{
			movingToFirstPoint = !movingToFirstPoint;
			if (movingToFirstPoint)
				targetWaypoint = firstWaypoint;
			else
				targetWaypoint = secondWaypoint;
		}
	}


}
