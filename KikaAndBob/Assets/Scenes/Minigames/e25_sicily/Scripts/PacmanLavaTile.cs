using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanLavaTile : PacmanTileItem 
{
	public GameObject lavaTrailPrefab = null;	
	protected float updateCheckSpeed = 1.0f;
	public List<PacmanTile> surroundingLavaTiles = new List<PacmanTile>();
	public List<PacmanTile> surroundingOpenTiles = new List<PacmanTile>();
	public List<PacmanTile> surroundingValidTiles = new List<PacmanTile>();

	public Sprite lava1Way = null;
	public Sprite lava2WayStraight = null;
	public Sprite lava2WayCorner = null;
	public Sprite lava3Way = null;
	public Sprite lava4Way = null;

	protected bool done = false;
	protected ILugusCoroutineHandle flowRoutineHandle = null;
	
	protected enum LavaTileType
	{
		None = -1,
		End = 0,
		Straight = 1,
		Corner = 2,
		TPiece = 3,
		Crossing = 4,
		Center = 5
	}

	protected LavaTileType lavaTileType = LavaTileType.None;

	public override void Initialize ()
	{
		RegisterSurroundingTiles();

		if (flowRoutineHandle == null || !flowRoutineHandle.Running)
			flowRoutineHandle = LugusCoroutines.use.StartRoutine(UpdateRoutine());	// starting this the old way - it doesn't need to be terminated, and this way it will be stopped if the object disappears
	}

	public void RegisterSurroundingTiles()
	{
		surroundingValidTiles.Clear();
		surroundingLavaTiles.Clear();
		surroundingOpenTiles.Clear();
		
		foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
		{
			bool isOpenTile = true;
			
			foreach (PacmanTileItem tileItem in tile.tileItems)
			{
				if (tileItem.GetComponent<PacmanLavaTile>() != null || tileItem.GetComponent<PacmanLavaTileStart>() != null)
				{
					surroundingLavaTiles.Add(tile);
					isOpenTile = false;
					break;
				}
				else if (tileItem.GetComponent<PacmanLavaStop>() != null)
				{
					isOpenTile = false;
					break;
				}
			}
			
			if (isOpenTile && tile.tileType != PacmanTile.TileType.Collide)
			{
				surroundingOpenTiles.Add(tile);
			}
		}

		// surroundingValidTiles = lavaTiles + open tiles - this is useful because sometimes we want to distinguish the two and sometimes not
		surroundingValidTiles = new List<PacmanTile>(surroundingLavaTiles);
		surroundingValidTiles.AddRange(surroundingOpenTiles);
	}

	protected void Update()
	{
		if (!done && PacmanGameManager.use.gameRunning && PacmanGameManager.use.GetActivePlayer().currentTile == parentTile)
			LugusCoroutines.use.StartRoutine(BurnUp());
	}
	
	public override void OnEnter (PacmanCharacter character)
	{
		if (done)
			return;
		
		
		LugusCoroutines.use.StartRoutine(BurnUp());
	}

	protected IEnumerator BurnUp()
	{
		PacmanGameManager.use.gameRunning = false;
		
		done = true;
		
		GameObject flameObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("FlameAnimation"));
		Vector3 playerPos = PacmanGameManager.use.GetActivePlayer().transform.position.zAdd(-1f);
		flameObject.transform.position = playerPos;
		flameObject.transform.parent = PacmanLevelManager.use.temporaryParent;
	
		PacmanGameManager.use.GetActivePlayer().HideCharacter();
		
		yield return new WaitForSeconds(0.5f);
		
		GameObject ashObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("AshPile"));
		ashObject.transform.position = playerPos;
		ashObject.transform.parent = PacmanLevelManager.use.temporaryParent;

		yield return new WaitForSeconds(1.5f);
		
		PacmanGameManager.use.LoseLife();
		
		yield break;
	}

	protected IEnumerator UpdateRoutine()
	{
		while (true)
		{
			if (surroundingLavaTiles.Count >= 4 || done)
				yield break;

			// don't want this continuing after winning or losing
			while (!PacmanGameManager.use.gameRunning)
				yield return null;

			// first update the surroundingOpenTiles list. An other lava tile might already have started claiming it!
			for (int i = surroundingOpenTiles.Count - 1; i >= 0; i--) 
			{
				foreach (PacmanTileItem tileItem in surroundingOpenTiles[i].tileItems)
				{
					if (tileItem.GetComponent<PacmanLavaTile>() != null || tileItem.GetComponent<PacmanLavaTileStart>() != null)
					{
						surroundingOpenTiles.Remove(surroundingOpenTiles[i]);
						break;
					}
				}	
			}

			foreach(PacmanTile tile in surroundingOpenTiles)
			{
				if (tile == null)	// should probably never happen, but doesn't hurt to check
					continue;

				if (this == null)
				{
					Debug.LogError("NULL");
					continue;
				}

				GameObject newLavaTileObject = (GameObject) Instantiate(lavaTrailPrefab);
				newLavaTileObject.transform.position = tile.GetWorldLocation().v3().z(this.transform.position.z);
				newLavaTileObject.name = "LavaTrail" + tile.ToString();

				if (PacmanLevelManager.use.temporaryParent != null)
				{
					newLavaTileObject.transform.parent = PacmanLevelManager.use.temporaryParent;
				}
				else
				{
					Debug.LogWarning("PacmanLavaTile: No temporary items parent found. This tile will not be removed in the next round!");
					newLavaTileObject.transform.parent = this.transform.parent;
				}
				
				PacmanLavaTileStart newLavaTile = newLavaTileObject.GetComponent<PacmanLavaTileStart>();
				newLavaTile.parentTile = tile;
				newLavaTile.originLavaTile = this;


				surroundingLavaTiles.Add(tile);
				tile.tileItems.Add(newLavaTile);

				newLavaTile.Initialize();
			} 

			yield return new WaitForSeconds(updateCheckSpeed);
		}
	}

	protected void OnDestroy()
	{
		if (flowRoutineHandle != null && flowRoutineHandle.Running)
			flowRoutineHandle.StopRoutine();
	}

	public override void Reset ()
	{
		RegisterSurroundingTiles();

		if (flowRoutineHandle == null || !flowRoutineHandle.Running)
			flowRoutineHandle = LugusCoroutines.use.StartRoutine(UpdateRoutine());
	}

    public override void OnTryEnter(PacmanCharacter character)
	{
	}

	public void InitializeSprite()
	{
		SpriteRenderer thisSpriteRenderer = GetComponent<SpriteRenderer>();
		
		Sprite newPickedSprite = PickLavaSprite();
		
		if (newPickedSprite != null)
			thisSpriteRenderer.sprite = newPickedSprite;
		
		RotateLavaSprite(gameObject);
	}

	protected Sprite PickLavaSprite()
	{
		int connectCount = surroundingValidTiles.Count;

		if (connectCount <= 1)	// end piece
		{
			lavaTileType = LavaTileType.End;
			return lava1Way;
		}
		else if (connectCount == 2)
		{
			if (surroundingValidTiles[0].gridIndices.x == surroundingValidTiles[1].gridIndices.x ||	// i.e. this is a straight piec
			    surroundingValidTiles[0].gridIndices.y == surroundingValidTiles[1].gridIndices.y)
    		{
				lavaTileType = LavaTileType.Straight;
				return lava2WayStraight;
			}
			else 																		// i.e. this is a corner
			{
				lavaTileType = LavaTileType.Corner;
				return lava2WayCorner;
			}
		}
		else if (connectCount == 3)	// T piece
		{
			lavaTileType = LavaTileType.TPiece;
			return lava3Way;
		}
		else
		{
			lavaTileType = LavaTileType.Crossing;
			return lava4Way;
		}



//		if (surroundingOpenTiles.Count <= 0 && surroundingLavaTiles.Count <= 1)	// end piece
//		{
//			lavaTileType = LavaTileType.End;
//			return lava1Way;
//		}
//		else if (surroundingOpenTiles.Count == 1 && surroundingLavaTiles.Count == 1)	// straight piece
//		{
//			if (surroundingOpenTiles[0].gridIndices.x == surroundingLavaTiles[0].gridIndices.x ||
//			    surroundingOpenTiles[0].gridIndices.y == surroundingLavaTiles[0].gridIndices.y)
//			{
//				lavaTileType = LavaTileType.Straight;
//				return lava2WayStraight;
//			}
//			else
//			{
//				lavaTileType = LavaTileType.Corner;
//				return lava2WayCorner;
//			}
//		}
//		else if (surroundingOpenTiles.Count == 2 && surroundingLavaTiles.Count == 1)	// T piece
//		{
//			lavaTileType = LavaTileType.TPiece;
//			return lava3Way;
//		}
//		else if (surroundingOpenTiles.Count +  surroundingLavaTiles.Count >= 4)	// crossing
//		{
//			lavaTileType = LavaTileType.Crossing;
//			return lava4Way;
//		}
//		else if (surroundingLavaTiles.Count == 2)	// edge corner
//		{
//			if (surroundingOpenTiles.Count == 0)
//			{
//				lavaTileType = LavaTileType.Corner;
//				return lava2WayCorner;
//			}
//			else if (surroundingOpenTiles.Count == 1)
//			{
//				lavaTileType = LavaTileType.TPiece;
//				return lava3Way;
//			}
//			else if (surroundingOpenTiles.Count == 2)
//			{
//				lavaTileType = LavaTileType.TPiece;
//				return lava3Way;
//			}
//		}
//
//
//		return null;
	}
	
	protected void RotateLavaSprite(GameObject target)
	{
		float zRotation = 0;

		if (lavaTileType == LavaTileType.End)	// end piece
		{
			foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
			{
				if (tile == surroundingLavaTiles[0])
					break;
				
				zRotation -= 90;
			}
		}
		else if (lavaTileType == LavaTileType.Straight)	
		{	
			foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
			{
				if (tile == null)
					continue;
				
				if (tile == surroundingValidTiles[0])											// ANY lava tile to align to will do
					break;
				
				zRotation -= 90.0f;
			}
		}
		else if (lavaTileType == LavaTileType.Corner)																					
		{
			// check tiles around this one in order UP - RIGHT - DOWN - LEFT
			// this tile is properly aligned if the current tile is open/lava and the next tile is also open/lava

			int nextIndex = 0;
			
			PacmanTile[] allSurroundingTiles = PacmanLevelManager.use.GetTilesAroundStraight(parentTile);	// iterate over ALL tiles, not just valid ones
			
			for (int i = 0; i < allSurroundingTiles.Length; i++) 
			{
				if (allSurroundingTiles[i] == null)
					continue;
				
				nextIndex = i+1;
				
				if (nextIndex >= allSurroundingTiles.Length)
				{
					nextIndex = 0;
				}

			
				if (i < allSurroundingTiles.Length - 1)
				{
					if (surroundingValidTiles.Contains(allSurroundingTiles[i]))	// don't just check for non-collider tile, this way we control what is a 'lava-able' tile from one point
					{
						if (surroundingValidTiles.Contains(allSurroundingTiles[nextIndex]))
						{
							break;
						}
					}
				}
				else 	// EXCEPT if we already tried all other options: the rotate just HAS to be correct then
				{
					if (surroundingValidTiles.Contains(allSurroundingTiles[i]))
					{
						break;
					}
				}
				
				zRotation -= 90.0f;
			}
		}
		else if (lavaTileType == LavaTileType.TPiece)																					
		{
			// check tiles around this one in order UP - RIGHT - DOWN - LEFT
			// this tile is properly aligned if the current tile is open/lava and the next tile is also open/lava
			
			int nextTileIndex = 0;
			
			PacmanTile[] allSurroundingTiles = PacmanLevelManager.use.GetTilesAroundStraight(parentTile);
			
			for (int i = 0; i < allSurroundingTiles.Length; i++) 
			{
				if (allSurroundingTiles[i] == null)
					continue;

				if (i < allSurroundingTiles.Length - 1)
				{
					if (surroundingValidTiles.Contains(allSurroundingTiles[i]))	// don't just check for non-collider tile, this way we control what is a 'lava-able' tile from one point
					{
						nextTileIndex = i+1;
						
						if (nextTileIndex >= allSurroundingTiles.Length)
						{
							nextTileIndex -= allSurroundingTiles.Length;
						}

						if (surroundingValidTiles.Contains(allSurroundingTiles[nextTileIndex]))
						{
							nextTileIndex++;

							if (nextTileIndex >= allSurroundingTiles.Length)
							{
								nextTileIndex -= allSurroundingTiles.Length;
							}

							if (surroundingValidTiles.Contains(allSurroundingTiles[nextTileIndex]))
							{
								break;
							}
						}
					}
				}
				else 	// EXCEPT if we already tried all other options: the rotate just HAS to be correct then
				{
					if (surroundingValidTiles.Contains(allSurroundingTiles[i]))
					{
						break;
					}
				}
				
				zRotation -= 90.0f;
			}
		}

		target.transform.localEulerAngles = new Vector3(0, 0, zRotation); 
	}

}
