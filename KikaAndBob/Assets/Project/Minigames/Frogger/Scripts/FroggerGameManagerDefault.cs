using UnityEngine;
using System.Collections;

public class FroggerGameManager : LugusSingletonExisting<FroggerGameManagerDefault> {
}

public class FroggerGameManagerDefault : MonoBehaviour 
{
	public bool gameRunning = false;
	private bool firstFrame = true;
	private int pickupCount = 0;

	public void StartNewGame()
	{
		Debug.Log ("Starting new game.");

		FroggerLevelManager.use.LoadLevel(0);
		//FroggerLaneManager.use.FindLanes();

		pickupCount = 0;

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

	public void IncreasePickupCount(int amount)
	{
		pickupCount ++;
		Debug.Log("Increased pickup count to " + pickupCount);
	}
}
