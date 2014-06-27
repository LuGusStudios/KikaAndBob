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

	protected override IEnumerator TrapRoutine()
	{
		Vector2 min, max;
		CalculateTrapBounds(out min, out max);

		while (CatchingMiceGameManager.use.gameRunning
			&& (stacks > 0)
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

				Stacks = Stacks - 1;

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

		float timeLeft = cloudTime;

		while (CatchingMiceGameManager.use.gameRunning
			&& (timeLeft > 0)
			&& (health > 0))
		{

			timeLeft -= Time.fixedDeltaTime;

			// Check whether an enemy is near
			Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

			// Add the enemy to the list of enemies that should be hit with every tick
			foreach (Collider2D coll2D in colliders)
			{
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

		if (cloudParticles != null)
		{
			cloudParticles.Stop();
		}
	}
}
