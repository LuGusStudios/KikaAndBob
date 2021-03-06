﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KikaAndBob.Runner;

public class RunnerCharacterControllerFasterSlower : LugusSingletonExisting<RunnerCharacterControllerFasterSlower>, IRunnerCharacterController, IRunnerCharacterController_FasterSlower
{


	public int direction = -1; // -1 is down, 1 is up
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public DataRange SpeedRange(){ return speedRange; }
	public Vector2 Velocity(){ return rigidbody2D.velocity; }
	public DataRange speedModifiers = new DataRange(0.5f, 1.5f);
	public float timeToMaxSpeed = 60.0f;
	public float horizontalSpeed = 4.0f;

	protected Joystick joystick = null;

	// speedRange.from is speedScale 1 (normal speed)
	// if higher or lower, this returns a modifier (typically in [0,2]) to indicate the relative speed to the normal speed
	// especially handy in things like ParallaxMover
	public Vector3 SpeedScale()
	{
		Vector3 modifier = Vector3.one;
		
		//modifier = modifier.x ( Mathf.Abs ( character.Velocity().x / character.SpeedRange().from ) );
		modifier = modifier.y ( Mathf.Abs ( Velocity().y / SpeedRange().from ) );
		
		return modifier;
	}

	[HideInInspector]
	public float speedPercentage = 0.0f;
	[HideInInspector] 
	public float speedModifierPercentage = 0.5f;
	
	public float SpeedPercentage(){ return speedPercentage; }
	public float SpeedModifierPercentage(){ return speedModifierPercentage; }

	protected float startTime = -1.0f;

	public event KikaAndBob.Runner.OnHit onHit;
	public event KikaAndBob.Runner.OnSpeedTypeChange onSpeedTypeChange;


	public KikaAndBob.Runner.SpeedType currentSpeedType = KikaAndBob.Runner.SpeedType.NORMAL;


	public void OnPickupHit(RunnerPickup pickup)
	{
		RunnerScoreManager.use.ProcessPickup(pickup);

		if( onHit != null )
			onHit( pickup );
	}


	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		startTime = Time.time;

		if (joystick == null)
			joystick = GameObject.FindObjectOfType<Joystick>();

		if (joystick == null)
			Debug.LogError("RunnerCharacterControllerFasterSlower: Missing joystick.");
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal(); 
	}
	
	protected void FixedUpdate ()  
	{
		if( !this.enabled )
			return;

		float timeDiff = Time.time - startTime;
		if( timeDiff > timeToMaxSpeed )
		{
			speedPercentage = 1.0f;
		}
		else
		{
			speedPercentage = timeDiff / timeToMaxSpeed;
		}

		float speedModifier = speedModifiers.ValueFromPercentage( speedModifierPercentage );
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		//if(Mathf.Abs(rigidbody2D.velocity.x) > speed)
			// ... set the player's velocity to the maxSpeed in the x axis.
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, speedRange.ValueFromPercentage(speedPercentage) * speedModifier * direction );

		// Debug.LogWarning( rigidbody2D.velocity ); 

		if (left == true || right == true)
		{
			if( left )
			{ 
				//if( this.rigidbody2D.velocity.x > 0.0f )
				//	this.rigidbody2D.velocity = new Vector3(this.rigidbody2D.velocity.x / 10.0f, this.rigidbody2D.velocity.y);
	
				this.rigidbody2D.velocity += Vector2.right * -1.0f * horizontalSpeed * speedModifier; 
			}
			
			if( right )
			{
				//if( this.rigidbody2D.velocity.x < 0.0f )
				//	this.rigidbody2D.velocity = new Vector3(this.rigidbody2D.velocity.x / 10.0f, this.rigidbody2D.velocity.y);
	
				this.rigidbody2D.velocity += Vector2.right  * horizontalSpeed * speedModifier; 
			}
		}
		else
		{
			if (joystick != null)
				this.rigidbody2D.velocity += (Vector2.right  * horizontalSpeed * speedModifier) * (joystick.position.x * 0.375f); 
		}



			
	}

	public bool left = false;
	public bool right = false;

	public void Update()
	{
		if( LugusInput.use.Key(KeyCode.LeftArrow) )
		{
			left = true;
		}
		else
		{
			left = false;
		}
		
		if( LugusInput.use.Key(KeyCode.RightArrow) )
		{
			right = true;
		}
		else
		{
			right = false;
		}

		CheckSpeedType();
	}

	protected void CheckSpeedType()
	{
		SpeedType targetType = SpeedType.NORMAL;
		if( LugusInput.use.Key (KeyCode.UpArrow) )
		{
			if( this.direction < 0 )// going DOWN, up is slowing down
				targetType = SpeedType.SLOW;
			else
				targetType = SpeedType.FAST;
		}
		else if( LugusInput.use.Key (KeyCode.DownArrow) )
		{
			if( this.direction < 0 )// going DOWN, down is faster
				targetType = SpeedType.FAST;
			else
				targetType = SpeedType.SLOW;
		}
		else if (joystick != null && joystick.enabled)
		{
			if (joystick.position.y > 0)
			{
				if( this.direction < 0 )// going DOWN, up is slowing down
					targetType = SpeedType.SLOW;
				else
					targetType = SpeedType.FAST;
			}
			else if (joystick.position.y < 0)
			{
				if( this.direction < 0 )// going DOWN, down is faster
					targetType = SpeedType.FAST;
				else
					targetType = SpeedType.SLOW;
			}
		}


		// in mexico, we switch between animations
		// Going down SLOW sways to the left side, so we need to shift our collider that way too
		if( this.direction < 0 ) // bit of a hack. Should only be done for MEXICO!
		{
			BoxCollider2D box = ( (BoxCollider2D) this.collider2D);
			if( targetType == SpeedType.SLOW )
			{
				box.center = new Vector2( -0.1f, box.center.y);
			}
			else
			{
				box.center = new Vector2(0.1014484f, box.center.y);;
			}
		}


		if( currentSpeedType != targetType )
		{
			SetSpeedType( targetType );
		}
	}

	public void SetSpeedType(SpeedType type)
	{
		SpeedType oldType = currentSpeedType;
		currentSpeedType = type;

		if ( LugusInput.use.Key (KeyCode.UpArrow) || LugusInput.use.Key (KeyCode.DownArrow))
		{
			if( currentSpeedType == SpeedType.NORMAL )
				speedModifierPercentage = 0.5f;
			else if( currentSpeedType == SpeedType.SLOW )
				speedModifierPercentage = 0.0f;
			else if( currentSpeedType == SpeedType.FAST )
				speedModifierPercentage = 1.0f;
		}
		else if (joystick != null && joystick.enabled)
		{
			speedModifierPercentage = 1 - Mathf.Abs( ( joystick.position.y * 0.5f ) + 0.5f );
		}


		if( onSpeedTypeChange != null )
			onSpeedTypeChange(oldType, type);
	}
}
