using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerGameManager : LugusSingletonExisting<FroggerGameManagerDefault> {
}

public class FroggerGameManagerDefault : MonoBehaviour 
{
	public bool gameRunning = false;
	private bool firstFrame = true;
	private int currentIndex = 0;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();
	protected float timer = 0;
	protected int pickupCount = 0;

	public void StartNewGame()
	{
		StartNewGame(currentIndex);
	}

	public void StartNewGame(int levelIndex)
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

			string levelData = levelLoader.GetLevelData(FroggerCrossSceneInfo.use.GetLevelIndex());

			if (!string.IsNullOrEmpty(levelData))
			{
				FroggerLevelDefinition newLevel = FroggerLevelDefinition.FromXML(levelData);
				FroggerLevelManager.use.levels = new FroggerLevelDefinition[]{newLevel};
			}
			else
			{
				Debug.LogError("FroggerGameManager: Invalid level data!");
			}

			// if a level wasn't found above, we can still load a default level
			StartNewGame();
		}
	}

	public void WinGame()
	{
		gameRunning = false;
		string saveKey = Application.loadedLevelName + "." + (FroggerCrossSceneInfo.use.GetLevelIndex() + 1).ToString();
		LugusConfig.use.User.SetBool(saveKey, true, true);
		LugusConfig.use.SaveProfiles();
		FroggerGUIManager.use.GameWon();

		//TO DO: STORE TIMER SCORE HERE!
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
