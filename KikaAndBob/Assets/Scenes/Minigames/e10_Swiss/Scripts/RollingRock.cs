using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RollingRock : MonoBehaviour 
{
	public float rotationSpeed = 75.0f; // angles per sec

	public RunnerMover mover = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only

		if( mover == null )
			mover = GetComponent<RunnerMover>();
		if( mover == null )
			mover = transform.parent.GetComponent<RunnerMover>();
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
		transform.Rotate( transform.forward, -mover.direction.x * rotationSpeed * Time.deltaTime );
	}
}
