using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanSicilyGUIConfig : IPacmanSpecificConfig 
{
	public override void DoSetup ()
	{
		HUDManager.use.GetElementForCommodity(KikaAndBob.CommodityType.Life).gameObject.SetActive(false);
		HUDManager.use.CounterSmallRight1.commodity = KikaAndBob.CommodityType.Dynamite;
		HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.Top, KikaAndBob.ScreenAnchor.Top);

		DirectionPad dp = (DirectionPad) FindObjectOfType(typeof(DirectionPad));

		if (dp != null)
		{
			PacmanDynamite[] dynamiteInLevel = (PacmanDynamite[]) FindObjectsOfType(typeof(PacmanDynamite));
		
			if (dynamiteInLevel.Length >= 1)
			{
				dp.transform.FindChild("Button1").gameObject.SetActive(true);
			}
			else
			{
				dp.transform.FindChild("Button1").gameObject.SetActive(false);
			}
		}
	}
}
