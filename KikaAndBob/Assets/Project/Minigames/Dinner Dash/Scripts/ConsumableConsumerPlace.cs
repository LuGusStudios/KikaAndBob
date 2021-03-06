﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsumableConsumerPlace : MonoBehaviour 
{
	// if null: no consumer is assigned to this place
	// if not null: this is the consumer that was assigned to this place. 
	// Note: this doesn't necessarily mean he's already sitting here graphically!
	public ConsumableConsumer consumer = null;

	// location where we will put the consumable
	// usually: placement of the plate with food on the table
	public Transform consumableLocation = null;

	public ConsumableListVisualizer orderVisualizer = null;
	public HappinessVisualizer happinessVisualizer = null;
	
	public void SetupLocal()
	{
		if( consumableLocation == null )
			consumableLocation = transform.FindChild("ConsumableLocation");
		
		if( consumableLocation == null )
			Debug.LogError (name + " : ConsumableLocation not found for this ConsumerPlace");

		if( orderVisualizer == null )
			orderVisualizer = transform.GetComponentInChildren<ConsumableListVisualizer>();

		if( orderVisualizer == null )
			Debug.LogError (name + " : OrderVisualizer not found for this ConsumerPlace");
		
		if( happinessVisualizer == null )
			happinessVisualizer = transform.GetComponentInChildren<HappinessVisualizer>();
		
		if( happinessVisualizer == null )
			Debug.LogError (name + " : happinessVisualizer not found for this ConsumerPlace");

	}
	
	public void SetupGlobal()
	{
		orderVisualizer.Hide ();
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
