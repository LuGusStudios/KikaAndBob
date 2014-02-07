using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IDinnerDashTutorial : MonoBehaviour 
{
	public IndicatorArrow arrow = null;

	public void SetupLocal()
	{
		LugusResources.use.ChangeLanguage("nl");

		// assign variables that have to do with this class only
		if( arrow == null )
		{
			arrow = GameObject.FindObjectOfType<IndicatorArrow>();
		}
		if( arrow == null )
		{
			Debug.LogError(transform.Path () + " : No IndicatorArrow found in the scene!");
		}
	}

	public void OnConsumerStateChanged(ConsumableConsumer consumer, ConsumableConsumer.State oldState, ConsumableConsumer.State newState )
	{
		Debug.Log(consumer.transform.Path() + " : State changed! " + oldState + " // " + newState);
		if( newState == ConsumableConsumer.State.Ordered || 
		   newState == ConsumableConsumer.State.Eating || 
		   newState == ConsumableConsumer.State.Done ||
		   newState == ConsumableConsumer.State.Paying ||
		   newState == ConsumableConsumer.State.Leaving
		   )
		{
			NextStep();
		}
	}
	
	public void OnConsumableUsed(IConsumableUser subject)
	{
		Debug.Log(subject.transform.Path() + " : User used!");
		NextStep();
	}

	public void OnProcessorEnd(Consumable consumable)
	{
		Debug.Log(consumable.transform.Path() + " : Processor ended!");
		NextStep(); 
	}


	public int currentTutorial = -1;
	public int stepCount = 0;

	public abstract void NextStep();

	// total of 5 steps
	public void ConsumerFinish(int startCount, string textBase)
	{
		string keyEnd =  ".customer." + (stepCount - startCount + 1);
		string text = LugusResources.use.Localized.GetText( textBase + keyEnd, "dinerdash.tutorial" + keyEnd );

		if( stepCount == startCount )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();

			DialogueManager.use.CreateBox( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().transform, text, "IconTime01" ).Show ();
		}
		else if( stepCount == startCount + 1 )
		{
			// customer is done: need to pick up the dishes
			//Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );

			DialogueManager.use.CreateBox( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().transform, text, "mouse.left.click" ).Show ();
		}
		else if( stepCount == startCount + 2 )
		{
			// customer is done eating
			//Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );

			DialogueManager.use.CreateBox( GameObject.Find ("GarbageBin").transform, text, "mouse.left.click" ).Show ();
		}
		else if( stepCount == startCount + 3 )
		{
			// dishwasher tapped
			//Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
			
			DialogueManager.use.CreateBox( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().transform, text, "mouse.left.click" ).Show ();
		}
		else if( stepCount == startCount + 4 )
		{
			// dishwasher tapped
			//Debug.Log ("TUTORIAL SUCCESS! Let's try that again with the next customer //" + LugusResources.use.Localized.GetText( textBase + keyEnd, "global.tutorial" + keyEnd ) + " @ " + textBase + " // " + keyEnd);

			if( !string.IsNullOrEmpty(text) )
			{	
				//Debug.LogWarning ("Diplaying autohide popup for customer.5 " + keyEnd);
				DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.TopLeft, text ).Show (5.0f);
			}
			else
			{
				DialogueManager.use.HideAll();  
			}
			arrow.Hide();
		}
	}

	// 7 steps in total (2 + 5) 
	public void SingleOrderFull(int startCount, string textBase, GameObject producer )
	{
		if( stepCount == startCount  ) 
		{
			DialogueManager.use.CreateBox( producer.transform, LugusResources.use.Localized.GetText(textBase + ".1"), "mouse.left.click" ).Show ();

			arrow.Show( producer );
			producer.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			DialogueManager.use.CreateBox( producer.transform, LugusResources.use.Localized.GetText(textBase + ".2"), "mouse.left.click" ).Show ();

			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
			producer.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
		}
		else
		{
			ConsumerFinish( startCount + 2, textBase );
		}
	}

	// 8 steps in total
	public void DoubleOrderFull(int startCount, string textBase, GameObject producer1, GameObject producer2)
	{
		if( stepCount == startCount )
		{
			DialogueManager.use.CreateBox( producer1.transform, LugusResources.use.Localized.GetText(textBase + ".1"), "mouse.left.click" ).Show ();

			arrow.Show( producer1 );
			producer1.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			DialogueManager.use.CreateBox( producer2.transform, LugusResources.use.Localized.GetText(textBase + ".2"), "mouse.left.click" ).Show ();

			producer1.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer2 );
			producer2.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 2 )
		{
			// sandwich tapped
			DialogueManager.use.CreateBox( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().transform, LugusResources.use.Localized.GetText(textBase + ".3"), "mouse.left.click" ).Show ();

			producer2.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;

			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else
		{
			ConsumerFinish( startCount + 3, textBase );
		}

		/*
		else if( stepCount == startCount + 3 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 4 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 5 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 6 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 7 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
		*/
	}

	// 10 steps in total
	public void SingleProcessorOrderFull(int startCount, string textBase, GameObject producer, GameObject processor)
	{
		if( stepCount == startCount )
		{
			DialogueManager.use.CreateBox( producer.transform, LugusResources.use.Localized.GetText(textBase + ".1"), "mouse.left.click" ).Show ();

			arrow.Show( producer );
			producer.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			DialogueManager.use.CreateBox( processor.transform, LugusResources.use.Localized.GetText(textBase + ".2"), "mouse.left.click" ).Show ();

			producer.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( processor );
			processor.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 2 )
		{
			// wait for processor to be finished
			DialogueManager.use.CreateBox( processor.transform, LugusResources.use.Localized.GetText(textBase + ".3"), "IconTime01" ).Show ();

			arrow.Hide();
			
			processor.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			processor.GetComponent<ConsumableProcessor>().onProcessingEnd += OnProcessorEnd;
			/*
			producer1.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer2 );
			producer2.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
			*/
		}
		else if( stepCount == startCount + 3 )
		{
			// sandwich tapped
			//Debug.Log ("TAP processor");
			DialogueManager.use.CreateBox( processor.transform, LugusResources.use.Localized.GetText(textBase + ".4"), "mouse.left.click" ).Show ();

			processor.GetComponent<ConsumableProcessor>().onProcessingEnd -= OnProcessorEnd;
			
			arrow.Show( processor );
			processor.GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 4 )
		{
			// sandwich tapped
			//Debug.Log ("TAP CUSTOMER");
			DialogueManager.use.CreateBox( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().transform, LugusResources.use.Localized.GetText(textBase + ".5"), "mouse.left.click" ).Show ();

			processor.GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		} 
		else
		{
			ConsumerFinish( startCount + 5, textBase );
		}

		/*
		else if( stepCount == startCount + 5 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 6 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 7 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 8 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<IConsumableUser>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 9 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
		*/
	}

	/*
	// 9 steps in total
	public void TripleOrderFull(int startCount, GameObject producer1, GameObject producer2, GameObject producer3)
	{
		if( stepCount == startCount )
		{
			Debug.Log ("This guy wants 3 things! tap first");
			arrow.Show( producer1 );
			producer1.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 1 )
		{
			// sandwich tapped
			Debug.Log ("TAP second");
			producer1.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer2 );
			producer2.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 2 )
		{
			// sandwich tapped
			Debug.Log ("TAP third");
			producer2.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
			
			arrow.Show( producer3 );
			producer3.GetComponent<ConsumableProducer>().onUsed += OnConsumableUsed;
		}
		else if( stepCount == startCount + 3 )
		{
			// sandwich tapped
			Debug.Log ("TAP CUSTOMER");
			producer3.GetComponent<ConsumableProducer>().onUsed -= OnConsumableUsed;
			
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		
		else if( stepCount == startCount + 4 )
		{
			// customer tapped. Customer is now eating... wait for the customer to be Done
			arrow.Hide ();
			
		}
		else if( stepCount == startCount + 5 )
		{
			// customer is done: need to pick up the dishes
			Debug.Log ("TAP CUSTOMER for DISHES");
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 6 )
		{
			// customer is done eating
			Debug.Log ("TAP DISHWASHER");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed += OnConsumableUsed;
			arrow.Show( GameObject.Find ("GarbageBin") );
		}
		else if( stepCount == startCount + 7 )
		{
			// dishwasher tapped
			Debug.Log ("TAP CUSTOMER for PAYMENT");
			GameObject.Find ("GarbageBin").GetComponent<ConsumableRemover>().onUsed -= OnConsumableUsed;
			arrow.Show( DinnerDashManager.use.consumerManager.GetNextActiveConsumer().gameObject );
		}
		else if( stepCount == startCount + 8 )
		{
			// dishwasher tapped
			Debug.Log ("TUTORIAL SUCCESS!");
			arrow.Hide();
		}
	}
	*/

	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}
}
