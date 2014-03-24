using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashTutorials_16Israel : IDinnerDashTutorial 
{
	DinnerDashConfig_16Israel config = null;

	public override void NextStep()
	{
		if( !DinnerDashManager.use.GameRunning )
			return;

		Debug.Log("NEXT STEP in tutorial starting now!");

		if( config == null )
		{
			config = (DinnerDashConfig_16Israel) IDinnerDashConfig.use;
		}

		if( currentTutorial == 0 )
		{
			Tutorial0_Step ();
		}
		else if( currentTutorial == 1 )
		{
			Tutorial1_Step ();
		}
		else if( currentTutorial == 2 )
		{
			Tutorial2_Step ();
		}
		else if( currentTutorial == 3 )
		{
			Tutorial3_Step (); 
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

		Debug.Log ("Tutorial0 step " + stepCount);

		if( stepCount == 1 )
		{
			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}
		else if( stepCount < 9 )
		{
			SingleOrderFull( 2, "e16_israel.tutorial.1.bread", config.BreadProducer );
		}
		else if( stepCount < 16 )
		{
			SingleOrderFull( 9, "e16_israel.tutorial.1.milk", config.MilkProducer );
		}
		else if( stepCount < 24 )
		{
			DoubleOrderFull( 16, "e16_israel.tutorial.1.doubleOrder", config.MilkProducer, config.BreadProducer );
		}

	}

	public void Tutorial1_Step()
	{
		++stepCount;
		
		Debug.Log ("Tutorial1 step " + stepCount);
		
		if( stepCount == 1 )
		{
			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}
		else if( stepCount < 12 )
		{
			SingleProcessorOrderFull( 2, "e16_israel.tutorial.2.macaroni" , config.MacaroniProducer, config.MacaroniPot );
		} 

		if( DinnerDashManager.use.consumerManager.currentOrderIndex == 3 )
		{
			DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		}
	}

	protected bool dumpFoodShown = false;

	public void Tutorial2_Step()
	{
		++stepCount;
		
		Debug.Log ("Tutorial2 step " + stepCount);
		
		if( stepCount == 1 )
		{
			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}
		else if( stepCount < 12 )
		{
			SingleProcessorOrderFull( 2, "e16_israel.tutorial.3.meat", config.MeatProducer, config.MeatPan );
		} 

		if( DinnerDashManager.use.consumerManager.currentOrderIndex == 3 )
		{
			DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
			// TODO: if excessive food: dishwasher, penalty

			if( !dumpFoodShown )
			{
				dumpFoodShown = true;
				DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.TopLeft, LugusResources.use.Localized.GetText("dinerdash.tutorial.dumpFood") ).Show (10.0f);
			}
		}
	}

	protected bool streakShown = false;
	public void Tutorial3_Step()
	{
		++stepCount;
		
		Debug.Log ("Tutorial3 step " + stepCount + " // orderindex : " + DinnerDashManager.use.consumerManager.currentOrderIndex);
		
		if( stepCount == 1 )
		{
			// TODO: if user becomes angry = less money
			// TODO: if similar actions in a row : bonus! 

			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}

		if( stepCount == 3 )
		{
			DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.TopLeft, LugusResources.use.Localized.GetText("dinerdash.tutorial.angry") ).Show (10.0f);
		}

		/*
		if( stepCount == 11 )
		{
			if( !streakShown ) 
			{
				streakShown = true;
				DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.TopLeft, LugusResources.use.Localized.GetText("dinerdash.tutorial.streak") ).Show (10.0f);
			}
		} 
		*/
	}
}
