using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepSettings : IMenuStep 
{
	protected Button exitButton = null;
	protected Button musicButton = null;
	protected Button soundButton = null;
	protected Button langButton = null;
	protected Button resetButton = null;

	protected TextMeshWrapper soundOnOffText = null;
	protected TextMeshWrapper musicOnOffText = null;
	protected TextMeshWrapper langNameText = null;

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

		if (langNameText == null)
			langNameText = transform.FindChild("TextLanguageSelected").GetComponent<TextMeshWrapper>();
		
		if (langNameText == null)
			Debug.LogError("MenuStepSettings: Missing language name text.");

		if (langButton == null)
			langButton = transform.FindChild("ButtonLanguage").GetComponent<Button>();
		
		if (langButton == null)
			Debug.LogError("MenuStepSettings: Missing language button.");

		if (resetButton == null)
			resetButton = transform.FindChild("ButtonReset").GetComponent<Button>();
		
		if (resetButton == null)
			Debug.LogError("MenuStepSettings: Missing reset button.");
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
		else if (musicButton.pressed)
		{
			bool musicMute = LugusConfig.use.System.GetBool("main.settings.musicmute", false);
			musicMute = !musicMute;
			LugusConfig.use.System.SetBool("main.settings.musicmute", musicMute, true);

			SetMusicMute(musicMute);
		}
		else if (soundButton.pressed)
		{
			bool soundMute = LugusConfig.use.System.GetBool("main.settings.soundmute", false);
			soundMute = !soundMute;
			LugusConfig.use.System.SetBool("main.settings.soundmute", soundMute, true);

			SetSoundMute(soundMute);
		}
		else if (langButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Language);
		}

		else if (resetButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Reset);
		}

	}

	public override void Activate (bool animate)
	{
		activated = true;
		this.gameObject.SetActive(true);

		// make sure buttons are already properly displaying mute/unmute state
		SetMusicMute(LugusConfig.use.System.GetBool("main.settings.musicmute", false));
		SetSoundMute(LugusConfig.use.System.GetBool("main.settings.soundmute", false));

		// give language display proper language
		string currentLangId  = LugusResources.use.GetLocalizedLangID();
		for (int i = 1; i <= 32; i++) 
		{
			if (currentLangId == LugusResources.use.Shared.GetText("lang.lang" + i + ".id"))
			{
				langNameText.SetText(LugusResources.use.Shared.GetText("lang.lang" + i + ".name"));
				break;
			}
		}


	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);

		// save settings
		LugusConfig.use.SaveProfiles();
	}

	protected void SetMusicMute(bool mute)
	{
		float alpha = 1;
		string textKey = "";

		if (mute)
		{
			alpha = 0.6f;
			LugusAudio.use.Music().UpdateVolumeFromOriginal(0);
			textKey = "global.settings.off";
		}
		else
		{
			LugusAudio.use.Music().UpdateVolumeFromOriginal(1);
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
			LugusAudio.use.SFX().UpdateVolumeFromOriginal(0);
			textKey = "global.settings.off";
		}
		else
		{
			LugusAudio.use.SFX().UpdateVolumeFromOriginal(1);
			textKey = "global.settings.on";
		}

		SpriteRenderer spriteRenderer = soundButton.GetComponent<SpriteRenderer>();
		spriteRenderer.color = spriteRenderer.color.a(alpha);
		soundOnOffText.SetTextKey(textKey);
	}
}
