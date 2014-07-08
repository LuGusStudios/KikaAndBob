using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceGUI : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{ 
		HUDManager.use.CounterSmallLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterSmallLeft1.commodity = KikaAndBob.CommodityType.Cheese;
		HUDManager.use.CounterSmallLeft1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterSmallLeft1.SetValue(0, false);

		HUDManager.use.CounterSmallLeft2.gameObject.SetActive(true);
		HUDManager.use.CounterSmallLeft2.commodity = KikaAndBob.CommodityType.Cookie;
		HUDManager.use.CounterSmallLeft2.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterSmallLeft2.SetValue(0, false);

		CatchingMiceLevelManager.use.OnCheeseRemoved += UpdateCheeseCount;
		CatchingMiceGameManager.use.onPickupCountChanged += UpdateCookieCount;
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
}
