using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanVaticanWinScreen : MonoBehaviour 
{
	public void SetupLocal()
	{
	}
	
	public void SetupGlobal()
	{
		PacmanGUIManager.use.onWinLevel += ShowWinScreen;
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}

	protected void ShowWinScreen(float timer)
	{
		HUDManager.use.PauseButton.gameObject.SetActive(false);
		
		HUDManager.use.StopAll();

		LugusCoroutines.use.StartRoutine(WinRoutine(timer));
	}

	protected IEnumerator WinRoutine(float timer)
	{
		yield return new WaitForSeconds(1.0f);

		HUDManager.use.LevelEndScreen.Show(true);
		
		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.TimeMS;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(timer);
		
		HUDManager.use.LevelEndScreen.Counter2.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter2.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.LevelEndScreen.Counter2.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.LevelEndScreen.Counter2.SetValue(PacmanLevelManager.use.itemsPickedUp);
	}
}
