using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanSicilyGUIConfig : IPacmanSpecificConfig 
{
	public override void DoSetup ()
	{
		HUDManager.use.CounterSmallRight1.commodity = KikaAndBob.CommodityType.Dynamite;
	}
}
