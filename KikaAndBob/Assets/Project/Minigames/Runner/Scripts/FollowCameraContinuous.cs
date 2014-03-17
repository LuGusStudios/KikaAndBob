using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowCameraContinuous : MonoBehaviour 
{
	public GameObject character = null;
	public float speed = 100.0f;

	public float xOffset = 0.0f;
	public float yOffset = 0.0f;

	public bool followX = true;
	public bool followY = false;

	public bool tracking = true;
	
	public void SetupLocal()
	{
		if( character == null )
		{
			character = GameObject.Find("Character");//.GetComponent<CharacterController>();
		}
		
		if( character == null )
		{
			Debug.LogError(name + " : no CharacterController found!");
		}
	}
	
	public void SetupGlobal()
	{
		if( tracking )
		{
			if( followX )
				transform.position = transform.position.x (character.transform.position.x + xOffset);
			
			if( followY )
				transform.position = transform.position.y( character.transform.position.y + yOffset);
		}
		else
		{
			DisableParallax();
		}
	}

	public void DisableParallax()
	{
		tracking = false;

		ParallaxMover[] movers = GameObject.FindObjectsOfType<ParallaxMover>();
		foreach( ParallaxMover mover in movers )
		{
			mover.move = false;
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
	
	protected void Update()
	{
		if( !tracking )
		{
			CheckVicinity();
			return;
		}

		// http://forum.unity3d.com/threads/162694-SmoothMovementTest-should-help-eliminate-Hiccup-sources
		// http://marrt.elementfx.com/SmoothMovementTest.html

		// WORKING if rigidbody is set to interpolate
		//transform.position = transform.position.x ( Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed) );

		float x = transform.position.x;
		if( followX )
			x = Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed);

		float y = transform.position.y;
		if( followY )
			y = Mathf.Lerp(transform.position.y, character.transform.position.y + yOffset, Time.deltaTime * speed);

		transform.position = transform.position.x (x).y(y);
	}

	protected float minDistanceX = 9000.0f;
	protected float minDistanceY = 9000.0f;

	public void CheckVicinity()
	{ 
		bool close = true;
		float treshold = 0.1f;

		if( followX )
		{
			float xDist = Mathf.Abs( transform.position.x - (character.transform.position.x + xOffset) );

			if( xDist - 0.001f > minDistanceX )
			{
				//Debug.LogError("Missed the treshold! " + xDist + " > " + minDistanceX);

				// we've missed our treshold (due to framerate)
				// so we force the follow from now on
				// logic: minDistanceX is constantly getting smaller. as soon as it gets larger again, we've missed the treshold
				xDist = -1.0f;
			}

			minDistanceX = xDist;

			close = close && (xDist < treshold);
		}

		if( followY )
		{
			float yDist = Mathf.Abs( transform.position.y - (character.transform.position.y + yOffset) );

			
			if( yDist - 0.001f > minDistanceY )
			{
				yDist = -1.0f;
			}
			
			minDistanceY = yDist;

			close = close && (yDist < treshold); 
		}

		tracking = close;

		if( tracking )
		{
			//Debug.LogError("Continuous camera just started tracking " + minDistanceX);

			ParallaxMover[] movers = GameObject.FindObjectsOfType<ParallaxMover>();
			foreach( ParallaxMover mover in movers )
			{
				mover.move = true;
			}
		}
	}
	
	protected void FixedUpdate () 
	{
		//transform.position = transform.position.x ( Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed) );
		
		//transform.position = transform.position.x( character.transform.position.x );
	}
}
