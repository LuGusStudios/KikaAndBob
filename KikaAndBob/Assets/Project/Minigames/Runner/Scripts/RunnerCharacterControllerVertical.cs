using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCharacterControllerVertical : LugusSingletonExisting<RunnerCharacterControllerVertical>, RunnerCharacterController
{
	public int direction = -1; // -1 is down, 1 is up
	public DataRange speedRange = new DataRange(13.0f, 26.0f);
	public float timeToMaxSpeed = 60.0f;
	public float horizontalSpeed = 4.0f;

	[HideInInspector]
	public float speedPercentage = 0.0f;

	protected float startTime = -1.0f;

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
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		//if(Mathf.Abs(rigidbody2D.velocity.x) > speed)
			// ... set the player's velocity to the maxSpeed in the x axis.
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, speedRange.ValueFromPercentage(speedPercentage) * direction );

		// Debug.LogWarning( rigidbody2D.velocity ); 


		if( left )
		{ 
			this.rigidbody2D.velocity += Vector2.right * -1.0f * horizontalSpeed; 
		}
		
		if( right )
		{
			this.rigidbody2D.velocity += Vector2.right  * horizontalSpeed; 
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
	}
}
