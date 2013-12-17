using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DefaultBoneAnimation : MonoBehaviour 
{
	public string clipName = "";
	public AnimationClip clip = null;

	public void SetupLocal()
	{
		if( GetComponent<BoneAnimation>() == null )
		{
			Debug.LogError(name + " : object has no BoneAnimation to set clip on...");
			this.enabled = false;
		}

		if( this.clip == null && clipName == "" )
		{
			Debug.LogError(name + " : no clip or clipname set...");
			this.enabled = false;
		}
	}
	
	public void SetupGlobal()
	{
		if( clipName == "" && clip != null )
		{
			clipName = clip.name;
		}

		GetComponent<BoneAnimation>().Play( clipName, PlayMode.StopAll );
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
