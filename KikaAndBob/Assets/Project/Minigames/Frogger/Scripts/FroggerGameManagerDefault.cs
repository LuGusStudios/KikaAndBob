using UnityEngine;
using System.Collections;

public class FroggerGameManager : LugusSingletonExisting<FroggerGameManagerDefault> {
}

public class FroggerGameManagerDefault : MonoBehaviour 
{
	bool firstFrame = true;

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
	}

	// TO DO Placeholder!!!
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
		Debug.Log("Game won!");
	}

	public void LoseGame()
	{
		Debug.Log("Game lost!");

		foreach(FroggerCharacter character in (FroggerCharacter[]) FindObjectsOfType(typeof(FroggerCharacter)))
		{
			character.Reset();
		}
	}
}
