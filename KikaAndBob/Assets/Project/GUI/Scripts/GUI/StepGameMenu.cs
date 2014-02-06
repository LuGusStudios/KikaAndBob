using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepGameMenu : IMenuStep 
{
	protected Button playButton = null;
	protected Button helpButton = null;
	
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
	}

	public override void Activate()
	{
		activated = true;
		gameObject.SetActive(true);
	}

	public override void Deactivate()
	{
		activated = false;
		gameObject.SetActive(false);
	}
}
