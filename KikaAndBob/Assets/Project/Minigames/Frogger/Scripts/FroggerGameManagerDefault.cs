using UnityEngine;
using System.Collections;

public class FroggerGameManager : LugusSingletonExisting<FroggerGameManagerDefault> {
}

public class FroggerGameManagerDefault : MonoBehaviour 
{
	public bool gameRunning = false;
	private bool firstFrame = true;

	public void StartNewGame()
	{
		Debug.Log ("Starting new game.");
		FroggerLaneManager.use.FindLanes();

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

		foreach(FroggerCharacter character in (FroggerCharacter[]) FindObjectsOfType(typeof(FroggerCharacter)))
		{
			character.Reset();
		}
	}
}
