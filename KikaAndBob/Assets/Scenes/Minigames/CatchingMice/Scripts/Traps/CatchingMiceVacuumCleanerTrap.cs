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
	protected List<CatchingMiceTile> surroundingTiles = new List<CatchingMiceTile>();

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		// Find the particle system
		dustParticles = transform.FindChild(particles).GetComponent<ParticleSystem>();

		routineHandle = LugusCoroutines.use.GetHandle();
		routineHandle.Claim();

		if (surroundingTiles.Count <= 0)
		{
			surroundingTiles.AddRange(CatchingMiceLevelManager.use.GetTilesAround(parentTile, tileRange));
		}

		StartCoroutine(TrapRoutine());
	}

	protected override IEnumerator TrapRoutine()
	{
		Vector2 min, max;
		CalculateTrapBounds(out min, out max);

		while (CatchingMiceGameManager.use.GameRunning
			&& (ammo > 0)
			&& (health > 0))
		{
//			// Check whether an enemy is near
//			List<CatchingMiceCharacterMouse> enemies = new List<CatchingMiceCharacterMouse>();
//			Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);
//
//			foreach (Collider2D coll2D in colliders)
//			{
//				CatchingMiceCharacterMouse enemy = null;
//
//				if (coll2D.transform.parent != null)
//					enemy = coll2D.transform.parent.GetComponent<CatchingMiceCharacterMouse>();
//
//				if (enemy != null)
//				{
//					enemies.Add(enemy);
//				}
//			}

			bool enemyFound = false;

			foreach(CatchingMiceCharacterMouse enemy in CatchingMiceLevelManager.use.Enemies)
			{
				if (surroundingTiles.Contains(enemy.currentTile))
				{
					enemyFound = true;
					break;
				}
			}
			
			// If there are enemies in the neighborhood
			// then start the vacuum routine
			if (enemyFound && (routineHandle != null && !routineHandle.Running))
			{
				yield return routineHandle.StartRoutine(VacuumRoutine());

				// Wait until the vacuum routine is done
				// and then wait another interval time
				// to start again
//				while (routineHandle.Running)
//				{
//					yield return new WaitForEndOfFrame();
//				}

				Ammo = Ammo - 1;

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
		if (dustParticles != null)
		{
			dustParticles.Play();
		}

		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(attackSoundKey));

		float activationTimer = 0;
		List<CatchingMiceCharacterMouse> caughtMice = new List<CatchingMiceCharacterMouse>();
		List<GameObject> caughtCookies = new List<GameObject>();

		while(activationTimer < suctionTime || caughtMice.Count > 0)
		{
			activationTimer += Time.deltaTime;

			if (activationTimer < suctionTime)	// only add mice if activation time isn't over
			{
				foreach(CatchingMiceCharacterMouse enemy in CatchingMiceLevelManager.use.Enemies)
				{
					if (caughtMice.Contains(enemy))
					{
						continue;
					}

					if (surroundingTiles.Contains(enemy.currentTile))
					{
						caughtMice.Add(enemy);
						enemy.StopCurrentBehaviour();
						enemy.gameObject.MoveTo(transform.position).Speed(2).EaseType(iTween.EaseType.easeInBack).Execute();
					}
				}
			}

			for (int i = caughtMice.Count - 1; i >= 0; i--) 
			{
				if (Vector2.Distance(caughtMice[i].transform.position, this.transform.position) < 0.1f)
				{
					caughtMice[i].Health = 0;
					caughtMice.Remove(caughtMice[i]);
				}
			}

			foreach(CatchingMiceTile tile in surroundingTiles)
			{
				if (tile.Cookies > 0)
				{
					AttractCookies(tile);
				}
			}

			yield return null;
		}

		// check for cookies one more time - the last mouse might have drop some
		foreach(CatchingMiceTile tile in surroundingTiles)
		{
			if (tile.Cookies > 0)
			{
				AttractCookies(tile);
			}
		}


		
		if (dustParticles != null)
		{
			dustParticles.Stop();
		}

		yield break;
	}

	protected void AttractCookies(CatchingMiceTile tile)
	{
		GameObject cookieObject = tile.ReleaseCookies();
		cookieObject.transform.position = cookieObject.transform.position.z(transform.position.z);
		cookieObject.MoveTo(transform.position).Time(1f).EaseType(iTween.EaseType.easeInBack).Execute();
		Destroy(cookieObject, 1f);
	}
}
