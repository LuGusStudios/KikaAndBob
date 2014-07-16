using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceWaypoint : MonoBehaviour  
{
    //[Flags]
    //public enum WaypointType
    //{
    //    Ground = 1,
    //    Furniture = 2,
    //    Collide = 4,
        
    //    Both = 3,
    //    None = -1 // place at the bottom for nicer auto-complete in IDE
    //}
	public bool debug = false;

	public int layerOrder = 0;
	
	[HideInInspector]
	public List<CatchingMiceWaypoint> neighbours = new List<CatchingMiceWaypoint>();
    public CatchingMiceTile.TileType waypointType = CatchingMiceTile.TileType.Ground;

    public CatchingMiceTile parentTile = null;

	[HideInInspector]
	public float AStarCost = 0.0f;
	[HideInInspector]
	public CatchingMiceWaypoint AStarParent = null;
	
	public int reachedCount = 0; // # of times this waypoint was reached in the current scene instance
	
	public delegate void OnReachedCallback(CatchingMiceWaypoint waypoint);
	public OnReachedCallback onReached;
	
	// Use this for initialization
	void Start () 
	{
	 
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	// called by external script
	public void OnReached()
	{
		reachedCount++;
		
		// TODO: set a timeout on this of about 2 seconds, so we're sure it doesn't trigger twice by accident
		// TODO: or maybe find a beter way to do this than a timeout (onMovedAway() or sthing like that)
		if( onReached != null )
			onReached(this);
	}

	/*
	public Waypoint FindClosestPathEnd()
	{
		// a path end is a node with just 1 neighbour
		// closest is not Vector3.Distance but rather closest in nr. of links to the waypoint
		
		// use a breadth-first algorithm. The first path end we find is the closest by definition then
		// http://en.wikipedia.org/wiki/Breadth-first_search
		
		if( this.neighbours.Count == 1 )
			return this;
		
		 
		Queue<Waypoint> q = new Queue<Waypoint>(); 
		q.Enqueue(this);
		
		List<Waypoint> visited = new List<Waypoint>();
		
		int iterations = 0;
		while( q.Count > 0 )
		{
			Waypoint current = q.Dequeue();
			
			if( current == null )
			{
				iterations++;
				continue;
			}
			
			if( current.neighbours.Count == 1 ) 
			{
				// we found our neighbour, excellent!
				return current;
			}
			
			visited.Add( current );
			
			foreach( Waypoint neighbour in current.neighbours )
			{
				if( !visited.Contains(neighbour) )
					q.Enqueue( neighbour );
			}
			
			iterations++;
			if( iterations > 50 )
			{
				Debug.LogError("Waypoint:FindClosestPathEnd : reached 50 iterations... SHOULD NOT HAPPEN! " + this.name);
				return null;
			}
		}
		
		return null;
	}
	*/
	
	
	void OnDrawGizmos()
	{	
		/*
		if( this.defaultSceneStartPosition )
		{
			Gizmos.color = new Color (0,1,0,0.5f); 
			Gizmos.DrawCube ( transform.position.y( transform.position.y + 40 * scaleMultiplier), new Vector3 (40 * scaleMultiplier, 40 * scaleMultiplier, 40 * scaleMultiplier) );
		}
		
		if( this.exitToScene != null )
		{
		
			Gizmos.color = new Color (0,0,1,0.5f); 
			Gizmos.DrawCube ( transform.position, new Vector3 (40 * scaleMultiplier, 40 * scaleMultiplier, 40 * scaleMultiplier) );
			
			/*
			// we are an end node
			foreach( Waypoint neighbour in this.neighbours )
			{
				if( neighbour.exitToScene == this.exitToScene )
				{
					Gizmos.DrawLine( this.transform.position, neighbour.transform.position );
					
					foreach( Waypoint neighbour2 in neighbour.neighbours )
					{ 
						if( neighbour2.exitToScene == neighbour.exitToScene )
						{
							Gizmos.DrawLine( neighbour2.transform.position, neighbour.transform.position );
						}
					} 
				}
				
			}
			
		}
		else 
		{ */
	    if (!debug)
	    {
	        Color[] colors = new Color[8];
	        colors[0] = Color.black;
	        colors[1] = Color.blue;
	        colors[2] = Color.green;
	        colors[3] = Color.red;
	        colors[4] = Color.white;
	        colors[5] = Color.cyan;
	        colors[6] = Color.magenta;
	        colors[7] = Color.yellow;

	        DataRange zRange = new DataRange(500, -500);
	        DataRange colorIndexRange = new DataRange(0, 7);

	        float percentageZ = zRange.PercentageInInterval(transform.position.z /*layerOrder*/);

	       
	        if (waypointType == CatchingMiceTile.TileType.Ground)
	        {
                Gizmos.color = colors[(int)colorIndexRange.ValueFromPercentage(percentageZ)].a(0.5f);
	        }
	        else
	        {
	            Gizmos.color = colors[7].a(0.5f);
	        }
	            //new Color (layerOrder / 5.0f, 0, 0, 0.5f); 
	    }
	    else
	    {
            Gizmos.color = new Color(0, 1, 0, 1.0f);
	    }
		
		Gizmos.DrawCube ( transform.position, new Vector3 (0.2f, 0.2f, 0.2f) );
			
		//}
		
		//Gizmos.color = new Color (1,0,0,0.5f); 
		foreach( CatchingMiceWaypoint neighbour in this.neighbours )
		{
			Gizmos.DrawLine( this.transform.position, neighbour.transform.position );
		}
	}
}
