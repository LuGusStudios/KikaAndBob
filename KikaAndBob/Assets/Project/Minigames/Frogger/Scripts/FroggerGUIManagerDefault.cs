using UnityEngine;
using System.Collections;

public class FroggerGUIManager : LugusSingletonExisting<FroggerGUIManagerDefault> 
{	
}

public class FroggerGUIManagerDefault : MonoBehaviour
{
	protected Transform gui = null;


	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		gui = GameObject.Find("GUI_Debug").transform;
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void ModifyTimer(float modifyValue)
	{
		HUDManager.use.CounterLargeLeft1.AddValue(modifyValue);
	}

	public void ResetGUI()
	{
		Debug.Log("FroggerGUIManager: Resetting GUI.");
		HUDManager.use.DisableAll();

		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.CounterLargeLeft1.StartTimer(HUDCounter.Formatting.TimeMS);

		HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.CounterLargeRight1.SetValue(0);
	}

	public void GameWon()
	{
		HUDManager.use.CounterLargeLeft1.StopTimer();
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
		HUDManager.use.CounterLargeLeft1.StopTimer();
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