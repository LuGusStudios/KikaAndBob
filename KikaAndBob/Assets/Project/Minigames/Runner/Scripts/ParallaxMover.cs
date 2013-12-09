using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxMover : MonoBehaviour 
{
	public float speed = -5.0f;

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
	
	protected void Update () 
	{
		transform.position += transform.right * speed * Time.deltaTime;
	}
}
