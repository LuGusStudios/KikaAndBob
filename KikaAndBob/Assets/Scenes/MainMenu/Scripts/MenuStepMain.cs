using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepMain : IMenuStep 
{
	Button settingsButton = null;
	Button playButton = null;
	Button avatarButton = null;

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
		if (!activated)
			return;

		if (playButton.pressed)
		{

		}

		if (settingsButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Settings);
		}
	
		if (avatarButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Avatar);
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
}
