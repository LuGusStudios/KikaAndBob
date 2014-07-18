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
	public ConsumableDefinition pizza = null;

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
	public GameObject PizzaProducer = null;

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
		
		if( PizzaProducer == null )
			PizzaProducer = GameObject.Find ("Producers/Pizza");
	}

	// Use this for initialization
	protected void Start () 
	{
		//LugusResources.use.ChangeLanguage("nl");
	}

	public override void LoadLevel(int index)
	{
		index = index - 1; // index passed by menu is 1-based. Here we want 0-based
		
		Debug.LogError("LOAD LEVEL diner dash " + index + " // " + DinnerDashCrossSceneInfo.use.levelToLoad);
		
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

	public void Level0()
	{
		DinnerDashManager.use.targetMoneyScore = 190;
		DinnerDashManager.use.timeout = -1.0f;
		SetupHUDForTutorial(); 


		// level 0 : only bob with the pastabox and bread
		DisableObjects( new GameObject[]{ MacaroniPot, PizzaProducer, WineProducer, MacaroniCheesePot, Blender, IceCreamMachine, OrangeProducer, TomatoProducer, MacaroniProducer } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);



		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(pastaBox) );
		orders.Add( CreateOrder(bread) );
		orders.Add( CreateOrder(pastaBox, bread) );

		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 1;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(4.0f, 6.0f);

		
		DinnerDashTutorials_12NewYork tutorials = gameObject.AddComponent<DinnerDashTutorials_12NewYork>(); 
		tutorials.currentTutorial = 0;
		tutorials.NextStep();
	}

	public void Level1()
	{
		DinnerDashManager.use.targetMoneyScore = 250;
		DinnerDashManager.use.timeout = -1.0f;
		SetupHUDForTutorial(); 

		// level 1 : introduce macaroni and wine
		DisableObjects( new GameObject[]{ Blender, PizzaProducer, MacaroniCheesePot, IceCreamMachine, OrangeProducer, TomatoProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(macaroni) );
		orders.Add( CreateOrder(macaroni, bread) );
		orders.Add( CreateOrder(pastaBox, wine) );
		orders.Add( CreateOrder(macaroni, wine) );
		
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 1;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);

		
		DinnerDashTutorials_12NewYork tutorials = gameObject.AddComponent<DinnerDashTutorials_12NewYork>(); 
		tutorials.currentTutorial = 1;
		tutorials.NextStep();
	}
	public void Level2()
	{
		DinnerDashManager.use.targetMoneyScore = 350;
		DinnerDashManager.use.timeout = -1.0f;
		SetupHUDForTutorial();

		// level 2 : introduce blender and macaroniCheese
		DisableObjects( new GameObject[]{ IceCreamMachine, PizzaProducer, TomatoProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);
		


		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(orange) );
		orders.Add( CreateOrder(orange, pastaBox) );
		orders.Add( CreateOrder(macaroniCheese, wine) );
		orders.Add( CreateOrder(macaroni, orange) );
		orders.Add( CreateOrder(macaroniCheese) );
		
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 1;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
		
		
		DinnerDashTutorials_12NewYork tutorials = gameObject.AddComponent<DinnerDashTutorials_12NewYork>(); 
		tutorials.currentTutorial = 2;
		tutorials.NextStep();
	}

	public void Level3()
	{
		DinnerDashManager.use.targetMoneyScore = 380;  
		DinnerDashManager.use.timeout = -1.0f;
		SetupHUDForTutorial();

		// level 3 : everything
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(10.0f, 20.0f);
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(macaroni, wine, iceCream) );
		orders.Add( CreateOrder(macaroniCheese) );
		orders.Add( CreateOrder(pastaBox, tomato) );
		orders.Add( CreateOrder(pizza) );
		orders.Add( CreateOrder(orange, macaroni, iceCream) );
		
		DinnerDashManager.use.consumerManager.RandomOrders = true;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(4.0f, 10.0f);
		
		DinnerDashTutorials_12NewYork tutorials = gameObject.AddComponent<DinnerDashTutorials_12NewYork>();
		tutorials.currentTutorial = 3;
		tutorials.NextStep();


	}

	public void Level4()
	{
		DinnerDashManager.use.targetMoneyScore = -1.0f;
		DinnerDashManager.use.timeout = 300.0f; 

		SetupHUDForGame();

		// level 4 : everything endless random
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(6.0f, 8.0f);
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		ConsumableDefinition[] poolArray = new ConsumableDefinition[]{ pastaBox, pizza, bread, orange, tomato, iceCream, macaroni, macaroniCheese, wine };
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
	
	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;
		
		GUILayout.BeginArea( new Rect(0, Screen.height - 150, 200, 150) );
		for (int i = 0; i < 5; i++) 
		{
			if (GUILayout.Button("Start Level " + i))
			{
				DinnerDashCrossSceneInfo.use.levelToLoad = (i + 1);
				LugusCoroutines.use.StopAllRoutines();
				Application.LoadLevel( Application.loadedLevelName );
			}
		}
		GUILayout.EndArea();
	}
}
