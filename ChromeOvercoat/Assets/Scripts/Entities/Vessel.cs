using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{

	[SerializeField] EnergyEel eelPrefab;
	[SerializeField] public Transform eelSpawnPoint;
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
	[SerializeField] VesselDatas vesselData = default;

	[Header("Audio")]
	public AudioClip upSound;
	public AudioClip downSound;

	public AudioSource audioSource;

    private void Start()
    {
		targetWaypoint = firstWaypoint;
		lifePoints = vesselData.maxLifePoints;
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
	//min should always bee 1
	private int minRandomValue = 1;
	private int randomMuzzleflashValue;
	public ParticleSystem muzzleParticles;
	public ParticleSystem sparkParticles;
	public Light muzzleflashLight;
	//Used for fire rate
	private float lastFired;


	[Header("Audio Source")]
	//Main audio source
	public AudioSource mainAudioSource;
	//Audio source used for shoot sound
	public AudioSource shootAudioSource;
	[System.Serializable]
	public class soundClips
	{
		public AudioClip shootSound;
		public AudioClip takeOutSound;
		public AudioClip aimSound;
	}
	public soundClips SoundClips = default;

	[System.Serializable]
	public class spawnpoints
	{
		[Header("Spawnpoints")]
		//Array holding casing spawn points 
		//(some weapons use more than one casing spawn)
		//Casing spawn point array
		public Transform casingSpawnPoint;
		//Bullet prefab spawn from this point
		public Transform bulletSpawnPoint;
	}
	public spawnpoints Spawnpoints;

	void Shoot()
	{
		if (!isShooting) return;
		if (root != null) root.LookAt(new Vector3(playerPosition.x, root.position.y, playerPosition.z));


		//If randomize muzzleflash is true, genereate random int values
		if (vesselData.randomMuzzleflash == true)
		{
			randomMuzzleflashValue = Random.Range(minRandomValue, vesselData.maxRandomValue);
		}

		//Shoot automatic
		if (Time.time - lastFired > 1 / vesselData.fireRate)
		{
			lastFired = Time.time;


			shootAudioSource.clip = SoundClips.shootSound;
			shootAudioSource.Play();

			//anim.Play("Fire", 0, 0f);

			//If random muzzle is false
			if (!vesselData.randomMuzzleflash &&
				vesselData.enableMuzzleflash == true)
			{
				muzzleParticles.Emit(1);
				//Light flash start
				StartCoroutine(MuzzleFlashLight());
			}
			else if (vesselData.randomMuzzleflash == true)
			{
				//Only emit if random value is 1
				if (randomMuzzleflashValue == 1)
				{
					if (vesselData.enableSparks == true)
					{
						//Emit random amount of spark particles
						sparkParticles.Emit(Random.Range(vesselData.minSparkEmission, vesselData.maxSparkEmission));
					}
					if (vesselData.enableMuzzleflash == true)
					{
						muzzleParticles.Emit(1);
						//Light flash start
						StartCoroutine(MuzzleFlashLight());
					}
				}
			}

			//Spawn bullet from bullet spawnpoint
			var bullet = (Transform)Instantiate(
				vesselData.Prefabs.bulletPrefab,
				Spawnpoints.bulletSpawnPoint.transform.position,
				Spawnpoints.bulletSpawnPoint.transform.rotation);

			//Add velocity to the bullet
			bullet.GetComponent<Rigidbody>().velocity =
				bullet.transform.forward * vesselData.bulletForce;

			bullet.GetComponent<BulletScript>().shotByPlayer = false;

			//Spawn casing prefab at spawnpoint
			Instantiate(vesselData.Prefabs.casingPrefab,
				Spawnpoints.casingSpawnPoint.transform.position,
				Spawnpoints.casingSpawnPoint.transform.rotation);

		}
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
		lifePoints = vesselData.maxLifePoints;
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
	bool isShooting = false;
	bool movingToFirstPoint = true;
	void moveToNextPoint()
    {
		if (isShooting || isDown || targetWaypoint == null) return;

		transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, vesselData.speed * Time.deltaTime);

		if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
		{
			movingToFirstPoint = !movingToFirstPoint;
			if (movingToFirstPoint)
				targetWaypoint = firstWaypoint;
			else
				targetWaypoint = secondWaypoint;
		}
	}

	//Show light when shooting, then disable after set amount of time
	private IEnumerator MuzzleFlashLight()
	{

		muzzleflashLight.enabled = true;
		yield return new WaitForSeconds(vesselData.lightDuration);
		muzzleflashLight.enabled = false;
	}


}
