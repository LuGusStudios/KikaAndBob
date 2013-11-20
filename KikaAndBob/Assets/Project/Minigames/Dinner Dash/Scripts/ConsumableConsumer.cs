using UnityEngine;
using System.Collections;

// Most often: A customer that will eat the food and later pay for it and leaves dirty dishes
// at the moment, consumers only accept consumables of a Processed state

public class ConsumableConsumer : IConsumableUser 
{
	public enum State
	{
		Idle = 1, // waiting to be seated
		Seated = 2, // looking over the menu
		Ordered = 3, // placed an order, waiting for the food
		Eating = 4, // eating the food
		Done = 5, // done eating the food, waiting for cleanup
		Paying = 6, // all done, waiting for mover to pickup the payment
		
		NONE = -1
	}
	
	public ConsumableDefinition[] consumables;
	
	public DataRange eatingTime = new DataRange(2.0f, 4.0f);
	public DataRange orderTime = new DataRange(2.0f, 4.0f); 
	
	public ConsumableConsumer.State state = ConsumableConsumer.State.Seated;
	public Consumable currentConsumable = null;
	
	public override bool Use()
	{
		if( state == State.Ordered )  
		{
			// 1. check if the Mover consumables we can consume
			// if it has multiple: 
			// - first one becomes currentConsumable
			// - other ones are just removed (for now. TODO: possibly keep all? but at the end, only 1 consumable in consumed state is left behind for cleanup)
			Consumable first = null;

			foreach( ConsumableDefinition def in consumables )
			{
				Consumable consumable = DinnerDashManager.use.Mover.TakeConsumable( def, Lugus.ConsumableState.Processed );
				
				if( consumable != null )
				{
					if( first == null )
					{
						first = consumable;
					}
					else
					{
						GameObject.Destroy( consumable.gameObject );
					}
				}
			}
			
			// 2. if yes: take over control of consumable and start consuming
			//    if no : return false
			
			if( first != null )
			{
				LugusCoroutines.use.StartRoutine( ConsumeRoutine(first) );
				return true;
			}
			else
			{
				return false;
			} 
		}
		
		if( state == State.Done )
		{
			// 1. transfer currentConsumable (which is consumed now) over to the Mover
			
			currentConsumable.transform.localScale *= 1.5f;
			DinnerDashManager.use.Mover.AddConsumable( currentConsumable );
			
			// pay for the food
			currentConsumable = null;
			DoneEating();
			
			return true; 
		}
		
		if( state == State.Paying )
		{
			// 1. add score (money)
			// TODO:
			
			//2. client leaves
			// TODO: decently move
			this.gameObject.MoveTo( Vector3.left * 1000 ).Speed ( 200.0f ).Execute();
			
			return true;
		}

		return false;
	}

	protected void DoneEating()
	{
		// 2 options: either pay or go back to looking at the menu and order another food item
		// for now: just pay
		// TODO: order yet another food item (ex. desert)

		state = State.Paying;
	}

	protected void OnSeated()
	{
		LugusCoroutines.use.StartRoutine( OrderRoutine() );
	}

	protected IEnumerator OrderRoutine()
	{
		state = State.Seated;

		// TODO: graphical update: consumer looking into menu

		yield return new WaitForSeconds( orderTime.Random() );

		// TODO: create an order (random combination of consumables?)

		state = State.Ordered; 
	}


	protected IEnumerator ConsumeRoutine(Consumable subject)
	{
		state = State.Eating;
		currentConsumable = subject;
		
		subject.transform.parent = this.transform.parent;
		subject.transform.position = this.transform.position + new Vector3(10,10,0);
		subject.transform.localScale *= 0.5f;

		
		yield return new WaitForSeconds( eatingTime.Random () );
		
		currentConsumable.State = Lugus.ConsumableState.Consumed;
		state = State.Done;
		
		// TODO: graphical indication client is ready?
	}
	
	protected void Awake()
	{
		if( consumables.Length == 0 )
		{
			Debug.LogError(name + " : no consumables found that can be consumed here!");
		}

		OnSeated();
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame 
	void Update () {
		
	}

	void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		Vector3 screenPos = LugusCamera.game.WorldToScreenPoint( this.transform.position );

		GUI.Label( new Rect(screenPos.x, Screen.height - screenPos.y, 100, 100), "" + this.state + "\n" + consumables[0].name ); 
	}
}
