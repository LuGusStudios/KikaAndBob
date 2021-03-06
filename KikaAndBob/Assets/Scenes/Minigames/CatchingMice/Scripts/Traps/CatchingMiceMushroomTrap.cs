using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceMushroomTrap : CatchingMiceWorldObjectTrapFurniture {

	public float cloudTime = 1f;
	public string particles = "";

	protected ILugusCoroutineHandle routineHandle = null;
	protected ParticleSystem cloudParticles = null;

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		// Find the particle system
		cloudParticles = transform.FindChild(particles).GetComponent<ParticleSystem>();

		routineHandle = LugusCoroutines.use.GetHandle();
		routineHandle.Claim();

		StartCoroutine(TrapRoutine());
	}

//	protected override void OnPlayerInteract ()
//	{
//		if (routineHandle != null && routineHandle.Running)
//		{
//			Debug.LogError("CatchingMiceMushroomTrap: Trap routine is still running. Not resetting.");
//			return;
//		}
//
//		if (CatchingMiceGameManager.use.PickupCount >= this.Cost)
//		{
//			if (spriteRenderer != null)
//			{
//				spriteRenderer.sprite = activeSprite;
//			}
//			
//			CatchingMiceGameManager.use.PickupCount -= (int) this.Cost;
//			
//			Ammo = originalAmmoCount;
//			
//			// this will not be running it anymore - it terminates itself the frame that ammo goes 0
//			StartCoroutine(TrapRoutine());
//			
//			CatchingMiceLogVisualizer.use.LogError("Refilling ammo on trap: " + transform.Path() + " to " + originalAmmoCount);
//		}
//	}

	protected override IEnumerator TrapRoutine()
	{
		Vector2 min, max;
		CalculateTrapBounds(out min, out max);

		while (CatchingMiceGameManager.use.GameRunning
			&& (ammo > 0)
			&& (health > 0))
		{
			// Check whether an enemy is near
			List<CatchingMiceCharacterMouse> enemies = new List<CatchingMiceCharacterMouse>();
			Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

			foreach (Collider2D coll2D in colliders)
			{
				CatchingMiceCharacterMouse enemy = coll2D.transform.parent.GetComponent<CatchingMiceCharacterMouse>();

				if (enemy != null)
				{
					enemies.Add(enemy);
				}
			}

			// If there are enemies in the neighborhood
			// then start the cloud routine
			if ((enemies.Count > 0) && (!routineHandle.Running))
			{
				routineHandle.StartRoutine(PoisonCloudRoutine());

				// Wait until the vacuum routine is done
				// and then wait another interval time
				// to start again
				while (routineHandle.Running)
				{
					yield return new WaitForEndOfFrame();
				}

				Ammo = Ammo - 1;

				yield return new WaitForSeconds(interval);
			}
			else
			{
				yield return new WaitForFixedUpdate();
			}
		}
	}

	protected IEnumerator PoisonCloudRoutine()
	{
		Vector2 min, max;
		CalculateTrapBounds(out min, out max);

		List<CatchingMiceCharacterMouse> enemiesInCloud = new List<CatchingMiceCharacterMouse>();
		List<float> enemyHitTimes = new List<float>();

		if (cloudParticles != null)
		{
			cloudParticles.Play();
		}

		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("e00_MushroomPoof01"));

		float timeLeft = cloudTime;
		float poofSoundTimer = 0.0f;

		while (CatchingMiceGameManager.use.GameRunning
			&& (timeLeft > 0)
			&& (health > 0))
		{

			timeLeft -= Time.fixedDeltaTime;
			poofSoundTimer += Time.fixedDeltaTime;

			if (poofSoundTimer >= 4.0f)
			{
				LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("e00_MushroomPoof01"));
				poofSoundTimer = 0;
			}

			// Check whether an enemy is near
			Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

			// Add the enemy to the list of enemies that should be hit with every tick
			foreach (Collider2D coll2D in colliders)
			{
				if (coll2D.transform.parent == null)
					continue;

				CatchingMiceCharacterMouse enemy = coll2D.transform.parent.GetComponent<CatchingMiceCharacterMouse>();

				if (enemy == null)
				{
					continue;
				}

				if (!enemiesInCloud.Contains(enemy))
				{
					enemiesInCloud.Add(enemy);
					enemyHitTimes.Add(1f);
				}
			}

			// Go over all of the registered enemies, and update their times
			for (int i = 0; i < enemiesInCloud.Count; ++i)
			{
				enemyHitTimes[i] -= Time.fixedDeltaTime;

				if (enemyHitTimes[i] < 0)
				{
					OnHit(enemiesInCloud[i]);
					enemyHitTimes[i] = 1f;

					if (enemiesInCloud[i].Health <= 0)
					{
						enemiesInCloud.RemoveAt(i);
						enemyHitTimes.RemoveAt(i);
					}
				}
			}

			yield return new WaitForFixedUpdate();
		}

		//OnEmptyAmmo();

		if (cloudParticles != null)
		{
			cloudParticles.Stop();
		}
	}
}
