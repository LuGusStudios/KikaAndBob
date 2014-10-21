using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class MenuStepMain : IMenuStep 
{
	protected Button settingsButton = null;
	protected Button playButton = null;
	protected Button avatarButton = null;
	protected Button catchingMiceButton = null;
	protected Button playRoomButton = null;
	protected bool leavingMenu = false;	// set to true when transitioning to mouse hunt game - disables further input
	protected BoneAnimation character = null;
	protected LugusAudioTrackSettings musicTrackSettings = null;
	protected ILugusCoroutineHandle musicLoopHandle = null;
	protected Button accountButton = null;
	
	public void SetupLocal()
	{
		if (settingsButton == null)
			settingsButton = transform.FindChild("ButtonSettings").GetComponent<Button>();

		if (settingsButton == null)
			Debug.LogError("MenuStepMain: Missing settings button.");

		if (playButton == null)
			playButton = transform.FindChild("ButtonPlay").GetComponent<Button>();
		
		if (playButton == null)
			Debug.LogError("MenuStepMain: Missing play button.");

		if (avatarButton == null)
			avatarButton = transform.FindChild("ButtonAvatar").GetComponent<Button>();
		
		if (avatarButton == null)
			Debug.LogError("MenuStepMain: Missing avatar button.");

		if (catchingMiceButton == null)
			catchingMiceButton = transform.FindChild("ButtonCatchingMice").GetComponent<Button>();

		if (catchingMiceButton == null)
			Debug.LogError("MenuStepMain: Missing catching mice button.");

		if (playRoomButton == null)
			playRoomButton = transform.FindChild("ButtonPlayRoom").GetComponent<Button>();
		
		if (playRoomButton == null)
			Debug.LogError("MenuStepMain: Missing play room button.");

		if (character == null)
			character = GetComponentInChildren<BoneAnimation>();
		
		if (character == null)
			Debug.LogError("MenuStepAvatar: Missing character.");

		if (accountButton == null)
			accountButton = transform.FindChild("Message").GetComponent<Button>();
		
		if (accountButton == null)
			Debug.LogError("MenuStepMain: Missing account button.");
	}

	public void SetupGlobal()
	{
		SetCat();

		musicTrackSettings = new LugusAudioTrackSettings().Loop(true);

		if (musicLoopHandle != null && musicLoopHandle.Running)
			musicLoopHandle.StopRoutine();
		
		musicLoopHandle = LugusCoroutines.use.StartRoutine(MusicLoop());

		LoadConfig();
	}

	protected void LoadConfig()
	{
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

		// load language

		string pickedLanguage = LugusConfig.use.System.GetString("main.settings.langID", LugusResources.use.GetSystemLanguageID());
		LugusResources.use.ChangeLanguage(pickedLanguage);
	}

	protected IEnumerator MusicLoop()
	{
		LugusAudio.use.Music().StopAll();
		
		ILugusAudioTrack startTrack = 
			LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("MenuIntro01"));
		
		while ( startTrack.Playing )
		{
			yield return new WaitForEndOfFrame();
		}
		
		LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("MenuLoop01"), true, musicTrackSettings);
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	protected void PlayIdleAnim(int index)
	{
		character.Play("Cat0" + index.ToString() + "Side_Idle");
	}

	protected void Update () 
	{
		if (!activated || leavingMenu)
			return;

		if (playButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Games);
		}
		else if (settingsButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Settings);
		}
		else if (avatarButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Avatar);
		}
		else if (catchingMiceButton.pressed)
		{
			LugusCoroutines.use.StartRoutine(LeavingMainMenu("e00_catchingmice"));
		}
		else if (playRoomButton.pressed)
		{
			LugusCoroutines.use.StartRoutine(LeavingMainMenu("playroom"));
		}
		else if (accountButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Login);
		}
	}



	protected IEnumerator LeavingMainMenu(string toScene)
	{
		leavingMenu = true;

		ScreenFader.use.FadeOut(0.5f);

		yield return new WaitForSeconds(0.5f);

		Resources.UnloadUnusedAssets();

		Application.LoadLevel(toScene);

		yield break;
	}

	public override void Activate (bool animate)
	{
		activated = true;
		this.gameObject.SetActive(true);
		SetCat();
	}

	protected void SetCat()
	{
		int currentCatIndex = LugusConfig.use.User.GetInt("CatIndex", 1);
		
		Debug.Log("MenuStepMain: Current cat index = " + currentCatIndex.ToString());
		
		string catName = LugusConfig.use.User.GetString("CatName", "");
		
		PlayIdleAnim(currentCatIndex);
	}

	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);
	}
}
