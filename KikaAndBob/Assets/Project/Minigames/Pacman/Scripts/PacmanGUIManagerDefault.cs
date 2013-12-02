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
		Debug.Log("Add game over GUI action here.");
	}
	
	
	public void ShowWinMessage()
	{
		Debug.Log("Add win GUI action here.");
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
