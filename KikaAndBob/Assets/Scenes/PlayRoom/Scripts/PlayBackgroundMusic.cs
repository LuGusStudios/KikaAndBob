using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayBackgroundMusic : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("MenuLoop01"), true, new LugusAudioTrackSettings().Loop(true));
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start() 
	{
		SetupGlobal();
	}
	
	protected void Update() 
	{
	
	}
}
