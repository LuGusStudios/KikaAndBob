using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomFlip : MonoBehaviour 
{
	public float flipChance = 0.5f;

	public bool flipX = true;
	public bool flipY = false;

	public void SetupLocal()
	{
		RunnerMover mover = GetComponent<RunnerMover>();

		if( Random.value > 0.5f  && flipX )
		{
			this.transform.localPosition   = Vector3.Scale( this.transform.localPosition, new Vector3(-1.0f, 1.0f, 1.0f) );
			this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(-1.0f, 1.0f, 1.0f) );

			if( mover != null )
			{
				mover.direction = Vector3.Scale( mover.direction, new Vector3(-1.0f, 1.0f, 1.0f) );
			}
		}

		if( Random.value < 0.5f && flipY )
		{
			this.transform.localPosition   = Vector3.Scale( this.transform.localPosition, new Vector3(1.0f, -1.0f, 1.0f) );
			this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(1.0f, -1.0f, 1.0f) );
			
			if( mover != null )
			{
				mover.direction = Vector3.Scale( mover.direction, new Vector3(1.0f, -1.0f, 1.0f) );
			}
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
