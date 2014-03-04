using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KikaAndBob.Runner;

public class RunnerCharacterControllerClimbing : LugusSingletonExisting<RunnerCharacterControllerClimbing>, IRunnerCharacterController, IRunnerCharacterController_FasterSlower
{


	public int direction = -1; // -1 is down, 1 is up
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public DataRange SpeedRange(){ return speedRange; }
	public Vector2 Velocity(){ return rigidbody2D.velocity; }
	public DataRange speedModifiers = new DataRange(0.5f, 1.5f);
	public float timeToMaxSpeed = 60.0f;
	public float horizontalSpeed = 4.0f; 

	protected Vector3 originalScale = Vector3.zero;

	public DataRange xEdges = new DataRange(-6.192653f, 8.181341f);

	// speedRange.from is speedScale 1 (normal speed)
	// if higher or lower, this returns a modifier (typically in [0,2]) to indicate the relative speed to the normal speed
	// especially handy in things like ParallaxMover
	public Vector3 SpeedScale()
	{
		/*
		Vector3 modifier = Vector3.one;
		
		//modifier = modifier.x ( Mathf.Abs ( character.Velocity().x / character.SpeedRange().from ) );
		modifier = modifier.y ( Mathf.Abs ( Velocity().y / SpeedRange().from ) );
		
		return modifier;
		*/

		return RunnerCameraPuller.use.SpeedScale(); // so ParallaxMover gets the right data
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

		if( !pickup.positive )
		{
			LugusCoroutines.use.StartRoutine( OnHitRoutine() );
		}

		if( onHit != null )
			onHit( pickup );
	}

	protected IEnumerator OnHitRoutine()
	{
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
			this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( speed * horizontalSpeedBoost );
			this.transform.localScale = originalScale.xMul(-1.0f);
		}
		else if( left )
		{
			this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( -1.0f * speed * horizontalSpeedBoost );
			this.transform.localScale = originalScale; 
		}

		horizontalSpeedBoost = 1.0f;

		// make sure the character doesn't leave the gameplay area
		// normally, we would do this with colliders
		// but sometimes we disable the collider of the character... so we have to resort to these foul methods
		if( this.transform.position.x < xEdges.from )
		{
			this.transform.position = this.transform.position.x ( xEdges.from );
		}
		else if( this.transform.position.x > xEdges.to )
		{
			this.transform.position = this.transform.position.x ( xEdges.to );
		}


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

		this.collider2D.enabled = false;

		/*
		// disable all blocking stones (under halfway?)
		RunnerBlockingStone[] stones = LayerManager.use.groundLayer.transform.GetComponentsInChildren<RunnerBlockingStone>();
		foreach( RunnerBlockingStone stone in stones )
		{
			stone.enabled = false;

			if( stone.collider != null )
				stone.collider.enabled = false;

			if( stone.collider2D != null )
				stone.collider2D.enabled = false;
		}
		*/


		yield return new WaitForSeconds( 1.5f );

		
		this.collider2D.enabled = true;

		// we can't just re-enable the character
		// chances are we would land on top of another blocking collider and we would be stuck
		// so: check around for other colliders that are too close and just disable them 
		// this is not 100% "physically correct" but makes possible gameplay bugs a lot less likely

		Collider2D[] stucks = Physics2D.OverlapCircleAll( this.transform.position.v2 (), 4 ); // character is about 2 units wide, so use double that as radius
		foreach( Collider2D stuck in stucks )
		{
			if( stuck == this.collider2D )
				continue;

			//Debug.LogError("We got stuck in " + stuck.transform.Path() +  " so we disabled it");

			// keep the positive pickups (feathers) enabled
			// changed: makes no difference... our collider was disabled for 1.5s anyway, so we didn't pick it up anyhow... hmz
			//RunnerPickup pickup = stuck.GetComponent<RunnerPickup>();
			//if( pickup == null || pickup.positive == false )
				stuck.enabled = false;

			// disable the feathers -> we cannot pick them up anyway, would only confuse the player
			RunnerPickup pickup = stuck.GetComponent<RunnerPickup>();
			if( pickup != null && pickup.positive  )
			{

			}

			// DEBUG, TODO: remove!
			//if( stuck.GetComponent<SpriteRenderer>() )
			//{
			//	stuck.GetComponent<SpriteRenderer>().enabled = false;
			//}
		}

		downDisabled = false;
		upDisabled = false;

		checkBottomBoundary = true;
	}


	public bool left = false;
	public bool right = false;
	public bool up = false;
	public bool down = false;

	public bool leftDisabled = false;
	public bool rightDisabled = false;
	public bool upDisabled = false;
	public bool downDisabled = false;

	public float horizontalSpeedBoost = 1.0f;
	public float leftStartTime = -1.0f; 
	public float rightStartTime = -1.0f; 

	public void Update()
	{
		if( !leftDisabled )
		{
			if( LugusInput.use.Key(KeyCode.LeftArrow) )
			{
				if( !left && (Time.time - leftStartTime < 0.3f) )
				{
					// dash to the left
					Debug.LogWarning("DASH LEFT");
					horizontalSpeedBoost = 500.0f;
				}

				leftStartTime = Time.time;

				left = true;
			}
			else
			{
				//leftStartTime = -1.0f;

				left = false;
			}
		}
		
		if(  !rightDisabled )
		{
			if( LugusInput.use.Key(KeyCode.RightArrow) )
			{
				if( !right && (Time.time - rightStartTime < 0.3f) )
				{
					// dash to the right
					Debug.LogWarning("DASH RIGHT");
					horizontalSpeedBoost = 500.0f;
				}
				
				rightStartTime = Time.time;

				right = true;
			}
			else
			{
				right = false;
			}
		}

		if( !upDisabled )
		{
			if( LugusInput.use.Key(KeyCode.UpArrow) )
			{
				up = true;
			}
			else
			{
				up = false;
			}
		}

		if( !downDisabled )
		{
			if( LugusInput.use.Key(KeyCode.DownArrow) )
			{
				down = true;
			}
			else
			{
				down = false;
			}
		}

		CheckSpeedType();
		CheckBoundaries();
	}


	protected void CheckSpeedType()
	{
		SpeedType targetType = SpeedType.STILL;

		if( left || right || up || down )
		{
			targetType = SpeedType.NORMAL;
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

		if( onSpeedTypeChange != null )
			onSpeedTypeChange(oldType, type);
	}

}
