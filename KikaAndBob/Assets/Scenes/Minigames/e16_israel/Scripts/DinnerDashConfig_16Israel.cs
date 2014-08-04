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
	public GameObject MeatPan = null;
	public GameObject SoupPot = null;

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
		
		if( MeatPan == null )
			MeatPan = GameObject.Find ("MeatPan");
		
		if( SoupPot == null )
			SoupPot = GameObject.Find ("SoupPot");

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
		//LugusResources.use.ChangeLanguage("nl");
	}

	public override void LoadLevel(int index)
	{
		index = index - 1; // index passed by menu is 1-based. Here we want 0-based
		
		Debug.Log("LOAD LEVEL diner dash " + index + " // " + DinnerDashCrossSceneInfo.use.levelToLoad);

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

		// level 0 : only bob with the pumpkin and bread
		DisableObjects( new GameObject[]{ IceCreamMachine, PastryProducer, MeatProducer, Meat2Producer, MacaroniProducer, WineProducer, ColaProducer, CheeseProducer, MacaroniPot, PumpkinProducer, SoupPot } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999); 


		PumpkinProducer.transform.position = new Vector3(9999.0f, 9999.0f, 9999.0f);


		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(bread) );
		orders.Add( CreateOrder(milk) );
		orders.Add( CreateOrder(milk, bread) );

		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 1;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f); 

		DinnerDashTutorials_16Israel tutorials = gameObject.AddComponent<DinnerDashTutorials_16Israel>(); 
		tutorials.currentTutorial = 0;
		tutorials.NextStep();
	}

	public void Level1()
	{
		DinnerDashManager.use.targetMoneyScore = 390;
		DinnerDashManager.use.timeout = -1.0f;
		SetupHUDForTutorial(); 

	
		// level 1 : just the dairy
		DisableObjects( new GameObject[]{ IceCreamMachine, PastryProducer, MeatProducer, Meat2Producer, WineProducer, ColaProducer } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();
		
		orders.Add( CreateOrder(macaroni) );
		orders.Add( CreateOrder(macaroni, bread) );
		orders.Add( CreateOrder(pumpkin, cheese) );
		orders.Add( CreateOrder(macaroni, milk) );
		orders.Add( CreateOrder(milk, cheese) );
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 1;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
		
		DinnerDashTutorials_16Israel tutorials = gameObject.AddComponent<DinnerDashTutorials_16Israel>(); 
		tutorials.currentTutorial = 1;
		tutorials.NextStep();
	}
	public void Level2()
	{
		DinnerDashManager.use.targetMoneyScore = 350;
		DinnerDashManager.use.timeout = -1.0f;
		SetupHUDForTutorial();

		// level 2 : just the meat and wine/cola
		DisableObjects( new GameObject[]{ IceCreamMachine, PastryProducer, MacaroniProducer, CheeseProducer, MilkProducer, MacaroniPot } );

		DinnerDashManager.use.consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
		DinnerDashManager.use.consumerManager.consumerWaitTimeBeforeAngry = new DataRange(9999,9999);
		
		
		
		// generate orders to be use by the customers in this game
		List< List<ConsumableDefinition> > orders = new List<List<ConsumableDefinition>>();

		orders.Add( CreateOrder(meat) );
		orders.Add( CreateOrder(meat, wine) );
		orders.Add( CreateOrder(meat2, cola) );
		orders.Add( CreateOrder(meat, pumpkin) ); 
		orders.Add( CreateOrder(meat2) );
		
		DinnerDashManager.use.consumerManager.RandomOrders = false;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 1;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(2.0f, 3.0f);
		
		DinnerDashTutorials_16Israel tutorials = gameObject.AddComponent<DinnerDashTutorials_16Israel>(); 
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
		orders.Add( CreateOrder(meat) );
		orders.Add( CreateOrder(bread, meat2) );
		orders.Add( CreateOrder(cheese) );
		orders.Add( CreateOrder(pumpkin, meat, pastry) );
		
		DinnerDashManager.use.consumerManager.RandomOrders = true;
		DinnerDashManager.use.consumerManager.orders = orders;
		DinnerDashManager.use.consumerManager.maxConcurrentConsumers = 2;
		
		DinnerDashManager.use.consumerManager.timeBetweenConsumers = new DataRange(4.0f, 10.0f);
		
		DinnerDashTutorials_16Israel tutorials = gameObject.AddComponent<DinnerDashTutorials_16Israel>(); 
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

				Resources.UnloadUnusedAssets();
				Application.LoadLevel( Application.loadedLevelName );
			}
		}
		GUILayout.EndArea();
	}
}
