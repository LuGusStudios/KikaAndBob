using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceInteraction : LugusSingletonRuntime<CatchingMiceInteraction>
{
	public List<CatchingMiceWaypoint> pathToWalk = new List<CatchingMiceWaypoint>();
	protected CatchingMiceCharacterPlayer currentSelectedCharacter = null;
	protected CatchingMiceTile _previousTile = null;
	protected CatchingMiceTile lastAddedTile = null;

	protected ScalableLineRenderer pathRenderer = null;

	protected float _timer = 0.0f;

	protected Dictionary<CatchingMiceWaypoint, ScalableLineRenderer> pathSections = new Dictionary<CatchingMiceWaypoint, ScalableLineRenderer>();
	protected List<CatchingMicePathVisualization> pathVisualizations = new List<CatchingMicePathVisualization>();
	
	
	protected class CatchingMicePathVisualization
	{
		public ICatchingMiceCharacter targetCharacter = null;
		public List<CatchingMiceWaypoint> wayPoints = new List<CatchingMiceWaypoint>();

		public Dictionary<CatchingMiceWaypoint, ScalableLineRenderer> pathSections = new Dictionary<CatchingMiceWaypoint, ScalableLineRenderer>();
	}

	// Use this for initialization
	void Start()
	{
		Transform pathRendererTransform = GameObject.Find("PlayerPathRenderer").transform;
		if (pathRendererTransform != null)
		{
			pathRenderer = pathRendererTransform.GetComponent<ScalableLineRenderer>();
			if (pathRenderer == null)
			{
				CatchingMiceLogVisualizer.use.LogError("PlayerPathRenderer not found");
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
	

	//	CatMovementInput();

	//	VisualizePath();

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
			pathToWalk.Clear();

			Transform hit = LugusInput.use.RayCastFromMouse(LugusCamera.game);

			if (hit == null)
			{
				return;
			}

			//pathToWalk.Clear();
			//CatchingMiceVisualizer.use.Log(hit.name);

			currentSelectedCharacter = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();
			if (currentSelectedCharacter != null)
			{
				// Check whether the character is in a cage or not
				if ((CatchingMiceLevelManager.use.Cage != null)
					&& (CatchingMiceLevelManager.use.Cage.capturedPlayers.Contains(currentSelectedCharacter)))
				{
					return;
				}

				//when it is still jumping, let the jump finish before stopping
				if (currentSelectedCharacter.jumping)
				{
					currentSelectedCharacter.interrupt = true;
					return;
				}

				for (int i = pathVisualizations.Count - 1; i >= 0; i--) 
				{
					if (pathVisualizations[i].targetCharacter == currentSelectedCharacter)
					{
						ClearPathSections(pathVisualizations[i].pathSections);
						pathVisualizations.Remove(pathVisualizations[i]);
					}
				}

				currentSelectedCharacter.StopCurrentBehaviour();

				CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(currentSelectedCharacter.transform.position.x, currentSelectedCharacter.transform.position.y);
				pathToWalk.Add(currentSelectedCharacter.currentTile.waypoint);

				//CatchingMiceTile tile =  CatchingMiceLevelManager.use.GetTileByLocation(hit.position.x, hit.position.y);
				//pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(tile.gridIndices));
				lastAddedTile = tile;
			}
		}

		//When dragging, try get the right swipe path
		else if (LugusInput.use.dragging && currentSelectedCharacter != null && pathToWalk.Count > 0)
		{
			//CheckDraggingPoints();   
			CheckDraggingPointsOffGrid();
		}

		//when not dragging anymore get the path converted from the waypoints
		else if (LugusInput.use.up && pathToWalk.Count > 1 && currentSelectedCharacter != null)
		{
			List<CatchingMiceWaypoint> path = new List<CatchingMiceWaypoint>(pathToWalk);

			CatchingMicePathVisualization newPathVisualization = new CatchingMicePathVisualization();
			newPathVisualization.targetCharacter = currentSelectedCharacter;
			newPathVisualization.wayPoints = path;
			pathVisualizations.Add(newPathVisualization);

			currentSelectedCharacter.MoveAlongPath(path);
		
			currentSelectedCharacter = null;
			pathToWalk.Clear();
		}
	}

	protected void CheckDraggingPointsOffGrid()
	{
		Transform hit = LugusInput.use.RayCastFromMouse(LugusCamera.game);
		float yOffset = 0.0f;

		if (hit != null)
		{
			//because the player is taller than 1 tile, when you click on the tile that is above your tile, you don't want that added
			//if you're still in the cat collider with your mouse, don't add
			//need to check this for the first count only
			if (pathToWalk.Count == 1)
			{
				CatchingMiceCharacterPlayer character = null;

				if (hit.parent != null)
					character = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();

				if (character != null && character == currentSelectedCharacter)
					return;
			}

			//check furniture tiles for shifts
			CatchingMiceWorldObject furnitureObject = null;
			furnitureObject = hit.parent.GetComponent<CatchingMiceWorldObject>();
			if (furnitureObject != null)
			{
				if (furnitureObject.parentTile != null)
				{
					//make sure we have the furniture and not a trap
					//if it is null, you hit a ground trap, so no furniture object
					if (furnitureObject.parentTile.furniture != null)
						yOffset = furnitureObject.parentTile.furniture.yOffset * CatchingMiceLevelManager.use.scale;
				}
			}
		}


//	Vector3 dragPoint = LugusInput.use.ScreenTo3DPoint(Input.mousePosition, currentSelectedCharacter.transform);	// this method also has an overload where the mousePosition is supplied implicitly, but this is more understandable here
//	CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(dragPoint.x, dragPoint.y - yOffset);

		CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileFromMousePosition(false);

		if (tile == null || tile == lastAddedTile || !lastAddedTile.waypoint.neighbours.Contains(tile.waypoint) )
		{
			return;
		}

		print (pathToWalk.Count);

		pathToWalk.Add(tile.waypoint);
		lastAddedTile = tile;


		// OLD VERSION -------------------------------------------------------------------------------------------

//		//only add a tile when its new tile is more than half a grid away
//		float distance = Vector2.Distance(tile.gridIndices, lastAddedTile.gridIndices);
//		if (distance < CatchingMiceLevelManager.use.scale * 0.5f)
//		{
//			return;
//		}
//
//		float maxDistance = CatchingMiceLevelManager.use.scale /** 2*/;
//
//		//if distance is more than x grids away interpolate
//		// A path can be broken when there are null-tiles in the list
//		bool brokenPath = false;
//		if (distance > maxDistance)
//		{
//			while ((distance > maxDistance) && !brokenPath)
//			{
//				// interpolated vector: value * (endpoint - beginpoint) + beginpoint --> value between begin and end point
//				Vector3 interpolated = (maxDistance) * Vector3.Normalize(tile.gridIndices.v3() - lastAddedTile.gridIndices.v3()) + lastAddedTile.gridIndices.v3();
//
//				CatchingMiceTile interpolatedTile = CatchingMiceLevelManager.use.GetTile(interpolated.v2());
//
//				if (interpolatedTile != null)
//				{
//					pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(interpolatedTile.gridIndices));
//					lastAddedTile = interpolatedTile;
//				}
//				else
//				{
//					brokenPath = true;
//				}
//
//				distance -= maxDistance;
//			}
//		}
//
//		if (!brokenPath)
//		{
//			// This seems all kinds of round-about. Tentatively fixing this.
//			//pathToWalk.Add(CatchingMiceLevelManager.use.GetWaypointFromTile(tile.gridIndices));
//			pathToWalk.Add(tile.waypoint);
//			lastAddedTile = tile;
//		}
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

		Transform hit = LugusInput.use.RayCastFromMouse(LugusCamera.game);

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
					if ((cage != null) && (cage.capturedPlayers != null))
					{
						cage.PlayerInteraction();
					}
				}
			}
		}
	}

	protected void ClearPathSections(Dictionary<CatchingMiceWaypoint, ScalableLineRenderer> pathSections)
	{
		foreach( KeyValuePair<CatchingMiceWaypoint, ScalableLineRenderer> pathSection in pathSections )
		{
			if (pathSection.Value != null && pathSection.Value.gameObject != null)
				Destroy(pathSection.Value.gameObject);
		}
	}

	public void VisualizePath()
	{
		// this part visualizes a new path being drawn before the mouse press has been released
		for (int i = 0; i < pathToWalk.Count; i++) 
		{
			if (i <= 0)
				continue;

			if (!pathSections.ContainsKey(pathToWalk[i]))
			{
				ScalableLineRenderer lr = null;
		
				lr = (ScalableLineRenderer) Instantiate(pathRenderer);
				lr.SetVertexCount(2);
				lr.SetPosition(0, pathToWalk[i-1].transform.position.z(-1));
				lr.SetPosition(1, pathToWalk[i].transform.position.z(-1));
				lr.ScaleMaterial();

				pathSections.Add(pathToWalk[i], lr);
			}
		}

		if (pathToWalk.Count <= 0)
		{
			ClearPathSections(pathSections);
			pathSections.Clear();
		}

		// this part visualizes paths one of the player characters is already following
		bool firstSection = false;

		for (int i = pathVisualizations.Count - 1; i >= 0; i--) 
		{
			CatchingMicePathVisualization currentPathVisualization = pathVisualizations[i];

			for (int j = 0; j < currentPathVisualization.wayPoints.Count; j++) 
			{
				if (j <= 0)
					continue;

				if (!currentPathVisualization.pathSections.ContainsKey(currentPathVisualization.wayPoints[j]))
				{
					ScalableLineRenderer lr = null;
					
					lr = (ScalableLineRenderer) Instantiate(pathRenderer);
					lr.SetVertexCount(2);
					lr.SetPosition(0, currentPathVisualization.wayPoints[j-1].transform.position.z(-1));
					lr.SetPosition(1, currentPathVisualization.wayPoints[j].transform.position.z(-1));
					lr.ScaleMaterial();

					currentPathVisualization.pathSections.Add(currentPathVisualization.wayPoints[j], lr);
				}

				if (firstSection == false && currentPathVisualization.pathSections[currentPathVisualization.wayPoints[j]] != null)
				{
					firstSection = true;

					Vector3 currentPosition = 
						currentPathVisualization.targetCharacter.transform.position.z(-1);

					ScalableLineRenderer lineRenderer = currentPathVisualization.pathSections[currentPathVisualization.wayPoints[j]];

					lineRenderer.SetPosition(0, currentPosition);
					lineRenderer.ScaleMaterial();

					if (currentPathVisualization.targetCharacter.currentTile == currentPathVisualization.wayPoints[j].parentTile)
					{
						Destroy( currentPathVisualization.pathSections[currentPathVisualization.wayPoints[j]].gameObject);
						//currentPathVisualization.pathSections.Remove(currentPathVisualization.wayPoints[j]);

						if (j == currentPathVisualization.wayPoints.Count - 1)
						{
							pathVisualizations.Remove(currentPathVisualization);
						}
					}
				}
			}
		}
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

