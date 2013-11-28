using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashManager : LugusSingletonExisting<IDinnerDashManager> 
{

}

public abstract class IDinnerDashManager : MonoBehaviour
{
	protected ConsumableMover _mover = null;
	public ConsumableMover Mover
	{
		get
		{
			if( _mover == null )
			{
				_mover = (ConsumableMover) GameObject.FindObjectOfType( typeof(ConsumableMover) ); 
			}
			
			return _mover;
		}
	}

	public ConsumableConsumerManager consumerManager = null;

	public abstract void StartGame();
	public abstract void StopGame();
}

public class DinnerDashManagerDefault : IDinnerDashManager
{


	public void SetupLocal()
	{
		if( consumerManager == null )
			consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}

	public override void StartGame()
	{
		StopGame ();
		Debug.Log ("Starting dinner dash");


		queueRoutineHandle = LugusCoroutines.use.StartRoutine( QueueRoutine() );

		consumerManager.StartConsumerGeneration();
	}

	public override void StopGame()
	{
		Debug.Log ("Stopping dinner dash");

		if( queueRoutineHandle != null )
		{
			queueRoutineHandle.StopRoutine();
			queueRoutineHandle = null;
		}

		if( consumerManager != null )
		{
			consumerManager.StopConsumerGeneration();
		}
	}

	public List<IConsumableUser> queue = new List<IConsumableUser>();

	protected ILugusCoroutineHandle queueRoutineHandle = null;

	protected void Update()
	{
		Transform hit = LugusInput.use.RayCastFromMouseUp( LugusCamera.game );
		if( hit == null )
			return;

		IConsumableUser user = hit.GetComponent<IConsumableUser>();
		if( user != null )
		{
			Debug.Log ("Added user to queue : " + user.name);
			queue.Add(user);
		}
	}




	//protected bool processing = false;

	protected IEnumerator QueueRoutine()
	{
		while( true )
		{
			if( queue.Count == 0 /*|| processing*/ )
			{
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				//processing = true;
				processHandle = LugusCoroutines.use.GetHandle();
				yield return processHandle.StartRoutine( Process(this.queue[0]) );
			}
		}
	}

	protected ILugusCoroutineHandle processHandle = null;
	protected IEnumerator Process(IConsumableUser user)
	{
		//Debug.Log ("Processing next item in queue : " + user.name );
		
		queue.Remove (user);
		
		Waypoint target = user.GetTarget(); 

		// TODO: move Mover to user location before actually using it : properly
		//Mover.transform.positionTo(3.0f, target); 
		//GoTween tween = Go.to( Mover.transform, 3.0f, new GoTweenConfig().position( target ) );

		List<Waypoint> graph = new List<Waypoint>( (Waypoint[]) GameObject.FindObjectsOfType(typeof(Waypoint)) );

		Waypoint start = null;
		// find closest waypoint to our current position
		float smallestDistance = float.MaxValue;
		foreach( Waypoint wp in graph )
		{
			float distance = Vector2.Distance( Mover.transform.position.v2 (), wp.transform.position.v2 () );
			if( distance < smallestDistance )
			{
				start = wp;
				smallestDistance = distance;
			}
		}


		bool fullPath = false;
		List<Waypoint> path = AStarCalculate( graph, start, target, out fullPath );

		int pathIndex = path.Count - 1;
		while( pathIndex > -1 )
		{
			Mover.gameObject.StopTweens();
			Mover.gameObject.MoveTo( path[pathIndex].transform.position.z(Mover.transform.position.z) ).Speed( 600.0f ).Execute();
			
			float maxDistance = 5.0f; // units (in this setup = pixels)
			bool reachedTarget = false;
			while( !reachedTarget )
			{
				// 4 frames
				yield return null;
				
				reachedTarget = (Vector2.Distance( Mover.transform.position.v2 (), path[pathIndex].transform.position.v2 () ) < maxDistance);
			}

			//Mover.renderer.sortingOrder = path[pathIndex].layerOrder;
			Mover.transform.position = Mover.transform.position.z( /*path[pathIndex].layerOrder*/ path[pathIndex].transform.position.z );

			pathIndex--;
		}

		// we have reached the final target now (or should have...)
		Mover.gameObject.StopTweens();
		user.Use ();

		/*
		//Mover.gameObject.StopTweens();
		//Mover.gameObject.MoveTo( target.transform.position ).Speed( 600.0f ).Execute(); 

		float maxDistance = 5.0f; // units (in this setup = pixels)

		bool reachedTarget = false;
		while( !reachedTarget )
		{
			yield return new WaitForSeconds(0.1f);

			reachedTarget = (Vector2.Distance( Mover.transform.position.v2 (), target.transform.position.v2 () ) < maxDistance);
		}

		//Mover.gameObject.StopTweens();

		user.Use ();
		*/
	}
	
	protected List<Waypoint> AStarCalculate( List<Waypoint> waypoints, Waypoint start, Waypoint stop, out bool wasFullPath )
	{
		// https://code.google.com/p/csharpgameprogramming/source/browse/trunk/Examples/AdventureGames/PathFinding/AStar.cs
		
		List<Waypoint> path = new List<Waypoint>();
		
		// 1. Setup : clear cost and parents of waypoints
		foreach( Waypoint waypoint in waypoints )
		{
			waypoint.AStarCost = 0.0f;
			waypoint.AStarParent = null;
		}
		
		List<Waypoint> openList = new List<Waypoint>();
		List<Waypoint> closedList = new List<Waypoint>();
		
		openList.Add( start );
		wasFullPath = true;
		
		
		// 2. 
		bool running = true;
		int count = 0;
		Waypoint current = null; 
		while( running ) 
		{
			
			if( openList.Count == 0 )
			{
				//Debug.LogWarning("OpenList is empty. Current is " + current.name );
				
				// TODO: ga closedList af, neem degene met laagste Cost als current : TEST of dat juiste resultaten geeft
				
				current = closedList[0];
				foreach( Waypoint candidate in closedList )
				{
					if( candidate.AStarCost < current.AStarCost )
						current = candidate;
				}
				
				// we have reached our final destination
				// backtrace to build the path
				Waypoint pointer = current; 
				while( pointer != null )
				{
					//Debug.LogWarning("ADDING PATH item " + pointer.name + " from " + pointer.AStarParent);
					path.Add ( pointer );
					pointer = pointer.AStarParent;
				}
				
				wasFullPath = false;
				running = false;
				
				continue; // skip the rest of the calculations
			}
			
			
			current = openList[0]; // initialize so it's not null
			
			// current should be the Best waypoint in the Open list (the one with the lowest cost at this time)
			foreach( Waypoint waypoint in openList )
			{
				if( waypoint.AStarCost < current.AStarCost )
				{
					current = waypoint;
				}
			}
			
			//Debug.Log ("ASTar iteration " + count + " : new current == " + current.name);
			
			
			if( current == stop ) 
			{
				// we have reached our final destination
				// backtrace to build the path
				Waypoint pointer = stop;
				while( pointer != null )
				{
					path.Add ( pointer );
					pointer = pointer.AStarParent;
				}
				
				wasFullPath = true;
				running = false;
				
				continue; // skip the rest of the calculations
			}
			
			// not sure if the current will still be a good option after these calculations
			// so remove it from the openList for now and only add if needed later
			openList.Remove( current );
			closedList.Add ( current );
			
			foreach( Waypoint neighbour in current.neighbours )
			{
				// http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
				
				// NOTE: This is not actually the best implementation of AStar heuristics, as that uses the distance to the goal as well
				// however, I find this one gives a bit more variation and more interesting paths in the current setup, so keep it for now
				
				// use the distance to the neighbour as a heuristic here
				float cost = current.AStarCost + Vector3.Distance(neighbour.transform.position, current.transform.position);//Vector3.Distance( neighbour.transform.position, stop.transform.position ); 
				
				// if the neighbour's cost is already higher than the cost for this node
				// the neighbour is never going to be the best path, so delete it from our calculations 
				if ( openList.Contains(neighbour) && cost < neighbour.AStarCost)
				{
					openList.Remove(neighbour); 
				}
				
				if ( closedList.Contains(neighbour ) && cost < neighbour.AStarCost)
				{
					closedList.Remove(neighbour);
				}
				
				// if neighbour has not yet been examined : put it up for examination
				if( !openList.Contains(neighbour) && !closedList.Contains(neighbour) )
				{
					//Debug.Log ("ASTAR : adding " + neighbour.name + " to open with cost " + cost);
					
					neighbour.AStarCost = cost;
					neighbour.AStarParent = current;
					openList.Add ( neighbour );
				}
			}
			
			// if we haven't found the shortest path after 50 iterations in this game, we probably won't find it ever
			if( count == 500 )
			{
				Debug.LogError("TownNavigationDefault:AStarCalculate : reached iteration limit of 50. Returning path of only the Stop waypoint");
				path.Add ( stop );
				wasFullPath = true;
				return path;
			}
			
			count++;
		}
		
		return path;
	}

	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start()
	{
		SetupGlobal();
	}

	protected bool debugAutoProcessing = true;
	protected void OnGUI()
	{
		GUILayout.BeginArea( new Rect(0, 0, 200, 400) );
		GUILayout.BeginVertical();

		if( GUILayout.Button("AutoProcessing : "+ debugAutoProcessing) )
		{
			debugAutoProcessing = !debugAutoProcessing;

			if( !debugAutoProcessing )
				queueRoutineHandle.StopRoutine();
			else
				queueRoutineHandle = LugusCoroutines.use.StartRoutine( QueueRoutine() );
		}

		if( !debugAutoProcessing && queue.Count > 0 )
		{
			bool go = true;
			if( processHandle != null && processHandle.Running )
				go = false;
			 
			if( go )
			{
				if( GUILayout.Button ("Process one") )
				{
					processHandle = LugusCoroutines.use.GetHandle();
					processHandle.StartRoutine( Process(this.queue[0]) );
				}
			}
		}


		GUILayout.Label( "Queue:" + ( (processHandle != null && processHandle.Running) ? "Busy" : "") );
		GUILayout.Label( "------");
		foreach( IConsumableUser user in queue )
		{
			GUILayout.Label( "" + user.name );
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
