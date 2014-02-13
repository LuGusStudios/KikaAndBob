using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : LugusSingletonExisting<MenuManagerDefault>
{
}

public class MenuManagerDefault: MonoBehaviour
{
	public Dictionary<MenuTypes, IMenuStep> menus = new Dictionary<MenuTypes, IMenuStep>();
	public Sprite backgroundSprite = null;

	protected Transform background = null;


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

		if (background == null)
			background = transform.FindChild("Background");
		if (background == null)
			Debug.LogError("MenuManager: Missing background!");
			
	}
	
	public void SetupGlobal()
	{
		SpriteRenderer backgroundRenderer = background.GetComponent<SpriteRenderer>();

		if (backgroundSprite != null)
		{
			backgroundRenderer.sprite = backgroundSprite;
		}
		else
		{
			string key = Application.loadedLevelName + ".main.background";

			Sprite newBackground = LugusResources.use.Shared.GetSprite(LugusResources.use.Levels.GetText(key));

			if (newBackground != LugusResources.use.errorSprite)
			{
				backgroundRenderer.sprite = newBackground;
			}
		}
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

		if (type == MenuTypes.NONE)
		{
			background.gameObject.SetActive(false);
			DeactivateAllMenus();
			return;
		}

		if (menus.ContainsKey(type))
		{
			nextStep = menus[type];
		}

		if (nextStep != null)
		{
			if (!background.gameObject.activeSelf)
				background.gameObject.SetActive(true);

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