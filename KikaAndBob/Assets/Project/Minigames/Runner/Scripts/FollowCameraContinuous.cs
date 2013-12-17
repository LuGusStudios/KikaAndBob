using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowCameraContinuous : MonoBehaviour 
{
	public GameObject character = null;
	public float speed = 100.0f;
	public float xOffset = 0.0f;
	
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
		transform.position = transform.position.x (character.transform.position.x + xOffset );
		
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
		transform.position = transform.position.x ( Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed) );
	}
	
	protected void FixedUpdate () 
	{
		//transform.position = transform.position.x ( Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed) );
		
		//transform.position = transform.position.x( character.transform.position.x );
	}
}
