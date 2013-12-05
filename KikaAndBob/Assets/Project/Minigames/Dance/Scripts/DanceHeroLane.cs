using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DanceHeroLane : MonoBehaviour 
{
	public List<DanceHeroLaneItem> items = new List<DanceHeroLaneItem>();

	// public GameObject character = null;

	public void Hide()
	{
		this.gameObject.SetActive( false );
	}

	public void Show()
	{
		this.gameObject.SetActive( true );
	}

	public void AddItem(float delay, KikaAndBob.LaneItemType type, float duration)
	{
		items.Add( new DanceHeroLaneItem(delay, type, duration) );
	}

	public void Begin()
	{
		LugusCoroutines.use.StartRoutine( LaneRoutine() );
	}

	protected IEnumerator LaneRoutine()
	{
		int currentItemIndex = 0;
		while( currentItemIndex < items.Count )
		{
			DanceHeroLaneItem item = null;

			yield return new WaitForSeconds( item.delay );
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
