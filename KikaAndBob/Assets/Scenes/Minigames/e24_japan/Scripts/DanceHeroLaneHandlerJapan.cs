using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

[RequireComponent(typeof(DanceHeroLane))]
public class DanceHeroLaneHandlerJapan : MonoBehaviour 
{
	protected DanceHeroLane lane = null;

	public void SetupLocal()
	{
		if (lane != null)
			return;

		lane = GetComponent<DanceHeroLane>();
		if (lane == null)
		{
			Debug.LogError(name + ": Could not find lane script!");
		}

		lane.onItemSpawned += OnItemSpawned;
		lane.onLaneBegin += OnLaneBegin;
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

	protected void OnLaneBegin()
	{

	}

	protected void OnItemSpawned(DanceHeroLaneItemRenderer laneItemRenderer)
	{
		if (laneItemRenderer == null)
		{
			Debug.LogError("Lane item renderer was null!");
			return;
		}
	
	}
}
