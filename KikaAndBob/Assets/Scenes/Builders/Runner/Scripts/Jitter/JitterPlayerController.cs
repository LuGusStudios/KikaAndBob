using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JitterPlayerController : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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
		if( this.rigidbody != null )
		{
			rigidbody.velocity = new Vector2(13, rigidbody.velocity.y);
		}
		else
		{
			rigidbody2D.AddForce(Vector2.right * 13 * 10);
			
			// If the player's horizontal velocity is greater than the maxSpeed...
			if(Mathf.Abs(rigidbody2D.velocity.x) > 13)
				// ... set the player's velocity to the maxSpeed in the x axis.
				rigidbody2D.velocity = new Vector2(13, rigidbody2D.velocity.y);
		}
	}
}
