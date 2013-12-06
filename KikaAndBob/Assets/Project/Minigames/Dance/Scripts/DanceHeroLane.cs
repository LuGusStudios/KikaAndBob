using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DanceHeroLane : MonoBehaviour 
{
	public List<DanceHeroLaneItem> items = new List<DanceHeroLaneItem>();

	public Transform actionPoint = null;

	public KikaAndBob.LaneItemActionType defaultActionType = KikaAndBob.LaneItemActionType.NONE;

	// public GameObject character = null;

	public void Hide()
	{
		this.gameObject.SetActive( false );
	}

	public void Show()
	{
		this.gameObject.SetActive( true );
	}

	public void AddItem(float delay, KikaAndBob.LaneItemActionType type, float duration = DanceHeroLaneItem.singleDuration)
	{
		items.Add( new DanceHeroLaneItem(this, delay, type, duration) );
	}

	public void AddItem(float delay, float duration = DanceHeroLaneItem.singleDuration )
	{
		items.Add( new DanceHeroLaneItem(this, delay, defaultActionType, duration) );
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
			DanceHeroLaneItem item = items[currentItemIndex];

			yield return new WaitForSeconds( item.delay );

			SpawnItem( item );


			++currentItemIndex;
		}
	}

	protected void SpawnItem(DanceHeroLaneItem item)
	{
		DanceHeroLaneItemRenderer.Create( item );
	}

	public void SetupLocal()
	{
		if( actionPoint == null )
		{
			actionPoint = transform.FindChild("ActionPoint");
		}

		if( actionPoint == null )
		{
			Debug.LogError(name + " : no ActionPoint known for this lane!");
		}
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
