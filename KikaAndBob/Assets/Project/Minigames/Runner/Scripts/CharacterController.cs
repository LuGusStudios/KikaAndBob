using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour 
{
	public float speed = 13.0f;
	public float jumpForce = 10.0f;

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

		//this.rigidbody2D.AddForce(Vector2.right * speed * 10 * Time.fixedDeltaTime);

		//this.rigidbody2D.velocity = this.rigidbody2D.velocity.x( speed );



		//rigidbody2D.AddForce(Vector2.right * speed * 10);
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		//if(Mathf.Abs(rigidbody2D.velocity.x) > speed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);



		if( jump )
		{
			//this.rigidbody2D.AddForce(Vector2.up * speed * Time.fixedDeltaTime);
			this.rigidbody2D.velocity += Vector2.up * jumpForce; 

			jump = false; 
		}


		//transform.position = transform.position.xAdd( speed * Time.deltaTime );

		//Transform camTrans = LugusCamera.game.transform;
		//camTrans.position = camTrans.position.x ( Mathf.Lerp(camTrans.position.x, this.transform.position.x, /*Time.fixedDeltaTime * speed*/ 0.5f) );

	}

	protected bool jump = false;

	public void Update()
	{
		
		//Debug.Log ("VELOCITY " + this.rigidbody2D.velocity);

		if( LugusInput.use.KeyDown (KeyCode.Space) && 
		   (this.rigidbody2D.velocity.y < 1.0f) && 
		   (this.rigidbody2D.velocity.y >= -0.1f) ) // if velocity y is between 1.0f and 0.0f, we are grounded
		{
			jump = true;
		}
	}
}
