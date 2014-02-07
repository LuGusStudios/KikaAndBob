using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : LugusSingletonExisting<MenuManagerDefault>
{
}

public class MenuManagerDefault: MonoBehaviour
{
	public Dictionary<MenuTypes, IMenuStep> menus = new Dictionary<MenuTypes, IMenuStep>();

	public enum MenuTypes
	{
		NONE = -1,
		GameMenu = 1,
		LevelMenu = 2,
		HelpMenu = 3
	}

	public MenuTypes startMenu = MenuTypes.GameMenu;

	public void SetupLocal()
	{
		StepGameMenu gameMenu = transform.FindChild("GameMenu").GetComponent<StepGameMenu>();
		if (gameMenu != null)
			menus.Add(MenuTypes.GameMenu, gameMenu);
		else
			Debug.LogError("MenuManager: Missing game menu!");
		
		StepLevelMenu levelMenu = transform.FindChild("LevelMenu").GetComponent<StepLevelMenu>();
		if (levelMenu != null)
			menus.Add(MenuTypes.LevelMenu, levelMenu);
		else
			Debug.LogError("MenuManager: Missing level menu!");

		StepHelpMenu helpMenu = transform.FindChild("HelpMenu").GetComponent<StepHelpMenu>();
		if (helpMenu != null)
			menus.Add(MenuTypes.HelpMenu, helpMenu);
		else
			Debug.LogError("MenuManager: Missing help menu!");
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
		ActivateMenu(MenuTypes.GameMenu);
	}
	
	protected void Update () 
	{
	}

	protected void DeactivateAllMenus()
	{
		foreach(IMenuStep step in menus.Values)
		{
			step.Deactivate();
		}
	}

	public void ActivateMenu(MenuTypes type)
	{
		IMenuStep nextStep = null;

		if (menus.ContainsKey(type))
		{
			nextStep = menus[type];
		}

		if (nextStep != null)
		{
			DeactivateAllMenus();
			nextStep.Activate();
		}
		else
		{
			Debug.LogError("MenuManagerDefault: Unknown menu!");
		}
	}

	public Transform GetChildMenu(string menuName)
	{
		if (string.IsNullOrEmpty(menuName))
		{
			Debug.LogError("MenuManagerDefault: String is empty!");
			return null;
		}

		foreach(Transform t in transform)
		{
			if (menuName == t.name)
			{
				return t;
			}
		}
		 
		Debug.LogError("MenuManagerDefault: Could not find child menu: " + menuName);
		return null;
	}
}
