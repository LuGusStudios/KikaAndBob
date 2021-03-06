﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCameraPuller : LugusSingletonExisting<RunnerCameraPuller>
{
	public int direction = -1; // -1 is down, 1 is up
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public DataRange SpeedRange(){ return speedRange; }
	public Vector2 Velocity(){ return rigidbody2D.velocity; }
	//public DataRange speedModifiers = new DataRange(0.5f, 1.5f);
	public float timeToMaxSpeed = 60.0f;
	public float horizontalSpeed = 4.0f; 

	public Vector3 SpeedScale()
	{
		Vector3 modifier = Vector3.one;
		
		//modifier = modifier.x ( Mathf.Abs ( character.Velocity().x / character.SpeedRange().from ) );
		modifier = modifier.y ( Mathf.Abs ( Velocity().y / SpeedRange().from ) );
		
		return modifier;
	}

	public float currentSpeed
	{
		get
		{
			//float speedModifier = speedModifiers.ValueFromPercentage( speedModifierPercentage );
			return speedRange.ValueFromPercentage(speedPercentage);// * speedModifier;
		}
	}

	[HideInInspector]
	public float speedPercentage = 0.0f;
	//[HideInInspector] 
	//public float speedModifierPercentage = 0.5f;
	
	protected float startTime = -1.0f;

	protected void FixedUpdate ()  
	{
		FirstUpdate();

		float timeDiff = Time.time - startTime;
		if( timeDiff > timeToMaxSpeed )
		{
			speedPercentage = 1.0f;
		}
		else
		{
			speedPercentage = timeDiff / timeToMaxSpeed;
		}
		
		//float speedModifier = speedModifiers.ValueFromPercentage( speedModifierPercentage );
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		//if(Mathf.Abs(rigidbody2D.velocity.x) > speed)
		// ... set the player's velocity to the maxSpeed in the x axis.
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, speedRange.ValueFromPercentage(speedPercentage) /** speedModifier*/ * direction );

		// TODO: remove this. just needed for debug OnGUI updates in IRunnerConfig
		if( RunnerManager.use.GameRunning )
			RunnerCharacterControllerClimbing.use.speedPercentage = this.speedPercentage;
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		startTime = Time.time;
	}
	
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

	protected bool firstUpdateDone = false;
	protected void FirstUpdate()
	{
		if( firstUpdateDone )
			return;
		
		if( !RunnerManager.use.GameRunning )
			return;

		firstUpdateDone = true;

		// get data from the characterController to allow easy-to-use IRunnerConfig
		this.speedRange = RunnerCharacterControllerClimbing.use.speedRange;
		this.timeToMaxSpeed = RunnerCharacterControllerClimbing.use.timeToMaxSpeed;
	}
}
