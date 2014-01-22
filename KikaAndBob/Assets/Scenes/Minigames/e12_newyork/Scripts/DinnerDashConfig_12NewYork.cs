using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashConfig_12NewYork : IDinnerDashConfig 
{
	public ConsumableDefinition pastaBox = null;
	public ConsumableDefinition bread = null;
	public ConsumableDefinition orange = null;
	public ConsumableDefinition tomato = null;
	public ConsumableDefinition iceCream = null;
	public ConsumableDefinition macaroni = null;
	public ConsumableDefinition macaroniCheese = null;
	public ConsumableDefinition wine = null;

	// processors
	public GameObject Blender = null;
	public GameObject MacaroniPot = null;
	public GameObject MacaroniCheesePot = null;

	// producers
	public GameObject IceCreamMachine = null;
	public GameObject PastaBoxProducer = null;
	public GameObject BreadProducer = null;
	public GameObject OrangeProducer = null;
	public GameObject TomatoProducer = null;
	public GameObject MacaroniProducer = null;
	public GameObject WineProducer = null;

	protected void Awake()
	{
		if( Blender == null )
			Blender = GameObject.Find ("Juicer");

		if( MacaroniPot == null )
			MacaroniPot = GameObject.Find ("MacaroniPot");
		
		if( MacaroniCheesePot == null )
			MacaroniCheesePot = GameObject.Find ("MacaroniCheesePot");

		
		if( IceCreamMachine == null )
			IceCreamMachine = GameObject.Find ("IceCreamMachine");

		if( PastaBoxProducer == null )
			PastaBoxProducer = GameObject.Find ("Producers/PastaBox");
		
		if( BreadProducer == null )
			BreadProducer = GameObject.Find ("Producers/Bread");
		
		if( OrangeProducer == null )
			OrangeProducer = GameObject.Find ("Producers/Orange");
		
		if( TomatoProducer == null )
			TomatoProducer = GameObject.Find ("Producers/Tomato");
		
		if( MacaroniProducer == null )
			MacaroniProducer = GameObject.Find ("Producers/Macaroni");
		
		if( WineProducer == null )
			WineProducer = GameObject.Find ("Producers/Wine");
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
		// level 0 : only bob with the pastabox and bread
		DisableObjects( new GameObject[]{ MacaroniPot, WineProducer, MacaroniCheesePot, Blender, IceCreamMachine, OrangeProducer, TomatoProducer, MacaroniProducer } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();



		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(pastaBox) );
		orders.Add( CreateOrder(bread) );
		orders.Add( CreateOrder(pastaBox, bread) );

		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}

	public void Level1()
	{
		// level 1 : introduce macaroni and wine
		DisableObjects( new GameObject[]{ Blender, MacaroniCheesePot, IceCreamMachine, OrangeProducer, TomatoProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(macaroni) );
		orders.Add( CreateOrder(pastaBox, wine) );
		orders.Add( CreateOrder(macaroni, bread) );
		orders.Add( CreateOrder(macaroni, wine) );
		
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
	}
	public void Level2()
	{
		// level 2 : introduce blender and macaroniCheese
		DisableObjects( new GameObject[]{ IceCreamMachine, TomatoProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(macaroniCheese) );
		orders.Add( CreateOrder(orange, pastaBox) );
		orders.Add( CreateOrder(macaroniCheese, wine) );
		orders.Add( CreateOrder(macaroni, orange) );
		orders.Add( CreateOrder(macaroniCheese) );
		
		
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
		orders.Add( CreateOrder(macaroniCheese) );
		orders.Add( CreateOrder(pastaBox, tomato) );
		orders.Add( CreateOrder(iceCream) );
		orders.Add( CreateOrder(orange, macaroni, iceCream) );
		
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

		ConsumableDefinition[] poolArray = new ConsumableDefinition[]{ pastaBox, bread, orange, tomato, iceCream, macaroni, macaroniCheese, wine };
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
