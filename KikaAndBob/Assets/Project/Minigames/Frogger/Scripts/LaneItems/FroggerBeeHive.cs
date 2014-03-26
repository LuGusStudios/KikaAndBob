using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerBeeHive : FroggerCollider 
{
	public float buzzingWidth = 5f;
	public float beesAliveTime = 10f;
	public float beesSpeed = 1f;
	
	public string buzzingSFXName = "";
	public float buzzingSFXVolume = 1f;

	public SpriteRenderer beeHive = null;
	public FroggerBees bees = null;

	protected FroggerCharacter player = null;

	public enum BeeHiveState
	{
		NONE = -1,
		IDLE = 1,
		BUZZING = 2
	};

	protected BeeHiveState beeHiveState = BeeHiveState.NONE;

	public override void SetUpLocal()
	{
		base.SetUpLocal();

		beeHiveState = BeeHiveState.IDLE;
	}
	
	public override void SetupGlobal()
	{
		base.SetupGlobal();

		// Find the character
		player = GameObject.Find("Player").GetComponent<FroggerCharacter>();
		if (player == null)
		{
			Debug.LogError(name + ": Could not find the player object.");
		}

		// Find the bee hive
		if (beeHive == null)
		{
			beeHive = transform.FindChild ("BeeHive").GetComponent<SpriteRenderer>();
			if (beeHive == null)
			{
				Debug.LogError(name + ": Could not find the bee hive sprite.");
			}
		}

		// Find the bees
		if (bees == null)
		{
			bees = gameObject.GetComponentInChildren<FroggerBees>();
			if (bees == null)
			{
				Debug.LogError(name + ": could not find the bees prefab.");
			}
			else
			{
				bees.gameObject.SetActive(false);
			}
		}
	}
	
	protected void Awake()
	{
		SetUpLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
		if (player == null)
		{
			player = GameObject.Find("Player").GetComponent<FroggerCharacter>();
			if (player == null)
			{
				return;
			}
		}

		// When the hive is idle, check for the character's current position,
		// and start buzzing when the character gets too close.
		if (beeHiveState == BeeHiveState.IDLE)
		{
			if ((player.CurrentLane == parentLane)
				&& (Mathf.Abs(player.transform.position.x - transform.position.x) < (buzzingWidth * 0.5f)))
			{
				beeHiveState = BeeHiveState.BUZZING;
				StartCoroutine(BuzzingRoutine());
			}
		}
		else if (beeHiveState == BeeHiveState.BUZZING)
		{
			if ((player.CurrentLane != parentLane)
				|| (Mathf.Abs(player.transform.position.x - transform.position.x) > (buzzingWidth * 0.5f)))
			{
				beeHiveState = BeeHiveState.IDLE;
			}
		}
	}

	private IEnumerator BuzzingRoutine()
	{
		// The buzzing of the bee hive makes the hive itself shake a little bit
		// Next, bees are spawned from the hive that will follow the player for
		// a little while

		// Play the sound effect
		if (!string.IsNullOrEmpty(buzzingSFXName))
		{
			AudioClip buzzingSFX = LugusResources.use.Shared.GetAudio(buzzingSFXName);
			if (buzzingSFX != LugusResources.use.errorAudio)
			{
				LugusAudio.use.SFX().Play(buzzingSFX);
			}
		}

		// Shake the beehive a little bit
		if (beeHive != null)
		{
			Vector3 currentScale = beeHive.transform.lossyScale;

			for (int i = 0; i < 3; ++i)
			{
				beeHive.gameObject.ScaleTo(currentScale * 0.9f).Time(0.1f).Execute();

				yield return new WaitForSeconds(0.1f);

				beeHive.gameObject.ScaleTo(currentScale * 1.1f).Time(0.1f).Execute();

				yield return new WaitForSeconds(0.1f);
			}

			beeHive.gameObject.ScaleTo(currentScale).Time(0.1f).Execute();

			yield return new WaitForSeconds(0.1f);

		}

		// Spawn bees and start following the player
		if (bees != null)
		{
			GameObject beesObj = (GameObject)GameObject.Instantiate(bees.gameObject);
			
			if (beeHive != null)
			{
				beesObj.transform.position = beeHive.transform.position;
			}
			else
			{
				beesObj.transform.position = transform.position;
			}

			beesObj.SetActive(true);

			FroggerBees beesCopy = beesObj.GetComponent<FroggerBees>();
			beesCopy.ScaleFade();

			// Wait until the bees are initialized
			while (!beesCopy.Initialized)
			{
				yield return new WaitForEndOfFrame();
			}

			beesCopy.FollowTarget(player.transform, beesAliveTime, beesSpeed);
		}

		yield break;
	}
}
