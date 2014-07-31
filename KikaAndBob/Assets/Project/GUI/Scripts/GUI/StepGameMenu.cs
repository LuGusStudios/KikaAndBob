using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepGameMenu : IMenuStep 
{
	protected Button playButton = null;
	protected Button helpButton = null;
	protected TextMeshWrapper title = null;
	protected TextMeshWrapper description = null;
	protected SpriteRenderer image = null;
	protected Vector3 originalPosition = Vector3.zero;
	protected LugusAudioTrackSettings musicTrackSettings;
	protected Button leaveButton = null;
	
	public void SetupLocal()
	{
		if (playButton == null)
		{
			playButton = transform.FindChild("PlayButton").GetComponent<Button>();
		}
		if (playButton == null)
		{
			Debug.Log("StepGameMenu: Missing play button.");
		}

		if (helpButton == null)
		{
			helpButton = transform.FindChild("HelpButton").GetComponent<Button>();
		}
		if (helpButton == null)
		{
			Debug.Log("StepGameMenu: Missing help button.");
		}

		if (title == null)
		{
			title = transform.FindChild("Title").GetComponent<TextMeshWrapper>();
		}
		if (title == null)
		{
			Debug.Log("StepGameMenu: Missing title!");
		}

		if (description == null)
		{
			description = transform.FindChild("Description").GetComponent<TextMeshWrapper>();
		}
		if (description == null)
		{
			Debug.Log("StepGameMenu: Missing description!");
		}

		if (image == null)
		{
			image = transform.FindChild("Image").GetComponent<SpriteRenderer>();
		}
		if (image == null)
		{
			Debug.Log("StepGameMenu: Missing image sprite renderer!");
		}

		if (leaveButton == null)
		{
			leaveButton = transform.FindChild("LeaveButton").GetComponent<Button>();
		}
		if (leaveButton == null)
		{
			Debug.Log("StepHelpMenu: Missing leave button.");
		}

		originalPosition = transform.position;


		musicTrackSettings = new LugusAudioTrackSettings().Loop(true);

		LoadConfig();
	}

	protected void LoadConfig()
	{
		// read if music and SFX need to be muted
		if (LugusConfig.use.User.GetBool("main.settings.musicmute", false) == true)
		{
			LugusAudio.use.Music().UpdateVolumeFromOriginal(0);
		}
		else
		{
			LugusAudio.use.Music().UpdateVolumeFromOriginal(1);
		}
		
		if (LugusConfig.use.User.GetBool("main.settings.soundmute", false) == true)
		{
			LugusAudio.use.SFX().UpdateVolumeFromOriginal(0);
		}
		else
		{
			LugusAudio.use.SFX().UpdateVolumeFromOriginal(1);
		}
		
		// load language
		
		string pickedLanguage = LugusConfig.use.User.GetString("main.settings.langID", LugusResources.use.GetSystemLanguageID());
		
		LugusResources.use.ChangeLanguage(pickedLanguage);
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
		if (!activated)
			return;

		if (playButton.pressed)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.LevelMenu);
		}
		else if (helpButton.pressed)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.HelpMenu);
		}
		else if (leaveButton.pressed)
		{
			LugusCoroutines.use.StartRoutine(LeaveRoutine());
		}
	}

	protected IEnumerator LeaveRoutine()
	{
		activated = false; // this handily blocks input before leaving

		ScreenFader.use.FadeOut(0.25f);

		yield return new WaitForSeconds(0.5f);

		Application.LoadLevel("MainMenu");

		yield break;
	}

	protected void LoadLevelData()
	{
		// TO DO: Set data about levels here (name, description, etc.) 
		string key = Application.loadedLevelName + ".main.";
	
		title.SetText(LugusResources.use.Levels.GetText(key + "title"));
		description.SetText(LugusResources.use.Levels.GetText(key + "description"));

		Sprite imageSprite = null;

		if (LugusResources.use.Levels.HasText(key + "image"))
		{
			imageSprite = LugusResources.use.Shared.GetSprite(LugusResources.use.Levels.GetText(key + "image"));
		}
		else
		{
			imageSprite = LugusResources.use.Shared.GetSprite( Application.loadedLevelName + "_Main_Image");
		}

		if (imageSprite == null || imageSprite == LugusResources.use.errorSprite)
		{
			image.gameObject.SetActive(false);
		}
		else
		{
			image.sprite = imageSprite;
			image.gameObject.SetActive(true);
		}
	
	}

	public override void Activate(bool animate = true)
	{
		activated = true;
		gameObject.SetActive(true);
		LoadLevelData();

		iTween.Stop(gameObject);

		transform.position = originalPosition + new Vector3(30, 0, 0);

		gameObject.MoveTo(originalPosition).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();

		LugusCoroutines.use.StartRoutine(MusicLoop());
	}

	protected IEnumerator MusicLoop()
	{
		LugusAudio.use.Music().StopAll();
		LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("MenuIntro01"));

		while ( LugusAudio.use.Music().IsPlaying )
		{
			yield return new WaitForEndOfFrame();
		}
	
		LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("MenuLoop01"), true, musicTrackSettings);
	}

	public override void Deactivate(bool animate = true)
	{
		activated = false;
		//gameObject.SetActive(false);

		iTween.Stop(gameObject);
		gameObject.MoveTo(originalPosition + new Vector3(-30, 0, 0)).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();
	}
}
