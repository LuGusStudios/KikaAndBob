using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemMissileLauncher : MonoBehaviour 
{
	public float missileFrequency = 10f;
	public float missileSpeed = 20f;
	public FroggerLaneItemMissile missile = null;
	
	public void SetupLocal()
	{
		
	}

	public void SetupGlobal()
	{
		if (missile == null)
		{
			missile = transform.FindChild("Missile").GetComponent<FroggerLaneItemMissile>();
			if (missile == null)
			{
				Debug.LogError("Could not find the missile!");
			}
			else
			{
				missile.gameObject.SetActive(false);
			}
		}

		// Start launching missiles at the player
		StartCoroutine(LaunchMissileRoutine());
	}

	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	private IEnumerator LaunchMissileRoutine()
	{
		// Periodically launches a missile, targeted at the player

		while (true)
		{
			yield return new WaitForSeconds(missileFrequency);

			// Spawn and launch missile
			Transform player = GameObject.Find("Player").transform;

			GameObject missileCopy = (GameObject)GameObject.Instantiate(missile.gameObject);
			missileCopy.transform.position = transform.position;
			missileCopy.transform.parent = transform.parent;
			missileCopy.gameObject.SetActive(true);
			missileCopy.GetComponent<FroggerLaneItemMissile>().Launch(player.position, missileSpeed);
			
		}
	}
}
