using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceInput : LugusSingletonRuntime<CatchingMiceInput>
{
	public List<CatchingMiceWaypoint> pathToWalk = new List<CatchingMiceWaypoint>();
	protected CatchingMiceCharacterPlayer _character = null;
	protected CatchingMiceTile _previousTile = null;
	protected CatchingMiceTile _lastAddedWaypoint = null;

	protected LineRenderer _lineRenderer = null;

	protected float _timer = 0.0f;
	// Use this for initialization
	void Start()
	{
		Transform lineRenderer = GameObject.Find("LineRenderer").transform;
		if (lineRenderer != null)
		{
			_lineRenderer = lineRenderer.GetComponent<LineRenderer>();
			if (_lineRenderer == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Line Renderer not found");
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		CatMovementInput();

		VisualizePath();

		CheckPlayerObjectInteraction();
	}
	//protected void CheckDraggingPoints()
	//{
	//    Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(_character.transform);
	//    //CatchingMiceVisualizer.use.Log(dragPoint);
	//    CatchingMiceTile currentTile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y);
	//    if (currentTile == null)
	//        return;

	//    if (currentTile == _previousTile || !_character.IsWalkable(currentTile))
	//        return;

	//    //CatchingMiceVisualizer.use.Log("Current Tile : " + currentTile);
	//    _previousTile = currentTile;

	//    //when the waypoints is already in the list, ignore it
	//    if (pathToWalk.Contains(currentTile.waypoint))
	//    {
	//        if (pathToWalk[pathToWalk.Count - 2] == currentTile.waypoint)
	//        {
	//            pathToWalk.Remove(pathToWalk[pathToWalk.Count - 1]);
	//            _lastAddedWaypoint = currentTile;
	//            _lineRenderer.SetVertexCount(pathToWalk.Count);

	//        }
	//        return;
	//    }


	//    //when you are on a new tile, and the new tile has not been found in the list,
	//    //check in the 4 directions to see if it can add it
	//    if (currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().xAdd(+1) ||
	//        currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().xAdd(-1) ||
	//        currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().yAdd(+1) ||
	//        currentTile.gridIndices.v3() == _lastAddedWaypoint.gridIndices.v3().yAdd(-1))
	//    {

	//        pathToWalk.Add(currentTile.waypoint);
	//        _lastAddedWaypoint = currentTile;
	//    }
	//}

	public void CatMovementInput()
	{
		//First downpress
		if (LugusInput.use.down)
		{
			Transform hit = LugusInput.use.RayCastFromMouse();

			if (hit == null)
				return;

			//pathToWalk.Clear();
			//CatchingMiceVisualizer.use.Log(hit.name);

			_character = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();
			if (_character != null)
			{
				// Check whether the character is a cage or not
				if ((CatchingMiceLevelManager.use.Cage != null)
					&& (CatchingMiceLevelManager.use.Cage.PlayerHold.Contains(_character)))
				{
					return;
				}

				//when it is still jumping, let the jump finish before stopping
				if (_character.jumping)
				{
					_character.interrupt = true;
					return;
				}

				_character.StopCurrentBehaviour();

				CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(_character.transform.position.x, _character.transform.position.y);
				pathToWalk.Add(_character.currentTile.waypoint);

				//CatchingMiceTile tile =  CatchingMiceLevelManager.use.GetTileByLocation(hit.position.x, hit.position.y);
				//pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(tile.gridIndices));
				_lastAddedWaypoint = tile;
			}

		}

		//When dragging, try get the right swipe path
		else if (LugusInput.use.dragging && _character != null && pathToWalk.Count > 0)
		{
			//CheckDraggingPoints();   
			CheckDraggingPointsOffGrid();
		}

		//when not dragging anymore get the path converted from the waypoints
		else if (LugusInput.use.up && pathToWalk.Count > 1 && _character != null)
		{

			//CatchingMiceVisualizer.use.Log("UP");
			//pathToWalk.Reverse();
			List<CatchingMiceWaypoint> path = new List<CatchingMiceWaypoint>(pathToWalk);
			_character.MoveWithPath(path);
			_character = null;
			pathToWalk.Clear();
		}
	}

	protected void CheckDraggingPointsOffGrid()
	{
		Transform hit = LugusInput.use.RayCastFromMouse();
		float yOffset = 0.0f;
		//on need to check this for the first count only
		//because the player is taller than 1 tile, when you click on the tile that is above your tile, you don't want that added
		//if you're still in the cat collider with your mouse, don't add
		if (hit != null)
		{
			if (pathToWalk.Count == 1)
			{
				CatchingMiceCharacterPlayer character = null;
				character = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();
				if (character != null && character == _character)
					return;
			}

			//check furniture tiles for shifts
			CatchingMiceWorldObject FurnitureObject = null;
			FurnitureObject = hit.parent.GetComponent<CatchingMiceWorldObject>();
			if (FurnitureObject != null)
			{
				if (FurnitureObject.parentTile != null)
				{
					//make sure we have the furniture and not a trap
					//if it is null than you hit a ground trap, so no furniture object
					if (FurnitureObject.parentTile.furniture != null)
						yOffset = FurnitureObject.parentTile.furniture.yOffset * CatchingMiceLevelManager.use.scale;
				}

			}

		}

		Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(_character.transform);
		CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y - yOffset);

		if (tile == null)
		{
			return;
		}

		//only add a tile when its new tile is more than half a grid away
		float distance = Vector2.Distance(tile.gridIndices, _lastAddedWaypoint.gridIndices);
		if (distance < CatchingMiceLevelManager.use.scale / 2)
		{
			return;
		}

		float maxDistance = CatchingMiceLevelManager.use.scale /** 2*/;

		//if distance is more then x grids away interpolate
		// A path can be broken when there are null-tiles in the list
		bool brokenPath = false;
		if (distance > maxDistance)
		{
			while ((distance > maxDistance) && !brokenPath)
			{
				// interpolated vector: value * (endpoint - beginpoint) + beginpoint --> value between begin and end point
				Vector3 interpolated = (maxDistance) * Vector3.Normalize(tile.gridIndices.v3() - _lastAddedWaypoint.gridIndices.v3()) + _lastAddedWaypoint.gridIndices.v3();

				CatchingMiceTile interpolatedTile = CatchingMiceLevelManager.use.GetTile(interpolated.v2());

				if (interpolatedTile != null)
				{
					pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(interpolatedTile.gridIndices));
					_lastAddedWaypoint = interpolatedTile;
				}
				else
				{
					brokenPath = true;
				}

				distance -= maxDistance;
			}
		}

		if (!brokenPath)
		{
			pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(tile.gridIndices));
			_lastAddedWaypoint = tile;
		}
	}

	public void CheckPlayerObjectInteraction()
	{
		// This will check whether the user wants to interact
		// with a trap or other world object, either to
		// reactivate it, or to do something else with it

		// This is done by casting a ray and checking for traps
		// When a trap is found, the player characters are searched
		// for and if they are in allowed ranged, the interaction can be executed

		if (!LugusInput.use.down)
		{
			return;
		}

		Transform hit = LugusInput.use.RayCastFromMouse();

		if (hit == null)
		{
			return;
		}

		CheckPlayerTrapInteraction(hit);
		CheckPlayerObstacleInteraction(hit);
	}

	protected void CheckPlayerTrapInteraction(Transform hit)
	{
		CatchingMiceTrap trap = hit.parent.GetComponent<CatchingMiceTrap>();
		if (trap == null)
		{
			return;
		}

		// Go over the characters and check if the object is in range
		List<CatchingMiceCharacterPlayer> characters = new List<CatchingMiceCharacterPlayer>(CatchingMiceLevelManager.use.Players);
		foreach (CatchingMiceCharacterPlayer character in characters)
		{
			CatchingMiceTile[] tilesAround = CatchingMiceLevelManager.use.GetTileAround(character.currentTile);
			foreach (CatchingMiceTile tile in tilesAround)
			{
				if (tile == null)
				{
					continue;
				}

				if (tile.trap != null)
				{
					// When the trap and player character are in range of each other
					// Interact with it, and return
					if (tile.trap == trap)
					{
						trap.PlayerInteraction();
					}
				}
			}
		}
	}

	protected void CheckPlayerObstacleInteraction(Transform hit)
	{
		CatchingMiceObstacle obstacle = hit.parent.GetComponent<CatchingMiceObstacle>();
		if (obstacle == null)
		{
			return;
		}

		// Go over the characters and check if the object is in range
		List<CatchingMiceCharacterPlayer> characters = new List<CatchingMiceCharacterPlayer>(CatchingMiceLevelManager.use.Players);
		foreach (CatchingMiceCharacterPlayer character in characters)
		{
			CatchingMiceTile[] tilesAround = CatchingMiceLevelManager.use.GetTileAround(character.currentTile);
			foreach (CatchingMiceTile tile in tilesAround)
			{
				if (tile == null)
				{
					continue;
				}

				if ((tile.obstacle != null) && (tile.obstacle == obstacle))
				{
					CatchingMiceCage cage = obstacle as CatchingMiceCage;
					if ((cage != null) && (cage.PlayerHold != null))
					{
						cage.PlayerInteraction();
					}
				}
			}
		}
	}

	public void VisualizePath()
	{
		//visualisation of the motion the player makes
		if (pathToWalk.Count > 0)
		{
			_lineRenderer.SetVertexCount(pathToWalk.Count);
			for (int i = 0; i < pathToWalk.Count; i++)
			{
				_lineRenderer.SetPosition(i, pathToWalk[i].transform.position.z(-1));
			}
		}
		else
			_lineRenderer.SetVertexCount(0);
	}

	protected void OnDrawGizmos()
	{
		if (pathToWalk.Count > 0)
		{
			foreach (CatchingMiceWaypoint path in pathToWalk)
			{
				Gizmos.DrawCube(path.transform.position, new Vector3(0.4f, 0.4f, 0.4f));
			}
		}
	}
}

