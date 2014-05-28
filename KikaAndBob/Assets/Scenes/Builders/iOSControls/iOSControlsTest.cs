using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iOSControlsTest : MonoBehaviour 
{
	Joystick joystick = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		joystick = GetComponent<Joystick>();
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
