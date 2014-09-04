using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepLevelMenu : IMenuStep 
{
	public int customLevelLoad = 0;	// Dirty fix. CAUTION - dont apply this value to the prefab if changed
	public bool levelsUnlockedElsewhere = false;


	protected Transform levelBarsParent = null;
	protected List<Transform> levelBars = new List<Transform>();
	protected List<Button> levelButtons = new List<Button>();
	protected List<Button> highScoreButtons = new List<Button>();
	protected Button buttonRight = null;
	protected Button buttonLeft = null;
	protected Button buttonLeave = null;
	protected Vector2 centerScreen =  new Vector2(10.24f, 7.68f);
	protected ILugusCoroutineHandle moveRoutine = null;
	protected float offScreenDistance = 20.0f;
	protected int pageCounter = 0;
	protected List<int> levelIndices = null; 
	protected bool switchingPages = false;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();
	protected Vector3 originalPosition = Vector3.zero;

	public void SetupLocal()
	{
		if (levelBarsParent == null)
		{
			levelBarsParent = transform.FindChild("LevelBars");
		}
		if (levelBarsParent == null)
		{
			Debug.LogError("StepLevelMenu: Missing level bars transform!");
		}

		if( levelBars.Count < 5 )
		{
			for (int i = 1; i <= 5; i++) 
			{
				Transform t = levelBarsParent.transform.FindChild("LevelBar0" + i);

				if (t != null)
				{
					levelBars.Add(t);
					levelButtons.Add(t.FindChild("ButtonPlay").GetComponent<Button>());
					highScoreButtons.Add(t.FindChild("Icon").GetComponent<Button>());
				}
			}
		}

		if (buttonLeft == null)
		{
			buttonLeft = transform.FindChild("ButtonLeft").GetComponent<Button>();
		}
		if (buttonLeft == null)
		{
			Debug.LogError("StepLevelMenu: Missing left button.");
		}

		if (buttonRight == null)
		{
			buttonRight = transform.FindChild("ButtonRight").GetComponent<Button>();
		}
		if (buttonRight == null)
		{
			Debug.LogError("StepLevelMenu: Missing right button.");
		}

		if (buttonLeave == null)
		{
			buttonLeave = transform.FindChild("ButtonLeave").GetComponent<Button>();
		}
		if (buttonLeave == null)
		{
			Debug.LogError("StepLevelMenu: Missing leave button.");
		}

		originalPosition = transform.position;
	}
	
	public void SetupGlobal()
	{
		if( levelIndices == null || levelIndices.Count == 0 )
		{
			if (customLevelLoad > 0)
				levelLoader.SetLevelLoadCountCap(customLevelLoad);

			levelIndices = levelLoader.FindLevels(); 
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

	private bool locked = false;

	protected IEnumerator ScreenHideRoutine(int returnedLevel)
	{
		locked = true;
		
		ScreenFader.use.FadeOut(0.5f);
		
		yield return new WaitForSeconds(0.5f);

		levelLoader.LoadLevel(returnedLevel);
	}
	
	protected void Update () 
	{
		if (!activated || locked)
			return;

		for (int i = 0; i < levelButtons.Count; i++) 
		{
			if (levelButtons[i].pressed)
			{
				int selectedButton = (pageCounter * 5) + i;

				if (selectedButton < 0 && selectedButton >= levelIndices.Count)
				{
					Debug.LogError("StepLevelMenu: Level index is out of bounds!");
					return;
				}

				int returnedLevel = levelIndices[selectedButton];
	
				LugusCoroutines.use.StartRoutine(ScreenHideRoutine(returnedLevel));
			}
		}

		for (int i = 0; i < highScoreButtons.Count; i++) 
		{
			if (highScoreButtons[i].pressed)
			{
				int selectedButton = (pageCounter * 5) + i;
				
				if (selectedButton < 0 && selectedButton >= levelIndices.Count)
				{
					Debug.LogError("StepLevelMenu: Level index is out of bounds!");
					return;
				}
				
				MenuManager.use.currentSelectedLevel = levelIndices[selectedButton];

				MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.HighScoreMenu);
			}
		}

		if (buttonLeft.pressed && !switchingPages)
		{
			if (pageCounter >= 1)
			{
				--pageCounter;
			}

			if (moveRoutine != null && moveRoutine.Running)
			{
				moveRoutine.StopRoutine();
			}

			LugusCoroutines.use.StartRoutine(SwitchPages(false));
		}

		if (buttonRight.pressed && !switchingPages)
		{
			if (pageCounter < Mathf.CeilToInt(levelIndices.Count / 5 ))		// TO DO: Figure out the maximum here based on nr of levels. Now set to 1 max.
			{
				++pageCounter;
			}

			if (moveRoutine != null && moveRoutine.Running)
			{
				moveRoutine.StopRoutine();
			}

			LugusCoroutines.use.StartRoutine(SwitchPages(true));
		}

		if (buttonLeave.pressed)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
	}

	public bool LoadSingleLevel()
	{
		SetupLocal(); // because it might be Awake() hasn't been called yet here
		SetupGlobal();

		if (levelIndices.Count <= 0)
		{
			Debug.LogError("StepLevelMenu: There are no level config files!");
			levelLoader.LoadLevel(1);
			EnableBars(0);
			return true;
		}
		else if (levelIndices.Count == 1)
		{
			Debug.Log("StepLevelMenu: There is only 1 level: load that one!");
			levelLoader.LoadLevel(1);
			return true;
		}

		return false;
	}

	public override void Activate(bool animate = true)
	{
//		if (levelIndices == null)
//			SetupLocal();

		locked = false;

		HUDManager.use.LevelEndScreen.gameObject.SetActive(false);
		HUDManager.use.DisableAll();

		// make sure the very first level is always available
		// it would make sense to put this under Start or Awake, but since this menu can start inactive, it's possible that those get called AFTER this method, which is unwanted
//		if (LugusConfig.use.User.GetBool(Application.loadedLevelName + ".1", false) == false)
//			LugusConfig.use.User.SetBool(Application.loadedLevelName + ".1", true, true);


		// always start on the page with the highest not-unlocked level
		float highestUnlocked = 1.0f;

		for (int i = levelLoader.levelIndices.Count - 1; i >= 1; i--) 
		{
			if 	(LugusConfig.use.User.GetBool(Application.loadedLevelName + "_level_" + levelIndices[i], false) == true)
			{
				if (i < levelIndices.Count - 1)
					highestUnlocked = i + 1;
				else
					highestUnlocked = i;

				break;
			}
		}
	
		pageCounter = Mathf.FloorToInt(highestUnlocked / 5.0f);


		if( LoadSingleLevel() )
			return;
		
		activated = true;
		gameObject.SetActive(true);
		transform.localPosition = Vector3.zero;

		//Debug.LogError("ACTIVATE stepLevel " + levelIndices.Count);

		UpdateAndFlyIn(true);
		LoadLevelData();
	}

	public override void Deactivate(bool animate = true)
	{
		//Debug.LogError("DE-ACTIVATE stepLevel");

		activated = false;


		if (animate)
		{
			gameObject.MoveTo(new Vector3(-30, 0, 0)).IsLocal(true).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();
			locked = true;
		}
		else
		{
			gameObject.SetActive(false);
		}
	}



	protected void LoadLevelData()
	{
		// TO DO: Set data about levels here (name, description, etc.)
	}

	protected void EnableBars(int amount)
	{
		for (int i = 0; i < levelBars.Count; i++) 
		{
			if (i < amount)
			{
				levelBars[i].gameObject.SetActive(true);

			}
			else
			{
				levelBars[i].gameObject.SetActive(false);
			}
		}

		if (levelIndices.Count <= 5)
		{
			buttonLeft.gameObject.SetActive(false);
			buttonRight.gameObject.SetActive(false);
		}
		else if ((levelIndices.Count - (pageCounter * 5)) <= 5)
		{
			buttonLeft.gameObject.SetActive(true);
			buttonRight.gameObject.SetActive(false);
		}
		else if (pageCounter == 0)
		{
			buttonLeft.gameObject.SetActive(false);
			buttonRight.gameObject.SetActive(true);
		}
		else
		{
			buttonLeft.gameObject.SetActive(true);
			buttonRight.gameObject.SetActive(true);
		}
	}

	protected IEnumerator SwitchPages(bool toRight)
	{
		switchingPages = true;

		FlyOut(toRight);

		yield return new WaitForSeconds(0.51f);	// a little longer than the iTween animation to prevent double iTweens

		UpdateAndFlyIn(toRight);

		switchingPages = false;
	}

	protected void FlyOut(bool toRight)
	{
		float delay = 0.0f;
		foreach(Transform t in levelBars)
		{
			iTween.Stop(t.gameObject);

			float nextPosition = 0;

			if (toRight)
				nextPosition = centerScreen.x - offScreenDistance;
			else
				nextPosition = centerScreen.x + offScreenDistance;

			t.gameObject.MoveTo(t.transform.localPosition.x(nextPosition)).Time(0.5f).IsLocal(true).Delay(delay).EaseType(iTween.EaseType.easeOutBack).Execute();
			delay += 0.1f;
		}
	}

	protected void UpdateAndFlyIn(bool toRight)
	{
		float delay = 0.0f;
		foreach(Transform t in levelBars)
		{
			iTween.Stop(t.gameObject);
			if (toRight)
				t.transform.localPosition = t.transform.localPosition.x(centerScreen.x + offScreenDistance);
			else
				t.transform.localPosition = t.transform.localPosition.x(centerScreen.x - offScreenDistance);
			
			t.gameObject.MoveTo(t.transform.localPosition.x(centerScreen.x)).Time(0.5f).IsLocal(true).Delay(delay).EaseType(iTween.EaseType.easeOutBack).Execute();
			delay += 0.1f;
		}

		int pageStart = pageCounter * 5;

		EnableBars(levelIndices.Count - pageStart);

		bool previousUnlocked = true;
		
		for (int i = pageStart; i < pageStart + 5; i++)
		{
			if (i >= levelIndices.Count)
				break;

			Transform bar = levelBars[i % 5]; // always count between 0 and 4

			if (!bar.gameObject.activeInHierarchy)
				continue;

			string levelKey = Application.loadedLevelName + "." + levelIndices[i];

			string levelName = LugusResources.use.Levels.GetText(levelKey + ".name");
			bar.FindChild("Name").GetComponent<TextMeshWrapper>().SetText(levelName);

			string levelDescription = LugusResources.use.Levels.GetText(levelKey + ".description");
			bar.FindChild("Description").GetComponent<TextMeshWrapper>().SetText(levelDescription); 

			bool currentUnlocked = LugusConfig.use.User.GetBool(levelKey, false);

			bool unlocked = false;

			if (levelsUnlockedElsewhere)
			{
				unlocked = LugusConfig.use.User.GetBool(Application.loadedLevelName + "_level_" + levelIndices[i], false);
			}
			else
			{
				if (i == 0)	// first item is always unlocked
				{
					unlocked = true;
				}
				else // subsequent items are unlocked if previous one has been won
				{
					unlocked = LugusConfig.use.User.GetBool(Application.loadedLevelName + "_level_" + levelIndices[i-1], false);
				}
			}

			bar.FindChild("ButtonPlay").gameObject.SetActive(unlocked);
			bar.FindChild("Icon").gameObject.SetActive(unlocked);

			foreach(SpriteRenderer sr in bar.GetComponentsInChildren<SpriteRenderer>(false))
			{
				if (unlocked)
					sr.color = sr.color.a(1.0f);
				else
					sr.color = sr.color.a(0.6f);
			}

			foreach(TextMesh tm in bar.GetComponentsInChildren<TextMesh>(false))
			{
				if (unlocked)
					tm.color = tm.color.a(1.0f);
				else
					tm.color = tm.color.a(0.6f);
			}


		}
	}
}
