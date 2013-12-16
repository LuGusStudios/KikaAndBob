using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashConfig_02Argentina : IDinnerDashConfig 
{
	public ConsumableDefinition burger = null;
	public ConsumableDefinition burgerLong = null;
	public ConsumableDefinition orange = null;
	public ConsumableDefinition tomato = null;
	public ConsumableDefinition iceCream = null;
	public ConsumableDefinition stew = null;


	// Use this for initialization
	void Start () 
	{
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();

		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(stew, burgerLong, iceCream) );
		orders.Add( CreateOrder(stew) );
		orders.Add( CreateOrder(burger, orange) );
		orders.Add( CreateOrder(iceCream) );
		orders.Add( CreateOrder(tomato, burgerLong) );
		//orders.Add( CreateOrder(burger, burger) ); // NOT GOOD: system cannot handle multiple consuambles of the same type in 1 order at this time

		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 4;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(4.0f, 10.0f);
	}

	public bool started = false;

	// Update is called once per frame
	void Update () 
	{
		if( !started )
		{
			started = true;
			DinnerDashManager.use.StartGame();
		}
	}
}
