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
		StartCoroutine(LaunchMissileRoutine(Random.Range(0f, missileFrequency)));
	}

	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	private IEnumerator LaunchMissileRoutine(float initialDelay)
	{
		// Periodically launches a missile, targeted at the player
		while (FroggerGameManager.use.GameRunning)
		{
			yield return new WaitForSeconds(missileFrequency);

			// Spawn and launch missile
			GameObject playerObj = GameObject.Find("Player");
			if (playerObj != null)
			{
				FroggerCharacter player = playerObj.GetComponent<FroggerCharacter>();

				if (player != null)
				{
					GameObject missileCopy = (GameObject)GameObject.Instantiate(missile.gameObject);
					missileCopy.transform.position = transform.position;
					missileCopy.transform.parent = transform.parent;
					missileCopy.gameObject.SetActive(true);
					missileCopy.GetComponent<FroggerLaneItemMissile>().Launch(player, missileSpeed);
				}
			}
		}
	}
}
