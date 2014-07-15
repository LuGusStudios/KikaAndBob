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
		Settings = 1,
		Avatar = 2,
		Games = 3,
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

		MenuStepAvatar avatarMenu = transform.FindChild("Avatar").GetComponent<MenuStepAvatar>();
		if (avatarMenu != null)
			menus.Add(MainMenuTypes.Avatar, avatarMenu);
		else
			Debug.LogError("MainMenuManager: Missing avatar menu!");
		
		MenuStepGames gamesMenu = transform.FindChild("Games").GetComponent<MenuStepGames>();
		if (gamesMenu != null)
			menus.Add(MainMenuTypes.Games, gamesMenu);
		else
			Debug.LogError("MainMenuManager: Missing games menu!");

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

	public void ShowMenu(MainMenuTypes type)
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
