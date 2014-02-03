using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

[RequireComponent(typeof(DanceHeroLane))]
public class DanceHeroLaneHandlerChina : MonoBehaviour 
{
	protected DanceHeroLane lane = null;
	public string attackAnimation = null;
	public string idleAnimation = null;

	[HideInInspector]
	public BoneAnimation characterAnim = null;
	// public GameObject character = null;

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


		if( characterAnim == null )
		{
			characterAnim = transform.FindChild("Character").GetComponent<BoneAnimation>();
		}
		
		if( characterAnim == null )
		{
			Debug.LogError(name + " : no character known for this lane!");
		}
		
		if (string.IsNullOrEmpty(attackAnimation))
		{
			Debug.LogError(name + "Attack animation name is not entered.");
		}
		
		if(string.IsNullOrEmpty(idleAnimation))
		{
			idleAnimation = characterAnim.GetComponent<DefaultBoneAnimation>().clipName;
		}
		
		if(string.IsNullOrEmpty(idleAnimation))
		{
			Debug.LogError(name + "Idle animation name is not entered or default bone animation component is missing.");
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

	protected void OnLaneBegin()
	{
		characterAnim.Play(idleAnimation);
	}

	protected void OnItemSpawned(DanceHeroLaneItemRenderer laneItemRenderer)
	{
		// only play a new fight anim if the previous one isn't still playing
		if (!characterAnim.IsPlaying(attackAnimation))
		{
			characterAnim.PlayQueued(attackAnimation, QueueMode.PlayNow, PlayMode.StopAll);
			characterAnim.PlayQueued(idleAnimation, QueueMode.CompleteOthers);
		}

		if (laneItemRenderer == null)
		{
			Debug.LogError("Lane item renderer was null!");
			return;
		}

		foreach(Transform t in laneItemRenderer.actionPoints)
		{
			Vector3 originalScale = t.localScale;
			t.localScale = Vector3.zero;
			float timeToReachCharacter = characterAnim.transform.localPosition.x / lane.speed;
			
			t.gameObject.ScaleTo(originalScale).Time(0.5f).EaseType(iTween.EaseType.spring).Delay(timeToReachCharacter).Execute();
			
			
			ParticleSystem particles = t.GetComponentInChildren<ParticleSystem>();
			particles.startDelay = timeToReachCharacter;
			particles.Play();
		}
	
	}
}
