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
	}

	public void SetupGlobal()
	{
		SetCat();
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
			LugusCoroutines.use.StartRoutine(LeavingMainMenu());
		}
		else if (playRoomButton.pressed)
		{

		}
	}



	protected IEnumerator LeavingMainMenu()
	{
		leavingMenu = true;

		ScreenFader.use.FadeOut(0.5f);

		yield return new WaitForSeconds(0.5f);

		Application.LoadLevel("e00_catchingmice");

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
