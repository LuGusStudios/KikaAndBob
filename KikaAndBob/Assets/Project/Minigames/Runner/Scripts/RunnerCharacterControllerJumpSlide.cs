using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RunnerCharacterControllerJumpSlide : LugusSingletonExisting<RunnerCharacterControllerJumpSlide>, IRunnerCharacterController, IRunnerCharacterController_JumpSlide
{
	//public float speed = 13.0f;
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public DataRange SpeedRange(){ return speedRange; }
	public Vector2 Velocity(){ return rigidbody2D.velocity; }
	public float timeToMaxSpeed = 60.0f;
	public float jumpForce = 30.0f; 

	public string jumpSound = "Jump01";
	protected AudioClip jumpClip = null;

	public string runningSound = "WalkLoop01"; 
	protected ILugusAudioTrack runningTrack = null;

	public string slideSound = "Slide01";
	protected ILugusAudioTrack slidingTrack = null;

	protected RunnerCharacterShadow shadow = null;
	protected Vector3 originalShadowScale = Vector3.one;

	// speedRange.from is speedScale 1 (normal speed)
	// if higher or lower, this returns a modifier (typically in [0,2]) to indicate the relative speed to the normal speed
	// especially handy in things like ParallaxMover
	public Vector3 SpeedScale()
	{
		Vector3 modifier = Vector3.one;

		modifier = modifier.x ( Mathf.Abs ( Velocity().x / SpeedRange().from ) );
		//modifier = modifier.y ( Mathf.Abs ( character.Velocity().y / character.SpeedRange().from ) );

		return modifier;
	}

	[HideInInspector] 
	public float speedPercentage = 0.0f;

	protected float startTime = -1.0f;

	public Transform groundCheck = null;
	
	public event KikaAndBob.Runner.OnHit onHit;
	public event KikaAndBob.Runner.OnJump onJump;
	public event KikaAndBob.Runner.OnSlide onSlide;


	public void OnPickupHit(RunnerPickup pickup)
	{
		RunnerScoreManager.use.ProcessPickup(pickup);

		if( onHit != null )
			onHit( pickup );
	}


	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( groundCheck == null )
		{
			groundCheck = transform.FindChild("GroundCheck");
		}

		if( groundCheck == null )
		{
			Debug.LogError(name + " : no GroundCheck found!");
		}

	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		startTime = Time.time;


		if( shadow == null )
		{
			shadow = GameObject.FindObjectOfType<RunnerCharacterShadow>();
		}
		
		if( shadow == null )
		{
			Debug.LogError(transform.Path () + " : No shadow found!"); 
		}
		else
		{
			originalShadowScale = shadow.originalScale; 
		}
		
		jumpClip = LugusResources.use.Shared.GetAudio( jumpSound );
		runningTrack = LugusAudio.use.SFX ().GetTrack();
		runningTrack.Play( LugusResources.use.Shared.GetAudio( runningSound ), new LugusAudioTrackSettings().Loop(true) );


		slidingTrack = LugusAudio.use.SFX ().GetTrack();
		slidingTrack.Play( LugusResources.use.Shared.GetAudio( slideSound ), new LugusAudioTrackSettings().Loop(true).Volume(0.05f) );
		slidingTrack.Pause();
	}

	public void OnDisable()
	{
		if( runningTrack != null )
		{
			runningTrack.Pause();
		}
		if( slidingTrack != null )
		{
			slidingTrack.Pause();
		}
	}

	public void OnEnable()
	{
		if( runningTrack != null )
		{
			runningTrack.Play();
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
	
	protected void FixedUpdate ()  
	{
		if( !this.enabled )
			return;

		//this.rigidbody2D.AddForce(Vector2.right * speed * 10 * Time.fixedDeltaTime);

		//this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( speed );



		//rigidbody2D.AddForce(Vector2.right * speed * 10);

		float timeDiff = Time.time - startTime;
		if( timeDiff > timeToMaxSpeed )
		{
			speedPercentage = 1.0f;
		}
		else
		{
			speedPercentage = timeDiff / timeToMaxSpeed;
		}
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		//if(Mathf.Abs(rigidbody2D.velocity.x) > speed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2(speedRange.ValueFromPercentage(speedPercentage), rigidbody2D.velocity.y);

		// Debug.LogWarning( rigidbody2D.velocity ); 


		if( triggerJump )
		{
			//this.rigidbody2D.AddForce(Vector2.up * speed * Time.fixedDeltaTime);
			this.rigidbody2D.velocity += Vector2.up * jumpForce; 

			triggerJump = false; 
		}


		//transform.position = transform.position.xAdd( speed * Time.deltaTime );

		//Transform camTrans = LugusCamera.game.transform;
		//camTrans.position = camTrans.position.x ( Mathf.Lerp(camTrans.position.x, this.transform.position.x, /*Time.fixedDeltaTime * speed*/ 0.5f) );

	}

	protected bool triggerJump = false;
	protected int jumpFrame = -1;
	public bool jumping = false; 

	protected float slideStartTime = -1.0f;
	public bool sliding = false;

	public bool Grounded
	{
		get
		{
			// first try: low vertical velocity means we're grounded
			// this doens't work, because vertical velocity changes sign at height of the jump, so grounded is true on the top of the jump as well (not correct!)
			//return (this.rigidbody2D.velocity.y < 1.0f) && (this.rigidbody2D.velocity.y >= -0.1f);

			// second try: put a transform slightly below the bottom of the character and raycast to see if it hits ground
			// seems to work, but the ground needs to be assigned a layer... which is non-ideal...
			return Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")); 

			//return true;
		}
	}

	/*
	public bool AlmostGrounded
	{
		get
		{
			return Physics2D.Linecast(transform.position, groundCheck.position + new Vector3(0.0f, -0.5f, 0.0f), 1 << LayerMask.NameToLayer("Ground")); 
		}
	}
	*/

	protected void CheckJump()
	{
		// both space and mouse button 1 (or single touch) work
		if( (LugusInput.use.KeyDown (KeyCode.Space) || LugusInput.use.down || LugusInput.use.KeyDown(KeyCode.UpArrow)) && this.Grounded )
		{
			triggerJump = true;
			jumping = true;
			jumpFrame = Time.frameCount;

			if( onJump != null )
				onJump(true);

			if( jumpClip != null )
				LugusAudio.use.SFX().Play( jumpClip );

			if( runningTrack != null )
				runningTrack.Pause();
		}
		else if( jumping && this.Grounded && (jumpFrame + 5 < Time.frameCount) ) // at least 5 frames after starting jump
		{
			jumping = false;

			if( onJump != null )
				onJump(false); 
			
			if( runningTrack != null )
				runningTrack.Play();

			CheckSlide(true);
		}
	}

	public void EnlargeShadow()
	{		
		shadow.originalScale = originalShadowScale.xMul (2.5f);
		shadow.xOffset = -0.35f;
	}

	protected void CheckSlide(bool checkKeyHold = false)
	{
		bool extra = false;
		if( checkKeyHold ) // make it possible to slide directly after jump touchdown by holding down while in mid-air
		{
			extra = LugusInput.use.Key(KeyCode.DownArrow) || Input.GetMouseButton(1);
			//Debug.LogError("Checking keyhold slide " + extra);
		}

		if( (LugusInput.use.KeyDown(KeyCode.DownArrow) || Input.GetMouseButtonDown(1) || extra) && this.Grounded)
		{
			//Debug.LogError(Time.frameCount + " SLIDING EXTRA? " + extra);

			sliding = true;
			slideStartTime = Time.time;
			
			EnlargeShadow();

			BoxCollider2D topCollider = GetComponent<BoxCollider2D>();
			if( topCollider != null )
			{
				topCollider.enabled = false;
			}
			else
			{
				Debug.LogError(name + " : Could not disable boxCollider while sliding...");
			}

			
			if( runningTrack != null )
				runningTrack.Pause();
			
			if( slidingTrack != null )
				slidingTrack.Play();

			if( onSlide != null )
				onSlide(true);
		}

		if( sliding && (LugusInput.use.KeyUp(KeyCode.DownArrow) ||  Input.GetMouseButtonUp(1) || (Time.time - slideStartTime > 1.5f)) )
		{
			//Debug.LogError(Time.frameCount + " SLIDING Stopped ");

			sliding = false;
			shadow.originalScale = originalShadowScale;
			shadow.xOffset = 0.0f;

			BoxCollider2D topCollider = GetComponent<BoxCollider2D>();
			if( topCollider != null )
			{
				topCollider.enabled = true;
			}

			
			if( runningTrack != null )
				runningTrack.Play();
			
			if( slidingTrack != null )
				slidingTrack.Pause();


			if( onSlide != null )
				onSlide(false);
		}
	}

	public void Update()
	{
		CheckJump();
		CheckSlide();

		//Debug.Log ("VELOCITY " + this.rigidbody2D.velocity);
   
	}
}
