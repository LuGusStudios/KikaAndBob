using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanSicilyGUIConfig : IPacmanSpecificConfig 
{
	public override void DoSetup ()
	{
		HUDManager.use.GetElementForCommodity(KikaAndBob.CommodityType.Life).gameObject.SetActive(false);
		HUDManager.use.CounterSmallRight1.commodity = KikaAndBob.CommodityType.Dynamite;
	}
}
