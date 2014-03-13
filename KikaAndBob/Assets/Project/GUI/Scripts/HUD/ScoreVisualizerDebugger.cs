using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ScoreVisualizerDebugger : LugusSingletonRuntime<ScoreVisualizerDebugger>
{
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

	bool started = false;
	
	protected void Update () 
	{
		if( !started )
		{
			started = true;
			Setup();
		}
		/*
		//if( Input.GetKeyDown(KeyCode.A) )
		{
			ScoreVisualizer.Score(KikaAndBob.CommodityType.Time, 20).Color (Color.red).Execute();
		}
		//if( Input.GetKeyDown(KeyCode.S) )
		{
			for( int i = 0; i < 10; ++i )
			{
				ScoreVisualizer.Score(KikaAndBob.CommodityType.Feather, 1).Time (i * 0.1f).Position(new Vector3(Random.value * Screen.width, 0, 0) ).Execute();
			}
		}
		//if( Input.GetKeyDown(KeyCode.D) )
		{
			ScoreVisualizer.Score(KikaAndBob.CommodityType.Money, 200).Position(new Vector3(Random.value * Screen.width, 0, 0) ).Execute(); 
		}
		
		//if( Input.GetKeyDown(KeyCode.F) ) 
		{
			ScoreVisualizer.Score(KikaAndBob.CommodityType.Life, -1).Color (Color.red).Position(new Vector3(Random.value * Screen.width, 0, 0) ).Execute(); 
		}
		
		//if( Input.GetKeyDown(KeyCode.G) )
		{
			ScoreVisualizer.Score(KikaAndBob.CommodityType.Distance, 200).Title ("Distance++!").Time (2.0f).Position(new Vector3(Random.value * Screen.width, 0, 0) ).Execute(); 
		}
		*/
	}

	public void Setup()
	{
		HUDManager.use.DisableAll();


		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.CounterLargeLeft1.formatting = HUDCounter.Formatting.TimeS;
		HUDManager.use.CounterLargeLeft1.StartTimer();
		
		HUDManager.use.CounterSmallLeft2.gameObject.SetActive(true);
		HUDManager.use.CounterSmallLeft2.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.CounterSmallLeft2.SetValue(0);

		
		HUDManager.use.CounterLargeRight2.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight2.commodity = KikaAndBob.CommodityType.Money;
		HUDManager.use.CounterLargeRight2.suffix = "/9000";
		HUDManager.use.CounterLargeRight2.SetValue(0);
		
		HUDManager.use.CounterSmallRight1.gameObject.SetActive(true);
		HUDManager.use.CounterSmallRight1.commodity = KikaAndBob.CommodityType.Life;
		HUDManager.use.CounterSmallRight1.suffix = "/3";
		HUDManager.use.CounterSmallRight1.SetValue(3);

		HUDManager.use.ProgressBarLeftBottom.gameObject.SetActive(true);
		HUDManager.use.ProgressBarLeftBottom.commodity = KikaAndBob.CommodityType.Distance;
		HUDManager.use.ProgressBarLeftBottom.valueRange = new DataRange(0, 3000);
		HUDManager.use.ProgressBarLeftBottom.SetValue(0);

	}
}
