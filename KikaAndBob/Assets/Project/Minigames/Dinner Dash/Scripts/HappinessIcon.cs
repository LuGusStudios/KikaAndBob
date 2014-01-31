using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HappinessIcon : MonoBehaviour 
{
	// states should be in ASCENDING order of happiness
	// so. 0 = angry, 4 = happy
	public Sprite[] states;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( states.Length != 5 )
		{
			Debug.LogError( transform.Path() + " : HappinessIcon doesn't have 5 states!");
		}
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
	
	}
}
