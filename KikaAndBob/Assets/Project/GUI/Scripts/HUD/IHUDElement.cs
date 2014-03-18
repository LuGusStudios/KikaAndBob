using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IHUDElement : MonoBehaviour 
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

	public abstract void SetValue(float value, bool animate = true);
	public abstract void AddValue(float value, bool animate = true, float minValue = Mathf.NegativeInfinity, float maxValue = Mathf.Infinity);

	public abstract void Stop();

	public void UpdateIconForCommodityType()
	{

		if( icon == null )
		{
			Debug.LogError("Missing icon!");
			return;
		}

		icon.enabled = true;

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
			iconKey = "IconScore01";
		else if( _commodity == KikaAndBob.CommodityType.Distance )
			iconKey = "IconSpeed01";
		else if( _commodity == KikaAndBob.CommodityType.Key01 )
			iconKey = "IconKey01";
		else if( _commodity == KikaAndBob.CommodityType.Key02 )
			iconKey = "IconKey02";
		else if( _commodity == KikaAndBob.CommodityType.Dynamite )
			iconKey = "Dynamite01";
		else if( _commodity == KikaAndBob.CommodityType.Custom )
		{
			icon.enabled = false;
		}

		//Debug.LogError("Setting icon " + iconKey + " for " + _commodity );

		if( iconKey != "" )
			icon.sprite = LugusResources.use.Shared.GetSprite( iconKey );
	}

	/*
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
	*/
}
