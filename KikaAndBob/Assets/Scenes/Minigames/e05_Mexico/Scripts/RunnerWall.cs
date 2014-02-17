using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerWall : MonoBehaviour 
{
	public RunnerPickup pickup = null;
	//public GameObject character = null;

	public RunnerWall otherSectionWall = null;
	
	public void SetupLocal()
	{
		pickup = GetComponent<RunnerPickup>();
		if( pickup == null )
		{
			Debug.LogError(name + " : RunnerPickup was null!");
		}
		else
		{
			pickup.onHit += OnHit;
		}

		if( otherSectionWall == null )
		{
			Debug.LogError(transform.Path() + " : please assign the other section wall!");
		}

		/*
		character = GameObject.Find ("Character");
		if( character == null )
		{
			Debug.LogError(name + " : Character was null!");
		}
		*/
	}
	
	protected ILugusCoroutineHandle handle = null;
	
	public void SetupGlobal()
	{
	}
	
	protected void OnHit(RunnerPickup pickup) 
	{
		handle = LugusCoroutines.use.StartRoutine( ToggleRoutine(true) );
	}

	// to be called from the Other Wall if that one is hit
	public void SimulateHit()
	{
		handle = LugusCoroutines.use.StartRoutine( ToggleRoutine(false) );
	}

	protected IEnumerator ToggleRoutine(bool affectOtherWall)
	{
		Debug.Log (Time.frameCount + " TOGGLE " + transform.Path () + " OFF ");
		this.pickup.activated = false;

		if( affectOtherWall && otherSectionWall != null )
			otherSectionWall.SimulateHit();

		yield return new WaitForSeconds(1.0f);

		this.pickup.activated = true;
		Debug.Log (Time.frameCount + " TOGGLE " + transform.Path () + " ON ");
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
