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
		DisableObjects( new GameObject[]{ IceCreamMachine, PastryProducer, MeatProducer, Meat2Producer, MacaroniProducer, WineProducer, ColaProducer, CheeseProducer, MilkProducer, MacaroniPot } );

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
		// level 1 : just the dairy
		DisableObjects( new GameObject[]{ IceCreamMachine, PastryProducer, MeatProducer, Meat2Producer, WineProducer, ColaProducer } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(cheese) );
		orders.Add( CreateOrder(macaroni, milk) );
		orders.Add( CreateOrder(pumpkin, cheese) );
		orders.Add( CreateOrder(macaroni, bread) );
		orders.Add( CreateOrder(milk) );

		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}
	public void Level2()
	{
		// level 2 : just the meat
		DisableObjects( new GameObject[]{ IceCreamMachine, PastryProducer, MacaroniProducer, CheeseProducer, MilkProducer, MacaroniPot } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(meat, cola) );
		orders.Add( CreateOrder(meat2, wine) );
		orders.Add( CreateOrder(pumpkin, meat) );
		orders.Add( CreateOrder(meat2, bread, wine) );
		orders.Add( CreateOrder(meat, pumpkin, cola) );
		
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}

	public void Level3()
	{
		// level 3 : everything
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		 
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(macaroni, wine, iceCream) );
		orders.Add( CreateOrder(meat, cola, bread) );
		orders.Add( CreateOrder(iceCream, milk) );
		orders.Add( CreateOrder(pumpkin, pastry) );
		orders.Add( CreateOrder(cola, macaroni) );
		
		DinnerDashManager.use.consumerManager.RandomOrders = true;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;

		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(4.0f, 10.0f);
	}

	public void Level4()
	{
		// level 4 : everything endless random
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		ConsumableDefinition[] commonArray = new ConsumableDefinition[]{ pumpkin, bread, pastry, wine, cola };
		//List<ConsumableDefinition> common = new List<ConsumableDefinition>();
		//common.AddRange( commonArray );
		
		ConsumableDefinition[] meatArray = new ConsumableDefinition[]{ meat, meat2 };
		List<ConsumableDefinition> meats = new List<ConsumableDefinition>();
		meats.AddRange( meatArray );
		meats.AddRange( commonArray );
		
		ConsumableDefinition[] dairyArray = new ConsumableDefinition[]{ milk, cheese, macaroni, iceCream };
		List<ConsumableDefinition> dairy = new List<ConsumableDefinition>();
		dairy.AddRange( dairyArray );
		dairy.AddRange( commonArray );


		for( int i = 0; i < 100; ++i ) 
		{
			if( Random.value < 0.5f )
			{
				orders.Add ( RandomOrder (meats, Random.Range(1,4)) );
			}
			else
			{
				orders.Add ( RandomOrder (dairy, Random.Range(1,4)) );
			}
		}
		
		DinnerDashManager.use.consumerManager.RandomOrders = false; // orders are already random
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
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
