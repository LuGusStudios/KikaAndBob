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
		if( followX )
			transform.position = transform.position.x (character.transform.position.x + xOffset);
		
		if( followY )
			transform.position = transform.position.y( character.transform.position.y + yOffset);
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
	
	protected void FixedUpdate () 
	{
		//transform.position = transform.position.x ( Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed) );
		
		//transform.position = transform.position.x( character.transform.position.x );
	}
}
