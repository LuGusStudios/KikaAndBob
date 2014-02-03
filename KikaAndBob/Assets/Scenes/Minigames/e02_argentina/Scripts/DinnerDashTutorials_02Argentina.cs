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

		if( DinnerDashManager.use.consumerManager.currentOrderIndex == 3 )
		{
			DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		}
	}

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
			SingleProcessorOrderFull( 2, config.OrangeProducer, config.Blender );
		} 
		
		if( DinnerDashManager.use.consumerManager.currentOrderIndex == 3 )
		{
			DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
			// TODO: if excessive food: dishwasher, penalty
		}
	}

	public void Tutorial3_Step()
	{
		++stepCount;
		
		Debug.LogError ("Tutorial3 step " + stepCount);
		
		if( stepCount == 1 )
		{
			// TODO: if user becomes angry = less money
			// TODO: if similar actions in a row : bonus! 

			// first step before beginning : CTOR
			DinnerDashManager.use.consumerManager.onConsumerStateChange += OnConsumerStateChanged;
		}
	}
}
