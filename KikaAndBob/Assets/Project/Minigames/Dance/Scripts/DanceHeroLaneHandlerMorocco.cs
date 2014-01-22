using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

[RequireComponent(typeof(DanceHeroLane))]
public class DanceHeroLaneHandlerMorocco : MonoBehaviour 
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

//		foreach(Transform t in laneItemRenderer.actionPoints)
//		{
//			Vector3 originalScale = t.localScale;
//			t.localScale = Vector3.zero;
//			float timeToReachCharacter = characterAnim.transform.localPosition.x / lane.speed;
//			
//			t.gameObject.ScaleTo(originalScale).Time(0.5f).EaseType(iTween.EaseType.spring).Delay(timeToReachCharacter).Execute();
//			
//			
//			ParticleSystem particles = t.GetComponentInChildren<ParticleSystem>();
//			particles.startDelay = timeToReachCharacter;
//			particles.Play();
//		}
	
	}
}
