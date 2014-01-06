using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanGUIManager : LugusSingletonExisting<PacmanGUIManagerDefault> 
{
}

public class PacmanGUIManagerDefault : MonoBehaviour
{
	protected Transform guiParent = null;
	protected TextMesh livesText = null;

	public void SetupLocal()
	{
		if (guiParent == null)
		{
			guiParent = GameObject.Find("GUI").transform;
		}
		if (guiParent == null)
		{
			Debug.LogError("Could not find GUI parent object.");
		}

		if (livesText == null)
		{
			livesText = guiParent.FindChild("Lives").GetComponent<TextMesh>();
		}
		if (livesText == null)
		{
			Debug.LogError("Could not find lives text mesh.");
		}
	}
	
	public void SetupGlobal()
	{
	
	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
	}
	
	public void UpdatePickupCounter(int newValue)
	{
	}

	public void UpdateLives(int lives)
	{
		livesText.text = lives.ToString();
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
	
	public void UpdateDoors(List<PacmanTile> doors)
	{
		foreach(PacmanTile door in doors)
		{
			if (door == null)
				continue;

			if (door.tileType == PacmanTile.TileType.Collide)
				door.sprite.SetActive(true);
			else if (door.tileType == PacmanTile.TileType.Open)
				door.sprite.SetActive(false);
		}
	}
}
