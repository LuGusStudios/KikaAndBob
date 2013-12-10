using UnityEngine;
using System.Collections;

public class FroggerGUIManager : LugusSingletonExisting<FroggerGUIManagerDefault> 
{	
}

public class FroggerGUIManagerDefault : MonoBehaviour
{
	protected Transform gui = null;

	private void Awake()
	{
		gui = GameObject.Find("GUI").transform;
	}

	public void GameWon()
	{
		LugusCoroutines.use.StartRoutine(WinRoutine());
		Debug.Log("Game won!");
	}

	private IEnumerator WinRoutine()
	{
		Transform child = gui.FindChild("YouWin");
		child.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);

		child.gameObject.SetActive(false);

		FroggerGameManager.use.StartNewGame();
	}

	public void GameLost()
	{
		LugusCoroutines.use.StartRoutine(LoseRoutine());
		Debug.Log("Game lost!");
	}

	private IEnumerator LoseRoutine()
	{
		Transform child = gui.FindChild("YouLose");
		child.gameObject.SetActive(true);
		
		yield return new WaitForSeconds(1f);
		
		child.gameObject.SetActive(false);

		FroggerGameManager.use.StartNewGame();
	}


}