using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class ICatchingMiceCharacter : MonoBehaviour
{
	#region Public
	public float zOffset = 0.0f;	// Offset from 0-->1
	public float tileTraversalTime = 0.5f;	// Time needed to traverse 1 tile

	public float damage = 1.0f;
	public float attackInterval = 0.5f;

	public CatchingMiceTile currentTile = null;
	public CatchingMiceWaypoint targetWaypoint = null;
	public CatchingMiceTile.TileType walkable = CatchingMiceTile.TileType.Ground;
	public Vector3 movementDirection = Vector3.zero;
	#endregion

	#region Protected
	protected List<CatchingMiceWaypoint> navigationGraph = null;

	[SerializeField]
	protected float health = 1.0f;
	#endregion

	#region Events
	public delegate void OnJump();
	public event OnJump onJump;

	public delegate void OnHit();
	public event OnHit onHit;

	public bool moving = false;
	public bool jumping = false;
	public bool attacking = false;
	public bool interrupt = false;
	#endregion

	public abstract float Health { get; set; }

	public virtual void CalculateTarget(CatchingMiceWaypoint target)
	{
		//go to target
		List<CatchingMiceWaypoint> graph = navigationGraph;

		CatchingMiceWaypoint currentWaypoint = currentTile.waypoint;

		bool fullPath = false;

		if (targetWaypoint != null)
		{
			List<CatchingMiceWaypoint> path = CatchingMiceUtil.FindPath(graph, currentWaypoint, targetWaypoint, out fullPath, walkable);

			if (fullPath)
			{
				MoveToDestination(path);
			}
		}
	}

	public abstract void DoCurrentTileBehaviour(int pathIndex);

	public abstract IEnumerator Attack();

	protected virtual void Awake()
	{
		SetupLocal();
	}

	protected virtual void Start()
	{
		SetupGlobal();
	}

	public virtual void SetupLocal()
	{
		movementDirection = Vector3.zero;
	}

	public virtual void SetupGlobal()
	{
		currentTile = CatchingMiceLevelManager.use.GetTileByLocation(transform.position.x, transform.position.y);
		navigationGraph = new List<CatchingMiceWaypoint>(CatchingMiceLevelManager.use.Waypoints);

		if (navigationGraph.Count == 0)
		{
			CatchingMiceLogVisualizer.use.LogError(transform.Path() + " : no navigationGraph found for this level!!");
		}
	}

	public virtual void MoveToDestination(List<CatchingMiceWaypoint> path)
	{
		StartCoroutine(MoveToDestinationRoutine(path));
	}

	public IEnumerator MoveToDestinationRoutine(List<CatchingMiceWaypoint> path)
	{
		int pathIndex = path.Count - 1;

		//when interrupting a jump complete the jump to the tile
		interrupt = false;

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

			float yOffset = 0.0f;
			if (currentTile.furniture != null)
			{
				yOffset = currentTile.furniture.yOffset;
			}

			movementDirection = Vector3.Normalize(path[pathIndex].parentTile.location.z(transform.position.z) - transform.position.yAdd(-yOffset));

			//check if the character needs to jump, when your type is not the same as you current tile, it mean the character is jumping
			if ((currentTile.waypoint.waypointType & path[pathIndex].waypointType) != path[pathIndex].waypointType)
			{
				jumping = true;
				if (onJump != null)
				{
					onJump();
				}

				yield return new WaitForSeconds(0.3f);
			}

			gameObject.MoveTo(movePosition).Time( 0.5f * ( Vector2.Distance( transform.position.v2(), path[pathIndex].transform.position.v2() ) / tileTraversalTime ) ).Execute();

//			gameObject.MoveTo(movePosition).Time(tileTraversalTime).Execute();

			while (transform.position.v2() != path[pathIndex].transform.position.v2())
			{
				yield return new WaitForEndOfFrame();
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

			//this bool is used for cat side because naming of animations are not consistant 
			//(example: Cat01_Idle and Cat01Side_Jump)
			jumping = false;
		}

		// we have reached the final target now (or should have...)
		gameObject.StopTweens();

		moving = false;
	}

	public virtual void StopCurrentBehaviour()
	{
		StopAllCoroutines();
		moving = false;

		CatchingMiceCharacterAnimation animation = GetComponent<CatchingMiceCharacterAnimation>();
		if (animation != null)
		{
			animation.PlayAnimation("DOWN/" + animation.characterNameAnimation + animation._frontAnimationClip + animation.idleAnimationClip);
		}
	}

	public virtual bool IsWalkable(CatchingMiceTile tile)
	{
		if (tile.waypoint.waypointType == CatchingMiceTile.TileType.Collide)
		{
			return false;
		}

		return true;
	}

	public void OnHitEvent()
	{
		if (onHit != null)
		{
			onHit();
		}
	}
}
