using UnityEngine;
using System.Collections;

// Most often: A machine that processes the food
// difference from Processor is that this one creates a new consumable instead of re-using the original one
// originally made for New York: Macaroni can result in either macaroni with ketchup or with cheese
// so we need 2 consumables from 1 ConsumableDefinition, so the MacaroniCheesePot needs to create MacaroniCheese consumable instead of normal Macaroni


public class ConsumableConvertor : IConsumableUser 
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

	public Consumable convertToThis = null;

	public float processingTime = 3.0f;
	public string processingSound = "";

	protected Sprite idleTexture = null;
	public Sprite processingTexture = null;

	public ConsumableConvertor.State state = ConsumableConvertor.State.Idle;
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




		currentConsumable = (Consumable) GameObject.Instantiate( convertToThis );
		currentConsumable.State = subject.State;

		currentConsumable.transform.parent = this.transform.parent;
		currentConsumable.renderer.enabled = false;
		
		// we no longer need to passed-in consumable, since we're creating a new one ourselves from convertToThis
		GameObject.Destroy( subject.gameObject );


		Transform spawnLocation = transform.FindChild("SpawnLocation");
		if( spawnLocation != null )
		{
			currentConsumable.transform.position = spawnLocation.position;
		}
		else 
		{
			currentConsumable.transform.position = this.transform.position + new Vector3(10,10, 0);
		}


		if( processingTexture != null )
			GetComponent<SpriteRenderer>().sprite = processingTexture;

		if( onProcessingStart != null )
			onProcessingStart( currentConsumable );

		if( !string.IsNullOrEmpty(processingSound) )
			LugusAudio.use.SFX().Play( LugusResources.use.Shared.GetAudio(processingSound) );

		yield return new WaitForSeconds( processingTime );
		
		currentConsumable.gameObject.AddComponent<ConsumableHighlight>();
		
		currentConsumable.renderer.enabled = true;

		currentConsumable.State = toState;
		state = State.Done;

		
		if( processingTexture != null )
			GetComponent<SpriteRenderer>().sprite = idleTexture;

		if( onProcessingEnd != null )
			onProcessingEnd( currentConsumable );
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

		if( convertToThis == null )
		{
			Debug.LogError(name + " : no consumable found to convert to!");
		}

		idleTexture = GetComponent<SpriteRenderer>().sprite;
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
