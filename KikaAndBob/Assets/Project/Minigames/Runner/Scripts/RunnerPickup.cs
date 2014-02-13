using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerPickup : MonoBehaviour 
{
	// if positive: disappear and grant points
	// if negative: remain, hit and possibly detract points
	public bool positive = true;

	public float scoreAmount = 10;
	public KikaAndBob.CommodityType commodityType = KikaAndBob.CommodityType.NONE;

	public bool negative
	{
		get{ return !positive; } 
	}

	// if deadly, the user dies immediately
	// else, the user will lose 1 of his/her lives
	public bool deadly = false;

	public delegate void OnHit(RunnerPickup pickup);
	public OnHit onHit;

	void OnTriggerEnter2D(Collider2D other) 
	{
		IRunnerCharacterController controller = other.transform.GetComponent( typeof(IRunnerCharacterController) ) as IRunnerCharacterController;
		if( controller != null )
		{
			ProcessHit( controller );
		}
	}

	public virtual void ProcessHit(IRunnerCharacterController character)
	{
		character.OnPickupHit(this);
		
		if( onHit != null )
			onHit( this );

		if( positive )
		{
			transform.position = LugusUtil.OFFSCREEN;
		}
		else
		{
			this.collider2D.enabled = false;
		}
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( this.collider2D == null || !this.collider2D.isTrigger )
		{
			Debug.LogError(transform.Path() + " : RunnerPickup has no collider or the collider is not set as Trigger!!");
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
