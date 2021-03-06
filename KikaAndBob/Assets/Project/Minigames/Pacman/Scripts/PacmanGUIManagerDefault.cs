using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanGUIManager : LugusSingletonExisting<PacmanGUIManagerDefault> 
{
}

public class PacmanGUIManagerDefault : MonoBehaviour
{
	protected Transform keysParent = null;
	protected Dictionary<string, HUDCounter> guiKeyItems = new Dictionary<string, HUDCounter>();
	public delegate void OnWinLevel(float timer, PacmanGameManagerDefault manager);
	public OnWinLevel onWinLevel = null;

	protected void OnDestroy()
	{
		//	Debug.Log("Clearing" + typeof (T).Name);
		
		this.enabled = false;
		PacmanGUIManager.Change(null);
	}

	public void SetupLocal()
	{
//		foreach(Transform t in keysParent)
//		{
//			guiKeyItems.Add(t);
//		}
	}
	
	public void SetupGlobal()
	{
		if (!guiKeyItems.ContainsKey("Key01"))
			guiKeyItems.Add("Key01", HUDManager.use.CounterSmallRight1);

		if (!guiKeyItems.ContainsKey("Key02"))
			guiKeyItems.Add("Key02", HUDManager.use.CounterSmallRight2);

		if (!guiKeyItems.ContainsKey("Dynamite"))
			guiKeyItems.Add("Dynamite", HUDManager.use.CounterSmallRight1);
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
		Debug.Log("PacmanGUIManager: Resetting GUI."); 
		HUDManager.use.DisableAll();
		
		HUDManager.use.PauseButton.gameObject.SetActive(true);

		HUDManager.use.CounterSmallLeft2.gameObject.SetActive(true);
		HUDManager.use.CounterSmallLeft2.commodity = KikaAndBob.CommodityType.Life;
		HUDManager.use.CounterSmallLeft2.SetValue(3, false);
		
		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Time;
		//HUDManager.use.CounterLargeLeft1.SetValue(0);
		HUDManager.use.CounterLargeLeft1.StartTimer(HUDCounter.Formatting.TimeS);
		
		HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.CounterLargeRight1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterLargeRight1.SetValue(0);

		// move these two counters around a bit to serve as the key GUI - these will only be shown if the respective keys are in the level
		HUDManager.use.CounterSmallRight1.transform.position = HUDManager.use.CounterSmallRight2.transform.position;
		HUDManager.use.CounterSmallRight2.transform.position += new Vector3(0, -1.1f, 0);

		// IMPORTANT: If these HUDCounter are inactive at loadtime, their Awakes will not be be called until they are set to active.
		// If SetupLocal hasn't run yet, the script may or may not have a reference to its icon renderer depending on execution order.
		// If it does not have a reference to it, the icon cannot be set properly. Hence, manually call SetActive, then deactivate again.
		// Ideally, a more robust solution is in order, but this will do for now.
		
		HUDManager.use.CounterSmallRight1.gameObject.SetActive(true);
		HUDManager.use.CounterSmallRight2.gameObject.SetActive(true);


		HUDManager.use.CounterSmallRight1.commodity = KikaAndBob.CommodityType.Key01;
		HUDManager.use.CounterSmallRight1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterSmallRight1.SetValue(0, false);

		HUDManager.use.CounterSmallRight2.commodity = KikaAndBob.CommodityType.Key02;
		HUDManager.use.CounterSmallRight2.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterSmallRight2.SetValue(0, false);

		HUDManager.use.CounterSmallRight1.gameObject.SetActive(false);
		HUDManager.use.CounterSmallRight2.gameObject.SetActive(false);

	}

	public void SetupPickupCounter(int startCount, int totalItems)
	{
		HUDManager.use.CounterLargeRight1.suffix = " / " + totalItems;
		HUDManager.use.CounterLargeRight1.SetValue(startCount, false);
	}

	public void UpdatePickupCounter(int newValue)
	{ 
		ScoreVisualizer.Score(KikaAndBob.CommodityType.Feather, 1).Audio("PickupSound").Position(PacmanGameManager.use.GetActivePlayer().transform.position).Execute();
		//HUDManager.use.CounterLargeRight1.SetValue(newValue, false);
	}

	public void UpdateLives(int lives)
	{
		HUDManager.use.CounterSmallLeft2.SetValue(lives, false);
	}

	public void PauseTimer(bool pause)
	{
		HUDManager.use.PauseButton.gameObject.SetActive(!pause);
		HUDManager.use.CounterLargeLeft1.PauseTimer(pause);
	}

	public void ShowGameOverMessage(float timer)
	{
		HUDManager.use.PauseButton.gameObject.SetActive(false);

		HUDManager.use.StopAll();
		HUDManager.use.LevelEndScreen.Show(false);
	
		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.TimeMS;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(timer);
	}

	public void ShowWinMessage(float timer, PacmanGameManagerDefault manager)
	{
		LugusCoroutines.use.StartRoutine(ShowWinMessageRoutine(timer, manager));
	}

	protected IEnumerator ShowWinMessageRoutine(float timer, PacmanGameManagerDefault manager)
	{
		// if we have a custon win screen handler, it will do the screen, if not, the default one below will be shown
		if (onWinLevel != null)
		{
			onWinLevel(timer, manager);
			yield break;
		}

		yield return LugusCoroutines.use.StartRoutine(manager.StoreScore(PacmanCrossSceneInfo.use.GetLevelIndex(), -timer));	// in standard pacman manager, scores are inverted
		
		HUDManager.use.PauseButton.gameObject.SetActive(false);
		
		HUDManager.use.StopAll();
		HUDManager.use.LevelEndScreen.Show(true);
		
		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.TimeMS;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(timer);
	}

	// this will get called each time a new key index has been added
	public void UpdateKeyGUIItems()
	{
		foreach(HUDCounter counter in guiKeyItems.Values)
		{
			counter.gameObject.SetActive(false);
		}

		foreach(string pickupKey in PacmanPickups.use.pickups.Keys)
		{
			if (guiKeyItems.ContainsKey(pickupKey))
			{
				guiKeyItems[pickupKey].gameObject.SetActive(true);
			}
		}


//		foreach(KeyValuePair<string, HUDCounter> keyGUI in guiKeyItems)
//		{
//			string key = keyGUI.Key;
//			if (PacmanPickups.use.pickups.ContainsKey(key))
//			{
//				keyGUI.Value.gameObject.SetActive(true);
//			}
//			else
//			{
//				keyGUI.Value.gameObject.SetActive(false);
//			}
//		}
	}

	public void DisplayKeyAmount(string key, int amount)
	{
		if (guiKeyItems.ContainsKey(key))
		{
			guiKeyItems[key].SetValue(amount);
		}
		else
		{
			Debug.LogError("PacmanGUIManager: Unknown key ID!");
		}



//		foreach(Transform t in guiKeyItems)
//		{
//			if (t.name == key)
//			{
//				TextMesh display = t.FindChild("Count").GetComponent<TextMesh>();
//				display.text = amount.ToString();
//				break;
//			}
//		}
	}

	public void ClearKeyGUI()
	{
		Debug.Log("Clearing key GUI.");
		foreach (HUDCounter keyGUI in guiKeyItems.Values)
		{
			keyGUI.gameObject.SetActive(false);
		}
	}

}
