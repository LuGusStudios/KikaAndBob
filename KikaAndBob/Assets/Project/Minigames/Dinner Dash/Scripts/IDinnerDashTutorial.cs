using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IDinnerDashTutorial : MonoBehaviour 
{
	public IndicatorArrow arrow = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( arrow == null )
		{
			arrow = GameObject.FindObjectOfType<IndicatorArrow>();
		}
		if( arrow == null )
		{
			Debug.LogError(transform.Path () + " : No IndicatorArrow found in the scene!");
		}
	}

	public void OnConsumerStateChanged(ConsumableConsumer consumer, ConsumableConsumer.State oldState, ConsumableConsumer.State newState )
	{
		Debug.LogError(consumer.transform.Path() + " : State changed! " + oldState + " // " + newState);
		if( newState == ConsumableConsumer.State.Ordered || 
		   newState == ConsumableConsumer.State.Eating || 
		   newState == ConsumableConsumer.State.Done ||
		   newState == ConsumableConsumer.State.Paying ||
		   newState == ConsumableConsumer.State.Leaving
		   )
		{
			NextStep();
		}
	}
	
	public void OnConsumableUsed(IConsumableUser subject)
	{
		Debug.LogError(subject.transform.Path() + " : User used!");
		NextStep();
	}

	public void OnProcessorEnd(Consumable consumable)
	{
		Debug.LogError(consumable.transform.Path() + " : Processor ended!");
		NextStep();
	}


	public int currentTutorial = -1;
	public int stepCount = 0;

	public abstract void NextStep();

	// total of 5 steps
	public void ConsumerFinish(int startCount)
	{
		if( stepCount == startCount )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 1 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 2 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 3 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 4 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS! Let's try that again with the next customer");
			arrow.Hide();
		}
	}

	// 7 steps in total (2 + 5)
	public void SingleOrderFull(int startCount, GameObject producer )
	{
		if( stepCount == startCount  )
		{
			Debug.Log ("TAP " + producer.name);
			arrow.Show( producer );
			producer.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			Debug.Log ("TAP CUSTOMER");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
			producer.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
		}
		else
		{
			ConsumerFinish( startCount + 2 );
		}
	}

	// 8 steps in total
	public void DoubleOrderFull(int startCount, GameObject producer1, GameObject producer2)
	{
		if( stepCount == startCount )
		{
			Debug.Log ("This guy wants 2 sandwiches: tap both after another and bring them to him");
			arrow.Show( producer1 );
			producer1.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			Debug.Log ("TAP BURGER LONG");
			producer1.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer2 );
			producer2.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 2 )
		{
			// sandwich tapped
			Debug.Log ("TAP CUSTOMER");
			producer2.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;

			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else
		{
			ConsumerFinish( startCount + 3 );
		}

		/*
		else if( stepCount == startCount + 3 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 4 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 5 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 6 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 7 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
		*/
	}

	// 10 steps in total
	public void SingleProcessorOrderFull(int startCount, GameObject producer, GameObject processor)
	{
		if( stepCount == startCount )
		{
			Debug.Log ("This guy wants 1 processed food");
			arrow.Show( producer );
			producer.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			Debug.Log ("TAP processor");
			producer.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( processor );
			processor.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 2 )
		{
			// sandwich tapped
			Debug.Log ("Wait for processor to be finished");
			arrow.Hide();
			
			processor.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			processor.GetComponent<ConsumableProcessor>().onProcessingEnd += OnProcessorEnd;
			/*
			producer1.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer2 );
			producer2.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
			*/
		}
		else if( stepCount == startCount + 3 )
		{
			// sandwich tapped
			Debug.Log ("TAP processor");
			processor.GetComponent<ConsumableProcessor>().onProcessingEnd -= OnProcessorEnd;
			
			arrow.Show( processor );
			processor.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 4 )
		{
			// sandwich tapped
			Debug.Log ("TAP CUSTOMER");
			processor.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else
		{
			ConsumerFinish( startCount + 5 );
		}

		/*
		else if( stepCount == startCount + 5 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 6 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 7 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 8 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 9 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
		*/
	}

	/*
	// 9 steps in total
	public void TripleOrderFull(int startCount, GameObject producer1, GameObject producer2, GameObject producer3)
	{
		if( stepCount == startCount )
		{
			Debug.Log ("This guy wants 3 things! tap first");
			arrow.Show( producer1 );
			producer1.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			Debug.Log ("TAP second");
			producer1.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer2 );
			producer2.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 2 )
		{
			// sandwich tapped
			Debug.Log ("TAP third");
			producer2.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer3 );
			producer3.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 3 )
		{
			// sandwich tapped
			Debug.Log ("TAP CUSTOMER");
			producer3.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
			
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		
		else if( stepCount == startCount + 4 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 5 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 6 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 7 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 8 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
	}
	*/

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
