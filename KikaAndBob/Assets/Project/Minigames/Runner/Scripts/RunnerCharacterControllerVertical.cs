using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCharacterControllerVertical : LugusSingletonExisting<RunnerCharacterControllerVertical>, RunnerCharacterController
{
	public enum SpeedType
	{
		NONE = -1,

		SLOW = 1, // modifierPercentage = 0
		NORMAL = 2, // modifierPercentage = 0.5f
		FAST = 3 // modifierPercentage = 1.0f
	}

	public int direction = -1; // -1 is down, 1 is up
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public DataRange speedModifiers = new DataRange(0.5f, 1.5f);
	public float timeToMaxSpeed = 60.0f;
	public float horizontalSpeed = 4.0f;

	[HideInInspector]
	public float speedPercentage = 0.0f;
	[HideInInspector]
	public float speedModifierPercentage = 0.5f;

	protected float startTime = -1.0f;

	public delegate void OnHit(RunnerPickup pickup);
	public OnHit onHit;
	
	public delegate void OnSpeedTypeChange(SpeedType oldType, SpeedType newType);
	public OnSpeedTypeChange onSpeedTypeChange;

	public SpeedType currentSpeedType = SpeedType.NORMAL;


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


		if( left )
		{ 
			this.rigidbody2D.velocity += Vector2.right * -1.0f * horizontalSpeed * speedModifier; 
		}
		
		if( right )
		{
			this.rigidbody2D.velocity += Vector2.right  * horizontalSpeed * speedModifier; 
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
			targetType = SpeedType.SLOW;
		}
		else if( LugusInput.use.Key (KeyCode.DownArrow) )
		{
			targetType = SpeedType.FAST;
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

		if( currentSpeedType == SpeedType.NORMAL )
			speedModifierPercentage = 0.5f;
		else if( currentSpeedType == SpeedType.SLOW )
			speedModifierPercentage = 0.0f;
		else if( currentSpeedType == SpeedType.FAST )
			speedModifierPercentage = 1.0f;


		if( onSpeedTypeChange != null )
			onSpeedTypeChange(oldType, type);
	}
}
