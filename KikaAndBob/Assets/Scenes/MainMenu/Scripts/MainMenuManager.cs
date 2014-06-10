using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : LugusSingletonExisting<MainMenuManager> 
{
	public Dictionary<MainMenuTypes, IMenuStep> menus = new Dictionary<MainMenuTypes, IMenuStep>();

	public enum MainMenuTypes
	{
		NONE = -1,
		Main = 0,
		Settings = 1
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		MenuStepMain gameMenu = transform.FindChild("Main").GetComponent<MenuStepMain>();
		if (gameMenu != null)
			menus.Add(MainMenuTypes.Main, gameMenu);
		else
			Debug.LogError("MainMenuManager: Missing main menu!");
		

		
		MenuStepSettings helpMenu = transform.FindChild("Settings").GetComponent<MenuStepSettings>();
		if (helpMenu != null)
			menus.Add(MainMenuTypes.Settings, helpMenu);
		else
			Debug.LogError("MainMenuManager: Missing setting menu!");

		ShowMenu(MainMenuTypes.Main);
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
	
	}

	protected void DeactivateAll()
	{
		foreach(IMenuStep step in menus.Values)
		{
			step.Deactivate();
		}
	}

	protected void ShowMenu(MainMenuTypes type)
	{
		if (menus.ContainsKey(type))
		{
			DeactivateAll();
			menus[type].Activate();
		}
		else
		{
			Debug.LogError("MainMenuManager: Turned on menu: " + type.ToString());
		}

	}
}
