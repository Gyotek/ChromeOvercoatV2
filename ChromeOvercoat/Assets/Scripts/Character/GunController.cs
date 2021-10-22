using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
	public class GunController : MonoBehaviour
	{
		[SerializeField]
		private GunDatas gunDatas = default;

		//Animator component attached to weapon
		Animator anim;

		[Header("Gun Camera")]
		//Main gun camera
		public Camera gunCamera;


		private Vector3 initialSwayPosition;

		//Used for fire rate
		private float lastFired;
		//Check if reloading
		private bool isReloading;
		[Tooltip("The bullet model inside the mag, not used for all weapons.")]
		public SkinnedMeshRenderer bulletInMagRenderer;

		//Check if running
		private bool isRunning;
		//Check if aiming
		private bool isAiming;
		//Check if walking
		private bool isWalking;

		//How much ammo is currently left
		private int currentAmmo;
		//Check if out of ammo
		private bool outOfAmmo;

		//min should always bee 1
		private int minRandomValue = 1;
		private int randomMuzzleflashValue;
		public ParticleSystem muzzleParticles;
		public ParticleSystem sparkParticles;
		public Light muzzleflashLight;


		[Header("Audio Source")]
		//Main audio source
		public AudioSource mainAudioSource;
		//Audio source used for shoot sound
		public AudioSource shootAudioSource;

		[Header("UI Components")]
		public Text currentAmmoText;
		public Text totalAmmoText;

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

		[System.Serializable]
		public class soundClips
		{
			public AudioClip shootSound;
			public AudioClip takeOutSound;
			public AudioClip holsterSound;
			public AudioClip reloadSoundOutOfAmmo;
			public AudioClip reloadSoundAmmoLeft;
			public AudioClip aimSound;
		}
		public soundClips SoundClips = default;

		private bool soundHasPlayed = false;

		private void Awake()
		{

			//Set the animator component
			anim = GetComponent<Animator>();
			//Set current ammo to total ammo value
			currentAmmo = gunDatas.ammo;

			muzzleflashLight.enabled = false;
		}

		private void Start()
		{
			//Set total ammo text from total ammo int
			totalAmmoText.text = gunDatas.ammo.ToString();

			//Weapon sway
			initialSwayPosition = transform.localPosition;

			//Set the shoot sound to audio source
			shootAudioSource.clip = SoundClips.shootSound;
		}

		private void LateUpdate()
		{

			//Weapon sway
			if (gunDatas.weaponSway == true)
			{
				float movementX = -Input.GetAxis("Mouse X") * gunDatas.swayAmount;
				float movementY = -Input.GetAxis("Mouse Y") * gunDatas.swayAmount;
				//Clamp movement to min and max values
				movementX = Mathf.Clamp
					(movementX, -gunDatas.maxSwayAmount, gunDatas.maxSwayAmount);
				movementY = Mathf.Clamp
					(movementY, -gunDatas.maxSwayAmount, gunDatas.maxSwayAmount);
				//Lerp local pos
				Vector3 finalSwayPosition = new Vector3
					(movementX, movementY, 0);
				transform.localPosition = Vector3.Lerp
					(transform.localPosition, finalSwayPosition +
						initialSwayPosition, Time.deltaTime * gunDatas.swaySmoothValue);
			}
		}

		private void Update()
		{

			//Aiming
			//Toggle camera FOV when right click is held down
			if (Input.GetButton("Fire2") && !isReloading && !isRunning)
			{
				isAiming = true;
				//Start aiming
				anim.SetBool("Aim", true);

				//When right click is released
				gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
					gunDatas.aimFov, gunDatas.fovSpeed * Time.deltaTime);

				if (!soundHasPlayed)
				{
					mainAudioSource.clip = SoundClips.aimSound;
					mainAudioSource.Play();

					soundHasPlayed = true;
				}
			}
			else
			{
				//When right click is released
				gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
					gunDatas.defaultFov, gunDatas.fovSpeed * Time.deltaTime);

				isAiming = false;
				//Stop aiming
				anim.SetBool("Aim", false);

				soundHasPlayed = false;
			}
			//Aiming end

			//If randomize muzzleflash is true, genereate random int values
			if (gunDatas.randomMuzzleflash == true)
			{
				randomMuzzleflashValue = Random.Range(minRandomValue, gunDatas.maxRandomValue);
			}

			//Set current ammo text from ammo int
			currentAmmoText.text = currentAmmo.ToString();

			//Continosuly check which animation 
			//is currently playing
			AnimationCheck();

			//If out of ammo
			if (currentAmmo == 0)
			{
				//Toggle bool
				outOfAmmo = true;
				//Auto reload if true
				if (gunDatas.autoReload == true && !isReloading)
				{
					StartCoroutine(AutoReload());
				}
			}
			else
			{
				//Toggle bool
				outOfAmmo = false;
				//anim.SetBool ("Out Of Ammo", false);
			}

			//AUtomatic fire
			//Left click hold 
			if (Input.GetMouseButton(0) && !outOfAmmo && !isReloading && !isRunning)
			{
				//Shoot automatic
				if (Time.time - lastFired > 1 / gunDatas.fireRate)
				{
					lastFired = Time.time;

					//Remove 1 bullet from ammo
					currentAmmo -= 1;

					shootAudioSource.clip = SoundClips.shootSound;
					shootAudioSource.Play();

					EventsHandler.instance.TriggerEvent(EventsHandler.events.GunShoot);
					if (!isAiming) //if not aiming
					{
						anim.Play("Fire", 0, 0f);
						//If random muzzle is false
						if (!gunDatas.randomMuzzleflash &&
							gunDatas.enableMuzzleflash == true)
						{
							muzzleParticles.Emit(1);
							//Light flash start
							StartCoroutine(MuzzleFlashLight());
						}
						else if (gunDatas.randomMuzzleflash == true)
						{
							//Only emit if random value is 1
							if (randomMuzzleflashValue == 1)
							{
								if (gunDatas.enableSparks == true)
								{
									//Emit random amount of spark particles
									sparkParticles.Emit(Random.Range(gunDatas.minSparkEmission, gunDatas.maxSparkEmission));
								}
								if (gunDatas.enableMuzzleflash == true)
								{
									muzzleParticles.Emit(1);
									//Light flash start
									StartCoroutine(MuzzleFlashLight());
								}
							}
						}
					}
					else //if aiming
					{

						anim.Play("Aim Fire", 0, 0f);

						//If random muzzle is false
						if (!gunDatas.randomMuzzleflash)
						{
							muzzleParticles.Emit(1);
							//If random muzzle is true
						}
						else if (gunDatas.randomMuzzleflash == true)
						{
							//Only emit if random value is 1
							if (randomMuzzleflashValue == 1)
							{
								if (gunDatas.enableSparks == true)
								{
									//Emit random amount of spark particles
									sparkParticles.Emit(Random.Range(gunDatas.minSparkEmission, gunDatas.maxSparkEmission));
								}
								if (gunDatas.enableMuzzleflash == true)
								{
									muzzleParticles.Emit(1);
									//Light flash start
									StartCoroutine(MuzzleFlashLight());
								}
							}
						}
					}

					//Spawn bullet from bullet spawnpoint
					var bullet = (Transform)Instantiate(
						gunDatas.Prefabs.bulletPrefab,
						Spawnpoints.bulletSpawnPoint.transform.position,
						Spawnpoints.bulletSpawnPoint.transform.rotation);

					//Add velocity to the bullet
					bullet.GetComponent<Rigidbody>().velocity =
						bullet.transform.forward * gunDatas.bulletForce;

					//Spawn casing prefab at spawnpoint
					Instantiate(gunDatas.Prefabs.casingPrefab,
						Spawnpoints.casingSpawnPoint.transform.position,
						Spawnpoints.casingSpawnPoint.transform.rotation);
				}
			}

			//Reload 
			if (Input.GetKeyDown(KeyCode.R) && !isReloading)
			{
				//Reload
				Reload();
			}

			//Walking when pressing down WASD keys
			if (Input.GetKey(KeyCode.Z) && !isRunning ||
				Input.GetKey(KeyCode.Q) && !isRunning ||
				Input.GetKey(KeyCode.S) && !isRunning ||
				Input.GetKey(KeyCode.D) && !isRunning)
			{
				anim.SetBool("Walk", true);
			}
			else
			{
				anim.SetBool("Walk", false);
			}

			//Running when pressing down W and Left Shift key
			if ((Input.GetKey(KeyCode.Z) ||
				Input.GetKey(KeyCode.Q) ||
				Input.GetKey(KeyCode.D)) &&
				Input.GetKey(KeyCode.LeftShift))
			{
				isRunning = true;
			}
			else
			{
				isRunning = false;
			}

			//Run anim toggle
			if (isRunning == true)
			{
				anim.SetBool("Run", true);
			}
			else
			{
				anim.SetBool("Run", false);
			}
		}


		private IEnumerator AutoReload()
		{
			//Wait set amount of time
			yield return new WaitForSeconds(gunDatas.autoReloadDelay);

			if (outOfAmmo == true)
			{
				//Play diff anim if out of ammo
				anim.Play("Reload Out Of Ammo", 0, 0f);

				mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
				mainAudioSource.Play();

				//If out of ammo, hide the bullet renderer in the mag
				//Do not show if bullet renderer is not assigned in inspector
				if (bulletInMagRenderer != null)
				{
					bulletInMagRenderer.GetComponent
					<SkinnedMeshRenderer>().enabled = false;
					//Start show bullet delay
					StartCoroutine(ShowBulletInMag());
				}
			}
			//Restore ammo when reloading
			currentAmmo = gunDatas.ammo;
			outOfAmmo = false;
		}

		//Reload
		private void Reload()
		{

			if (outOfAmmo == true)
			{
				//Play diff anim if out of ammo
				anim.Play("Reload Out Of Ammo", 0, 0f);

				mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
				mainAudioSource.Play();

				//If out of ammo, hide the bullet renderer in the mag
				//Do not show if bullet renderer is not assigned in inspector
				if (bulletInMagRenderer != null)
				{
					bulletInMagRenderer.GetComponent
					<SkinnedMeshRenderer>().enabled = false;
					//Start show bullet delay
					StartCoroutine(ShowBulletInMag());
				}
			}
			else
			{
				//Play diff anim if ammo left
				anim.Play("Reload Ammo Left", 0, 0f);

				mainAudioSource.clip = SoundClips.reloadSoundAmmoLeft;
				mainAudioSource.Play();

				//If reloading when ammo left, show bullet in mag
				//Do not show if bullet renderer is not assigned in inspector
				if (bulletInMagRenderer != null)
				{
					bulletInMagRenderer.GetComponent
					<SkinnedMeshRenderer>().enabled = true;
				}
			}
			//Restore ammo when reloading
			currentAmmo = gunDatas.ammo;
			outOfAmmo = false;
		}

		//Enable bullet in mag renderer after set amount of time
		private IEnumerator ShowBulletInMag()
		{

			//Wait set amount of time before showing bullet in mag
			yield return new WaitForSeconds(gunDatas.showBulletInMagDelay);
			bulletInMagRenderer.GetComponent<SkinnedMeshRenderer>().enabled = true;
		}

		//Show light when shooting, then disable after set amount of time
		private IEnumerator MuzzleFlashLight()
		{

			muzzleflashLight.enabled = true;
			yield return new WaitForSeconds(gunDatas.lightDuration);
			muzzleflashLight.enabled = false;
		}

		//Check current animation playing
		private void AnimationCheck()
		{

			//Check if reloading
			//Check both animations
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo") ||
				anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Ammo Left"))
			{
				if (!isReloading)
				{
					isReloading = true;
					EventsHandler.instance.TriggerEvent(EventsHandler.events.GunReloadStart);
				}
			}
			else
			{
                if (isReloading)
				{
					isReloading = false;
					EventsHandler.instance.TriggerEvent(EventsHandler.events.GunReloadEnd);
				}
			}
		}
	}
}
