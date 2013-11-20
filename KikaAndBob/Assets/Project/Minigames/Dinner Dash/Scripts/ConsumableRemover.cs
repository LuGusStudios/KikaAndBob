using UnityEngine;
using System.Collections;

// removes all current consumables from the Mover
// example: garbage bin
public class ConsumableRemover : IConsumableUser
{
	protected void Awake()
	{
	}
	
	public override bool Use()
	{
		// TODO: graphics update?
		// TODO: remove points for wasting food for consumables that didn't have Consumed state

		ConsumableMover mover = DinnerDashManager.use.Mover;
		foreach( Consumable consumable in mover.consumedItems )
		{
			GameObject.Destroy( consumable.gameObject );
		}
		mover.consumedItems.Clear();

		foreach( Consumable consumable in mover.unprocessedItems )
		{
			GameObject.Destroy( consumable.gameObject );
			// TODO: decrement score
		}
		mover.unprocessedItems.Clear();

		foreach( Consumable consumable in mover.processedItems )
		{
			GameObject.Destroy( consumable.gameObject );
			// TODO: decrement score
		}
		mover.processedItems.Clear();
		
		return true;
	}
}
