using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorUpdater : PacmanLevelUpdater {

	public float doorChangeTime = 5;
	public int minimumDoorsClosed = 2;
	public int minimumDoorsOpen =2;

	protected List<GameTile> doors = new List<GameTile>();

	public override void Activate()
	{
		if (loopHandle != null)
		{
			loopHandle.StopRoutine();
		}

		doors.Clear();

		foreach(GameTile tile in PacmanLevelManager.use.levelTiles)
		{
			if (tile.tileType == GameTile.TileType.Door)
			{
				doors.Add(tile);
			}
		}
		loopHandle = LugusCoroutines.use.StartRoutine(DoorUpdateRoutine());
	}

	public override void Deactivate()
	{
		loopHandle.StopRoutine();
	}

	protected IEnumerator DoorUpdateRoutine()
	{
		while(true)
		{
			while(!PacmanGameManager.use.gameRunning)
			{
				yield return new WaitForEndOfFrame();
			}

			ChangeDoors();
			
			yield return new WaitForSeconds(doorChangeTime);
		}
	}

	public void ResetDoors()
	{	
		foreach(GameTile door in doors)
		{
			door.tileType = GameTile.TileType.Open;
		}
		
		PacmanGUIManager.use.UpdateDoors(doors);
	}
	
	public void ChangeDoors()
	{
		if (!PacmanGameManager.use.gameRunning)
			return;
		
		int closedDoors = 0;

		// first, open all doors
		foreach(GameTile door in doors)
		{
			door.tileType = GameTile.TileType.Open;
		}
		
		// keep a minimum of doors open and a minimum of doors closed
		while(closedDoors + minimumDoorsOpen < doors.Count)
		{
			// close random door
			int randomIndex = Random.Range(0, doors.Count);
			GameTile doorTile = doors[randomIndex];
			doorTile.tileType = GameTile.TileType.Collide;
			closedDoors++;

			// update exit count for tiles around the door tile
			//GameTile updatedTile;

			foreach(GameTile updatedTile in PacmanLevelManager.use.GetTilesAroundStraight(doorTile))
			{
				updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
			}

//			updatedTile = PacmanLevelManager.use.GetTileInDirection(doorTile, Character.CharacterDirections.Up);
//			if(updatedTile != null)
//				updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
//			
//			updatedTile = PacmanLevelManager.use.GetTileInDirection(doorTile, Character.CharacterDirections.Down);
//			if(updatedTile != null)
//				updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
//
//			updatedTile = PacmanLevelManager.use.GetTileInDirection(doorTile, Character.CharacterDirections.Left);
//			if(updatedTile != null)
//				updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
//
//			updatedTile = PacmanLevelManager.use.GetTileInDirection(doorTile, Character.CharacterDirections.Right);
//			if(updatedTile != null)
//				updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
			
			if (closedDoors >= minimumDoorsClosed)
			{
				if (Random.value >= 0.5F)	//each iteration, 50% chance no further doors will be closed
					break;
			}
		}
		
		foreach(GameTile door in doors)
		{
			if (door.tileType == GameTile.TileType.Open)
			{
				// update exit count for tiles left and right of other doors - these may still have their exit counts lowered from having been closed before
				GameTile updatedTile;

				updatedTile = PacmanLevelManager.use.GetTile(door.gridIndices + new Vector2(-1,0));
				if(updatedTile != null)
					updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
				
				updatedTile = PacmanLevelManager.use.GetTile(door.gridIndices + new Vector2(1,0));
				if(updatedTile != null)
					updatedTile.exitCount = PacmanLevelManager.use.GetNumberOfExits(updatedTile);
			}
		}
		
		PacmanGUIManager.use.UpdateDoors(doors);
	}
}
