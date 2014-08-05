using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceInputManager : LugusSingletonRuntime<CatchingMiceInputManager> 
{
	protected CatchingMiceCharacterPlayer currentSelectedPlayer = null;
	protected CatchingMicePathVisualization currentDrawingPath = null;
	protected List<CatchingMicePathVisualization> pathVisualizations = new List<CatchingMicePathVisualization>();
	protected ScalableLineRenderer pathRenderer = null;

	protected class CatchingMicePathVisualization
	{
		public bool drawn = false;
		public CatchingMiceCharacterPlayer targetPlayer = null;
		public List<CatchingMiceWaypoint> wayPoints = new List<CatchingMiceWaypoint>();
		public List<ScalableLineRenderer> pathSections = new List<ScalableLineRenderer>();

		public void RemovePathSections()
		{
			for (int i = pathSections.Count - 1; i >= 0; i--) 
			{
				Destroy(pathSections[i].gameObject);
			}
			
			pathSections.Clear();
		}
	}

	public void SetupLocal()
	{
	}
	
	public void SetupGlobal()
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
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
		if (!CatchingMiceGameManager.use.GameRunning)
			return;

		CatPathInput();
		VisualizePaths();
	}

	protected void CatPathInput()
	{
		// if a player was moved to cage, remove their path
		if (CatchingMiceLevelManager.use.Cage != null)
		{
			for (int i = pathVisualizations.Count - 1; i >= 0; i--) 
			{
				if (CatchingMiceLevelManager.use.Cage.capturedPlayers.Contains(pathVisualizations[i].targetPlayer))
				{
					pathVisualizations[i].RemovePathSections();
					pathVisualizations.Remove(pathVisualizations[i]);
				}
			}
		}

		if (LugusInput.use.down)
		{
			currentSelectedPlayer = null;

			Transform hit = LugusInput.use.RayCastFromMouseDown(LugusCamera.game);

			if (hit == null)
				return;
	
			currentSelectedPlayer = hit.parent.GetComponent<CatchingMiceCharacterPlayer>();

			if (currentSelectedPlayer == null)
				return;

			// check whether the character is in a cage or not
			if ( (CatchingMiceLevelManager.use.Cage != null) && (CatchingMiceLevelManager.use.Cage.capturedPlayers.Contains(currentSelectedPlayer)) )
			{
				currentSelectedPlayer = null;
				return;
			}

			for (int i = pathVisualizations.Count - 1; i >= 0; i--) 
			{
				if (pathVisualizations[i].targetPlayer == currentSelectedPlayer)
				{
					pathVisualizations[i].RemovePathSections();
					pathVisualizations.Remove(pathVisualizations[i]);
				}
			}

			//when the player is jumping, let that jump finish before starting new path
			if (currentSelectedPlayer.jumping)
			{
				currentSelectedPlayer.interrupt = true;

				for (int i = pathVisualizations.Count - 1; i >= 0; i--) 
				{
					if (CatchingMiceLevelManager.use.Cage.capturedPlayers.Contains(pathVisualizations[i].targetPlayer))
					{
						pathVisualizations[i].RemovePathSections();
						pathVisualizations.Remove(pathVisualizations[i]);
					}
				}

				currentSelectedPlayer = null;
				currentDrawingPath = null;

				return;
			}

			currentSelectedPlayer.StopCurrentBehaviour();

			currentDrawingPath = new CatchingMicePathVisualization();
			currentDrawingPath.targetPlayer = currentSelectedPlayer;
		}
		else if (LugusInput.use.dragging && currentSelectedPlayer != null && currentDrawingPath != null)
		{
			CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileFromMousePosition(false);

			if (tile != null)
			{
				if (currentDrawingPath.wayPoints.Count == 0)
				{
					currentDrawingPath.wayPoints.Add(tile.waypoint);
					currentDrawingPath.drawn = false;	// setting this false will redraw path 
				}
				else if (currentDrawingPath.wayPoints[currentDrawingPath.wayPoints.Count-1] != tile.waypoint && 		// if not on same tile and on tile bordering on last once
				         currentDrawingPath.wayPoints[currentDrawingPath.wayPoints.Count-1].neighbours.Contains(tile.waypoint))
				{
					currentDrawingPath.wayPoints.Add(tile.waypoint);
					currentDrawingPath.drawn = false;	// setting this false will redraw path 
				}
			}
		}
		else if (LugusInput.use.up && currentSelectedPlayer != null && currentDrawingPath != null && currentDrawingPath.wayPoints.Count > 1)
		{
			currentSelectedPlayer.MoveAlongPath(currentDrawingPath.wayPoints);
			pathVisualizations.Add(currentDrawingPath);
			currentSelectedPlayer = null;
		}
	}

	public void ClearAllPaths()
	{
		// this is the path being drawn by the player
		if (currentDrawingPath != null)
		{
			currentDrawingPath.RemovePathSections();
			currentDrawingPath = null;
		}

		// these are paths currently being walked by the cats, which also require updating
		foreach(CatchingMicePathVisualization pathVisualization in pathVisualizations)
		{
			pathVisualization.RemovePathSections();
		}

		pathVisualizations.Clear();
	}

	protected void VisualizePaths()
	{
		// this is the path being drawn by the player
		if (currentDrawingPath != null)
			DrawPath(currentDrawingPath);

		// these are paths currently being walked by the cats, which also require updating
		foreach(CatchingMicePathVisualization pathVisualization in pathVisualizations)
		{
			DrawPath(pathVisualization);
		}
	}

	protected void DrawPath(CatchingMicePathVisualization path)
	{
		if (path.drawn == false)
		{
			path.drawn = true;

			path.RemovePathSections();

			if (path.wayPoints.Count > 1)
			{
				// starting at 1 here, because waypoint 0 doesn't have a path section
				for (int i = 1; i < path.wayPoints.Count; i++) 
				{
					ScalableLineRenderer lr = (ScalableLineRenderer) Instantiate(pathRenderer);
					lr.SetVertexCount(2);
					lr.SetPosition(0, path.wayPoints[i-1].transform.position.z(-1));
					lr.SetPosition(1, path.wayPoints[i].transform.position.z(-1));
					lr.ScaleMaterial();
					path.pathSections.Add(lr);
				}
			}
		}

		if (path.pathSections.Count > 0)
		{
			ScalableLineRenderer firstSection = path.pathSections[0];
			firstSection.SetPosition(0, path.targetPlayer.transform.position.z(-1));
			firstSection.ScaleMaterial();

			if (firstSection.GetLength() < 0.05f)
			{
				path.pathSections.Remove(firstSection);
				Destroy(firstSection.gameObject);
			}
		}
	}
}
