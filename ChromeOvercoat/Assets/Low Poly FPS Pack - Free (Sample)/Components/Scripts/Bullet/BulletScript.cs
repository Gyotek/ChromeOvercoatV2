using UnityEngine;
using System.Collections;

// ----- Low Poly FPS Pack Free Version -----
public class BulletScript : MonoBehaviour {

	[Range(5, 100)]
	[Tooltip("After how long time should the bullet prefab be destroyed?")]
	public float destroyAfter;
	[Tooltip("If enabled the bullet destroys on impact")]
	public bool destroyOnImpact = false;
	[Tooltip("Minimum time after impact that the bullet is destroyed")]
	public float minDestroyTime;
	[Tooltip("Maximum time after impact that the bullet is destroyed")]
	public float maxDestroyTime;

	[Header("Impact Effect Prefabs")]
	public Transform [] metalImpactPrefabs;

	public bool shotByPlayer = true;

	//FX
	public GameObject damagevesselVFX;

	private void Start () 
	{
		//Start destroy timer
		StartCoroutine (DestroyAfter ());
		EventsHandler.instance.TriggerEvent(EventsHandler.events.BulletSpawn);
	}
    //If the bullet collides with anything
    private void OnCollisionEnter(Collision collision)
	{
		//Debug.Log(collision.gameObject.name);

		//If destroy on impact is false, start 
		//coroutine with random destroy timer
		if (!destroyOnImpact) 
		{
			StartCoroutine (DestroyTimer ());
		}
		//Otherwise, destroy bullet on impact
		else 
		{
			Destroy (gameObject);
		}

		//If bullet collides with "Metal" tag
		if (collision.transform.tag == "Metal")
		{
			//Instantiate random impact prefab from array
			Instantiate (metalImpactPrefabs [Random.Range 
				(0, metalImpactPrefabs.Length)], transform.position, 
				Quaternion.LookRotation (collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position)));
			//Destroy bullet object
			Destroy(gameObject);
		}
		else if (collision.gameObject.GetComponent<Vessel>() && shotByPlayer == true)
		{

			ContactPoint contact = collision.contacts[0];
			Vector3 pos = contact.point;
			Vector3 normal = contact.normal;

			Debug.Log("HIT");
			Instantiate(damagevesselVFX, pos, Quaternion.FromToRotation(Vector3.up, normal));

			collision.transform.gameObject.GetComponent<Vessel>().TakeDamage(1);
			//Destroy bullet object
			Destroy(gameObject);
		}

		//If bullet collides with "Target" tag
		else if (collision.transform.tag == "Target" && shotByPlayer == true) 
		{
			//Toggle "isHit" on target object
			if (collision.transform.gameObject.GetComponent
				<TargetScript>().isHit != true)
			collision.transform.gameObject.GetComponent
				<TargetScript>().isHit = true;
			//Destroy bullet object
			Destroy(gameObject);
		}
			
		//If bullet collides with "ExplosiveBarrel" tag
		else if (collision.transform.tag == "ExplosiveBarrel") 
		{
			//Toggle "explode" on explosive barrel object
			collision.transform.gameObject.GetComponent
				<ExplosiveBarrelScript>().explode = true;
			//Destroy bullet object
			Destroy(gameObject);
		}
		else if (collision.gameObject.GetComponent<CharacterController>() && shotByPlayer == false)
		{
			collision.transform.gameObject.GetComponent<CharacterController>().TakeDamage(1);
			//Destroy bullet object
			Destroy(gameObject);
		}

		EventsHandler.instance.TriggerEvent(EventsHandler.events.BulletImpact);
	}

	private IEnumerator DestroyTimer () 
	{
		//Wait random time based on min and max values
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));
		//Destroy bullet object
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter () 
	{
		//Wait for set amount of time
		yield return new WaitForSeconds (destroyAfter);
		//Destroy bullet object
		Destroy (gameObject);
	}
}
// ----- Low Poly FPS Pack Free Version -----