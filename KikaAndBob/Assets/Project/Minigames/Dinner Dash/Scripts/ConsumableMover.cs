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
		item.transform.position = this.transform.position + new Vector3(10, 10, 0);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
