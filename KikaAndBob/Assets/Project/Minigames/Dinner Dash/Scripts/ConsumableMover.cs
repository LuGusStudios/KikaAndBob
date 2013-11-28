using UnityEngine;
using System.Collections;

// most often: the waiter / the one who brings around the consumables (food) to the customers
// basically just a collection of consumables that are currently being carried around
using System.Collections.Generic;


public class ConsumableMover : LugusSingletonExisting<ConsumableMover>
{
	public List<Consumable> processedItems = new List<Consumable>();
	public List<Consumable> unprocessedItems = new List<Consumable>();
	public List<Consumable> consumedItems = new List<Consumable>();

	public ConsumableListVisualizer processedVisualizer = null;
	public ConsumableListVisualizer unprocessedVisualizer = null;

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

	public void AddConsumable(Consumable item)
	{
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
		}


		//item.renderer.sortingOrder = this.renderer.sortingOrder;
		//item.transform.position = this.transform.position +  new Vector3(10, renderer.bounds.max.y - 10, 0);
	}

	public void SetupLocal()
	{
		if( processedVisualizer == null )
			processedVisualizer = transform.FindChild("Tray_processed").GetComponent<ConsumableListVisualizer>();

		if( processedVisualizer == null )
			Debug.LogError(name + " : no processed visualizer found!");  

		processedVisualizer.Hide();

		
		if( unprocessedVisualizer == null )
			unprocessedVisualizer = transform.FindChild("Tray_unprocessed").GetComponent<ConsumableListVisualizer>();
		
		if( unprocessedVisualizer == null )
			Debug.LogError(name + " : no unprocessed visualizer found!");
		
		unprocessedVisualizer.Hide();

	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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











