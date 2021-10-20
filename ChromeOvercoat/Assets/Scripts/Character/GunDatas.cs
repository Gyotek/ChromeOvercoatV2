using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    [CreateAssetMenu(menuName = "Character/Gun Data")]
    public class GunDatas : ScriptableObject
	{
		[Header("Gun Camera Options")]
		//How fast the camera field of view changes when aiming 
		[Tooltip("How fast the camera field of view changes when aiming.")]
		public float fovSpeed = 15.0f;
		//Default camera field of view
		[Tooltip("Default value for camera field of view (40 is recommended).")]
		public float defaultFov = 40.0f;
		public float aimFov = 25.0f;

		[Header("Weapon Sway")]
		//Enables weapon sway
		[Tooltip("Toggle weapon sway.")]
		public bool weaponSway;

		public float swayAmount = 0.02f;
		public float maxSwayAmount = 0.06f;
		public float swaySmoothValue = 4.0f;


		[Header("Weapon Settings")]
		//How fast the weapon fires, higher value means faster rate of fire
		[Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
		public float fireRate;
		//Eanbles auto reloading when out of ammo
		[Tooltip("Enables auto reloading when out of ammo.")]
		public bool autoReload;
		//Delay between shooting last bullet and reloading
		public float autoReloadDelay;

		//Totalt amount of ammo
		[Tooltip("How much ammo the weapon should have.")]
		public int ammo;


		[Header("Bullet Settings")]
		//Bullet
		[Tooltip("How much force is applied to the bullet when shooting.")]
		public float bulletForce = 400.0f;
		[Tooltip("How long after reloading that the bullet model becomes visible " +
			"again, only used for out of ammo reload animations.")]
		public float showBulletInMagDelay = 0.6f;

		[Header("Muzzleflash Settings")]
		public bool randomMuzzleflash = false;

		[Range(2, 25)]
		public int maxRandomValue = 5;

		public bool enableMuzzleflash = true;
		public bool enableSparks = true;
		public int minSparkEmission = 1;
		public int maxSparkEmission = 7;

		[Header("Muzzleflash Light Settings")]
		public float lightDuration = 0.02f;

		[System.Serializable]
		public class prefabs
		{
			[Header("Prefabs")]
			public Transform bulletPrefab;
			public Transform casingPrefab;
		}
		public prefabs Prefabs;

	}
}
