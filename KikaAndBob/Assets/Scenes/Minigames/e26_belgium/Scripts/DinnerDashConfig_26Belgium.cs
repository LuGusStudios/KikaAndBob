using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashConfig_26Belgium : IDinnerDashConfig 
{
	public ConsumableDefinition burger = null;
	public ConsumableDefinition burgerLong = null;
	public ConsumableDefinition orange = null;
	public ConsumableDefinition tomato = null;
	public ConsumableDefinition iceCream = null;
	public ConsumableDefinition fries = null;
	public ConsumableDefinition friesKetchup = null;
	public ConsumableDefinition sodaBlue = null;
	public ConsumableDefinition sodaGreen = null;
	public ConsumableDefinition sodaRed = null;
	public ConsumableDefinition gurkin = null;

	// processors
	public GameObject Blender = null;
	public GameObject Fryer = null;
	public GameObject FryerKetchup = null;

	// producers
	public GameObject IceCreamMachine = null;
	public GameObject BurgerLongProducer = null;
	public GameObject BurgerProducer = null;
	public GameObject OrangeProducer = null;
	public GameObject TomatoProducer = null;
	public GameObject PotatoProducer = null;
	public GameObject SodaBlueProducer = null;
	public GameObject SodaRedProducer = null;
	public GameObject SodaGreenProducer = null;
	public GameObject GurkinProducer = null;

	protected void Awake()
	{
		if( Blender == null )
			Blender = GameObject.Find ("Juicer");

		if( Fryer == null )
			Fryer = GameObject.Find ("Fryer");
		
		if( FryerKetchup == null )
			FryerKetchup = GameObject.Find ("FryerKetchup");

		
		if( IceCreamMachine == null )
			IceCreamMachine = GameObject.Find ("IceCreamMachine");

		if( OrangeProducer == null )
			OrangeProducer = GameObject.Find ("Producers/Orange");
		
		if( TomatoProducer == null )
			TomatoProducer = GameObject.Find ("Producers/Tomato");


		if( BurgerLongProducer == null )
			BurgerLongProducer = GameObject.Find ("Producers/BurgerLong");
		
		if( BurgerProducer == null )
			BurgerProducer = GameObject.Find ("Producers/Burger");

		
		if( PotatoProducer == null )
			PotatoProducer = GameObject.Find ("Producers/Potatoes");
		
		if( SodaBlueProducer == null )
			SodaBlueProducer = GameObject.Find ("Producers/SodaBlue");
		
		if( SodaRedProducer == null )
			SodaRedProducer = GameObject.Find ("Producers/SodaRed");
		
		if( SodaGreenProducer == null )
			SodaGreenProducer = GameObject.Find ("Producers/SodaGreen");
		
		if( GurkinProducer == null )
			GurkinProducer = GameObject.Find ("Producers/Gurkin");
	}

	// Use this for initialization
	protected void Start () 
	{
		LugusResources.use.ChangeLanguage("nl");
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


		// level 0 : only bob with the 2 burgers
		DisableObjects( new GameObject[]{ Fryer, FryerKetchup, SodaBlueProducer, SodaRedProducer, SodaGreenProducer, Blender, IceCreamMachine, OrangeProducer, TomatoProducer, PotatoProducer, GurkinProducer } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);



		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(burger) );
		orders.Add( CreateOrder(burgerLong) );
		orders.Add( CreateOrder(burgerLong, burger) );

		
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

		// level 1 : introduce ketchup fries and soda red/green
		DisableObjects( new GameObject[]{ Blender, Fryer, SodaBlueProducer, IceCreamMachine, OrangeProducer, TomatoProducer, GurkinProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(friesKetchup) );
		orders.Add( CreateOrder(friesKetchup, burger) );
		orders.Add( CreateOrder(burgerLong, sodaRed) );
		orders.Add( CreateOrder(friesKetchup, sodaGreen) );
		
		
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

		// level 2 : introduce blender and mayo fries
		DisableObjects( new GameObject[]{ IceCreamMachine, GurkinProducer, TomatoProducer } );
		
		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);
		


		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(orange) );
		orders.Add( CreateOrder(orange, burger) );
		orders.Add( CreateOrder(fries, sodaBlue) );
		orders.Add( CreateOrder(friesKetchup, orange) );
		orders.Add( CreateOrder(fries) );
		
		
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
		
		orders.Add( CreateOrder(friesKetchup, sodaBlue, iceCream) );
		orders.Add( CreateOrder(fries) );
		orders.Add( CreateOrder(burgerLong, tomato) );
		orders.Add( CreateOrder(gurkin) );
		orders.Add( CreateOrder(orange, friesKetchup, iceCream) );
		
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

		ConsumableDefinition[] poolArray = new ConsumableDefinition[]{ burger, burgerLong, sodaBlue, sodaRed, sodaGreen, orange, tomato, iceCream, fries, friesKetchup, gurkin };
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
