using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayBackgroundMusic : MonoBehaviour 
{
	public void SetupLocal()
	{
		LoadConfig();
	}

	protected void LoadConfig()
	{
		Debug.Log("PlayBackgroundMusic: Loading config settings.");
		
		// read if music and SFX need to be muted
		if (LugusConfig.use.System.GetBool("main.settings.musicmute", false) == true)
		{
			LugusAudio.use.Music().UpdateVolumeFromOriginal(0);
		}
		else
		{
			LugusAudio.use.Music().UpdateVolumeFromOriginal(1);
		}
		
		if (LugusConfig.use.System.GetBool("main.settings.soundmute", false) == true)
		{
			LugusAudio.use.SFX().UpdateVolumeFromOriginal(0);
		}
		else
		{
			LugusAudio.use.SFX().UpdateVolumeFromOriginal(1);
		}
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
