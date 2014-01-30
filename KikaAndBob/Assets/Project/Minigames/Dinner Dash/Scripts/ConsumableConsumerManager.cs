using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsumableConsumerManager : MonoBehaviour 
{
	
	public int maxConcurrentConsumers = 2;
	public DataRange timeBetweenConsumers = new DataRange(4.0f, 10.0f);
	
	// this is to be filled up in the Start() function of the level-specific config script for the game
	public List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
	protected int currentOrderIndex = 0;
	public bool RandomOrders = false;
	
	// different places where the consumers can be seated at
	// filled in automatically
	public List<ConsumableConsumerPlace> places = new List<ConsumableConsumerPlace>();

	// consumer prefabs (primarily graphical in nature)
	// should be positioned beneath the "Consumers" GameObject in the scene
	// filled in automatically
	public List<ConsumableConsumer> consumerPrefabs = new List<ConsumableConsumer>();

	// all consumers that have been spawned by this script
	public List<ConsumableConsumer> consumers = new List<ConsumableConsumer>();


	public DataRange consumerWaitTimeBeforeAngry = new DataRange(6.0f, 8.0f);


	protected ILugusCoroutineHandle generationHandle = null;

	public void StartConsumerGeneration()
	{
		generationHandle = LugusCoroutines.use.StartRoutine( ConsumerGeneratorRoutine() );
	}

	public void StopConsumerGeneration()
	{
		if( generationHandle == null )
			return;

		generationHandle.StopRoutine();
		generationHandle = null;
	} 

	protected int GetActiveConsumerCount()
	{
		int count = 0;
		
		foreach( ConsumableConsumer consumer in consumers )
		{
			if( consumer.IsActive() )
			{
				++count;
			}
		}
		
		return count;
	}

	protected ConsumableConsumerPlace NextConsumerPlace()
	{
		// make a list of the open places (= no consumer assigned) and pick one at random from those

		List<ConsumableConsumerPlace> openPlaces = new List<ConsumableConsumerPlace>();

		foreach( ConsumableConsumerPlace place in places )
		{
			if( place.consumer == null )
			{
				openPlaces.Add( place );
			}
		}

		if( openPlaces.Count == 0 )
		{
			Debug.LogError(name + " : No open place found! All places are taken! Should not happen");
			return null;
		}

		return openPlaces[ Random.Range(0, openPlaces.Count) ];
	}

	public void VisualizeNewConsumer(ConsumableConsumer newConsumer, Vector3 position)
	{
		/*
		newConsumer.transform.position = seat.transform.position + new Vector3(0.0f, -500.0f, 0.0f);
		newConsumer.gameObject.MoveTo( seat.transform.position ).Time (2.0f).EaseType(iTween.EaseType.easeOutBack).Execute();
		*/

		GameObject poofEffect = GameObject.Find ("PoofEffect");
		if( poofEffect != null )
		{
			GameObject poof = (GameObject) GameObject.Instantiate( poofEffect );
			poof.transform.position = position;
			
			GameObject.Destroy( poof, 10.0f );
		}
		
		Vector3 originalScale = newConsumer.transform.localScale;
		newConsumer.transform.localScale = Vector3.zero;
		newConsumer.gameObject.ScaleTo( originalScale ).Time (0.6f).EaseType(iTween.EaseType.easeOutBack).Execute();
		
		newConsumer.transform.position = position; // necessary for pathfinding to work! See IConsumableUser.GetTarget()

	}

	public void VisualizeRemoveConsumer(ConsumableConsumer consumer, Vector3 position)
	{
		GameObject poofEffect = GameObject.Find ("PoofEffect");
		if( poofEffect != null )
		{
			GameObject poof = (GameObject) GameObject.Instantiate( poofEffect );
			poof.transform.position = position;
			
			GameObject.Destroy( poof, 10.0f );
		}

		consumer.gameObject.ScaleTo( Vector3.zero ).Time (0.3f).EaseType(iTween.EaseType.linear).Execute(); 
	}
	
	protected IEnumerator ConsumerGeneratorRoutine()
	{
		if( orders.Count == 0 )
		{
			Debug.LogError(name + " : no consumer orders defined for this level or no more consumer orders available!");
			yield break;
		}
		
		while( true )
		{
			//Debug.Log ("Running ConsumerGeneratorRoutine " + GetActiveConsumerCount() + " / " + maxConcurrentConsumers + " // " + currentOrderIndex);
			
			if( GetActiveConsumerCount() < maxConcurrentConsumers && currentOrderIndex < orders.Count )
			{
				// 1. spawn a new consumer
				// TODO: add pooling mechanism that re-uses inactive spawned consumers?

				
				ConsumableConsumerPlace seat = NextConsumerPlace();
				// consumeras are named the same as the seats
				ConsumableConsumer newConsumer = null;
				foreach( ConsumableConsumer prefab in consumerPrefabs )
				{
					if( prefab.name == seat.name )
					{
						newConsumer = (ConsumableConsumer) GameObject.Instantiate( prefab );
						break;
					}
				}

				if( newConsumer == null )
				{
					Debug.LogError(name + " : no consumer prefab found for seat " + seat.name);
				}
				else
				{
					seat.consumer = newConsumer;
					newConsumer.place = seat;

					newConsumer.state = ConsumableConsumer.State.Seated; // directly seated now, maybe later add waiting for places to gameplay

					newConsumer.Reset();
					newConsumer.OnSeated(); 


					VisualizeNewConsumer( newConsumer, seat.transform.position );
					
					newConsumer.name = /*"Consumer" +*/ seat.transform.name;

					if( RandomOrders )
					{
						newConsumer.order = orders[ Random.Range(0, orders.Count) ];
						orders.Remove( newConsumer.order );
					}
					else
					{
						newConsumer.order = orders[currentOrderIndex];
						currentOrderIndex++;
					}

					consumers.Add( newConsumer );
				}
			}
			
			yield return new WaitForSeconds( timeBetweenConsumers.Random() );
		}
	}





	
	public void SetupLocal()
	{
		// PLACES
		ConsumableConsumerPlace[] allPlaces = (ConsumableConsumerPlace[]) GameObject.FindObjectsOfType( typeof(ConsumableConsumerPlace) );
		
		if( places == null )
			places = new List<ConsumableConsumerPlace>();
		
		places.AddRange( allPlaces );
		
		if( places.Count == 0 )
		{
			Debug.LogError(name + " : no ConsumerPlaces known! Should be at least one in scene!");
		}
		
		// CONSUMER PREFABS
		GameObject consumerContainer = GameObject.Find ("Consumers");
		ConsumableConsumer[] prefabs = consumerContainer.GetComponentsInChildren<ConsumableConsumer>();
		consumerPrefabs.AddRange( prefabs );
		
		if( consumerPrefabs.Count == 0 )
		{
			Debug.LogError (name + " : No Consumer prefabs found! Should be under the \"Consumers\" GameObject in the scene!");
		}
	}

	public void SetupGlobal()
	{
	}

	public void Awake()
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
