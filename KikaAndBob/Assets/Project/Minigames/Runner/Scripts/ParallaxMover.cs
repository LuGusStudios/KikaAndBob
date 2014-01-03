using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxMover : MonoBehaviour 
{
	public Vector3 speed = Vector3.zero;
	public RunnerCharacterController character = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		if( RunnerCharacterControllerHorizontal.Exists() )
			character = RunnerCharacterControllerHorizontal.use;
		else if( RunnerCharacterControllerVertical.Exists() )
			character = RunnerCharacterControllerVertical.use;

		if( character == null )
		{
			Debug.LogError(name + " : No character found!");
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
	
	protected void Update () 
	{
		Vector3 modifier = Vector3.one;
		if( character != null )
		{
			modifier = character.SpeedScale();
		}

		transform.position += Vector3.Scale (speed, modifier) * Time.deltaTime;
	}
}
