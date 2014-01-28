using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashConfig_16Israel : IDinnerDashConfig 
{
	public ConsumableDefinition bread = null;
	public ConsumableDefinition pumpkin = null;
	public ConsumableDefinition meat2 = null;
	public ConsumableDefinition meat = null;
	public ConsumableDefinition cola = null;
	public ConsumableDefinition wine = null;
	public ConsumableDefinition pastry = null;
	public ConsumableDefinition iceCream = null;
	public ConsumableDefinition macaroni = null;
	public ConsumableDefinition cheese = null;
	public ConsumableDefinition milk = null;

	// processors
	public GameObject MacaroniPot = null;

	// producers
	public GameObject IceCreamMachine = null;
	public GameObject PastryProducer = null;
	public GameObject PumpkinProducer = null;
	public GameObject BreadProducer = null;
	public GameObject MeatProducer = null;
	public GameObject Meat2Producer = null;
	public GameObject MacaroniProducer = null;
	public GameObject WineProducer = null;
	public GameObject ColaProducer = null;
	public GameObject CheeseProducer = null;
	public GameObject MilkProducer = null;

	protected void Awake()
	{
		
		if( MacaroniPot == null )
			MacaroniPot = GameObject.Find ("MacaroniPot");

		if( PastryProducer == null )
			PastryProducer = GameObject.Find ("Producers/Pastry");

		if( IceCreamMachine == null )
			IceCreamMachine = GameObject.Find ("IceCreamMachine");
		
		if( PumpkinProducer == null )
			PumpkinProducer = GameObject.Find ("Producers/Pumpkin");
		
		if( BreadProducer == null )
			BreadProducer = GameObject.Find ("Producers/Bread");
		
		
		if( MeatProducer == null )
			MeatProducer = GameObject.Find ("Producers/Meat");
		
		if( Meat2Producer == null )
			Meat2Producer = GameObject.Find ("Producers/Meat2");
		
		if( MacaroniProducer == null )
			MacaroniProducer = GameObject.Find ("Producers/Macaroni");
		
		if( WineProducer == null )
			WineProducer = GameObject.Find ("Producers/Wine");
		
		if( ColaProducer == null )
			ColaProducer = GameObject.Find ("Producers/Cola");

		if( CheeseProducer == null )
			CheeseProducer = GameObject.Find ("Producers/Cheese");
		
		if( MilkProducer == null )
			MilkProducer = GameObject.Find ("Producers/Milk");

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
		// level 0 : only bob with the pumpkin
		DisableObjects( new GameObject[]{ MacaroniPot, WineProducer, IceCreamMachine, MilkProducer, ColaProducer, MacaroniProducer } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();



		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(pumpkin) );
		orders.Add( CreateOrder(bread) );
		orders.Add( CreateOrder(pumpkin, bread) );

		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}

	public void Level1()
	{
		// level 1 : introduce macaroni and wine
		DisableObjects( new GameObject[]{ IceCreamMachine } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(macaroni) );
		orders.Add( CreateOrder(pumpkin, wine) );
		orders.Add( CreateOrder(macaroni, bread) );
		orders.Add( CreateOrder(macaroni, wine) );
		
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}
	public void Level2()
	{
		// level 2 : introduce blender and macaroni
		DisableObjects( new GameObject[]{ IceCreamMachine } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(cola, pumpkin) );
		orders.Add( CreateOrder(macaroni, wine) );
		orders.Add( CreateOrder(macaroni, milk) );
		orders.Add( CreateOrder(macaroni) );
		
		
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
		
		orders.Add( CreateOrder(macaroni, wine, iceCream) );
		orders.Add( CreateOrder(macaroni) );
		orders.Add( CreateOrder(pumpkin, milk) );
		orders.Add( CreateOrder(iceCream) );
		orders.Add( CreateOrder(cola, macaroni, iceCream) );
		
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

		ConsumableDefinition[] poolArray = new ConsumableDefinition[]{ pumpkin, bread, milk, cheese, iceCream, pastry, macaroni, wine, cola, meat, meat2 };
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
