using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomFlip : MonoBehaviour 
{
	public float flipChance = 0.5f;

	public bool flipX = true;
	public bool flipY = false;

	public GameObject[] brothers;
	
	public bool scaleOnly = false;

	public void SetupLocal()
	{
		FlipRandom();
	}

	public void FlipRandom()
	{
		bool x = Random.value < flipChance && flipX;
		bool y = Random.value < flipChance && flipY;
		
		
		Flip ( this.transform, x, y);
		foreach( GameObject obj in brothers )
		{
			if( obj != null )
			{
				Flip ( obj.transform, x, y );
			}
		}
	}

	protected void Flip( Transform target, bool x, bool y )
	{
		RunnerMover mover = target.GetComponent<RunnerMover>();

		if( x )
		{
			if( !scaleOnly )
				target.localPosition   = Vector3.Scale( target.localPosition, new Vector3(-1.0f, 1.0f, 1.0f) );

			target.localScale = Vector3.Scale(target.localScale, new Vector3(-1.0f, 1.0f, 1.0f) );
			
			if( mover != null && !scaleOnly )
			{
				mover.direction = Vector3.Scale( mover.direction, new Vector3(-1.0f, 1.0f, 1.0f) );
			}
		}

		if( y )
		{
			if( !scaleOnly )
				target.localPosition   = Vector3.Scale( target.localPosition, new Vector3(1.0f, -1.0f, 1.0f) );

			target.localScale = Vector3.Scale(target.localScale, new Vector3(1.0f, -1.0f, 1.0f) );
			
			if( mover != null && !scaleOnly )
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
}
