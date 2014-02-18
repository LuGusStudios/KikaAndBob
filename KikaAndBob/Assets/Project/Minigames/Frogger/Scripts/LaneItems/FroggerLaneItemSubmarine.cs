using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class FroggerLaneItemSubmarine : FroggerLaneItem 
{
	public FroggerLaneItemIceHole iceHolePrefab = null;
	public SpriteRenderer periscope = null;
	public SpriteRenderer iceHole = null;
	public SpriteRenderer submarine = null;

	public enum State
	{
		NONE = -1,
		UNDER = 1,
		
	}

	public State state = State.NONE;

	public void Surface()
	{
		StartCoroutine(SurfaceRoutine());
	}

	public void SetupLocal()
	{
		state = State.UNDER;
	}
	
	public void SetupGlobal()
	{
		if (periscope == null)
		{
			periscope = transform.FindChild("Periscope").GetComponent<SpriteRenderer>();
			if (periscope == null)
			{
				Debug.LogError("Could not find the sprite renderer for the periscope!");
			}
		}

		if (iceHole == null)
		{
			iceHole = transform.FindChild("IceHole").GetComponent<SpriteRenderer>();
			if (iceHole == null)
			{
				Debug.LogError("Could not find the sprite renderer for the ice hole!");
			}
		}

		if (submarine == null)
		{
			submarine = transform.FindChild("Submarine").GetComponent<SpriteRenderer>();
			if (submarine == null)
			{
				Debug.LogError("Could not find the sprite renderer for the submarine!");
			}
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

	private IEnumerator SurfaceRoutine()
	{

	}
}
