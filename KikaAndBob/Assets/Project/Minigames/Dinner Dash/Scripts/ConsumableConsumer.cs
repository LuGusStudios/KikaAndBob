using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Most often: A customer that will eat the food and later pay for it and leaves dirty dishes
// at the moment, consumers only accept consumables of a Processed state
using SmoothMoves;

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
		Leaving = 7, // payment is done, we're leaving
		
		NONE = -1
	}

	public enum AnimationType
	{
		NONE = -1,

		Idle = 1,
		Angry = 2,
		Eating = 3
	}

	// has to be set externally when this consumer is spawned
	public List<ConsumableDefinition> order;
	
	public DataRange eatingTime = new DataRange(2.0f, 4.0f);
	public DataRange orderTime = new DataRange(2.0f, 4.0f); 

	public DataRange waitingTimeBeforeAngry = new DataRange(6.0f, 8.0f);

	public bool useAlternativeAnimations = false; // for now only used in New York to support 1 character with 2 texture sets

	public float happiness = 10.0f;

	protected ConsumableConsumer.State _state = ConsumableConsumer.State.Seated;
	public ConsumableConsumer.State state
	{
		set
		{
			//Debug.Log("Consumer set state " + value );
			State oldState = _state;
			_state = value;

			if( _state != oldState )
			{
				if( DinnerDashManager.use.consumerManager.onConsumerStateChange != null )
					DinnerDashManager.use.consumerManager.onConsumerStateChange(this, oldState, _state);
			}
		}

		get
		{
			return _state;
		} 
	}

	public Consumable currentConsumable = null;

	// place (chair) the consumer is currently taking in
	public ConsumableConsumerPlace place = null;

	public Consumable moneyPrefab = null;



	protected ILugusCoroutineHandle waitingHandle = null;

	public override bool Use()
	{
		if( state == State.Ordered )  
		{
			// 1. check if the Mover has all the items in our order
			if( ! DinnerDashManager.use.Mover.HasConsumables(order, Lugus.ConsumableState.Processed) )
				return false;

			// if the order has multiple consumables
			// - first one becomes currentConsumable
			// - other ones are just removed (for now. TODO: possibly keep all? but at the end, only 1 consumable in consumed state is left behind for cleanup)
			Consumable first = null;

			foreach( ConsumableDefinition def in order ) 
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
				Debug.LogError(name + " : Consumer could nog consume what the mover is carrying");
				return false;
			} 
		}
		
		if( state == State.Done )
		{
			// 1. transfer currentConsumable (which is consumed now) over to the Mover

			
			GameObject.Destroy( currentConsumable.gameObject.GetComponent<ConsumableHighlight>() );

			currentConsumable.transform.localScale *= 1.5f;
			DinnerDashManager.use.Mover.AddConsumable( currentConsumable );
			
			// pay for the food
			currentConsumable = null;
			DoneEating();
			
			return true; 
		}
		
		if( state == State.Paying ) 
		{
			// we can be in state paying, but no money on the table yet. If we are hit then, just ignore it for now
			if( currentConsumable == null )
				return false;

			// 1. add score (money)
			// TODO:
			// currentConsumable is now the money. The Mover will process this accordingly
			GameObject.Destroy( currentConsumable.gameObject.GetComponent<ConsumableHighlight>() );
			DinnerDashManager.use.Mover.AddConsumable( currentConsumable, this );
			
			//2. client leaves

			place.happinessVisualizer.Hide();

			// TODO: decently move
			//this.gameObject.MoveTo( Vector3.left * 3000 ).Speed ( 999999.0f ).Execute();
			DinnerDashManager.use.consumerManager.VisualizeRemoveConsumer(this, this.transform.position);
			place.consumer = null;
			this.place = null;

			this.state = State.Leaving;

			LugusCoroutines.use.StartRoutine( SetStateDelayed(5.0f, State.NONE) );



			if( waitingHandle != null )
				waitingHandle.StopRoutine();

			return true;
		}

		return false;
	}

	public bool IsActive()
	{
		return (state != ConsumableConsumer.State.NONE && state != ConsumableConsumer.State.Leaving);
	}

	public IEnumerator SetStateDelayed(float delay, State newState)
	{
		yield return new WaitForSeconds(delay);

		this.state = newState;

		if( this.state == State.NONE )
		{
			DinnerDashManager.use.consumerManager.RemoveConsumerDelayed( this, 0.0f );
		}
	}

	protected void DoneEating()
	{
		// 2 options: either pay or go back to looking at the menu and order another food item
		// for now: just pay
		// TODO: order yet another food item (ex. desert)

		state = State.Paying;
		PlayAnimation( AnimationType.Idle );
		
		LugusCoroutines.use.StartRoutine( PaymentRoutine( orderTime.Random() ) );
	}

	protected IEnumerator PaymentRoutine(float delay) 
	{
		if( delay != 0.0f )
			yield return new WaitForSeconds( delay );
		
		Consumable money = (Consumable) GameObject.Instantiate( moneyPrefab );
		
		currentConsumable = money;
		
		currentConsumable.transform.parent = this.transform.parent;
		currentConsumable.transform.position = place.consumableLocation.position;
		currentConsumable.gameObject.AddComponent<ConsumableHighlight>();

		yield break;
	}

	protected void PlayAnimation(AnimationType type)
	{
		BoneAnimation animation = GetComponent<BoneAnimation>();
		if( animation == null )
		{
			animation = gameObject.GetComponentInChildren<BoneAnimation>();

			if( animation == null )
			{
				Debug.LogError(transform.Path () + " : No BoneAnimation found for this consumer!");
				return;
			}
		}

		string animationName = "";
		foreach( AnimationClipSM_Lite candidate in animation.mAnimationClips )
		{
			if( !useAlternativeAnimations )
			{
				if( type == AnimationType.Angry )
				{
					if( candidate.animationName.Contains("Stage2") ||
					   candidate.animationName.Contains("Angry") ||
					   candidate.animationName.Contains("Bang_table"))
					{
						animationName = candidate.animationName;
						break;
					}
				}
				else if( type == AnimationType.Idle ) 
				{
					if( candidate.animationName.Contains("Sit") ||
					    candidate.animationName.Contains("Idl") ||
					   candidate.animationName.Contains("Idle") ||
					   candidate.animationName.Contains("JewBoy") )
					{
						animationName = candidate.animationName;
						break;
					}
				}
				else if( type == AnimationType.Eating )
				{
					if( candidate.animationName.Contains("Eating") )
					{
						animationName = candidate.animationName;
						break;
					}
				}
			}
			else
			{
				// in NewYork, we use the same BoneAnimation for 2 different characters with 2 texture sets
				if( type == AnimationType.Angry )
				{
					if( candidate.animationName.Contains("Stage202") )
					{
						animationName = candidate.animationName;
						break;
					}
				}
				else if( type == AnimationType.Idle ) 
				{
					if( candidate.animationName.Contains("Idle02") )
					{
						animationName = candidate.animationName;
						break;
					}
				}
				else if( type == AnimationType.Eating )
				{
					if( candidate.animationName.Contains("Eating02") )
					{
						animationName = candidate.animationName;
						break;
					}
				}

			}
		}

		if( animationName == "" )
		{

			if( type != AnimationType.Idle )
			{
				Debug.LogWarning(transform.Path() + " : No animation found for type " + type + ". Default to Idle.");
				PlayAnimation(AnimationType.Idle);
				return;
			}
			else
			{
				Debug.LogError(transform.Path() + " : No animation found for type " + type + ".");
				return;
			}
		}

		//Debug.LogError("PlayingAnimation " + animationName + " // " + transform.Path () );
		animation.CrossFade( animationName, 1.0f );
		//animation.Play ( animationName );
	}

	protected IEnumerator WaitingRoutine(State beginningState)
	{
		yield return new WaitForSeconds( waitingTimeBeforeAngry.Random() );

		if( this.state == beginningState )
		{
			// still in the same state -> we're going to get annoyed now!
			PlayAnimation( AnimationType.Angry );
		}

		bool waiting = true;
		while( waiting )
		{
			if( this.state != beginningState )
			{
				// stopped waiting
				break;
			}
			else
			{
				happiness -= 1.0f;
				place.happinessVisualizer.Visualize( this.happiness );

				yield return new WaitForSeconds( waitingTimeBeforeAngry.Random() );

				if( happiness < -1.0f )
				{
					// waited long enough, i'm going home!
					LeaveAnnoyed();
					break;
				}
			}
		}

		yield break;
	}

	protected void LeaveAnnoyed()
	{
		// we are either in Ordered state (waiting to be served)
		// or in the Done state (empty dishes are set out in front)

		bool stop = true;
		if( state == State.Ordered )  
		{
			// hide the order
			place.orderVisualizer.Hide();
		}
		else if( state == State.Done )
		{
			// clear the dishes
			if( currentConsumable != null )
			{
				GameObject.Destroy( currentConsumable.gameObject );
			}
		}
		else
		{
			stop = false;
			Debug.LogError(transform.Path () + " : Consumer was in state " + state + " and cannot LeaveAnnoyed!");
		}

		if( stop ) 
		{
			place.happinessVisualizer.Hide();

			DinnerDashManager.use.consumerManager.VisualizeRemoveConsumer(this, this.transform.position);
			place.consumer = null;
			this.place = null;
			LugusCoroutines.use.StartRoutine( SetStateDelayed(5.0f, State.NONE) );

			if( waitingHandle != null )
				waitingHandle.StopRoutine();
		}

	}

	// called before re-using this consumer
	public void Reset()
	{
		PlayAnimation( AnimationType.Idle );
		this.waitingTimeBeforeAngry = DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry;

		this.happiness = 10.0f;
		place.happinessVisualizer.Visualize( this.happiness );
	}

	// TODO: now this is called from the outside
	// should be refactored so that it's called every time we set state = State.Seated?
	public void OnSeated()
	{ 
		LugusCoroutines.use.StartRoutine( OrderRoutine() );
	}

	protected IEnumerator OrderRoutine()
	{
		state = State.Seated;

		// TODO: graphical update: consumer looking into menu

		yield return new WaitForSeconds( orderTime.Random() );

		// TODO: show the order
		place.orderVisualizer.Visualize( order );

		state = State.Ordered;  

		waitingHandle = LugusCoroutines.use.StartRoutine( WaitingRoutine(State.Ordered) );
	}


	protected IEnumerator ConsumeRoutine(Consumable subject)
	{
		place.orderVisualizer.Hide();

		state = State.Eating;
		currentConsumable = subject;
		
		subject.transform.parent = this.transform.parent;
		//subject.renderer.sortingOrder = this.renderer.sortingOrder;
		subject.transform.position = place.consumableLocation.position; 
		//subject.transform.localScale *= 0.5f;

		ILugusAudioTrack track = LugusAudio.use.SFX ().Play ( LugusResources.use.Shared.GetAudio("Eating01"), false, new LugusAudioTrackSettings().Loop(true) );

		PlayAnimation(AnimationType.Eating);

		yield return new WaitForSeconds( eatingTime.Random () ); 

		track.Stop();
		
		currentConsumable.State = Lugus.ConsumableState.Consumed;
		state = State.Done;

		currentConsumable.gameObject.AddComponent<ConsumableHighlight>();
		
		PlayAnimation(AnimationType.Idle);
		waitingHandle = LugusCoroutines.use.StartRoutine( WaitingRoutine(State.Done) );


		
		// TODO: graphical indication client is ready?
	}
	
	public void SetupLocal()
	{
		if( moneyPrefab == null )
		{
			GameObject moneyGO = GameObject.Find ("Money");
			if( moneyGO != null )
			{
				moneyPrefab = moneyGO.GetComponent<Consumable>();
			}
		}

		if( moneyPrefab == null )
		{
			Debug.LogError(name + " : no money prefab found!");
		}
	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame 
	void Update () 
	{
		/*
		//if( LugusInput.use.KeyDown( KeyCode.P ) && this.place != null )
		{
			this.happiness = Random.Range(0, 11);
			LugusCoroutines.use.StartRoutine( PaymentRoutine(0.0f) );
			this.state = State.Paying;
			Use ();
		}
		*/

	}

	void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		Vector3 screenPos = LugusCamera.game.WorldToScreenPoint( this.transform.position );

		string orderDesc = "";
		if( order != null && order.Count > 0 )
		{
			foreach( ConsumableDefinition cd in order )
				orderDesc += cd.name + ", ";
		}

		GUI.Label( new Rect(screenPos.x, Screen.height - screenPos.y, 100, 100), "" + this.state + "\n" + orderDesc ); 
	}
}
