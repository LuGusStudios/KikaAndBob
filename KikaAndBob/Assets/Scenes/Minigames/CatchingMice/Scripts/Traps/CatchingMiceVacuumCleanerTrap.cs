using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The Vacuum Cleaner trap monitors its surrounding tiles
// for enemies, and starts "cleaning" when an enemy
// enters its range. The vacuum cleaner does its job
// for a certain period of time (suctionTime) and
// goes on recharge before it can be activated again
// (interval -- from base-class)
public class CatchingMiceVacuumCleanerTrap : CatchingMiceWorldObjectTrapGround {

	public float suctionTime = 1f;
	public float suctionSpeed = 1f;
	public string particles = "";

	protected ILugusCoroutineHandle routineHandle = null;
	protected ParticleSystem dustParticles = null;

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		// Find the particle system
		dustParticles = transform.FindChild(particles).GetComponent<ParticleSystem>();

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
			// then start the vacuum routine
			if ((enemies.Count > 0) && (!routineHandle.Running))
			{
				routineHandle.StartRoutine(VacuumRoutine());

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

	protected IEnumerator VacuumRoutine()
	{
		Vector2 min, max;
		CalculateTrapBounds(out min, out max);

		List<CatchingMiceCharacterMouse> caughtInSuction = new List<CatchingMiceCharacterMouse>();
		List<float> enemyHitTimes = new List<float>();

		float timeLeft = suctionTime;

		if (dustParticles != null)
		{
			dustParticles.Play();
		}

		while(CatchingMiceGameManager.use.gameRunning
			&& (timeLeft > 0)
			&& (health > 0))
		{
			timeLeft -= Time.fixedDeltaTime;

			// Check whether a new enemy is near
			Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

			foreach (Collider2D coll2D in colliders)
			{
				CatchingMiceCharacterMouse enemy = coll2D.transform.parent.GetComponent<CatchingMiceCharacterMouse>();

				if (enemy == null)
				{
					continue;
				}

				if (!caughtInSuction.Contains(enemy))
				{
					caughtInSuction.Add(enemy);
					enemyHitTimes.Add(1f);

					enemy.StopCurrentBehaviour();

					// Set the target position of the enemy
					// to be the center of the vacuum cleaner
					Vector3 targetPosition = transform.position;
					targetPosition = targetPosition.z(Mathf.Min(transform.position.z, enemy.transform.position.z));

					enemy.transform.position = enemy.transform.position.z(targetPosition.z);

					float distance = Vector2.Distance(targetPosition.v2(), enemy.transform.position.v2());
					float time = Mathf.Min(timeLeft, distance / suctionSpeed);

					enemy.gameObject.MoveTo(targetPosition).Time(time).Execute();
				}
			}

			// Go over all of the registered enemies, and update their times
			for (int i = caughtInSuction.Count - 1; i >= 0 ; --i)
			{
				enemyHitTimes[i] -= Time.fixedDeltaTime;

				if (enemyHitTimes[i] < 0)
				{
					OnHit(caughtInSuction[i]);
					enemyHitTimes[i] = 1f;

					if (caughtInSuction[i].Health <= 0)
					{
						caughtInSuction.RemoveAt(i);
						enemyHitTimes.RemoveAt(i);
					}

				}
			}

			yield return new WaitForFixedUpdate();
		}

		if (dustParticles != null)
		{
			dustParticles.Stop();
		}

		// For each enemy caught in the suction
		// re-assign their parent tile, and reactivate
		// them by letting them search for a new target
		foreach(CatchingMiceCharacterMouse enemy in caughtInSuction)
		{
			CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(enemy.transform.position.x, enemy.transform.position.y);
			enemy.currentTile = tile;
			enemy.GetTarget();
		}
	}
}
