using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceHole : CatchingMiceWorldObject
{
    public enum CharacterDirections
    {
        Up = 1,			// 0001
        Right = 2,		// 0010
        Down = 4,		// 0100
        Left = 8,		// 1000

        Undefined = -1
    }

    public string id = "";
    public CharacterDirections spawnDirection = CharacterDirections.Undefined;
    public Vector3 spawnPoint = Vector3.zero;
    
    public void SetHoleSpawnPoint(CharacterDirections direction, CatchingMiceTile tile)
    {
        spawnDirection = direction;
        float tileOffset = CatchingMiceLevelManager.use.scale;

		// Relocate and rotate the mice hole based on its direction
        switch (spawnDirection)
        {
            case CharacterDirections.Down:
                //you want to divide by 2 because you want your enemy to spawn right between 2 tiles, else the enemy appears to be floating 
                spawnPoint = parentTile.location.yAdd(tileOffset / 2);
                break;
            case CharacterDirections.Left:
                spawnPoint = parentTile.location.xAdd(tileOffset);
				transform.Rotate(new Vector3(0, 0, -90));
                break;
            case CharacterDirections.Right:
                spawnPoint = parentTile.location.xAdd(-tileOffset);
				transform.Rotate(new Vector3(0, 0, 90));
                break;
            case CharacterDirections.Up:
                spawnPoint = parentTile.location.yAdd(-tileOffset);
				transform.localScale = transform.localScale.y(-1);
                break;
            case CharacterDirections.Undefined:
				CatchingMiceLogVisualizer.use.LogError("Undefined direction passed. Mice hole could not be made.");
				
                break;
        }
    }

	public override void SetTileType(System.Collections.Generic.List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			tile.tileType = tile.tileType | tileType;
			tile.hole = this;
		}

		// Here we don't apply a vertical grid offset, because the sprite placement
		// already complements that
		transform.position = transform.position.zAdd(-0.1f);
	}

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		// A mice hole should only be placed on the ground

		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture)
		{
			CatchingMiceLogVisualizer.use.LogError("Mice hole " + transform.name + " cannot be placed on furniture.");
			return false;
		}

		return true;
	}

	public void VisualizePath(List<CatchingMiceWaypoint> graph, float visualiseTime)
	{
		// Make a copy of the graph
		List<CatchingMiceWaypoint> graphCopy = new List<CatchingMiceWaypoint>(graph);
		
		// Find the closest cheese tile
		CatchingMiceWaypoint target = null;
		float distance = float.MaxValue;

		foreach(CatchingMiceTile tile in CatchingMiceLevelManager.use.CheeseTiles)
		{
			float tempDistance = Vector2.Distance(parentTile.location.v2(), tile.location.v2());
			if (tempDistance < distance)
			{
				distance = tempDistance;
				target = tile.waypoint;
			}
		}

		if (target != null)
		{
			bool fullPath = false;
			List<CatchingMiceWaypoint> path = CatchingMiceUtil.FindPath(graphCopy, parentTile.waypoint, target, out fullPath, CatchingMiceTile.TileType.Ground);
			StartCoroutine(VisualizePathRoutine(path, visualiseTime));
		}
	}

	// TODO: Implement the actual visualization
	private IEnumerator VisualizePathRoutine(List<CatchingMiceWaypoint> path, float visualizeTime)
	{
		if (CatchingMiceLevelManager.use.miceStepsPrefab == null)
		{
			CatchingMiceLogVisualizer.use.LogError("No mice steps prefab could be found to visualize the mice steps.");
			yield break;
		}

		if (path.Count == 0)
		{
			yield break;
		}

		float timePerStep = visualizeTime / (path.Count * 2f);

		List<GameObject> miceSteps = new List<GameObject>();

		Vector2 dir = Vector2.zero;
		
		// Spawn the steps to the destination
		for (int i = path.Count - 1; i >= 0; --i )
		{
			// Direction for mice step
			if (i > 0)
			{
				CatchingMiceWaypoint current = path[i];
				CatchingMiceWaypoint next = path[i - 1];

				dir = Vector2.zero;

				if ((current.parentTile.gridIndices.x - next.parentTile.gridIndices.x) > 0.5f)
				{
					dir.x = -1f;
				}
				else if ((current.parentTile.gridIndices.x - next.parentTile.gridIndices.x) < -0.5f)
				{
					dir.x = 1f;
				}
				else if ((current.parentTile.gridIndices.y - next.parentTile.gridIndices.y) > 0.5f)
				{
					dir.y = -1f;
				}
				else if ((current.parentTile.gridIndices.y - next.parentTile.gridIndices.y) < -0.5f)
				{
					dir.y = 1f;
				}
			}

			GameObject miceStep = (GameObject)GameObject.Instantiate(CatchingMiceLevelManager.use.miceStepsPrefab);
			miceStep.transform.position = path[i].parentTile.location.zAdd(-0.1f);

			// Rotate the steps in the right direction
			if (dir.x == -1f)
			{
				miceStep.transform.Rotate(0f, 0f, -90f);
			}
			else if (dir.x == 1f)
			{
				miceStep.transform.Rotate(0f, 0f, 90f);
			}
			else if (dir.y == 1f)
			{
				miceStep.transform.localScale = miceStep.transform.localScale.y(-1f);
			}

			// Let the mice step increase in size
			Vector3 originalScale = miceStep.transform.localScale;
			miceStep.transform.localScale *= 0.1f;
			miceStep.ScaleTo(originalScale).Time(timePerStep).Execute();

			miceSteps.Add(miceStep);

			yield return new WaitForSeconds(timePerStep);
		}

		// Let the mice steps disappear again
		for (int i = 0; i < miceSteps.Count; ++i)
		{
			GameObject miceStep = miceSteps[i];

			// Let the mice step get smaller again and destroyed at the end
			Vector3 smallScale = miceStep.transform.localScale * 0.1f;
			miceStep.ScaleTo(smallScale).Time(timePerStep).Execute();

			GameObject.Destroy(miceStep, timePerStep);

			yield return new WaitForSeconds(timePerStep);
		}

		miceSteps.Clear();
	}
}
