using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepSettings : IMenuStep 
{
	protected Button exitButton = null;
	protected Button musicButton = null;
	protected Button soundButton = null;

	protected TextMeshWrapper soundOnOffText = null;
	protected TextMeshWrapper musicOnOffText = null;


	public void SetupLocal()
	{
		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepSettings: Missing exit button.");


		if (musicButton == null)
			musicButton = transform.FindChild("ButtonMusic").GetComponent<Button>();
		
		if (musicButton == null)
			Debug.LogError("MenuStepSettings: Missing music button.");


		if (soundButton == null)
			soundButton = transform.FindChild("ButtonSound").GetComponent<Button>();
		
		if (soundButton == null)
			Debug.LogError("MenuStepSettings: Missing sound button.");

		if (soundOnOffText == null)
			soundOnOffText = transform.FindChild("TextSoundOnOff").GetComponent<TextMeshWrapper>();

		if (soundOnOffText == null)
			Debug.LogError("MenuStepSettings: Missing sound on/off text.");

		if (musicOnOffText == null)
			musicOnOffText = transform.FindChild("TextMusicOnOff").GetComponent<TextMeshWrapper>();

		if (musicOnOffText == null)
			Debug.LogError("MenuStepSettings: Missing music on/off text.");
	}
	
	public void SetupGlobal()
	{
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
		if (exitButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);
		}

		if (musicButton.pressed)
		{
			bool musicMute = LugusConfig.use.User.GetBool("main.settings.musicmute", false);
			musicMute = !musicMute;
			LugusConfig.use.User.SetBool("main.settings.musicmute", musicMute, true);

			SetMusicMute(musicMute);
		}

		if (soundButton.pressed)
		{
			bool soundMute = LugusConfig.use.User.GetBool("main.settings.soundmute", false);
			soundMute = !soundMute;
			LugusConfig.use.User.SetBool("main.settings.soundmute", soundMute, true);
			
			SetSoundMute(soundMute);
		}

	}

	public override void Activate (bool animate)
	{
		activated = true;
		this.gameObject.SetActive(true);
	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);
	}

	protected void SetMusicMute(bool mute)
	{
		float alpha = 1;
		string textKey = "";

		if (mute)
		{
			alpha = 0.6f;
			LugusAudio.use.Music().VolumePercentage = 0;
			textKey = "global.settings.off";
		}
		else
		{
			LugusAudio.use.Music().VolumePercentage = 1;
			textKey = "global.settings.on";
		}

		SpriteRenderer spriteRenderer = musicButton.GetComponent<SpriteRenderer>();
		spriteRenderer.color = spriteRenderer.color.a(alpha);
		musicOnOffText.SetTextKey(textKey);
	}

	protected void SetSoundMute(bool mute)
	{
		float alpha = 1;
		string textKey = "";

		if (mute)
		{
			alpha = 0.6f;
			LugusAudio.use.Music().VolumePercentage = 0;
			textKey = "global.settings.off";
		}
		else
		{
			LugusAudio.use.Music().VolumePercentage = 1;
			textKey = "global.settings.on";
		}

		SpriteRenderer spriteRenderer = soundButton.GetComponent<SpriteRenderer>();
		spriteRenderer.color = spriteRenderer.color.a(alpha);
		soundOnOffText.SetTextKey(textKey);
	}
}
