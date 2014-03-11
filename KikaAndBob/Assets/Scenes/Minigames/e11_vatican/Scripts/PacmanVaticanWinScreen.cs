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
		Debug.Log("PacmanVaticanWinScreen: Showing custom win screen.");

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

		yield return new WaitForSeconds(1.0f);

		HUDManager.use.LevelEndScreen.Counter6.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter6.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.LevelEndScreen.Counter6.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.LevelEndScreen.Counter6.SetValue(0, false);

		yield return new WaitForSeconds(0.1f);

		int timeScore = 15000 - ( Mathf.FloorToInt(timer) * 50 );

		ScoreVisualizer
			.Score(KikaAndBob.CommodityType.Score, timeScore)
				.Position(HUDManager.use.LevelEndScreen.Counter1.transform.position)
				.HUDElement(HUDManager.use.LevelEndScreen.Counter6)
				.UseGUICamera(true)
				.Execute();

		yield return new WaitForSeconds(2.0f);

		int pickUpCount = PacmanLevelManager.use.itemsPickedUp;
		int step = 1;
		float pickupValue = 80;

		if (pickUpCount > 20)
		{
			step = 5;
		}

		for (int i = pickUpCount; i > 0 ; i -= step) 
		{
			ScoreVisualizer
				.Score(KikaAndBob.CommodityType.Score, step * pickupValue)
					.Position(HUDManager.use.LevelEndScreen.Counter2.transform.position)
					.HUDElement(HUDManager.use.LevelEndScreen.Counter6)
					.UseGUICamera(true)
					.Execute();

			if (pickUpCount - step > 0)
			{
				pickUpCount -= step;
				HUDManager.use.LevelEndScreen.Counter2.SetValue(pickUpCount);
			}

			yield return new WaitForSeconds(0.1f);
		}

		// do whatever's left
		if (pickUpCount > 0)
		{
			ScoreVisualizer
				.Score(KikaAndBob.CommodityType.Score, pickUpCount * pickupValue)
					.Position(HUDManager.use.LevelEndScreen.Counter2.transform.position)
					.HUDElement(HUDManager.use.LevelEndScreen.Counter6)
					.UseGUICamera(true)
					.Execute();

			pickUpCount = 0;
			HUDManager.use.LevelEndScreen.Counter2.SetValue(pickUpCount);
		}




//		int pickUpCount = PacmanLevelManager.use.itemsPickedUp;
//		int step = 1;
//
//		while (pickUpCount > 0)
//		{
//			ScoreVisualizer
//				.Score(KikaAndBob.CommodityType.Score, PacmanLevelManager.use.itemsPickedUp)
//					.Position(HUDManager.use.LevelEndScreen.Counter2.transform.position)
//					.HUDElement(HUDManager.use.LevelEndScreen.Counter6)
//					.
//					.UseGUICamera(true)
//					.Execute();
//		}



		
	}
}
