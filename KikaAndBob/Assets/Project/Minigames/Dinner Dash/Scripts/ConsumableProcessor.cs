using UnityEngine;
using System.Collections;

// Most often: A machine that processes the food


public class ConsumableProcessor : IConsumableUser 
{
	public enum State
	{
		Idle = 1, // waiting for work
		Processing = 2, // busy processing a consumable
		Done = 3, // done processing the consumable: state has changed, ready for pickup

		NONE = -1
	}

	public ConsumableDefinition[] consumables;

	public Lugus.ConsumableState fromState = Lugus.ConsumableState.NONE;
	public Lugus.ConsumableState toState = Lugus.ConsumableState.NONE;

	public float processingTime = 3.0f;

	public ConsumableProcessor.State state = ConsumableProcessor.State.Idle;
	public Consumable currentConsumable = null;

	public delegate void OnProcessing(Consumable consumable);

	public OnProcessing onProcessingStart;
	public OnProcessing onProcessingEnd;


	public override bool Use()
	{
		if( state == State.Idle )
		{
			// 1. check if the Mover has a consumable we can process (fromState + definition)
			// if it has multiple: just take the first one we see
			Consumable subject = null;
			// TODO:
			foreach( ConsumableDefinition def in consumables )
			{
				subject = DinnerDashManager.use.Mover.TakeConsumable( def, fromState );

				if( subject != null )
				{
					break;
				}
			}

			// 2. if yes: take over control and start processing
			//    if no : return false

			if( subject != null )
			{
				LugusCoroutines.use.StartRoutine( ProcessingRoutine(subject) );
				return true;
			}
			else
			{
				return false;
			} 
		}

		if( state == State.Done )
		{
			if( !DinnerDashManager.use.Mover.CanCarry(currentConsumable) )
			   return false; // TODO: highlight 

			
			GameObject.Destroy( currentConsumable.gameObject.GetComponent<ConsumableHighlight>() );

			// 1. transfer currentConsumable over to the Mover
			DinnerDashManager.use.Mover.AddConsumable( currentConsumable );

			// 2. empty this processor for a new round
			currentConsumable = null;
			state = State.Idle;
			
			return true;
		}

		return false;
	}

	protected IEnumerator ProcessingRoutine(Consumable subject)
	{
		state = State.Processing;
		currentConsumable = subject;

		subject.transform.parent = this.transform.parent;
		//subject.renderer.sortingOrder = this.renderer.sortingOrder;

		Transform spawnLocation = transform.FindChild("SpawnLocation");
		if( spawnLocation != null )
		{
			subject.transform.position = spawnLocation.position;
		}
		else 
		{
			subject.transform.position = this.transform.position + new Vector3(10,10, 0);
		}


		subject.renderer.enabled = false;

		// TODO: show graphic update of the subject being placed "inside" the processor
		// TODO: show graphical update indicating we're busy

		if( onProcessingStart != null )
			onProcessingStart( subject );

		yield return new WaitForSeconds( processingTime );
		
		currentConsumable.gameObject.AddComponent<ConsumableHighlight>();
		
		subject.GetComponent<SpriteRenderer>().enabled = true;

		currentConsumable.State = toState;
		state = State.Done;
		
		if( onProcessingStart != null )
			onProcessingEnd( subject );

		// TODO: indicate food is ready for pickup (graphically!)
	}

	public void SetupLocal()
	{
		if( fromState == Lugus.ConsumableState.NONE || toState == Lugus.ConsumableState.NONE )
		{
			Debug.LogError(name + " : fromState " + fromState + " or toState " + toState + " is NONE!");
		}
		
		if( consumables.Length == 0 )
		{
			Debug.LogError(name + " : no consumables found that can be processed here!");
		}
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
