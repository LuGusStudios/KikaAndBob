using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface RunnerCharacterController
{
	void OnPickupHit(RunnerPickup pickup);
}

public class RunnerCharacterControllerHorizontal : LugusSingletonExisting<RunnerCharacterControllerHorizontal>, RunnerCharacterController
{
	//public float speed = 13.0f;
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public float timeToMaxSpeed = 60.0f;
	public float jumpForce = 10.0f;

	[HideInInspector]
	public float speedPercentage = 0.0f;

	protected float startTime = -1.0f;

	public Transform groundCheck = null;

	public delegate void OnJump(bool start);
	public OnJump onJump;

	public delegate void OnHit(RunnerPickup pickup);
	public OnHit onHit;


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

	protected void CheckJump()
	{
		// both space and mouse button 1 (or single touch) work
		if( (LugusInput.use.KeyDown (KeyCode.Space) || LugusInput.use.down) && this.Grounded )
		{
			triggerJump = true;
			jumping = true;
			jumpFrame = Time.frameCount;
			
			if( onJump != null )
				onJump(true);
		}
		else if( jumping && this.Grounded && (jumpFrame + 5 < Time.frameCount) ) // at least 5 frames after starting jump
		{
			jumping = false;
			
			if( onJump != null )
				onJump(false); 
		}
	}

	public void Update()
	{
		CheckJump();
		
		//Debug.Log ("VELOCITY " + this.rigidbody2D.velocity);
   
	}
}
