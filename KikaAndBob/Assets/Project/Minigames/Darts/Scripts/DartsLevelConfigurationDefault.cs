using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsLevelConfiguration : LugusSingletonRuntime<DartsLevelConfigurationDefault>
{

}

public class DartsLevelConfigurationDefault :  IGameManager
{
	public DartsLevelDefinition[] levels;
	protected DartsFunctionalityGroup[] groups;
	protected int currentIndex = 0;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();
	protected bool gameRunning = false;
	protected float levelDuration = 0.0f;
	protected float levelTimer = 0.0f;
	protected int minScore = 0;

	public void SetupLocal()
	{
		groups = (DartsFunctionalityGroup[])FindObjectsOfType(typeof(DartsFunctionalityGroup));
	}
	
	public void SetupGlobal()
	{
		levelLoader.FindLevels();
	}

	public override bool GameRunning {
		get 
		{
			return gameRunning;
		}
	}

	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();

		if (DartsCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);
			StartGame();
		}
	}

	public override void StartGame ()
	{
		gameRunning = false;	// turn this off until the level is fully configured
		string levelData = levelLoader.GetLevelData(DartsCrossSceneInfo.use.GetLevelIndex());
		
		if (!string.IsNullOrEmpty(levelData))
		{	
			DartsLevelDefinition newLevel = DartsLevelDefinition.FromXML(levelData);
			ConfigureLevel(newLevel);

			levelDuration = newLevel.levelDuration;
			levelTimer = 0.0f;

			minScore = newLevel.minimumScore;

			HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
			HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Score;
			HUDManager.use.CounterLargeLeft1.formatting = HUDCounter.Formatting.Int;
			HUDManager.use.CounterLargeLeft1.SetValue(0, false);

			HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
			HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Custom;
			HUDManager.use.CounterLargeRight1.formatting = HUDCounter.Formatting.Int;
			HUDManager.use.CounterLargeRight1.SetValue(0, false);
			
			HUDManager.use.ProgressBarCenter.gameObject.SetActive(true);
			HUDManager.use.ProgressBarCenter.commodity = KikaAndBob.CommodityType.Time;
			HUDManager.use.ProgressBarCenter.SetTimer(levelDuration);

			HUDManager.use.PauseButton.gameObject.SetActive(true);
			HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.BottomRight, KikaAndBob.ScreenAnchor.BottomRight);


			DartsScoreManager.use.Reset();

			if (!string.IsNullOrEmpty(newLevel.backgroundMusicName))
			{
				AudioClip backgroundMusic = LugusResources.use.Shared.GetAudio(newLevel.backgroundMusicName);

				if (backgroundMusic != null || backgroundMusic != LugusResources.use.errorAudio)
					LugusAudio.use.Music().Play(backgroundMusic, true, new LugusAudioTrackSettings().Loop(true));
			}

			gameRunning = true;
		
			Debug.Log("Started new Darts level. Time: " + levelDuration + ". Target score: " + minScore +".");
		}
		else
		{
			Debug.LogError("DartsLevelConfiguration: Invalid level data!");
		}
	}

	public override void StopGame ()
	{
		gameRunning = false;

		LugusCoroutines.use.StartRoutine(StopGameRoutine());
	}

	protected IEnumerator StopGameRoutine()
	{
		Debug.Log("Darts level ended.");
		
		foreach (DartsFunctionalityGroup group in groups) 
		{
			group.SetEnabled(false);
		}

		// do this first before score window is displayed
		yield return StartCoroutine(StoreScore(DartsCrossSceneInfo.use.GetLevelIndex(), DartsScoreManager.use.totalScore));
		
		// TO DO: Currently not using version with target score
		//		bool success = DartsScoreManager.use.totalScore >= minScore;
		//
		//		HUDManager.use.LevelEndScreen.Show( DartsScoreManager.use.totalScore >= minScore );
		//
		//		if (success)
		//		{
		//			LugusConfig.use.User.SetBool(Application.loadedLevelName + "_level_" + DartsCrossSceneInfo.use.levelToLoad, true, true);
		//			LugusConfig.use.SaveProfiles();
		//		}
		
		HUDManager.use.DisableAll();
		
		HUDManager.use.LevelEndScreen.Show(true);
		
		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(DartsScoreManager.use.totalScore, true);
		HUDManager.use.PauseButton.gameObject.SetActive(false);
		
		CatchingMiceUnlockManager.use.CheckUnlock(levelLoader, DartsCrossSceneInfo.use);
		
		LugusConfig.use.User.SetBool(Application.loadedLevelName + "_level_" + DartsCrossSceneInfo.use.levelToLoad, true, true);
		LugusConfig.use.SaveProfiles();
	}
	
	protected void Update () 
	{
		if (gameRunning)
		{
			levelTimer += Time.deltaTime;

			if (levelTimer >= levelDuration)
			{
				StopGame();
			}
		}
	}

	public void ConfigureLevel(int index)
	{
		if (levels == null || levels.Length <= 0)
		{
			Debug.LogError("Level array was null or empty.");
			return;
		}
		
		if (index >= levels.Length || index < 0)
		{
			Debug.LogError("Level index out of bounds.");
			return;
		}
		
		Debug.Log("Loading level: " + index);
		
		DartsLevelDefinition level = levels[index];

		ConfigureLevel(level);
	}

	public void ConfigureLevel(DartsLevelDefinition level)
	{
		// first disable groups
		foreach (DartsFunctionalityGroup group in groups) 
		{
			group.SetEnabled(false);
		}
		
		foreach(DartsGroupDefinition groupDefinition in level.groupDefinitions)
		{
			DartsFunctionalityGroup foundGroup = null;
			
			foreach (DartsFunctionalityGroup group in groups) 
			{
				if (group.gameObject.name == groupDefinition.id)
				{
					foundGroup = group;
					break;
				}
			}
			
			if (foundGroup == null)
			{
				Debug.LogError("Level id: " + groupDefinition.id + " not found.");
				continue;
			}
			
			foundGroup.SetEnabled(true);
			foundGroup.itemsOnScreen = groupDefinition.itemsOnScreen;
			foundGroup.minTimeBetweenShows = groupDefinition.minTimeBetweenShows;
			foundGroup.autoHideTimes = groupDefinition.autoHideTimes;
			foundGroup.avoidRepeat = groupDefinition.avoidRepeat;
			foundGroup.score = groupDefinition.score;
		}
	}



	protected void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		foreach(int index in levelLoader.levelIndices)
		{
			if (GUILayout.Button("Load level: " + index))
			{
				levelLoader.LoadLevel(index);
			}
		}	
	}
}
