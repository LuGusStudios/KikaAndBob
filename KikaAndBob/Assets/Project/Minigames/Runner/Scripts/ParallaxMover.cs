using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxMover : MonoBehaviour 
{
	public Vector3 speed = Vector3.zero;

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
		transform.position += speed * Time.deltaTime;
	}
}
