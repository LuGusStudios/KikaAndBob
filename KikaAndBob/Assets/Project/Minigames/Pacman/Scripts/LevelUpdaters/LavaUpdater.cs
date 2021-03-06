using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LavaUpdater : PacmanLevelUpdater {

	public float lavaInterval = 10;
	public float lavaPassTime = 5;
	protected GameObject lavaPrefab = null;
	protected GameObject lavaStream = null;

	protected Dictionary<PacmanTile, PacmanTile.TileType> originalTileTypes = new Dictionary<PacmanTile, PacmanTile.TileType>();
	protected Dictionary<PacmanTile, Transform> lavaTiles = new Dictionary<PacmanTile, Transform>();


	public override void Activate()
	{
		if (loopHandle != null)
		{
			loopHandle.StopRoutine();
		}
	
		lavaPrefab = PacmanLevelManager.use.GetPrefab("LavaTile");
		lavaStream = PacmanLevelManager.use.GetPrefab("LavaStream");

		loopHandle = LugusCoroutines.use.StartRoutine(LavaUpdateRoutine());
	}

	public override void Deactivate()
	{
		loopHandle.StopRoutine();

		foreach(Transform t in lavaTiles.Values)
		{
			Destroy(t.gameObject);
		}
		
		lavaTiles.Clear();
		originalTileTypes.Clear();
	}

	protected IEnumerator LavaUpdateRoutine()
	{
		while(true)
		{
			while(!PacmanGameManager.use.gameRunning)
			{
				yield return new WaitForEndOfFrame();
			}

			// clear any lava sprites still present
			foreach(Transform t in lavaTiles.Values)
			{
				Destroy(t.gameObject);
			}
			
			lavaTiles.Clear();
			originalTileTypes.Clear();

			LugusCoroutines.use.StartRoutine(DropLava());
			
			yield return new WaitForSeconds(lavaPassTime + lavaInterval);
		}
	}

	protected IEnumerator DropLava()
	{
		int streamXLocation = 10;
		int streamYLocation = 1;
		PacmanTile startTile = PacmanLevelManager.use.GetTile(streamXLocation, PacmanLevelManager.use.height - 1);

		// stream first descends to cross entire level
		while (streamYLocation <= PacmanLevelManager.use.height)
		{
			foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesInDirection(startTile, streamYLocation, PacmanCharacter.CharacterDirections.Down))
			{
				if (tile == null)
					continue;

				if (!originalTileTypes.ContainsKey(tile))
				{
					originalTileTypes.Add(tile, tile.tileType);
				}

				if (tile.tileType != PacmanTile.TileType.Lethal)
				{
					tile.tileType = PacmanTile.TileType.Lethal;
				}

				if (!lavaTiles.ContainsKey(tile))
				{
					GameObject lavaTile = (GameObject) Instantiate(lavaPrefab);
					Transform lavaTileTransform = lavaTile.transform;

					lavaTileTransform.parent = PacmanLevelManager.use.effectsParent;
					lavaTileTransform.localPosition = tile.location;
					lavaTiles.Add(tile, lavaTileTransform);

					foreach(PacmanTile updatedTile in PacmanLevelManager.use.GetTilesAroundStraight(tile))
					{
						updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
					}
				}

			}

			streamYLocation ++;
			yield return new WaitForSeconds(lavaPassTime / ( PacmanLevelManager.use.height*2));
		}

		startTile = PacmanLevelManager.use.GetTile(streamXLocation, 0);

		// then have top of stream descend
		while (streamYLocation > 0)
		{
			List<PacmanTile> currentLavaTiles = new List<PacmanTile>(PacmanLevelManager.use.GetTilesInDirection(startTile, streamYLocation, PacmanCharacter.CharacterDirections.Up));
			List<PacmanTile> deleteList = new List<PacmanTile>();	// we can't delete things while iterating over the dictionary directly, so we add them a list of things to be deleted

			foreach (PacmanTile tile in lavaTiles.Keys)
			{
				if (!currentLavaTiles.Contains(tile))
				{
					tile.tileType = originalTileTypes[tile];
					deleteList.Add(tile);
				}
			}

			for (int i = 0; i < deleteList.Count; i++) 
			{
				foreach(PacmanTile updatedTile in PacmanLevelManager.use.GetTilesAroundStraight(deleteList[i]))
				{
					updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
				}
				Destroy(lavaTiles[deleteList[i]].gameObject);
				lavaTiles.Remove(deleteList[i]);
			}

			streamYLocation --;
			yield return new WaitForSeconds(lavaPassTime / ( PacmanLevelManager.use.height*2));
		}
	}
}
