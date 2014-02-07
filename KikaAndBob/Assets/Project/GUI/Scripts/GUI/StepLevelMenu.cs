using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepLevelMenu : IMenuStep 
{
	public void SetupLocal()
	{
	}
	
	public void SetupGlobal()
	{
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

	public override void Activate()
	{
		activated = true;
		gameObject.SetActive(true);
	}

	public override void Deactivate()
	{
		activated = false;
		gameObject.SetActive(false);
	}
}
