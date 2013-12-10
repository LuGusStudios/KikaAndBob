using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanGUIManager : LugusSingletonExisting<PacmanGUIManagerDefault> 
{
}

public class PacmanGUIManagerDefault : MonoBehaviour
{
	void Start () 
	{
	}
	
	void Update()
	{
	}
	
	public void UpdatePickupCounter(int newValue)
	{
	}

	public void UpdateLives(int lives)
	{
		Debug.Log("Add lives message here: " + lives);
		PacmanGameManager.use.StartNewRound();
	}

	
	public void ShowGameOverMessage()
	{
		LugusCoroutines.use.StartRoutine(PlaceholderGameOver());
		Debug.Log("Add game over GUI action here. Just restarting for now.");
	}

	IEnumerator PlaceholderGameOver()
	{
		GameObject gui = GameObject.Find("GUI");
		Transform child = gui.transform.FindChild("YouLose");

		child.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);

		child.gameObject.SetActive(false);

		PacmanGameManager.use.StartNewGame();
	}

	public void ShowWinMessage()
	{
		LugusCoroutines.use.StartRoutine(PlaceholderWin());
		Debug.Log("Add win GUI action here.Just restarting for now.");
	}

	IEnumerator PlaceholderWin()
	{
		GameObject gui = GameObject.Find("GUI");
		Transform child = gui.transform.FindChild("YouWin");

		child.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);

		child.gameObject.SetActive(false);

		PacmanGameManager.use.StartNewGame();
	}
	
	public void UpdateDoors(List<GameTile> doors)
	{
		foreach(GameTile door in doors)
		{
			if (door == null)
				continue;

			if (door.tileType == GameTile.TileType.Collide)
				door.sprite.SetActive(true);
			else if (door.tileType == GameTile.TileType.Open)
				door.sprite.SetActive(false);
		}
	}
}
