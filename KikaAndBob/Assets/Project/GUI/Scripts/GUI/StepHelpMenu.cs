using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepHelpMenu : IMenuStep 
{
	protected Button leaveButton = null;

	public void SetupLocal()
	{
		if (leaveButton == null)
		{
			leaveButton = transform.FindChild("LeaveButton").GetComponent<Button>();
		}
		if (leaveButton == null)
		{
			Debug.Log("StepGameMenu: Missing leave button.");
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
		
		if (leaveButton.pressed)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
	}

	protected void LoadLevelData()
	{
		// TO DO: Set data about levels here (name, description, etc.)
	}

	public override void Activate()
	{
		activated = true;
		gameObject.SetActive(true);
		LoadLevelData();
	}

	public override void Deactivate()
	{
		activated = false;
		gameObject.SetActive(false);
	}
}
