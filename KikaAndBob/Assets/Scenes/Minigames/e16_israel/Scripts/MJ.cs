using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJ : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

		gameObject.MoveTo( this.transform.position.x ( this.transform.position.x - 3000 ) ).Time (7.0f).Execute();
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
