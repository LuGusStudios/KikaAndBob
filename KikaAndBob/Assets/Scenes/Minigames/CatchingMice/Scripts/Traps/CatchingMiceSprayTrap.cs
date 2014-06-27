using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceSprayTrap : CatchingMiceWorldObjectTrapFurniture {

	public GameObject spraySplashPrefab = null;

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		if (spraySplashPrefab == null)
		{
			CatchingMiceLogVisualizer.use.LogWarning("The spray trap does not have a splash object.");
		}

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

			// If there are enemies, then search the closest one
			// Else, wait for the next fixed update
			if (enemies.Count > 0)
			{
				CatchingMiceCharacterMouse closestEnemy = null;
				float closest = float.MaxValue;
				foreach (CatchingMiceCharacterMouse enemy in enemies)
				{
					float tempDistance = Vector2.Distance(enemy.transform.position.v2(), transform.position.v2());
					if (tempDistance < closest)
					{
						closest = tempDistance;
						closestEnemy = enemy;
					}
				}

				StartCoroutine(SprayRoutine(closestEnemy));

				Stacks = Stacks - 1;

				yield return new WaitForSeconds(Interval);
			}
			else
			{
				yield return new WaitForFixedUpdate();
			}
		}
	}

	protected IEnumerator SprayRoutine(CatchingMiceCharacterMouse target)
	{
		if (spraySplashPrefab == null)
		{
			OnHit(target);
			yield break;
		}

		// Create a copy of the splash and fire it at the target
		float z = Mathf.Max(transform.position.z, target.transform.position.z);
		GameObject spraySplashCopy = (GameObject)GameObject.Instantiate(spraySplashPrefab);
		spraySplashCopy.transform.position = transform.position.z(z);

		// Rotate the splash so that it points to the target
		float angle = Vector2.Dot(Vector2.up, (target.transform.position.v2() - transform.position.v2()).normalized);
		spraySplashCopy.transform.Rotate(0f, 0f, angle * Mathf.Rad2Deg);

		Vector3 originalScale = spraySplashCopy.transform.localScale;
		//spraySplashCopy.transform.localScale = originalScale * 0.1f;

		float time = Vector2.Distance(transform.position.v2(), target.transform.position.v2()) * 0.25f;
		spraySplashCopy.MoveTo(target.transform.position.z(z)).Time(time).Execute();
		//spraySplashCopy.ScaleTo(originalScale).Time(0.1f).Execute();

		yield return new WaitForSeconds(Mathf.Max(0.1f, time));

		OnHit(target);
		GameObject.Destroy(spraySplashCopy);
	}
}
