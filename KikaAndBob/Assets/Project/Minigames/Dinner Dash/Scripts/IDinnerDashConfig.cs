﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IDinnerDashConfig : LugusSingletonRuntime<IDinnerDashConfig> 
{
	public virtual void LoadLevel(int index)
	{
		Debug.LogError(transform.Path () + " : LoadLevel not implemented!");
	}


	// ideally this should be implemented in all the configs
	// BUT this is a late in the project (unlikely levels will be added) and they all happen to have five levels anyway
	public virtual bool IsLastLevel(int currentLevel)
	{
		if (currentLevel >= 4)
			return true;
		else
			return false;
	}

	public void SetupHUDForTutorial()//int targetScore)
	{
		// top left is simple timer
		// top right is x/y for money

		HUDManager.use.DisableAll();

		HUDManager.use.PauseButton.gameObject.SetActive(true);

		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.CounterLargeLeft1.formatting = HUDCounter.Formatting.TimeS;
		HUDManager.use.CounterLargeLeft1.StartTimer();
		
		HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Money;
		HUDManager.use.CounterLargeRight1.suffix = "/" + DinnerDashManager.use.targetMoneyScore;
		HUDManager.use.CounterLargeRight1.SetValue(0);

		
		//HUDManager.use.Counter1.gameObject.SetActive(true);
		//HUDManager.use.Counter2.gameObject.SetActive(true);
		//HUDManager.use.Counter3.gameObject.SetActive(true); 
		
		//HUDManager.use.Counter1.SetValue(10);
		//HUDManager.use.Counter2.StartTimer(HUDCounter.Formatting.TimeMS);
		//HUDManager.use.Counter3.StartTimer();
	}
	
	public void SetupHUDForGame()
	{
		HUDManager.use.DisableAll();
		
		HUDManager.use.PauseButton.gameObject.SetActive(true);
		HUDManager.use.RepositionPauseButton( KikaAndBob.ScreenAnchor.TopRight, KikaAndBob.ScreenAnchor.TopLeft );
		
		HUDManager.use.ProgressBarLeft.gameObject.SetActive(true); 
		HUDManager.use.ProgressBarLeft.SetTimer(DinnerDashManager.use.timeout);
		
		//HUDManager.use.Counter1.gameObject.SetActive(true);
		//HUDManager.use.Counter2.gameObject.SetActive(true);
		//HUDManager.use.Counter3.gameObject.SetActive(true);
		
		HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Money;
		HUDManager.use.CounterLargeRight1.SetValue(0);
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


	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();

		output.Add ( one );

		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		
		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two, ConsumableDefinition three)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		output.Add ( three );
		
		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two, ConsumableDefinition three, ConsumableDefinition four)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		output.Add ( three );
		output.Add ( four );
		
		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two, ConsumableDefinition three, ConsumableDefinition four, ConsumableDefinition five)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		output.Add ( three );
		output.Add ( four );
		output.Add ( five ); 
		
		return output; 
	}

	public List<ConsumableDefinition> RandomOrder( List<ConsumableDefinition> pool, int orderLength = 3 )
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();

		orderLength = Mathf.Min( pool.Count, orderLength );

		while( output.Count < orderLength )
		{
			ConsumableDefinition chosen = null;

			do
			{
				chosen = pool[ Random.Range(0, pool.Count) ];
			}
			while( output.Contains(chosen) );

			output.Add ( chosen );
		}


		return output;
	}
}
