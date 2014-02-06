﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IHUDElement : MonoBehaviour 
{
	public SpriteRenderer icon = null;

	protected KikaAndBob.CommodityType _commodity = KikaAndBob.CommodityType.NONE;
	public KikaAndBob.CommodityType commodity
	{
		get
		{
			return _commodity;
		}
		set
		{
			_commodity = value;
			UpdateIconForCommodityType();
		}
	}

	public void UpdateIconForCommodityType()
	{
		if( icon == null )
			return;

		string iconKey = "";
		if( _commodity == KikaAndBob.CommodityType.Money )
			iconKey = "IconMoney01";
		else if( _commodity == KikaAndBob.CommodityType.Feather )
			iconKey = "IconFeather01";
		else if( _commodity == KikaAndBob.CommodityType.Time )
			iconKey = "IconTime01";
		else if( _commodity == KikaAndBob.CommodityType.Life )
			iconKey = "IconHeart01";
		else if( _commodity == KikaAndBob.CommodityType.Score )
			iconKey = "IconBob01";
		else if( _commodity == KikaAndBob.CommodityType.Distance )
			iconKey = "IconSpeed01";

		Debug.LogError("Setting icon " + iconKey + " for " + _commodity );

		if( iconKey != "" )
			icon.sprite = LugusResources.use.Shared.GetSprite( iconKey );
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