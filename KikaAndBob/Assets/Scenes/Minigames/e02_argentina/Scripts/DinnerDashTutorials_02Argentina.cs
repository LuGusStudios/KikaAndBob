using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashTutorials_02Argentina : IDinnerDashTutorial 
{
	DinnerDashConfig_02Argentina config = null;

	public override void NextStep()
	{
		if( !DinnerDashManager.use.GameRunning )
			return;

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
			SingleOrderFull( 2, "e02_argentina.tutorial.1.burger", config.BurgerProducer );
		}
		else if( stepCount < 16 )
		{
			SingleOrderFull( 9, "e02_argentina.tutorial.1.burgerLong", config.BurgerLongProducer );
		}
		else if( stepCount < 24 )
		{
			DoubleOrderFull( 16, "e02_argentina.tutorial.1.doubleOrder", config.BurgerProducer, config.BurgerLongProducer );
		}

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
			SingleProcessorOrderFull( 2, "e02_argentina.tutorial.2.stew" , config.VegetableProducer, config.StewPot );
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
		
		Debug.LogError ("Tutorial2 step " + stepCount);
		
		if( stepCount == 1 )
		{
			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}
		else if( stepCount < 12 )
		{
			SingleProcessorOrderFull( 2, "e02_argentina.tutorial.3.juice", config.OrangeProducer, config.Blender );
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
		
		Debug.LogError ("Tutorial3 step " + stepCount + " // orderindex : " + DinnerDashManager.use.consumerManager.currentOrderIndex);
		
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
