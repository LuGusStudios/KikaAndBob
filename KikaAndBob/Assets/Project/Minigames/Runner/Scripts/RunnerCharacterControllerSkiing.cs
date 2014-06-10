using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KikaAndBob.Runner;

public class RunnerCharacterControllerSkiing : LugusSingletonExisting<RunnerCharacterControllerSkiing>, IRunnerCharacterController, IRunnerCharacterController_FasterSlower
{


	public int direction = 1; // -1 is down, 1 is up
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public DataRange SpeedRange(){ return speedRange; }
	public Vector2 Velocity(){ return rigidbody2D.velocity; }
	public DataRange speedModifiers = new DataRange(0.5f, 1.5f);
	public float timeToMaxSpeed = 60.0f;

	protected Vector3 originalScale = Vector3.zero;
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

		/*
		if( !pickup.positive )
		{
			LugusCoroutines.use.StartRoutine( OnHitRoutine() );
		}
		*/

		if( onHit != null )
			onHit( pickup );
	} 

	protected IEnumerator OnHitRoutine()
	{
		/*
		upDisabled = true;
		leftDisabled = true;
		rightDisabled = true;
		downDisabled = true;

		up = false;
		left = false;
		down = false;
		right = false;

		yield return new WaitForSeconds(0.3f);

		upDisabled = false;
		leftDisabled = false;
		downDisabled = false;
		rightDisabled = false;
		*/
		yield break;
	}


	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		originalScale = this.transform.localScale;
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		startTime = Time.time;

		if (joystick == null)
		{
			joystick = GameObject.FindObjectOfType<Joystick>();
		}

		if (joystick == null)
		{
			Debug.LogWarning("RunnerCharacterControllerSkiing: Missing joystick. Continuing without.");
		}
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal(); 
	}

	/*
	// http://answers.unity3d.com/questions/141775/limit-local-rotation.html
	protected float ClampAngle(float angle,  float min, float max) 
	{
		
		if (angle<90 || angle>270){       // if angle in the critic region...
			if (angle>180) angle -= 360;  // convert all angles to -180..+180
			if (max>180) max -= 360;
			if (min>180) min -= 360;
		}   
		angle = Mathf.Clamp(angle, min, max);
		if (angle<0) angle += 360;  // if angle negative, convert to 0..360

		return angle;
	}
	*/

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

		float rotationSpeed = 95.0f; // degrees / second
		float rotationClamp = 45.0f;


		float rotationAngle = 0.0f;
		bool checkJoystick = joystick != null && joystick.enabled && !joystick.IsInDirection(Joystick.JoystickDirection.None);

		if( left )
		{
			if (checkJoystick)
			{
				rotationAngle = Mathf.Abs(joystick.position.x) * rotationSpeed * Time.deltaTime;
			}
			else
				rotationAngle = rotationSpeed * Time.deltaTime;
		}
		else if( right )
		{
			if (checkJoystick)
			{
				rotationAngle = Mathf.Abs(joystick.position.x) * -rotationSpeed * Time.deltaTime;
			}
			else
				rotationAngle = -rotationSpeed * Time.deltaTime;
		}

		if( rotationAngle != 0.0f )
		{
			transform.Rotate( transform.forward, rotationAngle );

			// transform.Rotate creates angles that are always positive
			// so left is for ex. 45.0f, right is 315.0f, not -45.0f

			float angle = transform.eulerAngles.z;
			if( angle > 0 && angle < 180 )
			{
				if( angle > rotationClamp )
					angle = rotationClamp;
			}
			else if( angle > 0 && angle > 180 )
			{
				if( angle < (360 - rotationClamp) )
					angle = (360 - rotationClamp);
			}
			else
			{
				Debug.LogError( transform.Path () + "Invalid angle detected! Algorithm cannot deal with this! expects angles between 0 and 360. " + angle );
			}
			
			transform.eulerAngles = transform.eulerAngles.z( angle );

			//transform.eulerAngles = transform.eulerAngles.z( Mathf.Clamp(transform.eulerAngles.z, 45.0f, -45.0f) );
			/*
			if( transform.eulerAngles.z > 45.0f )
				transform.eulerAngles = transform.eulerAngles.z( 45.0f );
			else if( transform.eulerAngles.z < 315.0f )
				transform.eulerAngles = transform.eulerAngles.z( 315.0f );
			*/
			
			//transform.eulerAngles = transform.eulerAngles.z( ClampAngle(transform.eulerAngles.z, 45.0f, -45.0f) );
			
		}

		
		float speedModifier = speedModifiers.ValueFromPercentage( speedModifierPercentage );

		
		// If the player's horizontal velocity is greater than the maxSpeed...
		//if(Mathf.Abs(rigidbody2D.velocity.x) > speed)
		// ... set the player's velocity to the maxSpeed in the x axis.
		rigidbody2D.velocity = speedModifier * speedRange.ValueFromPercentage(speedPercentage) * transform.up; //new Vector2(speedRange.ValueFromPercentage(speedPercentage), rigidbody2D.velocity.y);
		 


		/*
		if( up )
		{
			this.rigidbody2D.velocity = new Vector2( 0.0f, horizontalSpeed );
		}
		else if( down )
		{
			this.rigidbody2D.velocity = new Vector2( 0.0f, -1.0f * horizontalSpeed );
		}
		else if( right )
		{
			this.rigidbody2D.velocity = new Vector2( horizontalSpeed, 0.0f );
		}
		else if( left )
		{
			this.rigidbody2D.velocity = new Vector2( -1.0f * horizontalSpeed, 0.0f );
		}
		else
		{
			this.rigidbody2D.velocity = new Vector2( 0.0f, 0.0f );
		}
		*/



		/*
		float speed = RunnerCameraPuller.use.currentSpeed + 1.0f; //* 1.2f; // 20% faster than camera
		
		this.rigidbody2D.velocity = new Vector2( 0.0f, 0.0f );
		if( up )
		{
			this.rigidbody2D.velocity = this.rigidbody2D.velocity.y( speed * 1.2f );
			this.transform.localScale = originalScale;
		}
		else if( down )
		{
			this.rigidbody2D.velocity = this.rigidbody2D.velocity.y( -1.0f * speed * 0.8f );
			this.transform.localScale = originalScale.xMul(-1.0f);
		}

		if( right )
		{
			this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( speed );
			this.transform.localScale = originalScale.xMul(-1.0f);
		}
		else if( left )
		{
			this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( -1.0f * speed );
			this.transform.localScale = originalScale;
		}
		*/


		/*
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
			//if( this.rigidbody2D.velocity.x > 0.0f )
			//	this.rigidbody2D.velocity = new Vector3(this.rigidbody2D.velocity.x / 10.0f, this.rigidbody2D.velocity.y);

			this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( -1.0f * horizontalSpeed * speedModifier ); 
		}
		
		if( right )
		{
			//if( this.rigidbody2D.velocity.x < 0.0f )
			//	this.rigidbody2D.velocity = new Vector3(this.rigidbody2D.velocity.x / 10.0f, this.rigidbody2D.velocity.y);

			this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( horizontalSpeed * speedModifier ); 
		}
		*/
	}

	/*
	protected bool checkBottomBoundary = true;
	protected void CheckBoundaries()
	{
		Vector2 screenPos = LugusCamera.game.WorldToScreenPoint( this.transform.position );

		if( checkBottomBoundary && screenPos.y < -50 )
		{
			LugusCoroutines.use.StartRoutine( BottomBoundaryCross() );
		}

		if( screenPos.y >= (Screen.height - 100) )
		{
			this.transform.position = this.transform.position.y( LugusCamera.game.ScreenToWorldPoint( screenPos.y( (float) Screen.height - 101) ).y );
		}
	}

	protected IEnumerator BottomBoundaryCross()
	{
		checkBottomBoundary = false;

		RunnerManager.use.AddTime( 10.0f );
		ScoreVisualizer.Score(KikaAndBob.CommodityType.Time, 10.0f).Time (1.0f).Position( this.transform.position ).Audio("Collide01").Color(Color.red).Execute();


		if( onHit != null )
			onHit( null );

		downDisabled = true;
		upDisabled = true;
		down = false;
		up = true;

		yield return new WaitForSeconds( 2.0f );

		downDisabled = false;
		upDisabled = false;

		checkBottomBoundary = true;
	}
	*/

	/*
	public bool left = false;
	public bool right = false;

	public bool up = false;
	public bool down = false;

	public bool leftDisabled = false;
	public bool rightDisabled = false;
	public bool upDisabled = false;
	public bool downDisabled = false;
	*/

	public bool left = false;
	public bool right = false;
	
	public void Update()
	{
		if( LugusInput.use.Key(KeyCode.LeftArrow) || (joystick != null && joystick.position.x < 0) )
		{
			left = true;
		}
		else
		{
			left = false;
		}
		
		if( LugusInput.use.Key(KeyCode.RightArrow) || (joystick != null && joystick.position.x > 0) )
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
		if( LugusInput.use.Key (KeyCode.UpArrow)  || (joystick != null && joystick.position.y > 0) )
		{
			if( this.direction < 0 )// going DOWN, up is slowing down
				targetType = SpeedType.SLOW;
			else
				targetType = SpeedType.FAST;
		}
		else if( LugusInput.use.Key (KeyCode.DownArrow)  || (joystick != null && joystick.position.y < 0) )
		{
			if( this.direction < 0 )// going DOWN, down is faster
				targetType = SpeedType.FAST;
			else
				targetType = SpeedType.SLOW;
		}
		
		/*
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
		*/
		
		
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
