using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KikaAndBob
{
	public enum CommodityType
	{
		NONE = -1,

		Money = 1,
		Feather = 2,
		Time = 3,
		Life = 4,
		Score = 5,
		Distance = 6
	}
}

public class ScoreManager : MonoBehaviour 
{
	// PSEUDO
	public void RegisterScore(float value, KikaAndBob.CommodityType commodity )
	{
		IHUDElement hud = HUDManager.use.GetElementForCommodity(commodity);
		if( hud != null )
		{
			// move renderer to HUD in routine...
		}
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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
}
