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

	public void SetupLocal()
	{
		// assign variables that have to do with this class only

		ProgressBarCenter 		= transform.FindChild("ProgressBarCenter").GetComponent<ProgressBar>();
		ProgressBarLeft 		= transform.FindChild("ProgressBarLeft").GetComponent<ProgressBar>();
		ProgressBarRight 		= transform.FindChild("ProgressBarRight").GetComponent<ProgressBar>();
		ProgressBarLeftBottom 	= transform.FindChild("ProgressBarLeftBottom").GetComponent<ProgressBar>();

		ProgressBar = ProgressBarCenter;

		ProgressBarCenter.gameObject.SetActive( false );
		ProgressBarLeft.gameObject.SetActive( false );
		ProgressBarRight.gameObject.SetActive( false );
		ProgressBarLeftBottom.gameObject.SetActive( false ); 


		
		CounterLargeLeft1 = transform.FindChild("CounterLargeLeft1").GetComponent<HUDCounter>();
		CounterLargeLeft2 = transform.FindChild("CounterLargeLeft2").GetComponent<HUDCounter>();
		CounterLargeRight1 = transform.FindChild("CounterLargeRight1").GetComponent<HUDCounter>();
		CounterLargeRight2 = transform.FindChild("CounterLargeRight2").GetComponent<HUDCounter>();

		CounterSmallLeft1 = transform.FindChild("CounterSmallLeft1").GetComponent<HUDCounter>();
		CounterSmallLeft2 = transform.FindChild("CounterSmallLeft2").GetComponent<HUDCounter>();
		CounterSmallRight1 = transform.FindChild("CounterSmallRight1").GetComponent<HUDCounter>();
		CounterSmallRight2 = transform.FindChild("CounterSmallRight2").GetComponent<HUDCounter>();

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
