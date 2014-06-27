using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCooker : CatchingMiceObstacle
{
	public float damage = 1.0f;

	protected List<int> waveActivity = new List<int>();
	protected ILugusCoroutineHandle cookingHandle = null;

	public override void SetupLocal()
	{
		base.SetupLocal();
	}

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		CatchingMiceGameManager.use.WaveStarted += OnWaveStarted;
		CatchingMiceGameManager.use.WaveEnded += OnWaveEnded;

		cookingHandle = LugusCoroutines.use.GetHandle();
		cookingHandle.Claim();
	}

	public override void SetTileType(List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			tile.tileType = tile.tileType | tileType;
			tile.obstacle = this;
		}

		transform.position = transform.position.yAdd(tiles[0].furniture.yOffset + yOffset).zAdd(-tiles[0].furniture.zOffset - zOffset);
	}

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.furniture == null) || ((tile.tileType & CatchingMiceTile.TileType.Ground) == CatchingMiceTile.TileType.Ground))
		{
			CatchingMiceLogVisualizer.use.LogError("Obstacle " + transform.name + " cannot be placed on the ground.");
			return false;
		}
		else if ((tile.obstacle != null) || ((tile.tileType & CatchingMiceTile.TileType.Obstacle) == CatchingMiceTile.TileType.Obstacle))
		{
			CatchingMiceLogVisualizer.use.LogError("Obstacle " + transform.name + " cannot be placed because another obstacle is already present.");
			return false;
		}

		return true;
	}
	
	public override void FromXMLObstacleDefinition(string configuration)
	{
		TinyXmlReader parser = new TinyXmlReader(configuration);
		while (parser.Read("Configuration"))
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING)
				&& (parser.tagName == "Wave"))
			{
				waveActivity.Add(int.Parse(parser.content));
			}
		}
	}

	protected void OnWaveStarted(int waveIndex)
	{
		if (waveActivity.Contains(waveIndex))
		{
			if (spriteRenderer != null)
			{
				spriteRenderer.sprite = activeSprite;
			}

			cookingHandle.StartRoutine(CookingRoutine());
		}
	}

	protected void OnWaveEnded(int waveIndex)
	{
		cookingHandle.StopRoutine();

		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = inactiveSprite;
		}
	}

	protected IEnumerator CookingRoutine()
	{
		BoxCollider2D box2D = GetComponentInChildren<BoxCollider2D>();

		if (box2D == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The cooker obstacle could not find its collider.");
			yield break;
		}

		Vector2 min = box2D.transform.position.xAdd(-(box2D.size.x * 0.5f)).yAdd(-(box2D.size.y * 0.5f)).v2();
		Vector2 max = box2D.transform.position.xAdd(box2D.size.x * 0.5f).yAdd(box2D.size.y * 0.5f).v2();

		// List of enemies that have recently been hit by the
		// obstacle, and should be temporarily immune to the
		// obstacle's damage
		List<CatchingMiceCharacterMouse> tempImmunityList = new List<CatchingMiceCharacterMouse>();
		List<float> tempImmunityTime = new List<float>();

		while (CatchingMiceGameManager.use.gameRunning)
		{
			List<CatchingMiceCharacterMouse> enemies = new List<CatchingMiceCharacterMouse>();
			Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

			// Select those enemies that are currently not immune
			// and are on top of the cooker (z needs to be smaller)
			foreach (Collider2D coll2D in colliders)
			{
				CatchingMiceCharacterMouse enemy = coll2D.transform.parent.GetComponent<CatchingMiceCharacterMouse>();

				if ((enemy != null)
					&& (!tempImmunityList.Contains(enemy))
					&& (enemy.transform.position.z < box2D.transform.position.z))
				{
					enemies.Add(enemy);
				}
			}

			// Apply the damage to the chosen enemies, and place
			// them in the temporary immunity list
			foreach (CatchingMiceCharacterMouse enemy in enemies)
			{
				enemy.Health -= damage;

				tempImmunityList.Add(enemy);
				tempImmunityTime.Add(1.0f);
			}

			// Reduce the time of the immunity duration
			// for each enemy in the immunity list
			for (int i = tempImmunityList.Count - 1; i >= 0; --i)
			{
				tempImmunityTime[i] -= Time.fixedDeltaTime;

				if (tempImmunityTime[i] <= 0)
				{
					tempImmunityList.RemoveAt(i);
					tempImmunityTime.RemoveAt(i);
				}
			}

			yield return new WaitForFixedUpdate();
		}
	}

	protected void OnDrawGizmos()
	{
		BoxCollider2D box2D = GetComponentInChildren<BoxCollider2D>();

		if (box2D == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The cooker obstacle could not find its collider.");
			return;
		}

		Vector2 min = box2D.transform.position.xAdd(-(box2D.size.x * 0.5f)).yAdd(-(box2D.size.y * 0.5f)).v2();
		Vector2 max = box2D.transform.position.xAdd(box2D.size.x * 0.5f).yAdd(box2D.size.y * 0.5f).v2();

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(box2D.transform.position, new Vector3(max.x - min.x, max.y - min.y, 1f));
	}
}
