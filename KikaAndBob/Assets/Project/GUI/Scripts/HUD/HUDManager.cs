using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDManager : LugusSingletonRuntime<HUDManager> 
{
	public ProgressBar ProgressBar = null;
	public ProgressBar ProgressBarCenter = null;
	public ProgressBar ProgressBarLeft = null;
	public ProgressBar ProgressBarRight = null;
	public ProgressBar ProgressBarLeftBottom = null;

	public HUDCounter Counter1 = null;
	public HUDCounter Counter2 = null;
	public HUDCounter Counter3 = null;

	public HUDCounter CounterLargeLeft1 = null;
	public HUDCounter CounterLargeLeft2 = null;
	public HUDCounter CounterLargeRight1 = null;
	public HUDCounter CounterLargeRight2 = null;
	
	public HUDCounter CounterSmallLeft1 = null;
	public HUDCounter CounterSmallLeft2 = null;
	public HUDCounter CounterSmallRight1 = null;
	public HUDCounter CounterSmallRight2 = null;

	public HUDCounter CounterLargeBottomLeft1 = null;

	public Button PauseButton = null;
	public PausePopup PausePopup = null;

	public LevelEndScreen LevelEndScreen = null;
	public FailScreen FailScreen = null;


	public List<IHUDElement> elements = new List<IHUDElement>();

	public IHUDElement GetElementForCommodity(KikaAndBob.CommodityType commodity)
	{
		foreach( IHUDElement element in elements )
		{
			if( element.gameObject.activeSelf && element.commodity == commodity )
				return element;
		}

		return null;
	}

	public void DisableAll()
	{
		foreach( IHUDElement element in elements )
		{
			element.gameObject.SetActive(false);
		}
	}

	public void StopAll()
	{
		foreach( IHUDElement element in elements )
		{
			element.Stop();
		}
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only

		ProgressBarCenter 		= transform.FindChild("CommodityVisualizers/ProgressBarCenter").GetComponent<ProgressBar>();
		ProgressBarLeft 		= transform.FindChild("CommodityVisualizers/ProgressBarLeft").GetComponent<ProgressBar>();
		ProgressBarRight 		= transform.FindChild("CommodityVisualizers/ProgressBarRight").GetComponent<ProgressBar>();
		ProgressBarLeftBottom 	= transform.FindChild("CommodityVisualizers/ProgressBarLeftBottom").GetComponent<ProgressBar>();

		ProgressBar = ProgressBarCenter;

		ProgressBarCenter.gameObject.SetActive( false );
		ProgressBarLeft.gameObject.SetActive( false );
		ProgressBarRight.gameObject.SetActive( false );
		ProgressBarLeftBottom.gameObject.SetActive( false ); 


		
		CounterLargeLeft1 = transform.FindChild("CommodityVisualizers/CounterLargeLeft1").GetComponent<HUDCounter>();
		CounterLargeLeft2 = transform.FindChild("CommodityVisualizers/CounterLargeLeft2").GetComponent<HUDCounter>();
		CounterLargeRight1 = transform.FindChild("CommodityVisualizers/CounterLargeRight1").GetComponent<HUDCounter>();
		CounterLargeRight2 = transform.FindChild("CommodityVisualizers/CounterLargeRight2").GetComponent<HUDCounter>();

		CounterSmallLeft1 = transform.FindChild("CommodityVisualizers/CounterSmallLeft1").GetComponent<HUDCounter>();
		CounterSmallLeft2 = transform.FindChild("CommodityVisualizers/CounterSmallLeft2").GetComponent<HUDCounter>();
		CounterSmallRight1 = transform.FindChild("CommodityVisualizers/CounterSmallRight1").GetComponent<HUDCounter>();
		CounterSmallRight2 = transform.FindChild("CommodityVisualizers/CounterSmallRight2").GetComponent<HUDCounter>();

		CounterLargeBottomLeft1 = transform.FindChild("CommodityVisualizers/CounterLargeBottomLeft1").GetComponent<HUDCounter>();

		Counter1 = CounterSmallLeft1;
		Counter2 = CounterLargeLeft2;
		Counter3 = CounterSmallRight1;

		CounterLargeLeft1.gameObject.SetActive( false );
		CounterLargeLeft2.gameObject.SetActive( false );
		CounterLargeRight1.gameObject.SetActive( false );
		CounterLargeRight2.gameObject.SetActive( false );

		CounterSmallLeft1.gameObject.SetActive( false );
		CounterSmallLeft2.gameObject.SetActive( false );
		CounterSmallRight1.gameObject.SetActive( false );
		CounterSmallRight2.gameObject.SetActive( false );

		CounterLargeBottomLeft1.gameObject.SetActive( false );

		elements.Add ( ProgressBarCenter );
		elements.Add ( ProgressBarLeft );
		elements.Add ( ProgressBarRight );
		elements.Add ( ProgressBarLeftBottom );
		
		elements.Add ( CounterLargeLeft1 );
		elements.Add ( CounterLargeLeft2 );
		elements.Add ( CounterLargeRight1 );
		elements.Add ( CounterLargeRight2 );
		
		elements.Add ( CounterSmallLeft1 );
		elements.Add ( CounterSmallLeft2 );
		elements.Add ( CounterSmallRight1 );
		elements.Add ( CounterSmallRight2 );

		elements.Add ( CounterLargeBottomLeft1 );

		if( PauseButton == null )
		{
			PauseButton = transform.FindChild("PauseButton").GetComponent<Button>();
		}
		
		if( PausePopup == null )
		{
			PausePopup = transform.FindChild("PausePopup").GetComponent<PausePopup>();
		}

		if( LevelEndScreen == null )
		{
			LevelEndScreen = transform.FindChild("LevelEndScreen").GetComponent<LevelEndScreen>();
		}
		
		if( FailScreen == null )
		{
			FailScreen = transform.FindChild("FailScreen").GetComponent<FailScreen>();
		}
	}

	public void RepositionPauseButton(KikaAndBob.ScreenAnchor mainAnchor, KikaAndBob.ScreenAnchor subAnchor = KikaAndBob.ScreenAnchor.NONE)
	{
		Rect container = LugusUtil.UIScreenSize; // if subAnchor == NONE, we just use the full screen

		if( subAnchor != KikaAndBob.ScreenAnchor.NONE )
		{
			container = KikaAndBob.ScreenAnchorHelper.GetQuadrantRect(mainAnchor, LugusUtil.UIScreenSize);
			mainAnchor = subAnchor;
		}

		Rect buttonRect = (PauseButton.collider2D as BoxCollider2D).Bounds().ToRectXY();

		Vector2 position = KikaAndBob.ScreenAnchorHelper.ExtendTowards(mainAnchor, buttonRect, container, new Vector2(0.1f,0.1f) ); 
		PauseButton.transform.localPosition = position;
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		
		LevelEndScreen.gameObject.SetActive(true);
		LevelEndScreen.Hide();

		FailScreen.gameObject.SetActive (true);
		FailScreen.Hide();
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
		if( PauseButton.pressed )
		{
			this.PausePopup.Show();
		}
	}
}
