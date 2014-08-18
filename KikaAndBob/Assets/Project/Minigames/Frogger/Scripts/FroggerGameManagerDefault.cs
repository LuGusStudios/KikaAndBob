using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerGameManager : LugusSingletonExisting<FroggerGameManagerDefault> {
}

public class FroggerGameManagerDefault : IGameManager 
{
	public bool gameRunning = false;
	private bool firstFrame = true;
	private int currentIndex = 0;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();
	protected float timer = 0;
	protected int pickupCount = 0;
	protected float pickupBoost = 1;

	public override bool GameRunning
	{
		get{ return gameRunning; }
	}

	public override void StartGame()
	{
		string levelData = levelLoader.GetLevelData(FroggerCrossSceneInfo.use.GetLevelIndex());
		
		if (!string.IsNullOrEmpty(levelData))
		{
			FroggerLevelDefinition newLevel = FroggerLevelDefinition.FromXML(levelData);
			FroggerLevelManager.use.levels = new FroggerLevelDefinition[]{newLevel};
			SetUpLevel(0);
		}
		else
		{
			Debug.LogError("FroggerGameManager: Invalid level data!");
		}
		
		// if a level wasn't found above, we can still load a default level
	
	}

	public override void StopGame()
	{
		Debug.Log("FroggerGameManager: Won game!");
		
		gameRunning = false;

		CatchingMiceUnlockManager.use.CheckUnlock(levelLoader, FroggerCrossSceneInfo.use);

		string saveKey = Application.loadedLevelName + "_level_" + FroggerCrossSceneInfo.use.levelToLoad;
		LugusConfig.use.User.SetBool(saveKey, true, true);
		LugusConfig.use.SaveProfiles();

		int scoreTotal = Mathf.RoundToInt((timer - (pickupCount * pickupBoost)) * 100);
		//TO DO: STORE SCORE TOTAL HERE!
		
		LugusCoroutines.use.StartRoutine(EndGameRoutine(timer, pickupCount, scoreTotal));
	}

	public void SetUpLevel()
	{
		SetUpLevel(currentIndex);
	}

	public void SetUpLevel(int levelIndex)
	{
		currentIndex = levelIndex;
		Debug.Log ("Starting new game.");
		// TO DO: Give some sort of progression system!
		FroggerLevelManager.use.LoadLevel(levelIndex);

		FroggerPlayer lastPlayer = null;
		foreach(FroggerPlayer player in (FroggerPlayer[]) FindObjectsOfType(typeof(FroggerPlayer)))
		{
			player.Reset();
			lastPlayer = player;
		}

		FroggerCameraController.use.FocusOn(lastPlayer);

		FroggerGUIManager.use.ResetGUI();

		timer = 0;
		pickupCount = 0;

		gameRunning = true;
	}

	protected void Start()
	{
		levelLoader.FindLevels();

		FroggerLevelManager.use.ClearLevel();

		if (FroggerCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);
			StartGame();
		}
	}

	public void WinGame()
	{
		StopGame();
	}

	protected IEnumerator EndGameRoutine(float timer, int pickupCount, int scoreTotal)
	{
		//FroggerLevelManager.use.

		HUDManager.use.StopAll();

		yield return new WaitForSeconds(1.0f);

		FroggerGUIManager.use.GameWon(timer, pickupCount, scoreTotal);
	}

	public void LoseGame()
	{
		gameRunning = false;
		FroggerGUIManager.use.GameLost();
	}

	public void ModifyPickUpCount(int modifyValue)
	{
		pickupCount += modifyValue;
	}

	protected void Update()
	{
		if (gameRunning)
			timer += Time.deltaTime;
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
