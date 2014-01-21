﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashConfig_12NewYork : IDinnerDashConfig 
{
	public ConsumableDefinition burger = null;
	public ConsumableDefinition burgerLong = null;
	public ConsumableDefinition orange = null;
	public ConsumableDefinition tomato = null;
	public ConsumableDefinition iceCream = null;
	public ConsumableDefinition stew = null;

	// processors
	public GameObject Blender = null;
	public GameObject StewPot = null;

	// producers
	public GameObject IceCreamMachine = null;
	public GameObject BurgerProducer = null;
	public GameObject BurgerLongProducer = null;
	public GameObject OrangeProducer = null;
	public GameObject TomatoProducer = null;
	public GameObject VegetableProducer = null;

	protected void Awake()
	{
		if( Blender == null )
			Blender = GameObject.Find ("Juicer");

		if( StewPot == null )
			StewPot = GameObject.Find ("StewPot");
		
		if( IceCreamMachine == null )
			IceCreamMachine = GameObject.Find ("IceCreamMachine");

		if( BurgerProducer == null )
			BurgerProducer = GameObject.Find ("Producers/Burger");
		
		if( BurgerLongProducer == null )
			BurgerLongProducer = GameObject.Find ("Producers/BurgerLong");
		
		if( OrangeProducer == null )
			OrangeProducer = GameObject.Find ("Producers/Orange");
		
		if( TomatoProducer == null )
			TomatoProducer = GameObject.Find ("Producers/Tomato");
		
		if( VegetableProducer == null )
			VegetableProducer = GameObject.Find ("Producers/Stew");
	}

	// Use this for initialization
	protected void Start () 
	{
		LoadLevel( DinnerDashCrossSceneInfo.use.levelToLoad );
	}

	public void LoadLevel(int index)
	{
		if( index == 0 )
			Level0 ();
		else if( index == 1 )
			Level1 ();
		else if( index == 2 )
			Level2(); 
		else if (index == 3 )
			Level3();
		else if (index == 4 )
			Level4();
	}

	protected void DisableObjects(GameObject[] objects)
	{
		foreach( GameObject obj in objects )
		{
			if( obj == null )
			{
				Debug.LogError(name + " : DisableObjects : one of the objects was null!");
			}
			else
			{
				//Debug.LogError(name + " : De-activating " + obj.name);
				obj.SetActive(false);
			}
		}
	}

	public void Level0()
	{
		// level 0 : only bob with the 2 burgers
		DisableObjects( new GameObject[]{ StewPot, Blender, IceCreamMachine, OrangeProducer, TomatoProducer, VegetableProducer } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();



		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(burger) );
		orders.Add( CreateOrder(burgerLong) );
		orders.Add( CreateOrder(burger, burgerLong) );

		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}

	public void Level1()
	{
		// level 1 : introduce stew
		DisableObjects( new GameObject[]{ Blender, IceCreamMachine, OrangeProducer, TomatoProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(stew) );
		orders.Add( CreateOrder(burger) );
		orders.Add( CreateOrder(stew, burgerLong) );
		orders.Add( CreateOrder(stew) );
		
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}
	public void Level2()
	{
		// level 2 : introduce blender
		DisableObjects( new GameObject[]{ IceCreamMachine, TomatoProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(orange) );
		orders.Add( CreateOrder(orange, burgerLong) );
		orders.Add( CreateOrder(stew, burger) );
		orders.Add( CreateOrder(stew, orange) );
		
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 3;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}

	public void Level3()
	{
		// level 3 : everything
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(stew, burgerLong, iceCream) );
		orders.Add( CreateOrder(stew) );
		orders.Add( CreateOrder(burger, orange) );
		orders.Add( CreateOrder(iceCream) );
		orders.Add( CreateOrder(tomato, burgerLong) );
		
		DinnerDashManager.use.consumerManager.RandomOrders = true;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 4;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(4.0f, 10.0f);
	}

	public void Level4()
	{
		// level 4 : everything endless random
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		ConsumableDefinition[] poolArray = new ConsumableDefinition[]{ burger, burgerLong, orange, tomato, iceCream, stew };
		List<ConsumableDefinition> pool = new List<ConsumableDefinition>();
		pool.AddRange( poolArray );

		for( int i = 0; i < 100; ++i )
		{
			orders.Add ( RandomOrder (pool, Random.Range(1,4)) );
		}
		
		DinnerDashManager.use.consumerManager.RandomOrders = false; // orders are already random
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 4;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 5.0f);
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

	
	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		GUILayout.BeginArea( new Rect(0, Screen.height - 150, 200, 150) );
		for (int i = 0; i < 5; i++) 
		{
			if (GUILayout.Button("Start Level " + i))
			{
				DinnerDashCrossSceneInfo.use.levelToLoad = i;
				LugusCoroutines.use.StopAllRoutines();
				Application.LoadLevel( Application.loadedLevelName );
			}
		}
		GUILayout.EndArea();
	}
}
