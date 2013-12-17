using UnityEngine;
using System.Collections;

// produces basic ingredients (typically Unprocessed consumables)
// example: fruit in a basket
public class ConsumableProducer : IConsumableUser
{
	public Consumable produces = null; 

	protected void Awake()
	{
		if( produces == null )
		{
			Debug.LogError(name + " : Producer has no thing to produce!" );
		}
	}

	public override bool Use()
	{
		if( !DinnerDashManager.use.Mover.CanCarry(produces) )
			return false; 


		// in cooking dash, mover has 2 plateaus: 1 for cooked food, 1 for uncooked. Can carry both at once
		// also: some producers don't move anything to the tray (example when granny needs to make a sandwich, you just click on the sandwich and she begins)
		// -> in this case the mover shouldn't even move to this Producer!!! TODO
		// -> this is also the case if we have customers that need to be seated... TODO

		Consumable newProduct = (Consumable) GameObject.Instantiate( produces );
		newProduct.transform.position = this.transform.position + new Vector3(10, 10, 0);

		newProduct.State = produces.State;

		DinnerDashManager.use.Mover.AddConsumable(newProduct);

		return true;
	}
}
