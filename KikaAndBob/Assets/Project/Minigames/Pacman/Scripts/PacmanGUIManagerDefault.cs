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

	public void ResetGUI()
	{
		Debug.Log("PacmanGUIManager: Resetting GUI."); 
		HUDManager.use.DisableAll();
		
		HUDManager.use.PauseButton.gameObject.SetActive(true);

		HUDManager.use.CounterLargeLeft2.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft2.commodity = KikaAndBob.CommodityType.Life;
		HUDManager.use.CounterLargeLeft2.SetValue(3, false);
		
		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Time;
		//HUDManager.use.CounterLargeLeft1.SetValue(0);
		HUDManager.use.CounterLargeLeft1.StartTimer(HUDCounter.Formatting.TimeS);
		
		HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.CounterLargeRight1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterLargeRight1.suffix = " / " + PacmanLevelManager.use.itemsToBePickedUp;
		HUDManager.use.CounterLargeRight1.SetValue(0);
	}

	public void UpdatePickupCounter(int newValue)
	{
		ScoreVisualizer.Score(KikaAndBob.CommodityType.Feather, 1).Position(PacmanGameManager.use.GetActivePlayer().transform.position).Execute();
		//HUDManager.use.CounterLargeRight1.SetValue(newValue, false);
	}

	public void UpdateLives(int lives)
	{
		HUDManager.use.CounterLargeLeft2.SetValue(lives, false);
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

	public void ShowWinMessage(float timer)
	{
		HUDManager.use.PauseButton.gameObject.SetActive(false);

		HUDManager.use.StopAll();
		HUDManager.use.LevelEndScreen.Show(true);

		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.TimeMS;
		//HUDManager.use.LevelEndScreen.Counter1.se

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
