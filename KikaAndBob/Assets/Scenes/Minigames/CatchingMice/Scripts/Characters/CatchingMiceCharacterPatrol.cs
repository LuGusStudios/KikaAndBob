using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterPatrol : ICatchingMiceCharacter {

	public int visionTileRange = 1;
	public List<CatchingMiceWaypoint> patrolRoute = new List<CatchingMiceWaypoint>();

	protected ILugusCoroutineHandle walkHandle = null;

	public override float Health
	{
		get
		{
			throw new System.NotImplementedException();
		}
		set
		{
			throw new System.NotImplementedException();
		}
	}

	public override void SetupLocal()
	{
		base.SetupLocal();

		zOffset = 0.95f;
		walkHandle = LugusCoroutines.use.GetHandle();
		walkHandle.Claim();
	}

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		walkable = CatchingMiceTile.TileType.Ground;

		if (patrolRoute.Count == 0)
		{
			CatchingMiceLogVisualizer.use.LogError("The patrol route for the character " + this.name + " is empty.");
			return;
		}
		else
		{
			// Set the guard on the initial location
			transform.position = patrolRoute[0].parentTile.location.zAdd(0.1f);

			StartCoroutine(PatrolRoutine());
		}
	}

	protected IEnumerator PatrolRoutine()
	{
		int waypointIndex = 0;

		// For each set of waypoints, find the path between
		// them, and let the patrol character walk from waypoint
		// to waypoint
		while (CatchingMiceGameManager.use.gameRunning)
		{
			CatchingMiceWaypoint current = patrolRoute[waypointIndex];
			CatchingMiceWaypoint next = null;
			if (waypointIndex == (patrolRoute.Count -1))
			{
				next = patrolRoute[0];
			}
			else
			{
				next = patrolRoute[waypointIndex + 1];
			}

			// Find the path between the 2 waypoints
			bool fullPath = false;
			List<CatchingMiceWaypoint> path = CatchingMiceUtil.FindPath(navigationGraph, current, next, out fullPath, walkable);

			if (fullPath)
			{
				walkHandle.StartRoutine(MoveToDestination(path));

				while (walkHandle.Running)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				CatchingMiceLogVisualizer.use.LogError("Could not find a path from " + current.parentTile.ToString() + " to " + next.parentTile.ToString() + ".");
				yield break;
			}

			waypointIndex = (waypointIndex + 1) % patrolRoute.Count;
		}

		walkHandle.StopRoutine();
		walkHandle.Release();
	}

	public override void DoCurrentTileBehaviour(int pathIndex)
	{
		
	}

	// This method is essentially the same as the one in the super class,
	// but it will check for a player in the line of sight every fixed
	// frame update instead of when arriving on a tile
	// Jumping is also removed here
	public override IEnumerator MoveToDestination(List<CatchingMiceWaypoint> path)
	{
		int pathIndex = path.Count - 1;

		while ((pathIndex > -1) && !interrupt)
		{
			moving = true;
			Vector3 movePosition = path[pathIndex].transform.position;

			//check which zdepth the object must be
			//Left hand axis, bigger z is further away
			//when going up
			if (transform.position.z < movePosition.z)
			{
				movePosition.z = transform.position.z;
			}

			movementDirection = Vector3.Normalize(path[pathIndex].parentTile.location.z(transform.position.z) - transform.position);

			gameObject.MoveTo(movePosition).Time(tileTraversalTime).Execute();

			// While the patrol is moving, it checks in front of him
			// whether a player can be found.
			while (transform.position.v2() != path[pathIndex].transform.position.v2())
			{
				yield return new WaitForFixedUpdate();
				
				CatchingMiceCharacterPlayer player = FindPlayer();
				CatchingMiceCage cage = CatchingMiceLevelManager.use.Cage;
				if ((player != null)
					&& (cage != null)
					&& (!cage.capturedPlayers.Contains(player)))
				{
					cage.PlayerDetected(player);

					iTween.PunchScale(this.gameObject, Vector3.one, 0.5f);

					yield return new WaitForSeconds(0.5f);
				}
			}

			//z needs to be the next tile because else the object will be behind the next tile while on its way to the next tile
			if (pathIndex > 0)
			{
				if (path[pathIndex - 1].transform.position.z < path[pathIndex].transform.position.z)
				{
					transform.position = transform.position.z(path[pathIndex - 1].transform.position.z).zAdd(-zOffset);
				}
				else
				{
					transform.position = transform.position.z(path[pathIndex].transform.position.z).zAdd(-zOffset);
				}
			}
			else
			{
				transform.position = transform.position.z(path[pathIndex].transform.position.z).zAdd(-zOffset);
			}

			currentTile = path[pathIndex].parentTile;
			
			DoCurrentTileBehaviour(pathIndex);

			path.Remove(path[pathIndex]);

			pathIndex--;
		}

		// we have reached the final target now (or should have...)
		gameObject.StopTweens();

		moving = false;
	}

	public CatchingMiceCharacterPlayer FindPlayer()
	{
		// Find the tiles in front of the patrol character
		// It should not see through null-tiles (walls)

		if (movementDirection == Vector3.zero)
		{
			return null;
		}

		// Get the direction the character is looking in
		Vector2 lookingDirection = Vector2.zero;
		if (Mathf.Abs(movementDirection.x) > 0.5f)
		{
			if (movementDirection.x > 0)
			{
				lookingDirection.x = 1f;
			}
			else
			{
				lookingDirection.x = -1f;
			}
		}
		else if (Mathf.Abs(movementDirection.y) > 0.5f)
		{
			if (movementDirection.y > 0)
			{
				lookingDirection.y = 1f;
			}
			else
			{
				lookingDirection.y = -1f;
			}
		}

		// Find the visible tiles
		CatchingMiceCharacterPlayer playerFound = null;
		CatchingMiceTile[] visibleTiles = CatchingMiceLevelManager.use.GetTilesInDirection(currentTile, visionTileRange, lookingDirection);

		foreach (CatchingMiceTile tile in visibleTiles)
		{
			if (tile == null)
			{
				break;
			}

			foreach (CatchingMiceCharacterPlayer player in CatchingMiceLevelManager.use.Players)
			{
				if (player.currentTile == tile)
				{
					playerFound = player;
				}
			}
		}

		return playerFound;
	}

	public override IEnumerator Attack()
	{
		throw new System.NotImplementedException();
	}
}
