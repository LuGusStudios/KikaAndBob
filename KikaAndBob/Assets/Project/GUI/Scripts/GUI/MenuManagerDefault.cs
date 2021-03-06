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
	public int currentSelectedLevel = 1;

	protected Transform background = null;
	protected bool firstFrame = true;

	public enum MenuTypes
	{
		NONE = -1,
		GameMenu = 1,
		LevelMenu = 2,
		HelpMenu = 3,
		HighScoreMenu = 4
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

		StepHighscoreMenu highScoreMenu = transform.FindChild("HighScoreMenu").GetComponent<StepHighscoreMenu>();
		if (highScoreMenu != null)
			menus.Add(MenuTypes.HighScoreMenu, highScoreMenu);
		else
			Debug.LogError("MenuManager: Missing high score menu!");

		if (background == null)
			background = transform.FindChild("Background");
		if (background == null)
			Debug.LogError("MenuManager: Missing background!");

		
		LoadConfig();
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
			string backgroundName = Application.loadedLevelName + "BG01";
		
			if( LugusResources.use.Levels.HasText(key) )
			{
				Debug.Log("Loading menu background texture from Levels text at key:" + key);
				backgroundName = LugusResources.use.Levels.GetText(key);
			}

			//Debug.LogError("BACKGROUND SPRITE " + backgroundName);

			backgroundRenderer.enabled = true;

			Sprite newBackground = LugusResources.use.Shared.GetSprite(backgroundName);

			if (newBackground != LugusResources.use.errorSprite)
			{
				backgroundSprite = newBackground;
				backgroundRenderer.sprite = newBackground;
			}
			else
			{
				backgroundRenderer.enabled = false;
			}
		}

	}

	protected void LoadConfig()
	{
		Debug.Log("MenuManager: Loading config settings.");

		// load language
		string pickedLanguage = LugusConfig.use.System.GetString("main.settings.langID", LugusResources.use.GetSystemLanguageID());
		LugusResources.use.ChangeLanguage(pickedLanguage);


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
		if (firstFrame)
			firstFrame = false;
	}

	protected void DeactivateAllMenus()
	{
		foreach(IMenuStep step in menus.Values)
		{
			if (firstFrame)
			{
				step.Deactivate(false);
			}
			else
			{
				if (step.IsActive() == true)
				{
					step.Deactivate(true);
				}
				else
				{
					step.Deactivate(false);
				}
			}

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
			// if there is only one level, we want to bypass the level selection screen and go directly to the level
			bool proceed = true;
			if( nextStep.GetComponent<StepLevelMenu>()!= null )
			{
				proceed = !nextStep.GetComponent<StepLevelMenu>().LoadSingleLevel();
			}

			//Debug.LogError("PROCEED " + proceed);

			if( proceed )
			{
				if (!background.gameObject.activeSelf)
					background.gameObject.SetActive(true);

				DeactivateAllMenus();
				nextStep.Activate();
			}
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

	protected void OnDestroy()
	{
		//	Debug.Log("Clearing" + typeof (T).Name);
		
		this.enabled = false;
		MenuManager.Change(null);
	}
}
