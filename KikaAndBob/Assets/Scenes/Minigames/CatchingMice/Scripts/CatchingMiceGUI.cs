using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceGUI : LugusSingletonExisting<CatchingMiceGUI>
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{ 
	}

	public void ResetHUD()
	{
		Debug.Log("CatchingMiceGUI: Resetting HUD.");

		HUDManager.use.DisableAll();

		HUDManager.use.CounterSmallLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterSmallLeft1.commodity = KikaAndBob.CommodityType.Cheese;
		HUDManager.use.CounterSmallLeft1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterSmallLeft1.SetValue(CatchingMiceLevelManager.use.CheeseTiles.Count, false);
		
		HUDManager.use.CounterSmallLeft2.gameObject.SetActive(true);
		HUDManager.use.CounterSmallLeft2.commodity = KikaAndBob.CommodityType.Cookie;
		HUDManager.use.CounterSmallLeft2.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterSmallLeft2.SetValue(0, false);
		
		
		HUDManager.use.CounterLargeBottomLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeBottomLeft1.commodity = KikaAndBob.CommodityType.Custom;
		HUDManager.use.CounterLargeBottomLeft1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterLargeBottomLeft1.suffix = "/1";
		HUDManager.use.CounterLargeBottomLeft1.SetValue(1, false);
		
		
		CatchingMiceLevelManager.use.OnCheeseRemoved += UpdateCheeseCount;
		CatchingMiceGameManager.use.onPickupCountChanged += UpdateCookieCount;
		CatchingMiceGameManager.use.onWaveStarted += SetWaveCounter;
		CatchingMiceGameManager.use.onWaveEnded += SetWaveCounter;
		
		// set this once already, because there can be a little delay before the first wave begins
		HUDManager.use.CounterLargeBottomLeft1.suffix = " / " + CatchingMiceLevelManager.use.Waves.Count.ToString();
		HUDManager.use.CounterLargeBottomLeft1.SetValue(1);
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

	protected void UpdateCookieCount(int newCount)
	{
		HUDManager.use.CounterSmallLeft2.SetValue(newCount);
	}

	protected void UpdateCheeseCount(CatchingMiceTile tile)
	{
		HUDManager.use.CounterSmallLeft1.SetValue(CatchingMiceLevelManager.use.CheeseTiles.Count);
	}

	protected void SetWaveCounter(int index)
	{
		HUDManager.use.CounterLargeBottomLeft1.suffix = " / " + CatchingMiceLevelManager.use.Waves.Count.ToString();
		HUDManager.use.CounterLargeBottomLeft1.SetValue(index + 1);
	}
}
