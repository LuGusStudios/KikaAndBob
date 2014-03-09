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

	public void ResetGUI()
	{
		Debug.Log("FroggerGUIManager: Resetting GUI."); 
		HUDManager.use.DisableAll();

		HUDManager.use.PauseButton.gameObject.SetActive(true);

		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.CounterLargeLeft1.SetValue(0);
		HUDManager.use.CounterLargeLeft1.StartTimer(HUDCounter.Formatting.TimeMS);

		HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.CounterLargeRight1.SetValue(0);

		// Make a custom counter for required pick ups
		FroggerLaneItemConditionalWin cWin = GameObject.FindObjectOfType<FroggerLaneItemConditionalWin>();
		if (cWin != null)
		{
			HUDManager.use.CounterLargeRight2.gameObject.SetActive(true);
			HUDManager.use.CounterLargeRight2.commodity = KikaAndBob.CommodityType.Custom;
			HUDManager.use.CounterLargeRight2.SetValue(0);

			// Custom icon
			HUDManager.use.CounterLargeRight2.icon.sprite = cWin.pickupIcon;
			HUDManager.use.CounterLargeRight2.icon.enabled = true;
		}
	}

	public void GameWon(float timer, int pickupCount, int scoreTotal)
	{
		HUDManager.use.StopAll();

		HUDManager.use.PauseButton.gameObject.SetActive(false);

		HUDManager.use.LevelEndScreen.Show(true);

		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.TimeMS;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(timer, true);

		HUDManager.use.LevelEndScreen.Counter2.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter2.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.LevelEndScreen.Counter2.SetValue(pickupCount, true);

		HUDManager.use.LevelEndScreen.Counter3.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter3.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.LevelEndScreen.Counter3.SetValue(scoreTotal, true);

		Debug.Log("Game won!");
	}
	
	public void GameLost()
	{
		HUDManager.use.PauseButton.gameObject.SetActive(false);

		HUDManager.use.StopAll();

		LugusCoroutines.use.StartRoutine(LoseRoutine());
		Debug.Log("Game lost!");
	}

	private IEnumerator LoseRoutine()
	{
		yield return new WaitForSeconds(1.0f);

		float timer = 2.5f;

		HUDManager.use.FailScreen.Show(timer);

		yield return new WaitForSeconds(timer);

		FroggerGameManager.use.SetUpLevel();
	}


}