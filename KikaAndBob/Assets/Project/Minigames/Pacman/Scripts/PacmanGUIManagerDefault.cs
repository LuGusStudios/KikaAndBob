using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanGUIManager : LugusSingletonExisting<PacmanGUIManagerDefault> 
{
}

public class PacmanGUIManagerDefault : MonoBehaviour
{
	protected Transform guiParent = null;
	protected Transform keysParent = null;
	protected TextMesh livesText = null;
	protected List<Transform> guiKeyItems = new List<Transform>();

	public void SetupLocal()
	{
		if (guiParent == null)
		{
			guiParent = GameObject.Find("GUI_Debug").transform;
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

		if (keysParent == null)
		{
			keysParent = guiParent.FindChild("Keys");
		}
		if (keysParent == null)
		{
			Debug.LogError("Could not find Keys parent object.");
		}

		foreach(Transform t in keysParent)
		{
			guiKeyItems.Add(t);
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

		PacmanGameManager.use.StartNewLevel();
	}

	// this will get called each time a new key index has been added
	public void UpdateKeyGUIItems()
	{
		foreach(Transform t in guiKeyItems)
		{
			if (PacmanPickups.use.pickups.ContainsKey(t.name))
			{
				t.gameObject.SetActive(true);
			}
			else
			{
				t.gameObject.SetActive(false);
			}
		}
	}

	public void DisplayKeyAmount(string key, int amount)
	{
		foreach(Transform t in guiKeyItems)
		{
			if (t.name == key)
			{
				TextMesh display = t.FindChild("Count").GetComponent<TextMesh>();
				display.text = amount.ToString();
				break;
			}
		}
	}

	public void ClearKeyGUI()
	{
		Debug.Log("Clearing key GUI.");
		foreach(Transform t in guiKeyItems)
		{
			t.gameObject.SetActive(false);
		}
	}

}
