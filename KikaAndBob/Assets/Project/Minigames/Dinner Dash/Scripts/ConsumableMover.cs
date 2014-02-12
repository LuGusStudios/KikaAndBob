using UnityEngine;
using System.Collections;

// most often: the waiter / the one who brings around the consumables (food) to the customers
// basically just a collection of consumables that are currently being carried around
using System.Collections.Generic;


public class ConsumableMover : LugusSingletonExisting<ConsumableMover>
{
	public List<Waypoint> navigationGraph = null;

	public List<Consumable> processedItems = new List<Consumable>();
	public List<Consumable> unprocessedItems = new List<Consumable>();
	public List<Consumable> consumedItems = new List<Consumable>();

	public ConsumableListVisualizer processedVisualizer = null;
	public ConsumableListVisualizer unprocessedVisualizer = null;

	public bool moving = false;
	public Vector3 movementDirection = Vector3.zero;

	public delegate void OnStopped();
	public OnStopped onStopped;

	public IEnumerator MoveToRoutine(Waypoint target)
	{
		//Debug.Log ("MOVING TO target " + target.transform.Path());

		moving = true;

		List<Waypoint> graph = navigationGraph; //new List<Waypoint>( (Waypoint[]) GameObject.FindObjectsOfType(typeof(Waypoint)) );


		Waypoint start = null;
		// find closest waypoint to our current position
		float smallestDistance = float.MaxValue;
		foreach( Waypoint wp in graph )
		{
			float distance = Vector2.Distance( transform.position.v2 (), wp.transform.position.v2 () );
			//Debug.LogError("Distance to " + wp.transform.Path() + " is " + distance + " < " + smallestDistance);
			if( distance < smallestDistance )
			{
				start = wp;
				smallestDistance = distance;
			}
		}

		
		//Debug.Log ("START " + start.transform.Path());
		
		bool fullPath = false;
		List<Waypoint> path = AStarCalculate( graph, start, target, out fullPath );

		/*
		foreach( Waypoint wp in path )
		{
			Debug.Log ("PATH item " + wp.transform.Path());
		}
		*/
		
		int pathIndex = path.Count - 1;
		while( pathIndex > -1 )
		{
			gameObject.StopTweens();
			gameObject.MoveTo( path[pathIndex].transform.position.z(transform.position.z) ).Speed( 600.0f ).Execute();

			movementDirection = Vector3.Normalize( path[pathIndex].transform.position.z(transform.position.z) - transform.position );
			
			float maxDistance = 5.0f; // units (in this setup = pixels)
			bool reachedTarget = false;
			while( !reachedTarget )
			{
				yield return null;
				
				reachedTarget = (Vector2.Distance( transform.position.v2 (), path[pathIndex].transform.position.v2 () ) < maxDistance);
			}
			
			//Mover.renderer.sortingOrder = path[pathIndex].layerOrder;
			transform.position = transform.position.z( /*path[pathIndex].layerOrder*/ path[pathIndex].transform.position.z );
			
			pathIndex--;
		}
		
		// we have reached the final target now (or should have...)
		gameObject.StopTweens();

		moving = false;
	}

	// TODO: move this to Util?
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


	public bool HasConsumables(List<ConsumableDefinition> definitions, Lugus.ConsumableState state)
	{
		List<Consumable> items = null;
		if( state == Lugus.ConsumableState.Unprocessed )
		{
			items = unprocessedItems;
		}
		else if( state == Lugus.ConsumableState.Processed )
		{
			items = processedItems;
		}
		else
		{
			items = consumedItems;
		}

		bool itemFound = false;
		foreach( ConsumableDefinition definition in definitions )
		{
			itemFound = false;

			foreach( Consumable item in items )
			{ 
				if( item.definition == definition && item.State == state )
				{
					itemFound = true;
					break;
				}
			}

			if( !itemFound )
				return false;
		}
		
		return true;
	}

	public bool HasConsumable(ConsumableDefinition definition, Lugus.ConsumableState state)
	{
		List<ConsumableDefinition> l = new List<ConsumableDefinition>();
		l.Add( definition );
		return HasConsumables( l, state );
	}


	public Consumable TakeConsumable(ConsumableDefinition definition, Lugus.ConsumableState state)
	{
		List<Consumable> items = null;
		if( state == Lugus.ConsumableState.Unprocessed )
		{
			items = unprocessedItems;
		}
		else if( state == Lugus.ConsumableState.Processed )
		{
			items = processedItems;
		}
		else
		{
			items = consumedItems;
		}

		foreach( Consumable item in items )
		{ 
			if( item.definition == definition )
			{
				items.Remove( item );
				item.transform.parent = null;

				if( item.State == Lugus.ConsumableState.Unprocessed )
				{
					unprocessedVisualizer.Visualize( unprocessedItems );
				}
				else if( item.State == Lugus.ConsumableState.Processed )
				{
					processedVisualizer.Visualize( processedItems );
				}
				else if( item.State == Lugus.ConsumableState.Consumed )
				{
					// TODO: pick the one that's still empty. If not open... hmz, figure out what to do
					unprocessedVisualizer.Visualize( unprocessedItems );
				}


				return item;
			}
		}



		return null;
	}

	public bool CanCarry(Consumable item)
	{
		if( item.definition.isPayment )
			return true;

		List<Consumable> items = null;
		if( item.State == Lugus.ConsumableState.Unprocessed )
		{
			items = unprocessedItems;
		}
		else if( item.State == Lugus.ConsumableState.Processed )
		{
			items = processedItems;
		}
		else
		{
			return true; // can always carry consumed items
		}

		if( items.Count < 3 )
		{
			return true;
		}
		else
		{
			if( item.State == Lugus.ConsumableState.Unprocessed )
			{
				unprocessedVisualizer.Flash(Color.red);
			}
			else if( item.State == Lugus.ConsumableState.Processed )
			{
				processedVisualizer.Flash(Color.red);
			}

			return false;
		}
	}

	public void AddConsumable(Consumable item, ConsumableConsumer consumer = null)
	{
		if( item.definition.isPayment )
		{
			ProcessPayment(item, consumer);
			return;
		}

		List<Consumable> items = null;
		if( item.State == Lugus.ConsumableState.Unprocessed )
		{
			items = unprocessedItems;
		}
		else if( item.State == Lugus.ConsumableState.Processed )
		{
			items = processedItems;
		}
		else
		{
			items = consumedItems;
		}

		items.Add( item );
		
		// TODO: UPDATE GRAPHICS in a decent way
		item.transform.parent = this.transform; 
		item.transform.position = new Vector3(-9999.0f, -9999.0f, -9999.0f);

		if( item.State == Lugus.ConsumableState.Unprocessed )
		{
			unprocessedVisualizer.Visualize( unprocessedItems );
		}
		else if( item.State == Lugus.ConsumableState.Processed )
		{
			processedVisualizer.Visualize( processedItems );
		}
		else if( item.State == Lugus.ConsumableState.Consumed )
		{
			// TODO: pick the one that's still empty. If not open... hmz, figure out what to do
			unprocessedVisualizer.Visualize( unprocessedItems );

			LugusAudio.use.SFX ().Play ( LugusResources.use.Shared.GetAudio("Dish01") ); 
		}


		//item.renderer.sortingOrder = this.renderer.sortingOrder;
		//item.transform.position = this.transform.position +  new Vector3(10, renderer.bounds.max.y - 10, 0);
	}

	public void ProcessPayment(Consumable payment, ConsumableConsumer consumer)
	{
		float amount = 0.0f;
		string text = "";
		string title = "";
		if( consumer == null )
		{
			Debug.LogError(transform.Path () + " : Consumer was null when processing payment!");
			amount = 10.0f;

			text = "" + amount;
		}
		else
		{
			float order = 12.0f * consumer.order.Count;
			float extra = 5.0f * consumer.happiness;

			amount = order + extra;

			// happiness is in order of 0 - 10 -> reduce to 0-4
			int happinessBucket = 4; // 10 is multi happy already!
			
			if( consumer.happiness < 10 )
			{
				DataRange happinessRange = new DataRange(0,9);
				DataRange indexRange = new DataRange(0, 4);
				
				float percent = happinessRange.PercentageInInterval( consumer.happiness );
				happinessBucket = Mathf.RoundToInt( indexRange.ValueFromPercentage(percent) );
			}

			text = "" + order + " +" + extra;
			title = "" + LugusResources.use.Localized.GetText("dinerdash.tutorial.score.title." + happinessBucket);
		}

		// TODO: score!
		GameObject.Destroy( payment.gameObject );

		ScoreVisualizer
			.Score ( KikaAndBob.CommodityType.Money, amount )
			.Audio ("MoneyCheck01")
			.Color ( Color.white )
			.Position( payment.transform.position.yAdd ( 100.0f ) ) // 100 pixels
			.Text(text)
			.Title (title)
			.Time(3.0f)
			.Execute();
	}

	public void SetupLocal()
	{
		if( processedVisualizer == null )
			processedVisualizer = transform.FindChild("Tray_processed").GetComponent<ConsumableListVisualizer>();

		if( processedVisualizer == null )
			Debug.LogError(name + " : no processed visualizer found!");  


		
		if( unprocessedVisualizer == null )
			unprocessedVisualizer = transform.FindChild("Tray_unprocessed").GetComponent<ConsumableListVisualizer>();
		
		if( unprocessedVisualizer == null )
			Debug.LogError(name + " : no unprocessed visualizer found!");

		
		navigationGraph = new List<Waypoint>( (Waypoint[]) GameObject.FindObjectsOfType(typeof(Waypoint)) );
		
		if( navigationGraph.Count == 0 )
			Debug.LogError(transform.Path() + " : no navigationGraph found for this level!!");


		moving = false;
		movementDirection = Vector3.zero;
	}
	
	public void SetupGlobal()
	{
		processedVisualizer.Hide();
		unprocessedVisualizer.Hide();
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
		
	}
}











