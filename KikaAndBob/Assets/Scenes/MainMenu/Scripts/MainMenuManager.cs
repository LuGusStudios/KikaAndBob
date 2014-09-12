using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : LugusSingletonExisting<MainMenuManager> 
{
	public MainMenuTypes currentMenu
	{
		get	
		{
			return _currentMenu;
		}
	}
	protected MainMenuTypes _currentMenu = MainMenuTypes.NONE;

	public Dictionary<MainMenuTypes, IMenuStep> menus = new Dictionary<MainMenuTypes, IMenuStep>();

	public enum MainMenuTypes
	{
		NONE = -1,
		Main = 0,
		Settings = 1,
		Avatar = 2,
		Games = 3,
		Language = 4,
		Reset = 5,
		Login = 6,
		Register = 7
	}

	protected TextMeshWrapper loginMessage = null;

	public void SetupLocal()
	{
		if (loginMessage == null)
		{
			loginMessage = transform.FindChild("Main/Message").GetComponent<TextMeshWrapper>();
		}
		
		if (loginMessage == null)
		{
			Debug.LogError("MainMenuManager: Missing log in message.");
		}
	}
	
	public void SetupGlobal()
	{
		MenuStepMain gameMenu = transform.FindChild("Main").GetComponent<MenuStepMain>();
		if (gameMenu != null)
			menus.Add(MainMenuTypes.Main, gameMenu);
		else
			Debug.LogError("MainMenuManager: Missing main menu!");

		MenuStepSettings settingsMenu = transform.FindChild("Settings").GetComponent<MenuStepSettings>();
		if (settingsMenu != null)
			menus.Add(MainMenuTypes.Settings, settingsMenu);
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

		MenuStepLanguage languageMenu = transform.FindChild("Language").GetComponent<MenuStepLanguage>();
		if (languageMenu != null)
			menus.Add(MainMenuTypes.Language, languageMenu);
		else
			Debug.LogError("MainMenuManager: Missing language menu!");

		MenuStepReset resetMenu = transform.FindChild("Reset").GetComponent<MenuStepReset>();
		if (resetMenu != null)
			menus.Add(MainMenuTypes.Reset, resetMenu);
		else
			Debug.LogError("MainMenuManager: Missing reset menu!");

		MenuStepLogin loginMenu = transform.FindChild("Login").GetComponent<MenuStepLogin>();
		if (loginMenu != null)
			menus.Add(MainMenuTypes.Login, loginMenu);
		else
			Debug.LogError("MainMenuManager: Missing login menu!");

		MenuStepRegister registerMenu = transform.FindChild("Register").GetComponent<MenuStepRegister>();
		if (registerMenu != null)
			menus.Add(MainMenuTypes.Register, registerMenu);
		else
			Debug.LogError("MainMenuManager: Missing register menu!");


		if (MainCrossSceneInfo.use.lastLoadedGameLevel != "" && MainCrossSceneInfo.use.lastLoadedGameLevel != "e00_catchingmice")
		{
			ShowMenu(MainMenuTypes.Games);
		}
		else
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
//		if (Input.GetKeyDown(KeyCode.L))
//		{
//			ShowMenu(MainMenuTypes.Login);
//		}
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
			_currentMenu = type;
			menus[type].Activate();
		}
		else
		{
			Debug.LogError("MainMenuManager: Turned on menu: " + type.ToString());
		}

	}

	public void SetLoginMessage(string message)
	{
		loginMessage.SetText(message);
	}
}
