using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashTutorials_02Argentina : IDinnerDashTutorial 
{
	DinnerDashConfig_02Argentina config = null;

	public override void NextStep()
	{
		if( config == null )
		{
			config = (DinnerDashConfig_02Argentina) IDinnerDashConfig.use;
		}

		if( currentTutorial == 0 )
		{
			Tutorial0_Step ();
		}
		else if( currentTutorial == 1 )
		{
			Tutorial1_Step ();
		}
	}

	public void Tutorial0_Step()
	{
		/*
	- tap sandwich (bob will make more)
		- tap customer
			- tap customer
				- tap dishwasher
				- tap customer (payment)
		*/

		++stepCount;

		Debug.LogError ("Tutorial0 step " + stepCount);

		if( stepCount == 1 )
		{
			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}
		else if( stepCount < 9 )
		{
			SingleOrderFull( 2, config.BurgerProducer );
		}
		else if( stepCount < 16 )
		{
			SingleOrderFull( 9, config.BurgerLongProducer );
		}
		else if( stepCount < 24 )
		{
			DoubleOrderFull( 16, config.BurgerProducer, config.BurgerLongProducer );
		}

		/*
		else if( stepCount == 9 )
		{
			Debug.Log ("TAP Long burger!");
			arrow.Show( config.BurgerLongProducer );
			config.BurgerLongProducer.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == 10 )
		{
			// sandwich tapped
			Debug.Log ("TAP CUSTOMER");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
			config.BurgerLongProducer.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
		}
		else if( stepCount == 11 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == 12 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == 13 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == 14 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == 15 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
		*/

		/*
		else if( stepCount == 16 )
		{
			Debug.Log ("This guy wants 2 sandwiches: tap both after another and bring them to him");
			arrow.Show( config.BurgerProducer );
			config.BurgerProducer.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == 17 )
		{
			// sandwich tapped
			Debug.Log ("TAP BURGER LONG");
			config.BurgerProducer.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
			
			arrow.Show( config.BurgerLongProducer );
			config.BurgerLongProducer.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == 18 )
		{
			// sandwich tapped
			Debug.Log ("TAP CUSTOMER");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
			config.BurgerLongProducer.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
		}

		else if( stepCount == 19 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == 20 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == 21 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == 22 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == 23 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
		*/

	}

	public void Tutorial1_Step()
	{
		++stepCount;
		
		Debug.LogError ("Tutorial1 step " + stepCount);
		
		if( stepCount == 1 )
		{
			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}
		else if( stepCount < 12 )
		{
			SingleProcessorOrderFull( 2, config.VegetableProducer, config.StewPot );
		} 

		/*
		else if( stepCount < 17 )
		{
			SingleProcessorOrderFull( 9, config.VegetableProducer, config.StewPot, config.BurgerProducer );
		}
		*/

		if( DinnerDashManager.use.consumerManager.currentOrderIndex == 3 )
		{
			DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		}

		/*
		else if( stepCount < 17 )
		{
			SingleOrderFull( 9, config.BurgerLongProducer );
		}
		else if( stepCount < 24 )
		{
			DoubleOrderFull( 16, config.BurgerProducer, config.BurgerLongProducer );
		}
		*/
	}
}
