using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsumableConsumerManager : MonoBehaviour 
{
	
	public int maxConcurrentConsumers = 2;
	public DataRange timeBetweenConsumers = new DataRange(4.0f, 10.0f);
	
	// this is to be filled up in the Start() function of the level-specific config script for the game
	public List< List<ConsumableDefinition> > orders;
	protected int currentOrderIndex = 0;
	
	// different places where the consumers can be seated at
	// filled in automatically
	public List<ConsumableConsumerPlace> places = new List<ConsumableConsumerPlace>();

	// consumer prefabs (primarily graphical in nature)
	// should be positioned beneath the "Consumers" GameObject in the scene
	// filled in automatically
	public List<ConsumableConsumer> consumerPrefabs = new List<ConsumableConsumer>();

	// all consumers that have been spawned by this script
	public List<ConsumableConsumer> consumers = new List<ConsumableConsumer>();


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
			if( !consumer.IsActive() )
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
	
	protected IEnumerator ConsumerGeneratorRoutine()
	{
		if( orders.Count == 0 )
		{
			Debug.LogError(name + " : no consumer orders defined for this level!");
			yield break;
		}
		
		while( true )
		{
			//Debug.Log ("Running ConsumerGeneratorRoutine " + GetActiveConsumerCount() + " / " + maxConcurrentConsumers + " // " + currentOrderIndex);
			
			if( GetActiveConsumerCount() < maxConcurrentConsumers && currentOrderIndex < orders.Count )
			{
				// 1. pick a random consumer prefab and spawn a new consumer
				// TODO: add pooling mechanism that re-uses inactive spawned consumers?

				ConsumableConsumer newConsumer = (ConsumableConsumer) GameObject.Instantiate( consumerPrefabs[ Random.Range(0, consumerPrefabs.Count)] );
				newConsumer.state = ConsumableConsumer.State.Seated; // directly seated now, maybe later add waiting for places to gameplay
				newConsumer.OnSeated();
				
				ConsumableConsumerPlace seat = NextConsumerPlace();
				newConsumer.transform.position = seat.transform.position;
				newConsumer.name = /*"Consumer" +*/ seat.transform.name; // necessary for pathfinding to work! See IConsumableUser.GetTarget()
				seat.consumer = newConsumer;
				newConsumer.place = seat;

				newConsumer.order = orders[currentOrderIndex];
				currentOrderIndex++;

				consumers.Add( newConsumer );
			}
			
			yield return new WaitForSeconds( timeBetweenConsumers.Random() );
		}
	}





	
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}

	public void SetupGlobal()
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
