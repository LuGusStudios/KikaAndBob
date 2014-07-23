using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterMouse : ICatchingMiceCharacter
{
    public delegate void OnGetHit();
    public event OnGetHit onGetHit;

    public int cheeseBites = 3;
	public int cookieDrops = 1;

	public override float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            
			if (onGetHit != null)
			{
                onGetHit();
			}

            if (health <= 0)
            {
                DieRoutine();
            }
        }
    }

	public override void SetupLocal()
    {
        base.SetupLocal();

        zOffset = 0.75f;
        

    }
    
	protected virtual void OnEnable() 
    {
		CatchingMiceLevelManager.use.OnCheeseRemoved += TargetRemoved;
    }
    
	protected virtual void OnDisable()
    {
        //CatchingMiceLevelManager.use.CheeseRemoved -= TargetRemoved;
    }
    
	public virtual void GetTarget()
    {
        if (CatchingMiceLevelManager.use.CheeseTiles.Count <= 0)
        {
            //CatchingMiceLogVisualizer.use.LogWarning("No more cheese left!");
            return;
        }

        List<CatchingMiceTile> tiles = new List<CatchingMiceTile>(CatchingMiceLevelManager.use.CheeseTiles);
		tiles.AddRange(CatchingMiceLevelManager.use.FakeCheeseTiles);

        targetWaypoint = GetTargetWaypoint(tiles);

        if (targetWaypoint != null)
        {
            CalculateTarget(targetWaypoint);
        }
        else
        {
            CatchingMiceLogVisualizer.use.LogError("No target found");
        }
    }
   
    protected CatchingMiceWaypoint GetTargetWaypoint(List<CatchingMiceTile> tileList)
    {
        CatchingMiceWaypoint target = null;

        float smallestDistance = float.MaxValue;
        //Check which cheese tile is the closest
        foreach (CatchingMiceTile tile in tileList)
        {
            float distance = Vector2.Distance(transform.position.v2(), tile.location.v2());
            if (distance < smallestDistance)
            {
                smallestDistance = distance;

                target = tile.waypoint;
            }
        }

        if (target != null)
        {
            return target; 
        }
        else
        {
            CatchingMiceLogVisualizer.use.LogError("No target found");
            return null;
        }
    }
    
	public void TargetRemoved(CatchingMiceTile tile)
    {
        // Current waypoint is null when pooling 
        if (targetWaypoint == null)
		{
            return;
		}

        StopCurrentBehaviour();
		StartCoroutine(FindNewTarget());
    }
    
	public override void DoCurrentTileBehaviour(int pathIndex)
    {
        //CatchingMiceLogVisualizer.use.Log("Doing current tile " + currentTile +" behaviour " + currentTile.tileType );

        //if the current tile is a cheese tile ( bitwise comparison, because tile can be ground and cheese tile) and the last tile that it travelled
        if ((currentTile.tileType & CatchingMiceTile.TileType.Cheese) == CatchingMiceTile.TileType.Cheese && pathIndex==0)
        {
            //begin eating the cheese
            StartCoroutine(Attack());
        }
    }
    
//	public override IEnumerator MoveToDestination(List<CatchingMiceWaypoint> path)
//    {
//        yield return new WaitForSeconds(LugusRandom.use.Uniform.Next(0,0.5f));
//        yield return StartCoroutine(base.MoveToDestination(path));
//    }
//    

	public override void MoveToDestination (List<CatchingMiceWaypoint> path)
	{
		StartCoroutine( MoveDelayRoutine(path));
	}

	protected IEnumerator MoveDelayRoutine(List<CatchingMiceWaypoint> path)
	{
		yield return new WaitForSeconds(LugusRandom.use.Uniform.Next(0,0.5f));
		yield return StartCoroutine(MoveToDestinationRoutine(path));
	}

	public override IEnumerator Attack()
    {
        attacking = true;
        OnHitEvent();
        CatchingMiceTile cheeseTile = currentTile;
        int attacked = 0;

        while((health > 0)
			&& (cheeseTile.cheese != null)
			&& (cheeseTile.cheese.Health > 0)
			&& (attacked < cheeseBites))
        {
            cheeseTile.cheese.Health -= (int)damage;
            attacked++;

            yield return new WaitForSeconds(attackInterval);
        }

        attacking = false;

        if ((CatchingMiceLevelManager.use.CheeseTiles.Count > 0) && (cheeseTile.cheese.Health <= 0))
        {
            CatchingMiceLogVisualizer.use.Log("getting new target");
            GetTarget();
        }
        else
        {

            DieRoutine();
        }
    }

	protected IEnumerator FindNewTarget()
	{
		yield return new WaitForSeconds(LugusRandom.use.Uniform.Next(1f));
		GetTarget();
	}
    
	//TODO: Play Death animation (cloud particle)
	public virtual void DieRoutine()
    {
		currentTile.AddCookies(cookieDrops);
        CatchingMiceLevelManager.use.OnCheeseRemoved -= TargetRemoved;
		CatchingMiceLevelManager.use.EnemyDied(this);
		CatchingMiceGameManager.use.EnemiesAlive -= 1;

        gameObject.SetActive(false);
		GameObject.Destroy(this.gameObject);
    }

    public void GetHit(float damage)
    {
        Health -= damage;
        if (onGetHit != null)
            onGetHit();
    }
}
