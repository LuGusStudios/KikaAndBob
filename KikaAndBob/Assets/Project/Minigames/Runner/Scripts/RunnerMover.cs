using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerMover : MonoBehaviour 
{
	public Vector3 direction = Vector3.zero;
	public float speed = 1.0f;

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
		transform.position += direction * speed * Time.deltaTime;
	}
}
