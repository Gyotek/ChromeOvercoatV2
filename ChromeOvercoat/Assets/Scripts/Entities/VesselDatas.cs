using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Entities/Vessels Data")]
public class VesselDatas : ScriptableObject
{
	public int maxLifePoints = 3;
	public float speed = 15.0f;


	[Header("Weapon Settings")]
	//How fast the weapon fires, higher value means faster rate of fire
	[Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
	public float fireRate;
	[Header("Bullet Settings")]
	//Bullet
	[Tooltip("How much force is applied to the bullet when shooting.")]
	public float bulletForce = 400.0f;


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
