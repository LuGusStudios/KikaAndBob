using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowCameraPhysics : MonoBehaviour 
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

		if( this.rigidbody2D == null )
		{
			gameObject.AddComponent<Rigidbody2D>();

			rigidbody2D.isKinematic = true;
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
		//transform.position += transform.right * 1 * Time.deltaTime;
	}
	
	protected void FixedUpdate () 
	{
		this.rigidbody2D.velocity = Vector2.Lerp(this.rigidbody2D.velocity, character.rigidbody2D.velocity, Time.deltaTime * speed);

		//transform.position = transform.position.x ( Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed) );

		//transform.position = transform.position.x( character.transform.position.x );
	}
}
