using UnityEngine;
using System.Collections;

public class FroggerGameManager : LugusSingletonExisting<FroggerGameManagerDefault> {
}

public class FroggerGameManagerDefault : MonoBehaviour 
{
	public bool gameRunning = false;
	private bool firstFrame = true;
	private int currentIndex = 0;

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

		gameRunning = true;
	}

	// TO DO: Placeholder!!!
	void Update()
	{
		if (firstFrame)
		{
			StartNewGame();
			firstFrame = false;
		}
	}


	public void WinGame()
	{
		gameRunning = false;
		FroggerGUIManager.use.GameWon();
	}

	public void LoseGame()
	{
		gameRunning = false;
		FroggerGUIManager.use.GameLost();
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		for (int i = 0; i < FroggerLevelManager.use.levels.Length; i++) 
		{
			if (GUILayout.Button("Start Level " + i))
			{
				StartNewGame(i);
			}
		}
	}
}
