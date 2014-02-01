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


	public int currentTutorial = -1;
	public int stepCount = 0;

	public abstract void NextStep();

	// 7 steps in total
	public void BasicProcess(int startCount, GameObject producer )
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
		else if( stepCount == startCount + 2 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 3 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 4 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 5 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 6 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS! Let's try that again with the next customer");
			arrow.Hide();
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
