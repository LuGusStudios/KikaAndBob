using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCharacterPlayer : ICatchingMiceCharacter
{
    protected CatchingMiceCharacterMouse _enemy = null;
    protected bool _canAttack = true;

    protected ILugusCoroutineHandle walkHandle = null;
    
	public override float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }
    
    public override void SetupLocal()
    {
        base.SetupLocal();
      //  zOffset = 0.95f;
        walkHandle = LugusCoroutines.use.GetHandle();
    }

    public IEnumerator CalculatePath(List<CatchingMiceWaypoint> drawnPath)
    {
        //go to target
        List<CatchingMiceWaypoint> graph = navigationGraph;
        bool fullPath = false;
        CatchingMiceWaypoint currentWaypoint = null;

        for (int i = 1; i < drawnPath.Count; i++)
        {
            if (interrupt)
			{
                break;
			}

            currentWaypoint = drawnPath[i - 1];

            targetWaypoint = drawnPath[i];

			List<CatchingMiceWaypoint> path = CatchingMiceUtil.FindPath(graph, currentWaypoint, targetWaypoint, out fullPath, walkable);

            yield return walkHandle.StartRoutine(MoveToDestinationRoutine(path));
        }
    }

	// TODO: Add the cookies to the game manager when picked up
    public override void DoCurrentTileBehaviour(int pathIndex)
    {
		CheckCookies();
    }
    
	public override IEnumerator Attack()
    {
        //attack a mouse and wait until you can attack again
        if(_enemy != null)
        {
            attacking = true;
            _enemy.GetHit(damage);
            _canAttack = false;
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("e00_Attack01"));

			if (!moving)
            	OnHitEvent();	// this event will play the attack animation

            yield return new WaitForSeconds(attackInterval);
            _canAttack = true;
            attacking = false; 
        }
    }
	
    public void CheckForAttack()
    {
        // check all raycast hits
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + new Vector3(0, 0, 1000), this.transform.forward);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.parent != null)
            {
                _enemy = hit.transform.parent.GetComponent<CatchingMiceCharacterMouse>();
                //First mouse found, kill it and end
                if (_enemy != null)
                {
                    //attack the mouse
                    LugusCoroutines.use.StartRoutine(Attack());
                    break;
                }
            }
        }
    }
    
	public override void StopCurrentBehaviour()
    {
        base.StopCurrentBehaviour();
        
        if (walkHandle != null)
		{
            walkHandle.StopRoutine(); 
		}

        gameObject.StopTweens();
        interrupt = false;
    }
    
	public void MoveAlongPath(List<CatchingMiceWaypoint> path)
    {
        StopCurrentBehaviour();

        //post process before actually going through the path
//        int count = 2;
//        while (count < path.Count)
//        {
//            Vector2 wpStart = path[count - 2].parentTile.gridIndices;
//            Vector2 wpMiddle = path[count - 1].parentTile.gridIndices;
//            Vector2 wpEnd = path[count].parentTile.gridIndices;
//
//
//            //check if the tile in between is on the same x or y axis
//            if ((wpEnd.x == wpMiddle.x && wpMiddle.x == wpStart.x) ||
//                (wpEnd.y == wpMiddle.y && wpMiddle.y == wpStart.y))
//            {
//                //check if the middle tile is inbetween the tiles
//                if ((wpEnd.x > wpMiddle.x && wpMiddle.x > wpStart.x) ||
//                     (wpEnd.y > wpMiddle.y && wpMiddle.y > wpStart.y))
//                { 
//                    path.RemoveAt(count - 1);
//                    continue;
//                }
//            }
//
//            count++;
//            //when there are less the 3 waypoints to check 
//            if (path.Count < 2)
//                break;
//
//        }

        //CatchingMiceLogVisualizer.use.Log("path count : " + path.Count);
        StartCoroutine(CalculatePath(path));
        //handle.StartRoutine(CalculatePath(path));
    }
	
	// Update is called once per frame
	protected void Update () 
    {
	    if(_canAttack)
        {
            CheckForAttack();
        }

		CheckCookies();
	}

	protected void CheckCookies()
	{
		// As a player, pick up the cookies on a tile
		if (currentTile != null && currentTile.Cookies > 0)
		{
			int takencookies = currentTile.TakeCookies(currentTile.Cookies);

			CatchingMiceGameManager.use.PickupCount += takencookies;

			ScoreVisualizer.Score(KikaAndBob.CommodityType.Cookie, takencookies).Audio("e00_CookieEat01").Color(Color.gray).Position(currentTile.waypoint.transform.position).Execute();
		}
	}
}
